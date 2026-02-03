using Verse;

namespace OHFP;

public static class Tools
{
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

	public static void Warn(string warning, bool debug = false)
	{
		if (debug)
		{
			Log.Warning(warning);
		}
	}
}
