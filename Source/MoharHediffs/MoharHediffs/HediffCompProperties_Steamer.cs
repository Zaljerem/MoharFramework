using Verse;

namespace MoharHediffs;

public class HediffCompProperties_Steamer : HediffCompProperties
{
	public int MinTicksBetweenSprays = 200;

	public int MaxTicksBetweenSprays = 400;

	public int MinSprayDuration = 60;

	public int MaxSprayDuration = 120;

	public float puffingChance = 1f;

	public float temperatureIncreasePerPuff = 0.5f;

	public bool debug = false;

	public HediffCompProperties_Steamer()
	{
		compClass = typeof(HediffComp_Steamer);
	}
}
