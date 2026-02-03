using System;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class PawnCopyUtils
{
	public static void SetAge(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		ThingSettings chosenItem = comp.ChosenItem;
		checked
		{
			if (chosenItem.IsCopier && chosenItem.copyParent.age)
			{
				LifeStageDef LSDef = comp.Pawn.ageTracker.CurLifeStage;
				LifeStageAge lifeStageAge = comp.Pawn.def.race.lifeStageAges.Where((LifeStageAge LS) => LS.def == LSDef).FirstOrFallback();
				if (lifeStageAge != null)
				{
					newPawn.ageTracker.AgeBiologicalTicks = (long)(lifeStageAge.minAge * (float)MyDefs.OneYearTicks);
					newPawn.ageTracker.AgeChronologicalTicks = Math.Max(comp.Pawn.ageTracker.AgeBiologicalTicks, comp.Pawn.ageTracker.AgeChronologicalTicks);
				}
			}
			else
			{
				newPawn.ageTracker.AgeBiologicalTicks = MyDefs.OneYearTicks * chosenItem.biologicalAgeRange.RandomInRange;
				newPawn.ageTracker.AgeChronologicalTicks = MyDefs.OneYearTicks * chosenItem.chronologicalAgeRange.RandomInRange + newPawn.ageTracker.AgeBiologicalTicks;
			}
		}
	}

	public static void SetName(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.ChosenItem.copyParent.name)
		{
			newPawn.Name = comp.Pawn.Name;
		}
	}

	public static void SetGender(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.ChosenItem.copyParent.gender)
		{
			newPawn.gender = comp.Pawn.gender;
		}
	}

	public static void SetMelanin(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.ChosenItem.copyParent.melanin)
		{
			newPawn.story.SkinColorBase = comp.Pawn.story.SkinColorBase;
		}
	}

	public static void SetAlienSkinColor(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		AlienPartGenerator.AlienComp alienComp = comp.Pawn.TryGetComp<AlienPartGenerator.AlienComp>();
		AlienPartGenerator.AlienComp alienComp2 = newPawn?.TryGetComp<AlienPartGenerator.AlienComp>();
		if (alienComp != null && alienComp2 != null)
		{
			Color first = alienComp.GetChannel("skin").first;
			Color second = alienComp.GetChannel("skin").second;
			alienComp2.GetChannel("skin").first = first;
			alienComp2.GetChannel("skin").second = second;
		}
	}

	public static void SetAlienBodyAndHeadType(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.Pawn.IsAlien() && newPawn.IsAlien())
		{
			AlienPartGenerator.AlienComp alienComp = comp.Pawn.TryGetComp<AlienPartGenerator.AlienComp>();
			AlienPartGenerator.AlienComp alienComp2 = newPawn?.TryGetComp<AlienPartGenerator.AlienComp>();
			if (alienComp != null && alienComp2 != null)
			{
				newPawn.story.headType = comp.Pawn.story.headType;
				alienComp2.headMaskVariant = alienComp.headMaskVariant;
				alienComp2.headVariant = alienComp.headVariant;
				newPawn.story.bodyType = comp.Pawn.story.bodyType;
				alienComp2.bodyMaskVariant = alienComp.bodyMaskVariant;
				alienComp2.bodyVariant = alienComp.bodyVariant;
			}
		}
	}

	public static void SetHair(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.ChosenItem.copyParent.hair)
		{
			newPawn.story.hairDef = comp.Pawn.story.hairDef;
		}
	}

	public static void SetHairColor(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.ChosenItem.copyParent.hairColor)
		{
			newPawn.story.HairColor = comp.Pawn.story.HairColor;
		}
	}

	public static void SetHediff(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.ChosenItem.copyParent.hediff)
		{
			newPawn.health.hediffSet.hediffs = new List<Hediff>();
			List<Hediff> hediffs = (comp.ChosenItem.copyParent.HasHediffExclusion ? (from h in comp.Pawn.health.hediffSet.hediffs.ListFullCopy()
				where !comp.ChosenItem.copyParent.excludeHediff.Contains(h.def) && (!comp.ChosenItem.copyParent.excludeTendableHediffs || !h.def.tendable) && (!comp.ChosenItem.copyParent.excludePermanentHediffs || h.TryGetComp<HediffComp_GetsPermanent>() == null)
				select h).ToList() : comp.Pawn.health.hediffSet.hediffs.ListFullCopy());
			newPawn.health.hediffSet.hediffs = hediffs;
		}
	}

	public static void SetSkills(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, bool debug = false)
	{
		if (!comp.ChosenItem.copyParent.skills)
		{
			return;
		}
		string text = newPawn.LabelShort + " - SetSkills - ";
		checked
		{
			for (int i = 0; i < newPawn.skills.skills.Count; i++)
			{
				float randomInRange = comp.ChosenItem.copyParent.skillDecay.RandomInRange;
				int num = (int)((float)comp.Pawn.skills.skills[i].levelInt * randomInRange);
				if (debug)
				{
					Log.Warning(text + " browsing " + comp.Pawn.skills.skills[i].def.defName + " ori: " + comp.Pawn.skills.skills[i].levelInt + " new: " + newPawn.skills.skills[i].levelInt + " decayRatio: " + randomInRange + " wantedSkill: " + num);
				}
				if (num > newPawn.skills.skills[i].levelInt)
				{
					if (debug)
					{
						Log.Warning(text + "Calling gainskill");
					}
					comp.GainSkill(newPawn, num, i, debug);
				}
				else if (num < newPawn.skills.skills[i].levelInt)
				{
					if (debug)
					{
						Log.Warning(text + "Calling loseskill");
					}
					comp.LoseSkill(newPawn, num, i, debug);
				}
				if (debug)
				{
					Log.Warning(text + " copied skill [" + i + "]:" + comp.Pawn.skills.skills[i].def.defName + " new: " + newPawn.skills.skills[i].levelInt);
				}
			}
		}
	}

	public static void GainSkill(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, int wantedLevel, int index, bool debug = false)
	{
		string text = newPawn.LabelShort + " - GainSkill - ";
		if (debug)
		{
			Log.Warning(text + "Entering");
		}
		int num = 20;
		while (wantedLevel > newPawn.skills.skills[index].levelInt && num > 0)
		{
			float xpRequiredForLevelUp = newPawn.skills.skills[index].XpRequiredForLevelUp;
			if (debug)
			{
				Log.Warning(text + " loop: " + num + " xpInjected: " + xpRequiredForLevelUp + " ori: " + comp.Pawn.skills.skills[index].levelInt + " new: " + newPawn.skills.skills[index].levelInt);
			}
			newPawn.skills.skills[index].Learn(xpRequiredForLevelUp, direct: true);
			num = checked(num - 1);
		}
	}

	public static void LoseSkill(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, int wantedLevel, int index, bool debug = false)
	{
		string text = newPawn.LabelShort + " - LoseSkill - ";
		if (debug)
		{
			Log.Warning(text + "Entering");
		}
		int num = 20;
		checked
		{
			while (wantedLevel < newPawn.skills.skills[index].levelInt && num > 0)
			{
				float num2 = -(newPawn.skills.skills[index].levelInt * 1000);
				if (debug)
				{
					Log.Warning(text + " loop: " + num + " xpInjected: " + num2 + " ori: " + comp.Pawn.skills.skills[index].levelInt + " new: " + newPawn.skills.skills[index].levelInt);
				}
				newPawn.skills.skills[index].Learn(num2, direct: true);
				num--;
			}
		}
	}

	public static void SetPassions(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, bool debug = false)
	{
		if (comp.ChosenItem.copyParent.passions)
		{
			for (int i = 0; i < newPawn.skills.skills.Count; i = checked(i + 1))
			{
				newPawn.skills.skills[i].passion = comp.Pawn.skills.skills[i].passion;
			}
		}
	}

	public static void InitRememberBackstories(out BackstoryDef childBS, out BackstoryDef adultBS)
	{
		childBS = (adultBS = null);
	}

	public static void ResetBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		newPawn.story.Childhood = null;
		newPawn.story.Adulthood = null;
	}

	public static void RememberBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, out BackstoryDef childBS, out BackstoryDef adultBS)
	{
		childBS = newPawn.story.Childhood;
		adultBS = newPawn.story.Adulthood;
	}

	public static void ReinjectBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, BackstoryDef childBS, BackstoryDef adultBS)
	{
		newPawn.story.Childhood = childBS;
		newPawn.story.Adulthood = adultBS;
	}

	public static void SetBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (comp.ChosenItem.copyParent.childBS)
		{
			newPawn.story.Childhood = comp.Pawn.story.Childhood;
		}
		if (comp.ChosenItem.copyParent.adultBS)
		{
			newPawn.story.Adulthood = comp.Pawn.story.Adulthood;
		}
	}

	public static void SetTraits(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		checked
		{
			if (comp.ChosenItem.copyParent.traits)
			{
				for (int num = newPawn.story.traits.allTraits.Count - 1; num >= 0; num--)
				{
					newPawn.story.traits.allTraits.RemoveAt(num);
				}
				newPawn.story.traits.allTraits = comp.Pawn.story.traits.allTraits.ListFullCopy();
			}
		}
	}

	public static void UpdateDisabilities(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
	{
		if (newPawn.skills != null)
		{
			newPawn.skills.Notify_SkillDisablesChanged();
		}
	}
}
