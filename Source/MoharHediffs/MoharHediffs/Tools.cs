using System;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class Tools
{
	public static float Clamp(this float value, float min, float max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	public static void DestroyParentHediff(Hediff parentHediff, bool debug = false)
	{
		if (parentHediff.pawn != null && parentHediff.def.defName != null && debug)
		{
			Log.Warning(parentHediff.pawn.Label + "'s Hediff: " + parentHediff.def.defName + " says goodbye.");
		}
		parentHediff.Severity = 0f;
	}

	public static void DestroyHediff(this Pawn pawn, Hediff hediff, bool debug = false)
	{
		if (hediff.pawn != null && hediff.def.defName != null && debug)
		{
			Log.Warning(hediff.pawn.Label + "'s Hediff: " + hediff.def.defName + " says goodbye.");
		}
		pawn.health.RemoveHediff(hediff);
	}

	public static float GetPawnAgeOverlifeExpectancyRatio(Pawn pawn, bool debug = false)
	{
		float result = 1f;
		if (pawn == null)
		{
			if (debug)
			{
				Log.Warning("GetPawnAgeOverlifeExpectancyRatio pawn NOT OK");
			}
			return result;
		}
		result = pawn.ageTracker.AgeBiologicalYearsFloat / pawn.RaceProps.lifeExpectancy;
		if (debug)
		{
			Log.Warning(pawn.Label + " Age: " + pawn.ageTracker.AgeBiologicalYearsFloat + "; lifeExpectancy: " + pawn.RaceProps.lifeExpectancy + "; ratio:" + result);
		}
		return result;
	}

	public static float GetPawnAdultRatio(Pawn pawn, bool debug = false)
	{
		float result = 1f;
		if (!OkPawn(pawn))
		{
			if (debug)
			{
				Log.Warning("GetPawnAgeOverlifeExpectancyRatio pawn NOT OK");
			}
			return result;
		}
		return (pawn.ageTracker.AgeBiologicalYearsFloat - pawn.RaceProps.lifeStageAges.Last().minAge) / (pawn.RaceProps.lifeExpectancy - pawn.RaceProps.lifeStageAges.Last().minAge);
	}

	public static bool IsInjured(this Pawn pawn, bool debug = false)
	{
		if (pawn == null)
		{
			if (debug)
			{
				Log.Warning("pawn is null - wounded ");
			}
			return false;
		}
		float num = 0f;
		List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
		for (int i = 0; i < hediffs.Count; i = checked(i + 1))
		{
			if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
			{
				num += hediffs[i].Severity;
			}
		}
		if (debug && num > 0f)
		{
			Log.Warning(pawn.Label + " is wounded ");
		}
		return num > 0f;
	}

	public static bool IsHungry(this Pawn pawn, bool debug = false)
	{
		if (pawn == null)
		{
			if (debug)
			{
				Log.Warning("pawn is null - IsHungry ");
			}
			return false;
		}
		bool flag = pawn.needs.food != null && pawn.needs.food.CurCategory == HungerCategory.Starving;
		if (debug && flag)
		{
			Log.Warning(pawn.Label + " is hungry ");
		}
		return flag;
	}

	public static bool Negligible(this Pawn p)
	{
		int result;
		if (p != null && p.Spawned && p.Map != null)
		{
			_ = p.Position;
			result = 0;
		}
		else
		{
			result = 1;
		}
		return (byte)result != 0;
	}

	public static bool Negligible(this Thing thing)
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

	public static bool OkPawn(Pawn pawn)
	{
		return pawn != null && pawn.Map != null;
	}

	public static void Warn(string warning, bool debug = false)
	{
		if (debug)
		{
			Log.Warning(warning);
		}
	}

	public static bool IsAlien(this Pawn pawn)
	{
		return pawn.def.race.Humanlike && pawn.def != ThingDefOf.Human;
	}

	public static bool IsHuman(this Pawn pawn)
	{
		return pawn.def.race.Humanlike && pawn.def == ThingDefOf.Human;
	}

	public static AlienPartGenerator.AlienComp GetAlien(Pawn pawn = null)
	{
		AlienPartGenerator.AlienComp alienComp = null;
		return pawn?.TryGetComp<AlienPartGenerator.AlienComp>();
	}

	public static bool ForbiddenMote(this Vector3 loc, Map map)
	{
		if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
		{
			return true;
		}
		return false;
	}
}
