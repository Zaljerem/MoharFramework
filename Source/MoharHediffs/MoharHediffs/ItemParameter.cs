using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class ItemParameter
{
	public ThingDef thingToSpawn = null;

	public PawnKindDef pawnKindToSpawn = null;

	public IntRange spawnCount = new IntRange(1, 1);

	public ThingDef filthDef = null;

	public List<RandomFactionParameter> randomFactionParameters;

	public FloatRange daysB4Next = new FloatRange(1f, 2f);

	public float graceChance = 0f;

	public FloatRange graceDays = new FloatRange(1f, 2f);

	public float weight = 0f;

	public string spawnVerb = "delivery";

	public bool ThingSpawner => thingToSpawn != null && pawnKindToSpawn == null;

	public bool PawnSpawner => thingToSpawn == null && pawnKindToSpawn != null;

	public bool HasFactionParams => !randomFactionParameters.NullOrEmpty();

	public bool HasGraceChance => graceChance != 0f;

	public bool HasFilth => filthDef != null;

	public void LogParams(bool myDebug = false)
	{
		Tools.Warn(string.Concat("ThingSpawner:", ThingSpawner.ToString(), "; ", ThingSpawner ? thingToSpawn.defName : "", "PawnSpawner:", PawnSpawner.ToString(), "; ", PawnSpawner ? pawnKindToSpawn.defName : "", "spawnCount:", spawnCount, "; weight:", weight, "; "), myDebug);
	}

	public void ComputeRandomParameters(out int calculatedTickUntilSpawn, out int calculatedGraceTicks, out int calculatedSpawnCount)
	{
		checked
		{
			calculatedTickUntilSpawn = (int)(daysB4Next.RandomInRange * 60000f);
			calculatedSpawnCount = spawnCount.RandomInRange;
			calculatedGraceTicks = 0;
			if (HasGraceChance && Rand.Chance(graceChance))
			{
				calculatedGraceTicks = (int)(graceDays.RandomInRange * 60000f);
			}
		}
	}
}
