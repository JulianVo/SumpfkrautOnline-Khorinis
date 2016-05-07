﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUC.Animations;
using GUC.Network;

namespace GUC.Scripts.Sumpfkraut.Visuals
{   
    // create inherited classes for each type?
    public enum SetAnis
    {
        Attack2HFwd1,
        Attack2HFwd2,
        Attack2HFwd3,
        Attack2HFwd4,
        Attack2HLeft,
        Attack2HRight,
        Attack2HRun,

        Attack2HParry,
        Attack2HDodge,



        Attack1HFwd1,
        Attack1HFwd2,
        Attack1HFwd3,
        Attack1HFwd4,
        Attack1HLeft,
        Attack1HRight,
        Attack1HRun,

        Attack1HParry,
        Attack1HDodge,
    }

    public partial class ScriptAniJob : ScriptObject, AniJob.IScriptAniJob
    {
        #region Properties

        public bool IsFightMove { get { return this.ID >= (int)SetAnis.Attack2HFwd1 && this.ID <= (int)SetAnis.Attack1HDodge; } }
        public bool IsAttack { get { return (this.ID >= (int)SetAnis.Attack2HFwd1 && this.ID <= (int)SetAnis.Attack2HRun) || (this.ID >= (int)SetAnis.Attack1HFwd1 && this.ID <= (int)SetAnis.Attack1HRun); } }
        public bool IsCombo { get { return (this.ID >= (int)SetAnis.Attack2HFwd1 && this.ID <= (int)SetAnis.Attack2HFwd4) || (this.ID >= (int)SetAnis.Attack1HFwd1 && this.ID <= (int)SetAnis.Attack1HFwd4); } }

        public bool IsCreated { get { return this.baseAniJob.IsCreated; } }

        AniJob baseAniJob;
        public AniJob BaseAniJob { get { return this.baseAniJob; } }

        public string AniName { get { return this.baseAniJob.Name; } set { this.baseAniJob.Name = value; } }
        
        public ScriptAni DefaultAni { get { return (ScriptAni)this.baseAniJob.DefaultAni.ScriptObject; } }

        public int ID { get { return this.baseAniJob.ID; } set { this.baseAniJob.ID = value; } }

        #endregion

        void ValidateAni(ScriptAni ani)
        {
            if (ani == null)
                throw new Exception("Ani is null!");

            if (this.IsFightMove)
            {
                if (ani.ComboTime > ani.Duration)
                    throw new Exception("ComboTime > Duration");

                if (this.IsAttack)
                {
                    if (ani.HitTime > ani.ComboTime)
                        throw new Exception("HitTime > ComboTime");
                }
            }
        }

        public void SetDefaultAni(Animation ani)
        {
            this.SetDefaultAni((ScriptAni)ani.ScriptObject);
        }

        public void SetDefaultAni(ScriptAni ani)
        {
            ValidateAni(ani);
            this.baseAniJob.SetDefaultAni(ani.BaseAni);
        }

        public void AddOverlayAni(Animation ani, Overlay overlay)
        {
            this.AddOverlayAni((ScriptAni)ani.ScriptObject, (ScriptOverlay)overlay.ScriptObject);
        }

        public void AddOverlayAni(ScriptAni ani, ScriptOverlay ov)
        {
            ValidateAni(ani);
            this.baseAniJob.AddOverlayAni(ani.BaseAni, ov.BaseOverlay);
        }

        public void RemoveOverlayAni(Animation ani)
        {
            this.RemoveOverlayAni((ScriptAni)ani.ScriptObject);
        }

        public void RemoveOverlayAni(ScriptAni ani)
        {
            this.baseAniJob.RemoveOverlayAni(ani.BaseAni);
        }

        #region Constructors

        public ScriptAniJob()
        {
            this.baseAniJob = new AniJob();
            this.baseAniJob.ScriptObject = this;
        }

        #endregion

        #region Read & Write

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        #endregion
    }
}
