using Verse;

namespace MoharHediffs;

public class ThingRequirementSettings
{
	public ThingDef thingDef;

	public FloatRange distance = new FloatRange(0f, 300f);

	public bool spawnClose = false;

	public bool spawnInside = false;

	public bool sameFaction = true;

	public bool needsFueled = false;

	public bool needsPowered = false;

	public bool HasThingDef => thingDef != null;

	public bool HasContainerSpawn => HasThingDef && spawnInside;

	public bool HasCustomSpawn => HasThingDef && (spawnClose || spawnInside);
}
