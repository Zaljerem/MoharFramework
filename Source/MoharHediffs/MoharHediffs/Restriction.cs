using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharHediffs;

public class Restriction
{
	public bool onlyWhenMoving = true;

	public List<PawnPosture> allowedPostures;

	public TerrainRestriction terrain;

	public bool HasTerrainRestriction => terrain != null;

	public bool HasPostureRestriction => !allowedPostures.NullOrEmpty();
}
