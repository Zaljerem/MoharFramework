using Verse;

namespace DUDOD;

public static class Tools
{
	public static bool NegligeablePawn(this Pawn pawn)
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
}
