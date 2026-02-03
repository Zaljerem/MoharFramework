using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class HediffCompProperties_RandySpawner : HediffCompProperties
{
	public List<ItemParameter> itemParameters;

	public int spawnMaxAdjacent = -1;

	public bool spawnForbidden = false;

	public bool hungerRelative = false;

	public bool healthRelative = false;

	public bool logNextSpawn = false;

	public bool debug = false;

	public HediffCompProperties_RandySpawner()
	{
		compClass = typeof(HediffComp_RandySpawner);
	}
}
