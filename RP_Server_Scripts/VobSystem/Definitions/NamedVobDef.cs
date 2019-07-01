namespace RP_Server_Scripts.VobSystem.Definitions
{
    public abstract class NamedVobDef : VobDef
    {
        private string _Name = "";

        /// <summary>The standard name of this named vob.</summary>
        public virtual string Name
        {
            get => _Name;
            set => _Name = value ?? "";
        }

        protected NamedVobDef(string codeName, IBaseDefFactory baseDefFactory,  IVobDefRegistration registration) : base(codeName, baseDefFactory,  registration)
        {
        }
    }
}