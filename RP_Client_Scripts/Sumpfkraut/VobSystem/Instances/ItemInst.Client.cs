﻿namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{
    public partial class ItemInst
    {
        partial void pSpawn()
        {
            var gVob = this.BaseInst.gVob;
            gVob.Name.Set(this.Name);
            gVob.Material = (int)this.Material;
        }
    }
}
