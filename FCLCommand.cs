using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static FurgosChecklist.GlobalItemSetTooltips;

namespace FurgosChecklist
{
    public class FCLCommand : ModCommand
    {
        public override string Command => "fcl";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            switch (args.Length)
            {
                case 0:
                {
                    for (int i = 0; i < ItemListToDisplay.Count; i++)
                    {
                        Main.NewText($"{i} [i:{ItemListToDisplay[i]}]");
                    }

                    for (int i = 0; i < TooltipLineToDisplay.Count; i++)
                    {
                        Main.NewText($"{i + ItemListToDisplay.Count} {TooltipLineToDisplay[i]}");
                    }
                    break;
                }
                case 1:
                    Main.NewText("缺少参数", Color.Red);
                    break;
                case 2:
                    switch (args[0])
                    {
                        case var self when self.ToLower() == "add":
                            TooltipLineToDisplay.Add(args[1]);
                            NeedsRecalculateHighlight = true;
                            break;
                        case var self when self.ToLower() == "remove":
                            if (!int.TryParse(args[1], out int index) || index < 0)
                                return;
                            if (index < ItemListToDisplay.Count)
                            {
                                ItemListToDisplay.RemoveAt(index);
                                NeedsRecalculateHighlight = true;
                            }
                            else if (index < ItemListToDisplay.Count + TooltipLineToDisplay.Count)
                            {
                                TooltipLineToDisplay.RemoveAt(index - ItemListToDisplay.Count);
                                NeedsRecalculateHighlight = true;
                            }
                            else
                                Main.NewText(args[1] + "输入错误", Color.Red);
                            break;
                        case var self when self.ToLower() == "additem":
                            if (args[1].Split(":").Length != 2 || !int.TryParse(args[1].Split(":")[1][..^1], out int type) || type <= 0)
                                return;
                            ItemListToDisplay.Add(type < Main.maxItemTypes ? type.ToString() : ModContent.GetModItem(type)?.FullName);
                            NeedsRecalculateHighlight = true;
                            break;
                        case var self when self.ToLower() == "highlight" || self.ToLower() == "hl":
                            if (!int.TryParse(args[1], out int highlightIndex) || highlightIndex < 0 || highlightIndex >= ItemListToDisplay.Count + TooltipLineToDisplay.Count)
                                return;
                            Highlights.Add(highlightIndex);
                            NeedsRecalculateHighlight = true;
                            break;
                        default:
                            Main.NewText(args[0] + "输入错误", Color.Red);
                            break;
                    }
                    break;
                case 3:
                    switch (args[0])
                    {
                        case var self when self.ToLower() == "swap":
                            if (!int.TryParse(args[1], out int index1) || !int.TryParse(args[2], out int index2) || index1 < 0 || index2 < 0)
                                return;
                            if (index1 < ItemListToDisplay.Count && index2 < ItemListToDisplay.Count)
                            {
                                (ItemListToDisplay[index1], ItemListToDisplay[index2]) = (ItemListToDisplay[index2], ItemListToDisplay[index1]);
                            }
                            else if (index1 < ItemListToDisplay.Count + TooltipLineToDisplay.Count && index2 < ItemListToDisplay.Count + TooltipLineToDisplay.Count)
                            {
                                (TooltipLineToDisplay[index1 - ItemListToDisplay.Count], TooltipLineToDisplay[index2 - ItemListToDisplay.Count]) = 
                                (TooltipLineToDisplay[index2 - ItemListToDisplay.Count], TooltipLineToDisplay[index1 - ItemListToDisplay.Count]);
                            }
                            else
                                Main.NewText("输入错误", Color.Red);
                            break;
                        default:
                            Main.NewText(args[0] + "输入错误", Color.Red);
                            break;
                    }
                    break;

                default:
                    Main.NewText(args[0] + "输入错误", Color.Red);
                    break;
            }
        }
    }
}