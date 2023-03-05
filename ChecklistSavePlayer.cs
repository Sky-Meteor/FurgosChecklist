using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace FurgosChecklist
{
    public partial class FCLPlayer
    {
        public List<ChecklistLine> ChecklistLines;
        public override void LoadData(TagCompound tag)
        {
            ChecklistLines = tag.GetString($"FurgosChecklist.{Player.name}.FCLChecklistLines").ToValue<List<ChecklistLine>>() ?? new List<ChecklistLine>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add($"FurgosChecklist.{Player.name}.FCLChecklistLines", ChecklistLines.ToJson());
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