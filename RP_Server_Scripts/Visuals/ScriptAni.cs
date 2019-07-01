using System;
using System.Collections;
using System.Collections.Generic;
using GUC.Animations;
using GUC.Network;
using GUC.Utilities;

namespace RP_Server_Scripts.Visuals
{
    public class ScriptAni : ExtendedObject, Animation.IScriptAnimation, IEnumerable<SpecialFrame>
    {
        public ScriptAni()
        {
            BaseAni = new Animation(this);
        }

        public Animation BaseAni { get; }

        /// <summary> Frame speed </summary>
        public float FPS
        {
            get => BaseAni.FPS;
            set => BaseAni.FPS = value;
        }

        public float StartFrame
        {
            get => BaseAni.StartFrame;
            set => BaseAni.StartFrame = value;
        }

        public float EndFrame
        {
            get => BaseAni.EndFrame;
            set => BaseAni.EndFrame = value;
        }

        /// <summary> The overlay number of this animation. </summary>
        public ScriptOverlay Overlay => (ScriptOverlay) BaseAni.Overlay?.ScriptObject;

        public ScriptAniJob AniJob => (ScriptAniJob) BaseAni.AniJob.ScriptObject;

        public bool IsCreated => BaseAni.IsCreated;

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        public ScriptAni(int startFrame, int endFrame) : this()
        {
            StartFrame = startFrame;
            EndFrame = endFrame;
        }

        public void Add(SpecialFrame index, float value)
        {
            SetSpecialFrame(index, value);
        }

        private Dictionary<SpecialFrame, float> _SpecialFrames;

        public bool TryGetSpecialFrame(SpecialFrame index, out float value)
        {
            return _SpecialFrames.TryGetValue(index, out value);
        }

        public void SetSpecialFrame(SpecialFrame index, float value)
        {
            if (_SpecialFrames == null)
            {
                _SpecialFrames = new Dictionary<SpecialFrame, float>(1);
            }
            else if (_SpecialFrames.ContainsKey(index))
            {
                _SpecialFrames[index] = value;
                return;
            }

            _SpecialFrames.Add(index, value);
        }

        public IEnumerator<SpecialFrame> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}