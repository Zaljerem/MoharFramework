using System.Collections.Generic;
using Verse;

namespace YAHA;

public class HediffCompProperties_YetAnotherHediffApplier : HediffCompProperties
{
	public List<HediffAssociation> associations;

	public int checkFrequency = 1800;

	public int UnspawnedGrace = 3000;

	public bool debug;

	public int UpdateNumthisTickLimit = 10;

	public bool PeriodicCheck => checkFrequency > 0;

	public HediffCompProperties_YetAnotherHediffApplier()
	{
		compClass = typeof(HediffComp_YetAnotherHediffApplier);
	}
}
