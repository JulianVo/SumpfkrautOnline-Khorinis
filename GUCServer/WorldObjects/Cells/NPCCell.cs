﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Types;

namespace GUC.WorldObjects.Cells
{
    class NPCCell : WorldCell
    {
        public readonly DynamicCollection<NPC> npcs = new DynamicCollection<NPC>();

        public NPCCell(World world, int x, int y) : base(world, x, y)
        {
        }

        public const int Size = 3000;
        public static int[] GetCoords(Vec3f pos)
        {
            return WorldCell.GetCoords(pos, Size);
        }
    }
}
