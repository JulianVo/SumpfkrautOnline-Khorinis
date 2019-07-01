namespace RP_Server_Scripts.Authentication
{
    public abstract class AccountCreationResult
    {
        protected AccountCreationResult(bool successful)
        {
            Successful = successful;
        }

        public bool Successful { get; }
    }
}
