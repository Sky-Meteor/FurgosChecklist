using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FurgosChecklist
{
    public class GlobalItemSetTooltips : GlobalItem
    {
        public static List<string> TooltipLineToDisplay;
        public static Dictionary<int, Tuple<string, int, bool>> ItemDictToDisplay;
        public static List<string> HighlightLines;
        public static bool NeedsRecalculate;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (NeedsRecalculate)
            {
                Recalculate();
                NeedsRecalculate = false;
            }

            if (item.type == ModContent.ItemType<Checklist>())
            {
                foreach (TooltipLine line in GetItemTooltipLine())
                {
                    tooltips.Add(line);
                }
                foreach (TooltipLine line in GetTooltipLine())
                {
                    tooltips.Add(line);
                }
            }
        }



        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (item.type != ModContent.ItemType<Checklist>() || !HighlightLines.Contains(line.Text) || line.Name != "ChecklistTooltip")
                return true;
            Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Main.DiscoColor);
            return false;
        }

        private void Recalculate()
        {
            ItemDictToDisplay = GetRecalculatedItemDict();
        }

        public static List<TooltipLine> GetItemTooltipLine()
        {
            if (ItemDictToDisplay.Count == 0)
                return new List<TooltipLine>();

            List<TooltipLine> returnValue = new List<TooltipLine>();

            for (int i = 0; i < ItemDictToDisplay.Count; i++)
            {
                string text = default;
                if (int.TryParse(ItemDictToDisplay[i].Item1, out int type) && type < Main.maxItemTypes)
                {
                    text = Lang.GetItemNameValue(type);
                }
                else if (ModContent.TryFind(ItemDictToDisplay[i].Item1, out ModItem modItem))
                {
                    text = modItem.DisplayName.GetTranslation(Language.ActiveCulture);
                    if (text == modItem.DisplayName.Key)
                        text = "";
                }
                returnValue.Add(new TooltipLine(FurgosChecklist.Instance, "ChecklistTooltip", $"[i/s{ItemDictToDisplay[i].Item2}:{ItemDictToDisplay[i].Item1}] {text}"));
            }
            
            return returnValue;
        }

        public static List<TooltipLine> GetTooltipLine()
        {
            if (TooltipLineToDisplay.Count == 0)
                return new List<TooltipLine>();

            List<TooltipLine> returnValue = new List<TooltipLine>();

            foreach (string line in TooltipLineToDisplay)
            {
                returnValue.Add(new TooltipLine(FurgosChecklist.Instance, "ChecklistTooltip", line));
            }

            return returnValue;
        }

        private static Dictionary<int, Tuple<string, int, bool>> GetRecalculatedItemDict()
        {
            Dictionary<int, Tuple<string, int, bool>> returnValue = new Dictionary<int, Tuple<string, int, bool>>();
            int i = 0;
            foreach (var item in ItemDictToDisplay)
            {
                returnValue.Add(i, item.Value);
                i++;
            }

            return returnValue;
        }

        public override void Load()
        {
            ItemDictToDisplay = new Dictionary<int, Tuple<string, int, bool>>();
            TooltipLineToDisplay = new List<string>();
            HighlightLines = new List<string>();
        }

        public override void Unload()
        {
            ItemDictToDisplay = null;
            TooltipLineToDisplay = null;
            HighlightLines = null;
        }
    }
}