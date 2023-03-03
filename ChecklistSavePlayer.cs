using System.Collections.Generic;
using System.Text.Json;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static FurgosChecklist.GlobalItemSetTooltips;

namespace FurgosChecklist
{
    public class ChecklistSavePlayer : ModPlayer
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { IncludeFields = true };
        public override void LoadData(TagCompound tag)
        {
            ChecklistLines = JsonToValue<List<ChecklistLine>>(tag.GetString($"FurgosChecklist.{Player.name}.FCLChecklistLines")) ?? new List<ChecklistLine>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add($"FurgosChecklist.{Player.name}.FCLChecklistLines", ValueToJson(ChecklistLines));
        }

        private static string ValueToJson(object obj) => JsonSerializer.Serialize(obj, jsonOptions);

        private static T JsonToValue<T>(string json) => string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json, jsonOptions);
    }
}