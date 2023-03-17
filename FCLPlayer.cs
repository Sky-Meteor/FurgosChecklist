using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FurgosChecklist
{
    public partial class FCLPlayer : ModPlayer
    {
        public bool RemoveWhenAddOrInsert;

        public string NeedsOpenChatWithText;
        public int InsertIndex = -1;

        /*public override void OnEnterWorld(Player player)
        {
            NeedsRecalculate = true;
        }*/

        public override void PostUpdateMiscEffects()
        {
            if (NeedsOpenChatWithText != default)
            {
                Main.OpenPlayerChat();
                Main.chatText = NeedsOpenChatWithText;
                NeedsOpenChatWithText = default;
            }

            CheckItemCompletion();
        }

        private void CheckItemCompletion()
        {
            if (ChecklistLines == null)
                return;
            foreach (ChecklistLine line in ChecklistLines.ToList())
            {
                if (string.IsNullOrEmpty(line.ItemType))
                    continue;

                if (!line.CheckCompletion)
                    continue;

                int type = GetType(line.ItemType);
                if (type == -1)
                    continue;

                int stack = 0;
                foreach (Item item in Player.inventory)
                {
                    if (stack >= line.ItemStack)
                    {
                        Main.NewText($"[i/s{line.ItemStack}:{type}] 已完成！", Color.Gold);
                        ChecklistLines.Remove(line);
                        break;
                    }
                    if (item.type == type)
                        stack += item.stack;
                }
            }
        }

        public static int GetType(string input)
        {
            if (int.TryParse(input, out int type) && type < Main.maxItemTypes)
                return type;
            if (ModContent.TryFind(input, out ModItem modItem))
            {
                return modItem.Type;
            }

            return -1;
        }
    }
}