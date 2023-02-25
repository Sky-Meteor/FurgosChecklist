using System;
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
                        for (int i = 0; i < ItemDictToDisplay.Count; i++)
                        {
                            Main.NewText($"{i} [i:{ItemDictToDisplay[i].Item1}]");
                        }

                        for (int i = 0; i < TooltipLineToDisplay.Count; i++)
                        {
                            Main.NewText($"{i + ItemDictToDisplay.Count} {TooltipLineToDisplay[i]}");
                        }
                        break;
                    }
                case 1:
                    switch (args[0].ToLower())
                    {
                        case "addhoveritem":
                        case "addhover":
                            int hoverItemType = Main.HoverItem.type;
                            ItemDictToDisplay.Add(ItemDictToDisplay.Count, new Tuple<string, int, bool>((hoverItemType < Main.maxItemTypes ? hoverItemType.ToString() : ModContent.GetModItem(hoverItemType)?.FullName), 1, true));
                            NeedsRecalculate = true;
                            break;
                        case "reset":
                        case "clear":
                            ItemDictToDisplay.Clear();
                            TooltipLineToDisplay.Clear();
                            HighlightLines.Clear();
                            NeedsRecalculate = true;
                            break;
                        default:
                            Main.NewText("缺少参数", Color.Red);
                            break;
                    }
                    break;
                case 2:
                    switch (args[0].ToLower())
                    {
                        case "add":
                            TooltipLineToDisplay.Add(args[1]);
                            NeedsRecalculate = true;
                            break;
                        case "remove":
                            if (!int.TryParse(args[1], out int index) || index < 0)
                                return;
                            if (index < ItemDictToDisplay.Count)
                            {
                                HighlightLines.Remove(GetItemTooltipLine()[index].Text);
                                ItemDictToDisplay.Remove(index);
                                NeedsRecalculate = true;
                            }
                            else if (index < ItemDictToDisplay.Count + TooltipLineToDisplay.Count)
                            {
                                HighlightLines.Remove(GetTooltipLine()[index - ItemDictToDisplay.Count].Text);
                                TooltipLineToDisplay.RemoveAt(index - ItemDictToDisplay.Count);
                                NeedsRecalculate = true;
                            }
                            else
                                Main.NewText(args[1] + "输入错误", Color.Red);
                            break;
                        case "additem":
                            if (args[1].Split(":").Length != 2 || !int.TryParse(args[1].Split(":")[1][..^1], out int type) || type <= 0)
                                return;
                            ItemDictToDisplay.Add(ItemDictToDisplay.Count, new Tuple<string, int, bool>(type < Main.maxItemTypes ? type.ToString() : ModContent.GetModItem(type)?.FullName, 1, true));
                            NeedsRecalculate = true;
                            break;
                        case "addhoveritem":
                        case "addhover":
                            if (!int.TryParse(args[1], out int stack) || stack < 0)
                                return;
                            int hoverItemType = Main.HoverItem.type;
                            ItemDictToDisplay.Add(ItemDictToDisplay.Count, new Tuple<string, int, bool>(hoverItemType < Main.maxItemTypes ? hoverItemType.ToString() : ModContent.GetModItem(hoverItemType)?.FullName, stack, true));
                            NeedsRecalculate = true;
                            break;
                        case "highlight":
                        case "hl":
                            if (!int.TryParse(args[1], out int highlightIndex) || highlightIndex < 0 || highlightIndex >= ItemDictToDisplay.Count + TooltipLineToDisplay.Count)
                                return;
                            if (highlightIndex < ItemDictToDisplay.Count)
                                HighlightLines.Add(GetItemTooltipLine()[highlightIndex].Text);
                            else if (highlightIndex < ItemDictToDisplay.Count + TooltipLineToDisplay.Count)
                                HighlightLines.Add(GetTooltipLine()[highlightIndex - ItemDictToDisplay.Count].Text);
                            NeedsRecalculate = true;
                            break;
                        default:
                            Main.NewText(args[0] + "输入错误", Color.Red);
                            break;
                    }
                    break;
                case 3:
                    switch (args[0].ToLower())
                    {
                        case "swap":
                            if (!int.TryParse(args[1], out int index1) || !int.TryParse(args[2], out int index2) || index1 < 0 || index2 < 0 || (index1 >= ItemDictToDisplay.Count && index2 < ItemDictToDisplay.Count) || (index2 >= ItemDictToDisplay.Count && index1 < ItemDictToDisplay.Count))
                                return;
                            if (index1 < ItemDictToDisplay.Count && index2 < ItemDictToDisplay.Count)
                            {
                                (ItemDictToDisplay[index1], ItemDictToDisplay[index2]) = (ItemDictToDisplay[index2], ItemDictToDisplay[index1]);
                                NeedsRecalculate = true;
                            }
                            else if (index1 < ItemDictToDisplay.Count + TooltipLineToDisplay.Count && index2 < ItemDictToDisplay.Count + TooltipLineToDisplay.Count)
                            {
                                (TooltipLineToDisplay[index1 - ItemDictToDisplay.Count], TooltipLineToDisplay[index2 - ItemDictToDisplay.Count]) =
                                (TooltipLineToDisplay[index2 - ItemDictToDisplay.Count], TooltipLineToDisplay[index1 - ItemDictToDisplay.Count]);
                                NeedsRecalculate = true;
                            }
                            else
                                Main.NewText("输入错误", Color.Red);
                            break;
                        case "additem":
                            if (args[1].Split(":").Length != 2 || !int.TryParse(args[1].Split(":")[1][..^1], out int type) || type <= 0)
                                return;
                            if (!int.TryParse(args[2], out int stack) || stack < 0)
                                return;
                            ItemDictToDisplay.Add(ItemDictToDisplay.Count, new Tuple<string, int, bool>(type < Main.maxItemTypes ? type.ToString() : ModContent.GetModItem(type)?.FullName, stack, true));
                            NeedsRecalculate = true;
                            break;
                        case "addhoveritem":
                        case "addhover":
                            if (!int.TryParse(args[1], out int hoverItemStack) || hoverItemStack < 0)
                                return;
                            int hoverItemType = Main.HoverItem.type;
                            bool checkCompletion;
                            switch (args[2].ToLower())
                            {
                                case "t":
                                case "true":
                                    checkCompletion = true;
                                    break;
                                case "f":
                                case "false":
                                    checkCompletion = false;
                                    break;
                                default:
                                    return;
                            }
                            ItemDictToDisplay.Add(ItemDictToDisplay.Count, new Tuple<string, int, bool>(hoverItemType < Main.maxItemTypes ? hoverItemType.ToString() : ModContent.GetModItem(hoverItemType)?.FullName, hoverItemStack, checkCompletion));
                            NeedsRecalculate = true;
                            break;
                        default:
                            Main.NewText(args[0] + "输入错误", Color.Red);
                            break;
                    }
                    break;
                case 4:
                    switch (args[0])
                    {
                        case "additem":
                            if (args[1].Split(":").Length != 2 || !int.TryParse(args[1].Split(":")[1][..^1], out int type) || type <= 0)
                                return;
                            if (!int.TryParse(args[2], out int stack) || stack < 0)
                                return;
                            bool checkCompletion;
                            switch (args[3].ToLower())
                            {
                                case "t":
                                case "true":
                                    checkCompletion = true;
                                    break;
                                case "f":
                                case "false":
                                    checkCompletion = false;
                                    break;
                                default:
                                    return;
                            }

                            ItemDictToDisplay.Add(ItemDictToDisplay.Count, new Tuple<string, int, bool>(type < Main.maxItemTypes ? type.ToString() : ModContent.GetModItem(type)?.FullName, stack, checkCompletion));
                            NeedsRecalculate = true;
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