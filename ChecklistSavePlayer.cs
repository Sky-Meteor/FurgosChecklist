using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static FurgosChecklist.GlobalItemSetTooltips;

namespace FurgosChecklist
{
    public class ChecklistSavePlayer : ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            GlobalItemSetTooltips.NeedsRecalculateHighlight = true;
        }

        public override void LoadData(TagCompound tag)
        {
            ItemListToDisplay = tag.GetList<string>($"FurgosChecklist.{Player.name}.FCLItemListToDisplay").ToList();
            TooltipLineToDisplay = tag.GetList<string>($"FurgosChecklist.{Player.name}.FCLTooltipLineToDisplay").ToList();
            Highlights = tag.GetList<int>($"FurgosChecklist.{Player.name}.FCLHighlights").ToList();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add($"FurgosChecklist.{Player.name}.FCLItemListToDisplay", ItemListToDisplay);
            tag.Add($"FurgosChecklist.{Player.name}.FCLTooltipLineToDisplay", TooltipLineToDisplay);
            tag.Add($"FurgosChecklist.{Player.name}.FCLHighlights", Highlights);
        }
    }
}