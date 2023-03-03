using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FurgosChecklist
{
    public struct ChecklistLine
    {
        public string Text;
        public ChecklistLineStatus Status;
        public string ItemType;
        public int ItemStack;
        public bool CheckCompletion;

        public ChecklistLine()
        {
            Status = ChecklistLineStatus.Normal;
            ItemType = "";
            ItemStack = -1;
            CheckCompletion = true;
            Text = "";
        }

        public ChecklistLine(string text, ChecklistLineStatus status = ChecklistLineStatus.Normal)
        {
            Status = status;
            ItemType = "";
            ItemStack = -1;
            CheckCompletion = true;
            Text = text;
        }

        public ChecklistLine(string type, int stack, bool checkCompletion, ChecklistLineStatus status = ChecklistLineStatus.Normal)
        {
            Status = status;
            ItemType = type;
            ItemStack = stack;
            CheckCompletion = checkCompletion;
            Text = GetItemText(type, stack);
        }

        public static string GetItemText(string type, int stack)
        {
            if (string.IsNullOrEmpty(type))
                return string.Empty;
            string text = default;
            if (int.TryParse(type, out int typeNum) && typeNum < Main.maxItemTypes)
            {
                text = Lang.GetItemNameValue(typeNum);
            }
            else if (ModContent.TryFind(type, out ModItem modItem))
            {
                text = modItem.DisplayName.GetTranslation(Language.ActiveCulture);
                if (text == modItem.DisplayName.Key)
                    text = "";
            }
            text = $"[i/s{stack}:{type}] {text}";
            return text;
        }
    }
}