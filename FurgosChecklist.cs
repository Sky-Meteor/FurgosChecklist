using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FurgosChecklist
{
	public class Checklist : ModItem
    {
        public override string Texture => "Terraria/Images/UI/UI_quickicon1";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("备忘清单");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}