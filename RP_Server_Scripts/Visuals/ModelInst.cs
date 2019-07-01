using System;
using GUC.Animations;
using GUC.Models;
using GUC.Network;
using GUC.Utilities;
using RP_Server_Scripts.VobSystem.Instances;

namespace RP_Server_Scripts.Visuals
{
    public class ModelInst : ExtendedObject, Model.IScriptModel
    {
        public ModelInst(VobInst vob)
        {
            Vob = vob ?? throw new ArgumentNullException(nameof(vob));
        }

        public VobInst Vob { get; }

        public Model BaseInst => Vob.BaseInst.Model;

        public void ApplyOverlay(Overlay overlay)
        {
            ApplyOverlay((ScriptOverlay) overlay.ScriptObject);
        }

        public void RemoveOverlay(Overlay overlay)
        {
            RemoveOverlay((ScriptOverlay) overlay.ScriptObject);
        }

        public void ApplyOverlay(ScriptOverlay overlay)
        {
            BaseInst.ApplyOverlay(overlay.BaseOverlay);
        }

        public void RemoveOverlay(ScriptOverlay overlay)
        {
            BaseInst.RemoveOverlay(overlay.BaseOverlay);
        }

        public bool IsAniActive(ScriptAniJob job)
        {
            return BaseInst.GetActiveAniFromAniJob(job.BaseAniJob) != null;
        }

        public void ForEachActiveAni(Action<ActiveAni> action)
        {
            BaseInst.ForEachActiveAni(action);
        }

        public ActiveAni GetActiveAniFromLayer(int layer)
        {
            return BaseInst.GetActiveAniFromLayerID(layer);
        }

        public bool TryGetAniFromJob(ScriptAniJob aniJob, out ScriptAni ani)
        {
            if (BaseInst.TryGetAniFromJob(aniJob.BaseAniJob, out Animation baseAni))
            {
                ani = (ScriptAni) baseAni.ScriptObject;
                return true;
            }

            ani = null;
            return false;
        }

        public void StartAniJobUncontrolled(AniJob aniJob)
        {
            StartAniJobUncontrolled((ScriptAniJob) aniJob.ScriptObject);
        }

        public void StartAniJobUncontrolled(ScriptAniJob aniJob)
        {
            BaseInst.StartUncontrolledAni(aniJob.BaseAniJob);
        }

        public ActiveAni StartAniJob(AniJob aniJob, float fpsMult, float progress)
        {
            return StartAniJob((ScriptAniJob) aniJob.ScriptObject, fpsMult, progress);
        }

        public void StopAnimation(ActiveAni ani, bool fadeOut)
        {
            BaseInst.StopAnimation(ani, fadeOut);
        }

        public bool IsInAnimation()
        {
            return BaseInst.IsInAnimation();
        }


        public ActiveAni StartAniJob(ScriptAniJob aniJob, Action onEnd)
        {
            return StartAniJob(aniJob, 1, 0, FrameActionPair.OnEnd(onEnd));
        }

        public ActiveAni StartAniJob(ScriptAniJob aniJob, params FrameActionPair[] pairs)
        {
            return StartAniJob(aniJob, 1, 0, pairs);
        }


        public ActiveAni StartAniJob(ScriptAniJob aniJob, float fpsMult = 1, float progress = 0,
            params FrameActionPair[] pairs)
        {
            return BaseInst.StartAniJob(aniJob.BaseAniJob, fpsMult, progress, pairs);
        }


        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }
    }
}