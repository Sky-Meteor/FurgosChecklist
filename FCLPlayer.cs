using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static FurgosChecklist.GlobalItemSetTooltips;

namespace FurgosChecklist
{
    public class FCLPlayer : ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            NeedsRecalculate = true;
        }

        public override void PostUpdateMiscEffects()
        {
            foreach ((int key, var itemTuple) in ItemDictToDisplay)
            {
                if (!itemTuple.Item3)
                    continue;

                int type = GetType(itemTuple.Item1);
                if (type == -1)
                    continue;
                int stack = 0;
                bool completed = false;
                foreach (Item item in Player.inventory)
                {
                    if (stack >= itemTuple.Item2)
                    {
                        Main.NewText($"[i/s{itemTuple.Item2}:{type}] 已完成！", Color.Gold);
                        ItemDictToDisplay.Remove(key);
                        completed = true;
                        break;
                    }
                    if (item.type == type)
                        stack += item.stack;
                }

                if (completed)
                    continue;

                foreach (Item item in Player.bank4.item)
                {
                    if (stack >= itemTuple.Item2)
                    {
                        Main.NewText($"[i/s{itemTuple.Item2}:{type}] 已完成！", Color.Gold);
                        ItemDictToDisplay.Remove(key);
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