namespace RP_Server_Scripts.Authentication
{
    public abstract class LoginResult
    {
        protected LoginResult(bool successful)
        { 
            Successful = successful;
        }

        public bool Successful { get; }
    }
}
