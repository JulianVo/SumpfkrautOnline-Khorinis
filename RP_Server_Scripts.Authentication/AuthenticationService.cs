using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GUC;
using GUC.Network;
using RP_Server_Scripts.Authentication.Login;
using RP_Server_Scripts.Client;
using RP_Server_Scripts.Database.Account;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Server_Scripts.Threading;
using RP_Shared_Script;
using RP_Shared_Script.Login;

namespace RP_Server_Scripts.Authentication
{
    /// <summary>
    /// Service class for creating and managing <see cref="Account"/> and <see cref="Session"/>s for <see cref="Client"/>s.
    /// </summary>
    public sealed class AuthenticationService
    {
        private readonly IAuthenticationContextFactory _ContextFactory;
        private readonly IPasswordService _PasswordService;
        private readonly IMainThreadDispatcher _Dispatcher;
        private readonly IPacketWriterPool _PacketWriterPool;
        private readonly object _Lock = new object();
        private readonly Dictionary<Client.Client, Session> _SessionByClient = new Dictionary<Client.Client, Session>();
        private readonly ILogger _Log;

        /// <summary>
        /// Array for quick look up of the login state of a <see cref="Client"/>.
        /// </summary>
        private readonly bool[] _ClientIdLoggedIn;

        /// <summary>
        /// Invokes all registered handlers after a <see cref="Client"/> was logged in.
        /// </summary>
        public event GenericEventHandler<AuthenticationService, LoginEventArgs> ClientLoggedIn;

        /// <summary>
        /// Invokes all registered handlers after a client was logged out.
        /// </summary>
        public event GenericEventHandler<AuthenticationService, LogoutEventArgs> ClientLoggedOut;

        /// <summary>
        /// Invokes all registered handlers after a new account has been created.
        /// </summary>
        public event GenericEventHandler<AuthenticationService, AccountCreatedEventArgs> AccountCreated;



        internal AuthenticationService(ClientList clientList, IAuthenticationContextFactory contextFactory, IPasswordService passwordService, IMainThreadDispatcher dispatcher, ILoggerFactory loggerFactory, ServerOptionsProvider optionsProvider, IPacketWriterPool packetWriterPool)
        {
            if (clientList == null)
            {
                throw new ArgumentNullException(nameof(clientList));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (optionsProvider == null)
            {
                throw new ArgumentNullException(nameof(optionsProvider));
            }

            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _PasswordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _PacketWriterPool = packetWriterPool ?? throw new ArgumentNullException(nameof(packetWriterPool));
            _Log = loggerFactory.GetLogger(GetType());

            _ClientIdLoggedIn = new bool[optionsProvider.Slot];


            clientList.ClientDisconnected += (sender, args) => LogoutClient(args.Client);
            ClientLoggedOut += (sender, args) => _Log.Info($"User '{args.Account.UserName}' has logged out");
            ClientLoggedIn += (sender, args) => _Log.Info($"User '{args.Session.Account.UserName}' logged has logged in");
            AccountCreated += (sender, args) => _Log.Info($"Account with username '{args.NewAccount.UserName}' has been created");
        }

        /// <summary>
        /// Logs in the given <see cref="Client"/> and binds its <see cref="Account"/> to the <see cref="Client"/> with a <see cref="Session"/>.
        /// <remarks>Fails if the <see cref="Client"/> is already logged in.</remarks>
        /// </summary>
        /// <param name="client">The <see cref="Client"/> that should be logged in.</param>
        /// <param name="userName">The username of the <see cref="Account"/> that should be used in the login.</param>
        /// <param name="password">The password of the <see cref="Account"/> that should be used in the login.</param>
        /// <returns>A <see cref="LoginResult"/> indicating the success of the operation and the newly created <see cref="Session"/>.</returns>
        public Task<LoginResult> LoginClientAsync(Client.Client client, string userName, string password)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            //Run the database access in another thread.
            var task = new Task<LoginResult>(() =>
             {
                 //Check that neither the client is already logged in nor the account with the given username is already in use by another client.
                 lock (_Lock)
                 {
                     if (_SessionByClient.ContainsKey(client))
                     {
                         throw new InvalidOperationException(@"The given client is already logged in.");
                     }

                     if (_SessionByClient.Values.Any(session =>
                         session.Account.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)))
                     {
                         return new LoginFailedResult(LoginFailedReason.AccountAlreadyLoggedIn, string.Empty);
                     }


                     using (var context = _ContextFactory.CreateContext())
                     {
                         //Try to get an Account with the given username
                         AccountEntity accountEntity = context.Accounts.FirstOrDefault(ac =>
                              ac.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

                         //An account with the given name does not exist or the password is wrong or is banned.
                         if (accountEntity == null || !_PasswordService.VerifyPassword(password, accountEntity.PasswordHash))
                         {
                             return new LoginFailedResult(LoginFailedReason.InvalidLoginData, string.Empty);
                         }

                         if (accountEntity.IsBanned)
                         {
                             return new LoginFailedResult(LoginFailedReason.Banned, accountEntity.BannedReasonText);
                         }

                         //Save last login info to the database.
                         accountEntity.LastLogin = DateTime.Now;
                         accountEntity.LastIpAddress = client.SystemAddress;
                         context.SaveChanges();

                         // Login successful, create an Account object and bind it to the client with a session.
                         Account account = new Account(accountEntity.UserName, accountEntity.AccountId, accountEntity.PasswordHash);
                         Session session = new Session(client, account);
                         _SessionByClient.Add(client, session);
                         _ClientIdLoggedIn[client.Id] = true;

                         //Invoke the login event. Enqueue it on the dispatcher, so it will be executed after we leave the current lock.
                         _Dispatcher.EnqueueAction(() => ClientLoggedIn?.Invoke(this, new LoginEventArgs(session)));

                         return new LoginSuccessfulResult(session);
                     }
                 }
             });

            //Run the task.
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates an <see cref="Account"/> with the given username and password.
        /// </summary>
        /// <param name="userName">The username of the new <see cref="Account"/></param>
        /// <param name="password">The password of the new <see cref="Account"/></param>
        /// <returns>A <see cref="Task{TResult}"/> that can be awaited to get the <see cref="AccountCreationResult"/></returns>
        public Task<AccountCreationResult> CreateAccountAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            return CheckAccountExistsAsync(userName).ContinueWith(nameCheckTask =>
            {
                try
                {
                    if (nameCheckTask.Result)
                    {
                        // The given username is already in use. Return unsuccessful.
                        return (AccountCreationResult)new AccountCreationFailed("Username already in use.");
                    }

                    using (var context = _ContextFactory.CreateContext())
                    {
                        //Create the account in the database.
                        AccountEntity accountEntity = new AccountEntity
                        {
                            UserName = userName,
                            PasswordHash = _PasswordService.CreatePasswordHash(password),
                            CreationTime = DateTime.Now,
                            LastLogin = DateTime.Now,
                            LastIpAddress = string.Empty
                        };

                        context.Accounts.Add(accountEntity);
                        context.SaveChanges();
                    }

                    return new AccountCreationSuccessful();
                }
                catch (Exception e)
                {
                    _Log.Error($"Something went wrong while creating a new account. Exception: {e}");
                    return new AccountCreationFailed("Internal server error.");
                }
            });
        }

        /// <summary>
        /// Creates a new <see cref="Account"/> and binds it to the given <see cref="Client"/> with a <see cref="Session"/>.
        /// <remarks>Removes any session that is already active on the given client(if the creation was successful).</remarks>
        /// </summary>
        /// <param name="client">The <see cref="Client"/> to which the new <see cref="Account"/> should be bound with a <see cref="Session"/>.</param>
        /// <param name="userName">The username of the new <see cref="Account"/>.</param>
        /// <param name="password">The password of the new <see cref="Account"/>.</param>
        /// <returns>A <see cref="LoginResult"/> indicating the success of the operation and the newly created <see cref="Session"/>.</returns>
        public Task<LoginResult> CreateAndLoginAccountAsync(Client.Client client, string userName, string password)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            //Chain the tasks for checking the usernames existence and the creation of the new account. All of this is done in another thread.
            return CheckAccountExistsAsync(userName).ContinueWith(nameCheckTask =>
            {
                try
                {
                    if (nameCheckTask.Result)
                    {
                        // The given username is already in use. Return unsuccessful.
                        return (LoginResult)new LoginFailedResult(LoginFailedReason.UserNameAlreadyInUse, "Username already in use.");
                    }

                    using (var context = _ContextFactory.CreateContext())
                    {
                        //Create the account in the database.
                        AccountEntity accountEntity = new AccountEntity
                        {
                            UserName = userName,
                            PasswordHash = _PasswordService.CreatePasswordHash(password),
                            CreationTime = DateTime.Now,
                            LastLogin = DateTime.Now,
                            LastIpAddress = client.SystemAddress
                        };

                        context.Accounts.Add(accountEntity);
                        context.SaveChanges();


                        //Login the new account
                        lock (_Lock)
                        {
                            //Logout the client just in case it is already logged in(this is a case that should not be allowed by the Frontend => Gothic client).
                            LogoutClient(client);

                            Account account = new Account(accountEntity.UserName, accountEntity.AccountId, accountEntity.PasswordHash);
                            Session session = new Session(client, account);
                            _SessionByClient.Add(client, session);
                            _ClientIdLoggedIn[client.Id] = true;

                            //Invoke account creation event. Enqueue it on the dispatcher, so it will be executed after we leave the current lock.
                            _Dispatcher.EnqueueAction(() => AccountCreated?.Invoke(this, new AccountCreatedEventArgs(account)));

                            //Invoke the login event. Enqueue it on the dispatcher, so it will be executed after we leave the current lock.
                            _Dispatcher.EnqueueAction(() => ClientLoggedIn?.Invoke(this, new LoginEventArgs(session)));

                            return (LoginResult)new LoginSuccessfulResult(session);
                        }
                    }
                }
                catch (Exception e)
                {
                    _Log.Error($"Something went wrong while creating a new account and login in. Exception: {e}");
                    return new LoginFailedResult(LoginFailedReason.None,"Internal server error.");
                }
            });
        }

        /// <summary>
        /// Checks whether the an <see cref="Account"/> with the given username does exist.
        /// </summary>
        /// <param name="userName">The username that should be checked for an existing account.</param>
        /// <returns>A <see cref="Task{TResult}"/> that can be awaited for to get the resulting <see cref="bool"/> result.</returns>
        public Task<bool> CheckAccountExistsAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            //Run the database access in another thread.
            Task<bool> task = new Task<bool>(() =>
            {
                try
                {
                    using (var context = _ContextFactory.CreateContext())
                    {
                        return context.Accounts.Any(ac => ac.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
                    }
                }
                catch (Exception e)
                {
                    _Log.Error($"Something went wrong while checking for the existence of an account with the name '{userName}'. Exception: {e}");
                    throw;
                }
            });

            task.Start();
            return task;
        }

        /// <summary>
        /// Gets an enumerable that can be used to enumerate over all currently active <see cref="Session"/>s.
        /// </summary>
        public IEnumerable<Session> ActiveSessions
        {
            get
            {
                lock (_Lock)
                {
                    return _SessionByClient.Values;
                }
            }
        }

        /// <summary>
        /// Tries to get the <see cref="Session"/> of the given <see cref="Client"/> if one is active.
        /// </summary>
        /// <param name="client">The <see cref="Client"/> for which its <see cref="Session"/> should be found.</param>
        /// <param name="session">The <see cref="Session"/> that was found.</param>
        /// <returns>Returns true if a <see cref="Session"/> for was found for the given <see cref="Client"/>. False if not.</returns>
        public bool TryGetSession(Client.Client client, out Session session)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            lock (_Lock)
            {
                return _SessionByClient.TryGetValue(client, out session);
            }
        }

        /// <summary>
        /// Does a quick lookup to check whether the given client is currently logged in.
        /// </summary>
        /// <param name="client">The <see cref="Client"/> for which the login state should be looked up.</param>
        /// <returns>True if the given <see cref="Client"/> is logged in, false if not</returns>
        public bool IsLoggedIn(Client.Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            lock (_Lock)
            {
                return _ClientIdLoggedIn[client.Id];
            }
        }

        /// <summary>
        /// Logs out the given <see cref="Client"/>.
        /// <remarks>Does nothing if the <see cref="Client"/> is not logged in.</remarks>
        /// </summary>
        /// <param name="client">The <see cref="Client"/> that should be logged out.</param>
        public void LogoutClient(Client.Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            //Check if the client is logged in and if that is the case remove it from the dictionary of logged in clients.
            Account account = null;
            lock (_Lock)
            {
                if (_SessionByClient.TryGetValue(client, out Session session))
                {
                    _SessionByClient.Remove(client);
                    account = session.Account;
                    _ClientIdLoggedIn[session.ClientId] = false;
                    session.Invalidate();
                }
            }

            //If the Client ist still connected we have to send the messages that are required to bring the client back to the state of being able to log in again.
            if (client.IsConnected)
            {
                using (var message = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.LogoutAcknowledged))
                {
                    client.SendScriptMessage(message, NetPriority.High, NetReliability.ReliableOrdered);
                }
            }

            //Invoke the logout event.
            if (account != null)
            {
                _Dispatcher.RunOrEnqueue(() =>
                {
                    var eventArgs = new LogoutEventArgs(client, account);
                    account.OnLoggedOut(eventArgs);
                    ClientLoggedOut?.Invoke(this, eventArgs);
                });
            }
        }
    }
}