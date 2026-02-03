using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class TerrainRestriction
{
	public bool allowedInWater = false;

	public FloatRange allowedSnowDepth = new FloatRange(0f, 0.4f);

	public List<TerrainDef> forbiddenTerrains;

	public bool HasForbiddenTerrains => !forbiddenTerrains.NullOrEmpty();

	public bool HasRelevantSnowRestriction => allowedSnowDepth.min != 0f && allowedSnowDepth.min != 1f;
}
