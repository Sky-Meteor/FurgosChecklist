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
        public static List<string> ItemListToDisplay;
        public static List<int> Highlights;
        public static List<string> HighlightLines;
        public static bool NeedsRecalculateHighlight;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (NeedsRecalculateHighlight)
            {
                RecalculateHighlight();
                NeedsRecalculateHighlight = false;
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

        private void RecalculateHighlight()
        {
            HighlightLines = GetHighlightTooltipLine();
        }

        private List<TooltipLine> GetItemTooltipLine()
        {
            if (ItemListToDisplay.Count == 0)
                return new List<TooltipLine>();

            List<TooltipLine> returnValue = new List<TooltipLine>();

            foreach (string item in ItemListToDisplay)
            {
                string text = default;
                int id = 0;
                if (int.TryParse(item, out int type) && type < Main.maxItemTypes)
                {
                    text = Lang.GetItemNameValue(type);
                    id = type;
                }
                else if (ModContent.TryFind(item, out ModItem modItem))
                {
                    text = modItem.DisplayName.GetTranslation(Language.ActiveCulture);
                    id = modItem.Type;
                }
                returnValue.Add(new TooltipLine(Mod, "ChecklistTooltip", $"[i:{id}] {text}"));
            }

            return returnValue;
        }

        private List<TooltipLine> GetTooltipLine()
        {
            if (TooltipLineToDisplay.Count == 0)
                return new List<TooltipLine>();

            List<TooltipLine> returnValue = new List<TooltipLine>();

            foreach (string line in TooltipLineToDisplay)
            {
                returnValue.Add(new TooltipLine(Mod, "ChecklistTooltip", line));
            }

            return returnValue;
        }

        private List<string> GetHighlightTooltipLine()
        {
            List<string> returnValue = new List<string>();
            if (ItemListToDisplay.Count == 0 && TooltipLineToDisplay.Count == 0)
                return new List<string>();

            foreach (int line in Highlights)
            {
                if (line < ItemListToDisplay.Count)
                    returnValue.Add(GetItemTooltipLine()[line].Text);
                else if (line < ItemListToDisplay.Count + TooltipLineToDisplay.Count)
                    returnValue.Add(GetTooltipLine()[line - ItemListToDisplay.Count].Text);
            }

            return returnValue;
        }

        public override void Load()
        {
            ItemListToDisplay = new List<string>();
            TooltipLineToDisplay = new List<string>();
            Highlights = new List<int>();
            HighlightLines = new List<string>();
        }

        public override void Unload()
        {
            ItemListToDisplay = null;
            TooltipLineToDisplay = null;
            Highlights = null;
            HighlightLines = null;
        }
    }
}