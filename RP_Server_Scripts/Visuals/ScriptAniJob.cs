using System;
using GUC.Animations;
using GUC.Network;
using GUC.Utilities;

namespace RP_Server_Scripts.Visuals
{
    public class ScriptAniJob : ExtendedObject, AniJob.IScriptAniJob
    {
        public bool IsCreated => BaseAniJob.IsCreated;

        public AniJob BaseAniJob { get; }

        public string AniName
        {
            get => BaseAniJob.Name;
            set => BaseAniJob.Name = value;
        }

        public ScriptAni DefaultAni => (ScriptAni) BaseAniJob.DefaultAni?.ScriptObject;

        public ModelDef ModelDef => (ModelDef) BaseAniJob.ModelInstance?.ScriptObject;

        public int ID
        {
            get => BaseAniJob.ID;
            set => BaseAniJob.ID = value;
        }

        public int Layer
        {
            get => BaseAniJob.Layer;
            set => BaseAniJob.Layer = value;
        }

        public ScriptAniJob NextAni
        {
            get => (ScriptAniJob) BaseAniJob.NextAni.ScriptObject;
            set => BaseAniJob.NextAni = value.BaseAniJob;
        }

        public void SetDefaultAni(Animation ani)
        {
            SetDefaultAni((ScriptAni) ani.ScriptObject);
        }

        public virtual void SetDefaultAni(ScriptAni ani)
        {
            BaseAniJob.SetDefaultAni(ani.BaseAni);
        }

        public void AddOverlayAni(Animation ani, Overlay overlay)
        {
            AddOverlayAni((ScriptAni) ani.ScriptObject, (ScriptOverlay) overlay.ScriptObject);
        }

        public virtual void AddOverlayAni(ScriptAni ani, ScriptOverlay ov)
        {
            BaseAniJob.AddOverlayAni(ani.BaseAni, ov.BaseOverlay);
        }

        public void RemoveOverlayAni(Animation ani)
        {
            RemoveOverlayAni((ScriptAni) ani.ScriptObject);
        }

        public void RemoveOverlayAni(ScriptAni ani)
        {
            BaseAniJob.RemoveOverlayAni(ani.BaseAni);
        }

        public ScriptAniJob()
        {
            BaseAniJob = new AniJob(this);
        }

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }


        private string _CodeName;

        public string CodeName
        {
            get => _CodeName;
            set
            {
                if (IsCreated)
                {
                    throw new Exception("CodeName can't be changed when the object is already created!");
                }

                _CodeName = value == null ? "" : value.ToUpperInvariant();
            }
        }

        public ScriptAniJob(string codeName) : this()
        {
            CodeName = codeName;
        }

        public ScriptAniJob(string codeName, string gothicName) : this(codeName)
        {
            AniName = gothicName;
        }

        public ScriptAniJob(string codeName, ScriptAni defaultAni) : this(codeName)
        {
            SetDefaultAni(defaultAni);
        }

        public ScriptAniJob(string codeName, string gothicName, ScriptAni defaultAni) : this()
        {
            CodeName = codeName;
            AniName = gothicName;
            SetDefaultAni(defaultAni);
        }
    }
}