using System;
using GUC.Network;
using GUC.Models;
using GUC.Animations;
using GUC.Utilities;

namespace GUC.Scripts.Sumpfkraut.Visuals
{
    public class ModelDef : ExtendedObject, ModelInstance.IScriptModelInstance
    {
        public ModelInstance BaseDef { get; }

        public int ID => BaseDef.ID;
        public bool IsStatic => BaseDef.IsStatic;
        public bool IsCreated => this.BaseDef.IsCreated;

        public string Visual { get => BaseDef.Visual;
            set => BaseDef.Visual = value;
        }

        // By IDs

        public static bool Contains(int id)
        {
            return ModelInstance.Contains(id);
        }

        public static bool TryGetModel(int id, out ModelDef model)
        {
            ModelInstance m;
            if (ModelInstance.TryGet(id, out m))
            {
                model = (ModelDef)m.ScriptObject;
                return true;
            }
            model = null;
            return false;
        }

        // Loops

        public static void ForEachModel(Action<ModelDef> action)
        {
            ModelInstance.ForEach(m => action((ModelDef)m.ScriptObject));
        }

        public static void ForEachModelPredicate(Predicate<ModelDef> predicate)
        {
            ModelInstance.ForEachPredicate(m => predicate((ModelDef)m.ScriptObject));
        }

        public int GetCount()
        {
            return ModelInstance.Count;
        }

        public ModelDef()
        {
            this.BaseDef = new ModelInstance(this);
        }

        public void Create()
        {
            this.BaseDef.Create();
        }

        public void Delete()
        {
            this.BaseDef.Delete();
        }

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        public void AddOverlay(Overlay overlay)
        {
            AddOverlay((ScriptOverlay)overlay.ScriptObject);
        }

        public void RemoveOverlay(Overlay overlay)
        {
            RemoveOverlay((ScriptOverlay)overlay.ScriptObject);
        }

        public void AddOverlay(ScriptOverlay overlay)
        {
            this.BaseDef.AddOverlay(overlay.BaseOverlay);
        }

        public void RemoveOverlay(ScriptOverlay overlay)
        {
            this.BaseDef.RemoveOverlay(overlay.BaseOverlay);
        }

        public void AddAniJob(AniJob aniJob)
        {
            AddAniJob((ScriptAniJob)aniJob.ScriptObject);
        }

        public void RemoveAniJob(AniJob aniJob)
        {
            RemoveAniJob((ScriptAniJob)aniJob.ScriptObject);
        }

        public void AddAniJob(ScriptAniJob aniJob)
        {
            this.BaseDef.AddAniJob(aniJob.BaseAniJob);
        }

        public void RemoveAniJob(ScriptAniJob aniJob)
        {
            this.BaseDef.RemoveAniJob(aniJob.BaseAniJob);
        }
    }
}
