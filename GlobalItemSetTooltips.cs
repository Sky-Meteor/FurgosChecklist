﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FurgosChecklist
{
    public class GlobalItemSetTooltips : GlobalItem
    {
        public static List<ChecklistLine> ChecklistLines;
        public static bool NeedsRecalculate;
        public static bool RemoveWhenAddOrInsert;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<Checklist>())
            {
                if (NeedsRecalculate)
                {
                    Recalculate();
                    NeedsRecalculate = false;
                }

                foreach (ChecklistLine line in ChecklistLines)
                {
                    var tooltipLine = new TooltipLine(Mod, "ChecklistTooltip", line.Text);
                    tooltipLine.OverrideColor = line.Status switch
                    {
                        ChecklistLineStatus.Highlight => Main.DiscoColor,
                        ChecklistLineStatus.ToBeDeleted => Color.Red,
                        _ => null
                    };
                    tooltips.Add(tooltipLine);
                }
            }
        }

        /*public static void RefreshChecklistLines()
        {
            List<ChecklistLine> itemList = new List<ChecklistLine>();
            List<ChecklistLine> textList = new List<ChecklistLine>();

            foreach (ChecklistLine line in ChecklistLines)
            {
                if (!string.IsNullOrEmpty(line.ItemType))
                    itemList.Add(line);
                else
                    textList.Add(line);
            }
            foreach (ChecklistLine line in textList)
            {
                itemList.Add(line);
            }
            ChecklistLines = itemList;
        }*/

        private static void Recalculate()
        {
            //RefreshChecklistLines();
        }

        public override void Load()
        {
            ChecklistLines = new List<ChecklistLine>();
        }

        public override void Unload()
        {
            ChecklistLines = null;
        }
    }
}