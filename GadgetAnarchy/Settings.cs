using UnityModManagerNet;

namespace GadgetAnarchy
{
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		public readonly string? version = Main.mod?.Info.Version;

		[Draw("Enable logging")]
		public bool isLoggingEnabled =
#if DEBUG
			true;
#else
            false;
#endif

		[Draw("Enable placement space anarchy")]
		public bool spaceAnarchy = true;

		[Draw("Enable drilling location anarchy")]
		public bool drillAnarchy = true;

		[Draw("Enable mounting size anarchy")]
		public bool mountAnarchy = true;

		[Draw("Enable instant drill")]
		public bool instantDrill = true;

		public override void Save(UnityModManager.ModEntry entry)
		{
			Save(this, entry);
		}

		public void OnChange() { }
	}
}
