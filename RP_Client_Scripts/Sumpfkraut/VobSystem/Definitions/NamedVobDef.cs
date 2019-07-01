namespace GUC.Scripts.Sumpfkraut.VobSystem.Definitions
{

    public abstract partial class NamedVobDef : VobDef
    {
        partial void pConstruct();
        public NamedVobDef()
        {
            pConstruct();
        }

        protected string name = "";
        /// <summary>The standard name of this named vob.</summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value ?? ""; }
        }
    }

}
