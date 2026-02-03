using Verse;

namespace MoharHediffs;

public class HediffCompProperties_RandySpawnUponDeath : HediffCompProperties
{
	public GeneralSettings settings;

	public RedressSettings redressParent;

	public RequirementSettings requirements;

	public IntRange iterationRange = new IntRange(1, 1);

	public bool excludeAlreadyPickedOptions = false;

	public int spawnMaxAdjacent = -1;

	public bool spawnForbidden = false;

	public bool debug = false;

	public bool HasRequirements => requirements != null;

	public bool HasParentRedress => redressParent != null;

	public HediffCompProperties_RandySpawnUponDeath()
	{
		compClass = typeof(HediffComp_RandySpawnUponDeath);
	}
}
