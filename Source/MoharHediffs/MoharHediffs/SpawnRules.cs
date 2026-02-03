using Verse;

namespace MoharHediffs;

public class SpawnRules
{
	public int spawnedMax = 2;

	public IntRange period = new IntRange(15, 25);

	public bool IsUnlimited => spawnedMax <= 0;
}
