using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static FurgosChecklist.GlobalItemSetTooltips;

namespace FurgosChecklist
{
    public class ChecklistSavePlayer : ModPlayer
    {

        public override void LoadData(TagCompound tag)
        {
            ChecklistLines = tag.GetString($"FurgosChecklist.{Player.name}.FCLChecklistLines").ToValue<List<ChecklistLine>>() ?? new List<ChecklistLine>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add($"FurgosChecklist.{Player.name}.FCLChecklistLines", ChecklistLines.ToJson());
        }
    }
}