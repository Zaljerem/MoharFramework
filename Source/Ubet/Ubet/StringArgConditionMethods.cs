using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Ubet;

public static class StringArgConditionMethods
{
	public static bool PawnBelongsToLifeStage(this Pawn p, List<string> parameters)
	{
		foreach (string parameter in parameters)
		{
			LifeStageDef named = DefDatabase<LifeStageDef>.GetNamed(parameter);
			if (p.ageTracker.CurLifeStage == named)
			{
				return true;
			}
		}
		return false;
	}

	public static bool RacePawnBelongsToLifeStage(this Pawn p, List<string> parameters)
	{
		if (p.def.race.lifeStageAges == null)
		{
			return false;
		}
		LifeStageAge lifeStageAge = null;
		foreach (LifeStageAge lifeStageAge2 in p.def.race.lifeStageAges)
		{
			if ((float)p.ageTracker.AgeBiologicalYears >= lifeStageAge2.minAge)
			{
				lifeStageAge = lifeStageAge2;
			}
		}
		if (lifeStageAge == null)
		{
			return false;
		}
		return parameters.Contains(lifeStageAge.def.defName);
	}

	public static bool PawnIsPawnKind(this Pawn p, List<string> parameters)
	{
		foreach (string parameter in parameters)
		{
			PawnKindDef named = DefDatabase<PawnKindDef>.GetNamed(parameter);
			if (p.kindDef == named)
			{
				return true;
			}
		}
		return false;
	}

	public static bool PawnIsFromRace(this Pawn p, List<string> parameters)
	{
		return parameters.Contains(p.def.defName);
	}

	public static bool PawnHasTrait(this Pawn p, List<string> parameters)
	{
		foreach (string parameter in parameters)
		{
			TraitDef TD = DefDatabase<TraitDef>.GetNamed(parameter);
			if (p.story.traits.allTraits.Any((Trait t) => t.def == TD))
			{
				return true;
			}
		}
		return false;
	}

	public static bool PawnIsPerformingJob(this Pawn p, List<string> parameters)
	{
		if (p.CurJob == null)
		{
			if (parameters.NullOrEmpty())
			{
				return true;
			}
			return false;
		}
		foreach (string parameter in parameters)
		{
			JobDef named = DefDatabase<JobDef>.GetNamed(parameter);
			if (p.CurJobDef == named)
			{
				return true;
			}
		}
		return false;
	}

	public static bool PawnMapWeather(this Pawn p, List<string> parameters)
	{
		if (p.Map == null)
		{
			return false;
		}
		foreach (string parameter in parameters)
		{
			WeatherDef named = DefDatabase<WeatherDef>.GetNamed(parameter);
			if (p.Map.weatherManager.curWeather == named)
			{
				return true;
			}
		}
		return false;
	}

	public static bool PawnMapSeason(this Pawn p, List<string> parameters)
	{
		if (p.Map == null)
		{
			return false;
		}
		Season season = GenLocalDate.Season(p.Map);
		using (List<string>.Enumerator enumerator = parameters.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case "Spring":
					if (season == Season.Spring)
					{
						return true;
					}
					break;
				case "Summer":
					if (season == Season.Summer)
					{
						return true;
					}
					break;
				case "Fall":
					if (season == Season.Fall)
					{
						return true;
					}
					break;
				case "Winter":
					if (season == Season.Winter)
					{
						return true;
					}
					break;
				case "PermanentSummer":
					if (season == Season.PermanentSummer)
					{
						return true;
					}
					break;
				case "PermanentWinter":
					if (season == Season.PermanentWinter)
					{
						return true;
					}
					break;
				}
			}
		}
		return false;
	}

	public static bool PawnHasBackstory(this Pawn p, List<string> parameters)
	{
		if (p.story == null)
		{
			return false;
		}
		return p.story.AllBackstories.Any((BackstoryDef b) => parameters.Contains(b.identifier));
	}

	public static bool PawnHasRelation(this Pawn p, List<string> parameters, bool alive)
	{
		if (p.relations == null || !p.relations.RelatedToAnyoneOrAnyoneRelatedToMe)
		{
			return false;
		}
		IEnumerable<PawnRelationDef> PRD = DefDatabase<PawnRelationDef>.AllDefs.Where((PawnRelationDef x) => parameters.Contains(x.defName));
		if (PRD.EnumerableNullOrEmpty())
		{
			return false;
		}
		if (p.relations.DirectRelations.Any((DirectPawnRelation r) => PRD.Contains(r.def) && (alive ? (!r.otherPawn.Dead) : r.otherPawn.Dead)))
		{
			return true;
		}
		return false;
	}

	public static bool PawnHasDeadRelation(this Pawn p, List<string> parameters)
	{
		return p.PawnHasRelation(parameters, alive: false);
	}

	public static bool PawnHasAliveRelation(this Pawn p, List<string> parameters)
	{
		return p.PawnHasRelation(parameters, alive: true);
	}

	public static bool PawnHasBodyPart(this Pawn p, List<string> parameters)
	{
		IEnumerable<BodyPartRecord> enumerable = from bpr in p.health.hediffSet.GetNotMissingParts()
			where parameters.Contains(bpr.untranslatedCustomLabel) || parameters.Contains(bpr.def.defName)
			select bpr;
		return !enumerable.EnumerableNullOrEmpty();
	}

	public static bool ThingIsMadeOfStuff(this Thing t, List<string> parameters)
	{
		return t.def.MadeFromStuff && t.Stuff != null && parameters.Contains(t.Stuff.defName);
	}

	public static bool ThingHasIngredient(this Thing t, List<string> parameters)
	{
		return !t.def.costList.NullOrEmpty() && t.def.costList.Any((ThingDefCountClass b) => parameters.Contains(b.thingDef.defName));
	}

	public static bool PawnWearsApparelMadeOf(this Pawn p, List<string> parameters)
	{
		if (p.apparel == null || p.apparel.WornApparelCount == 0)
		{
			return false;
		}
		return p.apparel.WornApparel.Any((Apparel a) => a.ThingIsMadeOfStuff(parameters) || a.ThingHasIngredient(parameters));
	}

	public static bool PawnUsesWeaponMadeOf(this Pawn p, List<string> parameters)
	{
		if (p.equipment == null || p.equipment.Primary.DestroyedOrNull())
		{
			return false;
		}
		ThingWithComps primary = p.equipment.Primary;
		return primary.ThingIsMadeOfStuff(parameters) || primary.ThingHasIngredient(parameters);
	}

	public static bool PawnUsesWeapon(this Pawn p, List<string> parameters)
	{
		if (p.equipment == null || p.equipment.Primary.DestroyedOrNull())
		{
			return false;
		}
		ThingWithComps primary = p.equipment.Primary;
		return parameters.Contains(primary.def.defName);
	}

	public static bool PawnUsesApparel(this Pawn p, List<string> parameters)
	{
		if (p.apparel == null || p.apparel.WornApparelCount == 0)
		{
			return false;
		}
		return p.apparel.WornApparel.Any((Apparel a) => parameters.Contains(a.def.defName));
	}

	public static bool PawnUsesWeaponContains(this Pawn p, List<string> parameters)
	{
		if (p.equipment == null || p.equipment.Primary.DestroyedOrNull())
		{
			return false;
		}
		foreach (string parameter in parameters)
		{
			if (p.equipment.Primary.def.defName.Contains(parameter))
			{
				return true;
			}
		}
		return false;
	}

	public static bool PawnIsInSpecificMentalState(this Pawn p, List<string> MentalStateDefName)
	{
		if (p.MentalState == null)
		{
			return false;
		}
		return MentalStateDefName.Contains(p.MentalStateDef.defName);
	}

	public static bool PawnHasAnyHediff(this Pawn p, List<string> Hediff)
	{
		if (p.health.hediffSet.hediffs.NullOrEmpty())
		{
			return false;
		}
		return p.health.hediffSet.hediffs.Any((Hediff h) => Hediff.Contains(h.def.defName));
	}

	public static bool PawnHasAllHediffs(this Pawn p, List<string> Hediff)
	{
		if (p.health.hediffSet.hediffs.NullOrEmpty())
		{
			return false;
		}
		bool flag = true;
		foreach (string hStr in Hediff)
		{
			flag &= p.health.hediffSet.hediffs.Any((Hediff h) => hStr == h.def.defName);
			if (!flag)
			{
				return false;
			}
		}
		return flag;
	}

	public static bool PawnHasAllHediffsOnBodyParts(this Pawn p, List<string> Hediff, List<string> BodyPart)
	{
		if (p.health.hediffSet.hediffs.NullOrEmpty())
		{
			return false;
		}
		if (Hediff.NullOrEmpty() || BodyPart.NullOrEmpty())
		{
			return false;
		}
		if (Hediff.Count != BodyPart.Count)
		{
			return false;
		}
		bool flag = true;
		for (int i = 0; i < Hediff.Count; i++)
		{
			string hStr = Hediff[i];
			string bpStr = BodyPart[i];
			flag &= p.health.hediffSet.hediffs.Any((Hediff h) => hStr == h.def.defName && (bpStr.NullOrEmpty() ? (h.Part == null) : (h.Part.untranslatedCustomLabel == bpStr || h.Part.def.defName == bpStr)));
			if (!flag)
			{
				return false;
			}
		}
		return flag;
	}
}
