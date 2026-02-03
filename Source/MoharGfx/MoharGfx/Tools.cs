using UnityEngine;
using Verse;

namespace MoharGfx;

public static class Tools
{
	public static Vector3 BetweenTouchingCells(this IntVec3 A, IntVec3 B)
	{
		return A.ToVector3Shifted() + (B - A).ToVector3().normalized * 0.5f;
	}

	public static bool NegligiblePawn(this Pawn pawn)
	{
		int result;
		if (pawn != null && pawn.Spawned && pawn.Map != null)
		{
			_ = pawn.Position;
			result = 0;
		}
		else
		{
			result = 1;
		}
		return (byte)result != 0;
	}

	public static bool NegligibleThing(this Thing thing)
	{
		int result;
		if (thing != null && thing.Spawned && thing.Map != null)
		{
			_ = thing.Position;
			result = 0;
		}
		else
		{
			result = 1;
		}
		return (byte)result != 0;
	}
}
