using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using static FurgosChecklist.GlobalItemSetTooltips;

namespace FurgosChecklist
{
    public class FCLCommand : ModCommand
    {
        public override string Command => "fcl";

        public override string Usage =>
@"
所有指令都不限制大小写，{}中为相同指令的不同写法，()为必选参数，[]为可选参数，不填写则按照默认值执行，指令中的true/false可以简写为t/f
/fcl  =>  输出带序号的清单条目，可用来确定行数
/fcl {reset|clear}  =>  清空清单
/fcl add (text)  =>  添加一行文字条目
/fcl {remove|rm} (line)  =>  移除指定行，line为整数
/fcl {removes|rms} (line1,line2,line3......)  =>  移除指定的多行，line为整数，用逗号连接
/fcl {insert|ins} (line)  =>  下次添加条目时，将其插入指定行，line为整数
/fcl {insert|ins}  =>  取消插入操作
/fcl {additem|ai} (item) [stack = 1] [checkCompletion = true]  =>  添加一行物品条目，item格式为[i:物品ID]或[i:Mod内部名/Mod物品内部名]，或使用Alt+左键把物品快捷输入聊天栏，堆叠数默认为1，当获得指定数量物品时自动移除此条目
/fcl {additem|ai} (item) (checkCompletion)  =>  添加一行物品条目，item格式为[i:物品ID]或[i:Mod内部名/Mod物品内部名]，或使用Alt+左键把物品快捷输入聊天栏，并设置是否自动移除
/fcl {addhoveritem|addhover|ahi|ah|hi} [stack = 1] [checkCompletion = true]  =>  把鼠标所指物品添加到清单，堆叠数默认为1，当获得指定数量物品时自动移除此条目
/fcl {addhoveritem|addhover|ahi|ah|hi} (checkCompletion)  =>  把鼠标所指物品添加到清单，并设置是否自动移除
/fcl {highlight|hl} (line)  =>  高亮指定行，当指定行已被高亮时取消其高亮，line为整数
/fcl move (lineFrom) (lineTo)  =>  把某行的内容移动到目标行，line为整数
/fcl swap (line1) (line2)  =>  交换两行的内容，不可以在文字行与物品行之间交换，line为整数
/fcl edit (line)  =>  下次添加条目时，将指定行替换为添加的内容，line为整数
/fcl edit  =>  取消编辑操作
/fcl {savecustom|save} (key)  =>  把当前的全部清单条目以特定的key保存到电脑
/fcl {loadcustom|load} (key)  =>  通过特定的key从电脑中加载保存过的清单条目，可以跨人物跨世界加载
/fcl {removecustom|rmc} (key)  =>  通过特定的key移除保存的清单条目
";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            switch (args.Length)
            {
                #region Print lines
                case 0:
                    {
                        int i = 0;
                        Main.NewText("清单行：");
                        foreach (ChecklistLine line in ChecklistLines)
                        {
                            Main.NewText($"{++i} {line.Text}");
                        }
                        Main.NewText("预设：");
                        foreach (string key in _savedChecklist.GetAllKeys())
                        {
                            Main.NewText(key);
                        }
                        break;
                    }
                #endregion
                #region 1 arg
                case 1:
                    switch (args[0].ToLower())
                    {
                        case "addhoveritem":
                        case "addhover":
                        case "ahi":
                        case "ah":
                        case "hi":
                            int hoverItemType = Main.HoverItem.type;
                            if (hoverItemType == 0)
                                return;
                            ChecklistLines.AddOrInsert(new ChecklistLine(hoverItemType < Main.maxItemTypes ? hoverItemType.ToString() : ModContent.GetModItem(hoverItemType)?.FullName, 1, true));
                            break;
                        case "reset":
                        case "clear":
                            ChecklistLines.Clear();
                            break;
                        case "edit":
                            for (int i = 0; i < ChecklistLines.Count; i++)
                            {
                                if (ChecklistLines[i].Status == ChecklistLineStatus.ToBeDeleted)
                                {
                                    SwitchStatus(i, ChecklistLineStatus.ToBeDeleted);
                                    break;
                                }
                            }
                            RemoveWhenAddOrInsert = false;
                            break;
                        case "insert":
                        case "ins":
                            FCLPlayer.InsertIndex = -1;
                            break;
                        default:
                            Main.NewText("指令错误", Color.Red);
                            break;
                    }
                    break;
                #endregion
                #region 2 args
                case 2:
                    switch (args[0].ToLower())
                    {
                        case "add":
                            ChecklistLines.AddOrInsert(new ChecklistLine(args[1]));
                            break;
                        case "remove":
                        case "rm":
                            if (!int.TryParse(args[1], out int index) || index <= 0 || index > ChecklistLines.Count)
                                return;
                            index--;
                            ChecklistLines.RemoveAt(index);
                            break;
                        case "additem":
                        case "ai":
                            string[] splitArg1 = args[1].Split(":");
                            if (splitArg1.Length != 2 || !int.TryParse(splitArg1[1][..^1], out int type) || type <= 0)
                                return;
                            ChecklistLines.AddOrInsert(new ChecklistLine(type < Main.maxItemTypes ? type.ToString() : ModContent.GetModItem(type)?.FullName, 1, true));
                            break;
                        case "addhoveritem":
                        case "addhover":
                        case "ahi":
                        case "ah":
                        case "hi":
                            int hoverItemType = Main.HoverItem.type;
                            if (hoverItemType == 0)
                                return;
                            bool checkCompletion = true;
                            if (!int.TryParse(args[1], out int stack) || stack < 0)
                            {
                                switch (args[1].ToLower())
                                {
                                    case "t":
                                    case "true":
                                        stack = 1;
                                        break;
                                    case "f":
                                    case "false":
                                        checkCompletion = false;
                                        stack = 1;
                                        break;
                                    default:
                                        return;
                                }
                            }
                            ChecklistLines.AddOrInsert(new ChecklistLine(hoverItemType < Main.maxItemTypes ? hoverItemType.ToString() : ModContent.GetModItem(hoverItemType)?.FullName, stack, checkCompletion));
                            break;
                        case "highlight":
                        case "hl":
                            if (!int.TryParse(args[1], out int highlightIndex) || highlightIndex <= 0 || highlightIndex > ChecklistLines.Count)
                                return;
                            highlightIndex--;
                            SwitchStatus(highlightIndex, ChecklistLineStatus.Highlight);
                            break;
                        case "edit":
                            if (!int.TryParse(args[1], out int editIndex) || editIndex <= 0 || editIndex > ChecklistLines.Count)
                                return;
                            for (int i = 0; i < ChecklistLines.Count; i++)
                            {
                                if (ChecklistLines[i].Status == ChecklistLineStatus.ToBeDeleted)
                                {
                                    SwitchStatus(i, ChecklistLineStatus.ToBeDeleted);
                                    break;
                                }
                            }
                            editIndex--;
                            string editText;
                            string command;
                            var line = ChecklistLines[editIndex];
                            if (string.IsNullOrEmpty(line.ItemType))
                            {
                                command = "add";
                                editText = line.Text;
                            }
                            else
                            {
                                command = "additem";
                                editText = $"[i/s{line.ItemStack}:{line.ItemType}]";
                            }
                            SwitchStatus(editIndex, ChecklistLineStatus.ToBeDeleted);
                            RemoveWhenAddOrInsert = true;
                            FCLPlayer.NeedsOpenChatWithText = $"/fcl {command} {editText}";
                            break;
                        case "insert":
                        case "ins":
                            if (!int.TryParse(args[1], out int insIndex) || insIndex <= 0 || insIndex > ChecklistLines.Count)
                                return;
                            insIndex--;
                            FCLPlayer.InsertIndex = insIndex;
                            FCLPlayer.NeedsOpenChatWithText = "/fcl ";
                            break;
                        case "removes":
                        case "rms":
                            var rms = new List<int>();
                            foreach (string rm in args[1].Split(',', '，'))
                            {
                                if (!int.TryParse(rm.Trim(), out int removeIndex) || removeIndex <= 0 || removeIndex > ChecklistLines.Count)
                                    continue;
                                removeIndex--;
                                if (!rms.Contains(removeIndex))
                                    rms.Add(removeIndex);
                            }
                            rms.Sort((index1, index2) => index2.CompareTo(index1));
                            foreach (int removeIndex in rms)
                                ChecklistLines.RemoveAt(removeIndex);
                            break;
                        case "savecustom":
                        case "save":
                            SaveCustom(args[1], ChecklistLines);
                            break;
                        case "loadcustom":
                        case "load":
                            ChecklistLines = LoadCustom(args[1]);
                            break;
                        case "removecustom":
                        case "rmc":
                            RemoveCustom(args[1]);
                            break;
                        default:
                            Main.NewText("指令错误", Color.Red);
                            break;
                    }
                    break;
                #endregion
                #region 3 args
                case 3:
                    switch (args[0].ToLower())
                    {
                        case "swap":
                            if (!int.TryParse(args[1], out int index1) || !int.TryParse(args[2], out int index2) || index1 <= 0 || index2 <= 0 || index1 > ChecklistLines.Count || index2 > ChecklistLines.Count)
                                return;
                            index1--;
                            index2--;
                            (ChecklistLines[index1], ChecklistLines[index2]) = (ChecklistLines[index2], ChecklistLines[index1]);
                            break;
                        case "move":
                            if (!int.TryParse(args[1], out int indexFrom) || !int.TryParse(args[2], out int indexTo) || indexFrom <= 0 || indexTo <= 0 || indexFrom > ChecklistLines.Count || indexTo > ChecklistLines.Count || indexFrom == indexTo)
                                return;
                            indexFrom--;
                            indexTo--;
                            ChecklistLine line = ChecklistLines[indexFrom];
                            ChecklistLines.RemoveAt(indexFrom);
                            ChecklistLines.Insert(indexTo, line);
                            break;
                        case "additem":
                        case "ai":
                            string[] splitArg1 = args[1].Split(":");
                            if (splitArg1.Length != 2 || !int.TryParse(splitArg1[1][..^1], out int type) || type <= 0)
                                return;
                            bool checkCompletionI = true;
                            if (!int.TryParse(args[2], out int stack) || stack < 0)
                            {
                                switch (args[2].ToLower())
                                {
                                    case "t":
                                    case "true":
                                        stack = 1;
                                        break;
                                    case "f":
                                    case "false":
                                        checkCompletionI = false;
                                        stack = 1;
                                        break;
                                    default:
                                        return;
                                }
                            }
                            ChecklistLines.AddOrInsert(new ChecklistLine(type < Main.maxItemTypes ? type.ToString() : ModContent.GetModItem(type)?.FullName, stack, checkCompletionI));
                            break;
                        case "addhoveritem":
                        case "addhover":
                        case "ahi":
                        case "ah":
                        case "hi":
                            if (!int.TryParse(args[1], out int hoverItemStack) || hoverItemStack < 0)
                                return;
                            int hoverItemType = Main.HoverItem.type;
                            if (hoverItemType == 0)
                                return;
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
                            ChecklistLines.AddOrInsert(new ChecklistLine(hoverItemType < Main.maxItemTypes ? hoverItemType.ToString() : ModContent.GetModItem(hoverItemType)?.FullName, hoverItemStack, checkCompletion));
                            break;
                        default:
                            Main.NewText("指令错误", Color.Red);
                            break;
                    }
                    break;
                #endregion
                #region 4 args
                case 4:
                    switch (args[0])
                    {
                        case "additem":
                        case "ai":
                            string[] splitArg1 = args[1].Split(":");
                            if (splitArg1.Length != 2 || !int.TryParse(splitArg1[1][..^1], out int type) || type <= 0)
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

                            ChecklistLines.AddOrInsert(new ChecklistLine(type < Main.maxItemTypes ? type.ToString() : ModContent.GetModItem(type)?.FullName, stack, checkCompletion));
                            break;
                        default:
                            Main.NewText("指令错误", Color.Red);
                            break;
                    }
                    break;
                default:
                    Main.NewText("指令过长", Color.Red);
                    break;
                    #endregion
            }
        }

        public static void SwitchStatus(int index, ChecklistLineStatus status)
        {
            ChecklistLine line = ChecklistLines[index];
            ChecklistLines.RemoveAt(index);
            line.Status = line.Status == status ? ChecklistLineStatus.Normal : status;
            ChecklistLines.Insert(index, line);
        }
#region Preferences
        private static Preferences _savedChecklist;
        private static readonly string path = Path.Combine(Main.SavePath, "ModConfigs", "FurgosChecklist_SavedChecklist.json");
        public override void Load()
        {
            _savedChecklist = new Preferences(path);
            _savedChecklist.Load();
        }

        public override void Unload()
        {
            _savedChecklist.Save();
            _savedChecklist = null;
        }

        private static List<ChecklistLine> LoadCustom(string key) => _savedChecklist.Get(key, string.Empty).ToValue<List<ChecklistLine>>() ?? new List<ChecklistLine>();

        private static void SaveCustom(string key, List<ChecklistLine> obj)
        {
            _savedChecklist.Put(key, obj.ToJson());
            _savedChecklist.Save();
        }

        private static void RemoveCustom(string key)
        {
            FieldInfo dataField = _savedChecklist.GetType().GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic);
            if (dataField?.GetValue(_savedChecklist) is Dictionary<string, object> dict)
            {
                dict.Remove(key);
                dataField.SetValue(_savedChecklist, dict);
            }

            _savedChecklist.Save();
        }
        #endregion
    }

    public static class Util
    {
        public static void AddOrInsert<T>(this List<T> list, T item)
        {
            if (RemoveWhenAddOrInsert && item is ChecklistLine i)
            {
                for (int index = 0; index < ChecklistLines.Count; index++)
                {
                    if (ChecklistLines[index].Status == ChecklistLineStatus.ToBeDeleted)
                    {
                        ChecklistLines.RemoveAt(index);
                        ChecklistLines.Insert(index, i);
                        RemoveWhenAddOrInsert = false;
                        return;
                    }
                }
            }

            if (FCLPlayer.InsertIndex == -1)
                list.Add(item);
            else
            {
                list.Insert(FCLPlayer.InsertIndex, item);
                FCLPlayer.InsertIndex = -1;
            }
        }

        private static readonly JsonSerializerOptions jsonOptions = new() { IncludeFields = true };

        public static T ToValue<T>(this string json) => string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json, jsonOptions);

        public static string ToJson(this object obj) => JsonSerializer.Serialize(obj, jsonOptions);
    }
}