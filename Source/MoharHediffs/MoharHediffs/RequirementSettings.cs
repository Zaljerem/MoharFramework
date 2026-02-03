using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class RequirementSettings
{
	public List<HediffRequirementSettings> hediff;

	public List<ThingRequirementSettings> thing;

	public bool HasHediffRequirement => !hediff.NullOrEmpty() && hediff.Any((HediffRequirementSettings h) => h.HasHediffDef);

	public bool HasThingRequirement => !thing.NullOrEmpty() && thing.Any((ThingRequirementSettings t) => t.HasThingDef);

	public bool HasAtLeastOneRequirementSetting => HasHediffRequirement || HasThingRequirement;

	public bool HasContainerSpawn => HasThingRequirement && thing.Any((ThingRequirementSettings t) => t.HasContainerSpawn);
}
