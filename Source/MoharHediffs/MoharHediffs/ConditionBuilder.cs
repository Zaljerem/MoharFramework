using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public static class ConditionBuilder
{
	public static void CopyHediffCondition(HediffCondition source, HediffCondition dest, bool debug = false)
	{
		string text = (debug ? "CopyHediffCondition - " : "");
		if (source.HasBodypartCondition)
		{
			Tools.Warn(text + "found HasBodypartCondition, copying", debug);
			if (source.bodyPart.HasDef)
			{
				dest.bodyPart.bodyPartDef = source.bodyPart.bodyPartDef.ListFullCopy();
			}
			if (source.bodyPart.HasLabel)
			{
				dest.bodyPart.bodyPartLabel = source.bodyPart.bodyPartLabel.ListFullCopy();
			}
			if (source.bodyPart.HasTag)
			{
				dest.bodyPart.bodyPartTag = source.bodyPart.bodyPartTag.ListFullCopy();
			}
		}
		if (source.HasPawnCondition)
		{
			Tools.Warn(text + "found HasPawnCondition, copying", debug);
			if (source.pawn.HasRace)
			{
				dest.pawn.race = source.pawn.race.ListFullCopy();
			}
			if (source.pawn.HasGender)
			{
				dest.pawn.gender = source.pawn.gender.ListFullCopy();
			}
			dest.pawn.ageRange = source.pawn.ageRange;
		}
	}

	public static HediffCondition GetDefaultPlusSpecificHediffCondition(HediffCondition defaultHC, HediffCondition specificHC, bool debug = false)
	{
		string text = (debug ? "GetDefaultPlusSpecificHediffCondition - " : "");
		Tools.Warn(text + "allocating answerHC", debug);
		HediffCondition hediffCondition = new HediffCondition
		{
			bodyPart = new BodyPartCondition
			{
				bodyPartDef = new List<BodyPartDef>(),
				bodyPartLabel = new List<string>(),
				bodyPartTag = new List<BodyPartTagDef>()
			},
			pawn = new PawnCondition
			{
				race = new List<ThingDef>(),
				gender = new List<Gender>()
			}
		};
		if (defaultHC != null)
		{
			Tools.Warn(text + "found defaultHC, copying", debug);
			CopyHediffCondition(defaultHC, hediffCondition, debug);
		}
		if (specificHC != null)
		{
			Tools.Warn(text + "found specificHC, copying", debug);
			CopyHediffCondition(specificHC, hediffCondition, debug);
		}
		return hediffCondition;
	}
}
