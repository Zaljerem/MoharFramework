using System.Collections.Generic;
using RimWorld;
using Verse;

namespace OHFP;

public class CompProperties_OHFP_Hatcher : CompProperties
{
	public float hatcherDaystoHatch = 1f;

	public List<PawnKindDef> hatcherPawnList;

	public List<RandomAdoption> randomAdoption;

	public FactionDef forcedFaction = null;

	public bool findRandomMotherIfNull = false;

	public bool findRandomFatherIfNull = false;

	public float manhunterChance = 0f;

	public float newBornChance = 0f;

	public bool debug = false;

	public bool HasForcedFaction => forcedFaction != null;

	public bool IsRandomlyAdopted => !randomAdoption.NullOrEmpty();

	public CompProperties_OHFP_Hatcher()
	{
		compClass = typeof(Comp_OHFP_Hatcher);
	}
}
