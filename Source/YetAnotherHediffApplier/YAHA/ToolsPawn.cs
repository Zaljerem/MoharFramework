using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace YAHA;

public static class ToolsPawn
{
	public static bool HasHediff(this Pawn pawn, HediffDef hediffDef)
	{
		return pawn.health.hediffSet.HasHediff(hediffDef);
	}

	public static string PawnResumeString(this Pawn pawn)
	{
		return pawn?.LabelShort.CapitalizeFirst() + ", " + (pawn?.ageTracker?.AgeBiologicalYears).Value + " y/o " + pawn?.gender.ToString() + ", curLifeStage: " + pawn?.ageTracker.CurLifeStageRace.minAge + "=>" + pawn?.ageTracker.CurLifeStageRace.def.ToString();
	}

	public static List<BodyPartRecord> GetBP(this Pawn pawn, List<string> BP, bool debug = false)
	{
		IEnumerable<BodyPartRecord> enumerable = from bpr in pawn.health.hediffSet.GetNotMissingParts()
			where BP.Contains(bpr.untranslatedCustomLabel) || BP.Contains(bpr.def.defName)
			select bpr;
		if (enumerable.EnumerableNullOrEmpty())
		{
			if (debug)
			{
				Log.Warning(string.Concat("Cant find BPR with def/label: ", BP, ", skipping"));
			}
			return null;
		}
		return enumerable.ToList();
	}

	public static Season GetSeason(this Pawn pawn)
	{
		if (pawn.Map == null)
		{
			return Season.Undefined;
		}
		return GenLocalDate.Season(pawn.Map);
	}

	public static WeatherDef GetWeather(this Pawn pawn)
	{
		if (pawn.Map == null)
		{
			return WeatherDefOf.Clear;
		}
		return pawn.Map.weatherManager.curWeather;
	}

	public static bool GetOutdoor(this Pawn pawn)
	{
		if (pawn.needs.mood == null)
		{
			return false;
		}
		return pawn.needs.mood.recentMemory.TicksSinceOutdoors == 0;
	}
}
