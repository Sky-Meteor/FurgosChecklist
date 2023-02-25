using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            NeedsRecalculate = true;
        }

        public override void LoadData(TagCompound tag)
        {
            ItemDictToDisplay = JsonToValue<Dictionary<int, Tuple<string, int, bool>>>(tag.GetString($"Instance.{Player.name}.FCLItemDictToDisplay"))?? new Dictionary<int, Tuple<string, int, bool>>();
            TooltipLineToDisplay = tag.GetList<string>($"Instance.{Player.name}.FCLTooltipLineToDisplay").ToList();
            HighlightLines = tag.GetList<string>($"Instance.{Player.name}.FCLHighlightLines").ToList();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add($"Instance.{Player.name}.FCLItemDictToDisplay", ValueToJson(ItemDictToDisplay));
            tag.Add($"Instance.{Player.name}.FCLTooltipLineToDisplay", TooltipLineToDisplay);
            tag.Add($"Instance.{Player.name}.FCLHighlightLines", HighlightLines);
        }

        private static string ValueToJson(object obj) => JsonSerializer.Serialize(obj);

        private static T JsonToValue<T>(string json) => string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }
}