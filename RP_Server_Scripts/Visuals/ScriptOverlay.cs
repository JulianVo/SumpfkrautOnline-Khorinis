using System;
using GUC.Animations;
using GUC.Network;
using GUC.Utilities;

namespace RP_Server_Scripts.Visuals
{
    public class ScriptOverlay : ExtendedObject, Overlay.IScriptOverlay
    {
        public Overlay BaseOverlay { get; }

        public int ID => BaseOverlay.ID;
        public ModelDef Model => (ModelDef) BaseOverlay.Model.ScriptObject;

        public string Name
        {
            get => BaseOverlay.Name;
            set => BaseOverlay.Name = value;
        }

        public ScriptOverlay()
        {
            BaseOverlay = new Overlay(this);
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
                    throw new Exception("CodeName can't be changed when the object is created!");
                }

                _CodeName = value == null ? "" : value.ToUpper();
            }
        }

        public bool IsCreated => BaseOverlay.IsCreated;

        public ScriptOverlay(string codeName) : this(codeName, null)
        {
        }

        public ScriptOverlay(string codeName, string name) : this()
        {
            CodeName = codeName;
            Name = name;
        }
    }
}