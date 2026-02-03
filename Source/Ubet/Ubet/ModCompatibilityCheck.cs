using System.Linq;
using Verse;

namespace Ubet;

[StaticConstructorOnStartup]
public class ModCompatibilityCheck
{
	private const string alienRacesMod_ModName = "Humanoid Alien Races";

	public static bool AlienRacesIsActive => ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "Humanoid Alien Races");
}
