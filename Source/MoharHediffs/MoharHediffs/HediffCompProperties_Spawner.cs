using Verse;

namespace MoharHediffs;

public class HediffCompProperties_Spawner : HediffCompProperties
{
	public ThingDef thingToSpawn;

	public int spawnCount = 1;

	public bool animalThing = false;

	public PawnKindDef animalToSpawn;

	public bool factionOfPlayerAnimal = false;

	public float minDaysB4Next = 1f;

	public float maxDaysB4Next = 2f;

	public float randomGrace = 0f;

	public float graceDays = 0.5f;

	public int spawnMaxAdjacent = -1;

	public bool spawnForbidden = false;

	public bool hungerRelative = false;

	public bool healthRelative = false;

	public bool ageWeightedQuantity = false;

	public bool ageWeightedPeriod = false;

	public bool olderSmallerPeriod = false;

	public bool olderBiggerQuantity = false;

	public bool exponentialQuantity = false;

	public int exponentialRatioLimit = 15;

	public string spawnVerb = "delivery";

	public bool debug = false;

	public HediffCompProperties_Spawner()
	{
		compClass = typeof(HediffComp_Spawner);
	}
}
