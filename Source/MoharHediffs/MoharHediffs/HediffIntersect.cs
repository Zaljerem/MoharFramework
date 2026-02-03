using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs;

public static class HediffIntersect
{
	public static bool RemoveHediffAndReturnTrue(Pawn p, Hediff h, bool debug = false)
	{
		string warning = (debug ? (p.LabelShort + " - " + p.def.defName + " - RemoveHediff " + h.def.defName) : "");
		Tools.Warn(warning, debug);
		p.health.RemoveHediff(h);
		return true;
	}

	public static bool TreatLightCondition(this Pawn p, LightCondition LC, Hediff h, float lightLevel, bool outside, bool debug = false)
	{
		if ((LC.RequiresLightLevel && !LC.level.Value.Includes(lightLevel)) || (LC.requiresOutside && !outside) || (LC.requiresInside && outside))
		{
			return RemoveHediffAndReturnTrue(p, h, debug);
		}
		return false;
	}

	public static bool TreatNeedCondition(this Pawn p, List<NeedCondition> needs, Hediff h, bool debug = false)
	{
		string text = (debug ? (p.LabelShort + " TreatNeedCondition - ") : "");
		foreach (NeedCondition NC in needs)
		{
			Tools.Warn(text + $"checking {NC.needDef.defName} => {NC.level.min} > x > {NC.level.max}", debug);
			Need need = p.needs.AllNeeds.Where((Need n) => n.def == NC.needDef && !NC.level.Includes(n.CurLevelPercentage)).FirstOrFallback();
			if (need == null)
			{
				continue;
			}
			Tools.Warn(text + $"Found {need.def.defName} out of range: {need.CurLevelPercentage}", debug);
			return RemoveHediffAndReturnTrue(p, h, debug);
		}
		return false;
	}

	public static bool TreatHediffSeverityCondition(this Pawn p, List<HediffSeverityCondition> destroyingHediffs, Hediff h, bool debug = false)
	{
		foreach (HediffSeverityCondition HSC in destroyingHediffs)
		{
			Hediff hediff = p.health.hediffSet.hediffs.Where((Hediff dh) => dh.def == HSC.hediffDef && !HSC.acceptableSeverity.Includes(dh.Severity)).FirstOrFallback();
			if (hediff == null)
			{
				continue;
			}
			return RemoveHediffAndReturnTrue(p, hediff, debug);
		}
		return false;
	}

	public static bool TreatRelevantHediffsAndReportIfStillHediffsToCheck(this HediffComp_OnTheCarpet comp)
	{
		bool myDebug = comp.MyDebug;
		bool flag = false;
		bool flag2 = false;
		Pawn pawn = comp.Pawn;
		string text = (myDebug ? (pawn.LabelShort + " TreatRelevant - ") : "");
		Tools.Warn(text + " Entering", myDebug);
		float ambientTemperature = pawn.AmbientTemperature;
		float lightLevel = pawn.Map.glowGrid.GroundGlowAt(pawn.Position);
		bool outside = pawn.GetRoom()?.PsychologicallyOutdoors ?? true;
		List<Hediff> hediffs = comp.Pawn.health.hediffSet.hediffs;
		checked
		{
			int num = hediffs.Count - 1;
			while (num >= 0 && !hediffs.NullOrEmpty())
			{
				Hediff H = hediffs[num];
				foreach (HediffItemToRemove item in comp.Props.hediffPool.Where((HediffItemToRemove h) => h.hediffDef == H.def))
				{
					Tools.Warn(text + " found intersect hediff: " + H.def.defName, myDebug);
					flag2 = true;
					HediffKeepingCondition defaultPlusSpecificHediffCondition = HediffRemovalConditionBuilder.GetDefaultPlusSpecificHediffCondition(comp.Props.defaultCondition, item.specificCondition, myDebug);
					bool flag3 = false;
					if (defaultPlusSpecificHediffCondition.HasLightCondition)
					{
						Tools.Warn(text + H.def.defName + "checking light", myDebug);
						flag3 = pawn.TreatLightCondition(defaultPlusSpecificHediffCondition.light, H, lightLevel, outside, myDebug);
					}
					else if (defaultPlusSpecificHediffCondition.HasTemperatureCondition && !defaultPlusSpecificHediffCondition.temperature.Value.Includes(ambientTemperature))
					{
						Tools.Warn(text + H.def.defName + "checking temperature", myDebug);
						flag3 = RemoveHediffAndReturnTrue(pawn, H, myDebug);
					}
					else if (defaultPlusSpecificHediffCondition.HasNeedCondition)
					{
						Tools.Warn(text + H.def.defName + "checking " + defaultPlusSpecificHediffCondition.needs.Count + "need", myDebug);
						flag3 = pawn.TreatNeedCondition(defaultPlusSpecificHediffCondition.needs, H, myDebug);
					}
					else if (defaultPlusSpecificHediffCondition.HasDestroyingHediffs)
					{
						Tools.Warn(text + H.def.defName + "checking other hediffs", myDebug);
						flag3 = pawn.TreatHediffSeverityCondition(defaultPlusSpecificHediffCondition.destroyingHediffs, H, myDebug);
					}
					flag |= (flag2 = !flag3);
					if (flag3)
					{
						return true;
					}
				}
				num--;
			}
			Tools.Warn(text + "exiting", myDebug);
			return flag;
		}
	}
}
