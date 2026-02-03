using Verse;

namespace MoharHediffs;

public class HediffCompProperties_Filther : HediffCompProperties
{
	public int MinTicksBetweenSprays = 60;

	public int MaxTicksBetweenSprays = 120;

	public ThingDef filthDef = null;

	public bool debug = false;

	public HediffCompProperties_Filther()
	{
		compClass = typeof(HediffComp_Filther);
	}
}
