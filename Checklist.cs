using Terraria.ModLoader;

namespace FurgosChecklist
{
	public class FurgosChecklist : Mod
    {
        public static FurgosChecklist Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
    }
}