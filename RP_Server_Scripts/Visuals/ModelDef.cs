using System;
using System.Collections.Generic;
using GUC.Animations;
using GUC.Models;
using GUC.Network;
using GUC.Utilities;
using RP_Server_Scripts.ReusedClasses;

namespace RP_Server_Scripts.Visuals
{
    public class ModelDef : ExtendedObject, ModelInstance.IScriptModelInstance
    {
        public ModelInstance BaseDef { get; }

        public int ID => BaseDef.ID;
        public bool IsStatic => BaseDef.IsStatic;
        public bool IsCreated => BaseDef.IsCreated;

        public string Visual
        {
            get => BaseDef.Visual;
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
            BaseDef = new ModelInstance(this);
        }


        public void Create()
        {
            BaseDef.Create();
            if (string.IsNullOrWhiteSpace(CodeName))
            {
                throw new Exception("CodeName is null or white space!");
            }

            _Names.Add(CodeName, this);
        }


        public void Delete()
        {
            BaseDef.Delete();
            _Names.Remove(CodeName);
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
            BaseDef.AddOverlay(overlay.BaseOverlay);
            if (string.IsNullOrWhiteSpace(overlay.CodeName))
            {
                throw new ArgumentException("CodeName of ScriptOverlay is null or white space!");
            }

            _OvNames.Add(overlay.CodeName, overlay);
        }


        public void RemoveOverlay(ScriptOverlay overlay)
        {
            BaseDef.RemoveOverlay(overlay.BaseOverlay);
            _OvNames.Remove(overlay.CodeName);
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
            BaseDef.AddAniJob(aniJob.BaseAniJob);
            if (string.IsNullOrWhiteSpace(aniJob.CodeName))
            {
                throw new ArgumentException("CodeName of ScriptAniJob is null or white space!");
            }

            _AniNames.Add(aniJob.CodeName, aniJob);

            Catalog?.AddJob((ScriptAniJob)aniJob);
        }


        public void RemoveAniJob(ScriptAniJob aniJob)
        {
            BaseDef.RemoveAniJob(aniJob.BaseAniJob);
            if (_AniNames.Remove(aniJob.CodeName))
            {
                Catalog?.RemoveJob((ScriptAniJob)aniJob);
            }
        }

        private static readonly Dictionary<string, ModelDef> _Names =
            new Dictionary<string, ModelDef>(StringComparer.OrdinalIgnoreCase);

        // By Names

        public static bool Contains(string codeName)
        {
            if (codeName == null)
            {
                throw new ArgumentNullException(nameof(codeName));
            }

            return _Names.ContainsKey(codeName);
        }

        public static bool TryGetModel(string codeName, out ModelDef model)
        {
            if (codeName == null)
            {
                throw new ArgumentNullException(nameof(codeName));
            }

            var returnVal = _Names.TryGetValue(codeName, out ModelDef outVal);
            model = outVal;
            return returnVal;
        }

        public AniCatalog Catalog { get; private set; }

        public void SetAniCatalog(AniCatalog catalog)
        {
            if (Catalog != null)
            {
                ForEachAniJob(job => catalog.RemoveJob((ScriptAniJob)job));
            }

            Catalog = catalog;

            if (Catalog != null)
            {
                ForEachAniJob(job => catalog.AddJob((ScriptAniJob)job));
            }
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

                _CodeName = value;
            }
        }

        private float _Radius = 1;

        public float Radius
        {
            get => _Radius;
            set
            {
                if (IsCreated)
                {
                    throw new Exception("Radius can't be changed when the object is created!");
                }

                _Radius = value;
            }
        }

        private float _Height = 1;

        public float HalfHeight
        {
            get => _Height;
            set
            {
                if (IsCreated)
                {
                    throw new Exception("Height can't be changed when the object is created!");
                }

                _Height = value;
            }
        }

        private float _CenterOffset = 1;

        public float CenterOffset
        {
            get => _CenterOffset;
            set
            {
                if (IsCreated)
                {
                    throw new Exception("CenterOffset can't be changed when the object is created!");
                }

                _CenterOffset = value;
            }
        }

        private float _FistRange = 1;

        public float FistRange
        {
            get => _FistRange;
            set
            {
                if (IsCreated)
                {
                    throw new Exception("FistRange can't be changed when the object is created!");
                }

                _FistRange = value;
            }
        }

        public bool IsNpcModel()
        {
            return Visual.EndsWith(".MDS");
        }

        public ModelDef(string codeName) : this(codeName, null)
        {
        }

        public ModelDef(string codeName, string visual) : this()
        {
            Visual = visual;
            CodeName = codeName;
        }


        private readonly Dictionary<string, ScriptAniJob> _AniNames =
            new Dictionary<string, ScriptAniJob>(StringComparer.OrdinalIgnoreCase);

        // By Names

        public bool ContainsAniJob(string codeName)
        {
            if (codeName == null)
            {
                return false;
            }

            return _AniNames.ContainsKey(codeName);
        }

        public bool TryGetAniJob(string codeName, out ScriptAniJob job)
        {
            if (codeName == null)
            {
                job = null;
                return false;
            }

            return _AniNames.TryGetValue(codeName, out job);
        }

        public void ForEachAniJob(Action<ScriptAniJob> action)
        {
            BaseDef.ForEachAniJob(job => action((ScriptAniJob)job.ScriptObject));
        }

        private readonly Dictionary<string,ScriptOverlay> _OvNames =
            new Dictionary<string, ScriptOverlay>(StringComparer.OrdinalIgnoreCase);

        public bool ContainsOverlay(string codeName)
        {
            if (codeName == null)
            {
                return false;
            }

            return _OvNames.ContainsKey(codeName);
        }

        public bool TryGetOverlay(string codeName, out ScriptOverlay ov)
        {
            if (codeName == null)
            {
                ov = null;
                return false;
            }

            return _OvNames.TryGetValue(codeName, out ov);
        }

        public void ForEachOverlay(Action<ScriptOverlay> action)
        {
            BaseDef.ForEachOverlay(o => action((ScriptOverlay)o.ScriptObject));
        }

        public void ForEachOverlayPredicate(Predicate<ScriptOverlay> predicate)
        {
            BaseDef.ForEachOverlayPredicate(o => predicate((ScriptOverlay)o.ScriptObject));
        }
    }
}