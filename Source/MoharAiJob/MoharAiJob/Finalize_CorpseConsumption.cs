using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoharAiJob;

public static class Finalize_CorpseConsumption
{
	public static bool TrySpawnFilth(Thing refT, float filthRadius, ThingDef filthDef)
	{
		if (refT.Map == null)
		{
			return false;
		}
		if (CellFinder.TryFindRandomReachableNearbyCell(refT.Position, refT.Map, filthRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors), (IntVec3 x) => x.Standable(refT.Map), (Region x) => true, out var result))
		{
			return FilthMaker.TryMakeFilth(result, refT.Map, filthDef);
		}
		return false;
	}

	public static ThingDef GetFilthDef(this WorkFlow WF, Corpse corpse)
	{
		if (WF.bloodFilth)
		{
			return corpse.InnerPawn.RaceProps.BloodDef;
		}
		if (WF.filthDef != null)
		{
			return WF.filthDef;
		}
		return null;
	}

	public static void SpawnFilth(this WorkFlow WF, Corpse corpse, Map map, bool MyDebug = false)
	{
		ThingDef filthDef = WF.GetFilthDef(corpse);
		if (filthDef != null)
		{
			int num = Math.Max(1, (int)(corpse.InnerPawn.RaceProps.baseHealthScale * WF.filthPerHealthScale.RandomInRange));
			for (int i = 0; i < num; i++)
			{
				bool flag = TrySpawnFilth(corpse, WF.filthRadius, filthDef);
			}
			TrySpawnFilth(corpse, 1f, filthDef);
		}
	}

	public static Toil SpawnProductDespawnCorpse(this CorpseRecipeSettings CRS, Pawn ParentPawn, Corpse corpse, bool MyDebug = false)
	{
		return new Toil
		{
			initAction = delegate
			{
				CRS.product.SpawnConsumptionProduct(corpse, ParentPawn, corpse.Position, MyDebug);
				if (CRS.HasWorkFlow && CRS.workFlow.MustStrip)
				{
					CRS.workFlow.strip.StripAndDamageBelongings(corpse);
				}
				corpse.Destroy(DestroyMode.KillFinalize);
			},
			atomicWithPrevious = true
		};
	}

	public static void StripAndDamageBelongings(this StripAndDamage SPD, Corpse corpse)
	{
		Pawn innerPawn = corpse.InnerPawn;
		if (SPD.mustStrip || corpse.NegligibleThing() || !corpse.AnythingToStrip())
		{
			return;
		}
		if (SPD.mustDamage)
		{
			if ((innerPawn.inventory != null) & !innerPawn.inventory.innerContainer.EnumerableNullOrEmpty())
			{
				foreach (Thing item in innerPawn.inventory.innerContainer)
				{
					item.HitPoints = Math.Max(1, (int)((float)item.HitPoints * SPD.inventoryDamagingRatio.RandomInRange));
				}
			}
			if (innerPawn.equipment != null && !innerPawn.equipment.Primary.DestroyedOrNull())
			{
				innerPawn.equipment.Primary.HitPoints = Math.Max(1, (int)((float)innerPawn.equipment.Primary.HitPoints * SPD.primaryDamagingRatio.RandomInRange));
			}
			if (innerPawn.apparel != null && !innerPawn.apparel.WornApparel.NullOrEmpty())
			{
				foreach (Apparel item2 in innerPawn.apparel.WornApparel)
				{
					item2.HitPoints = Math.Max(1, (int)((float)item2.HitPoints * SPD.apparelsDamagingRatio.RandomInRange));
				}
			}
		}
		corpse.Strip();
	}

	public static bool MakeManhunter(this Pawn p, bool MyDebug = false)
	{
		if (p.NegligiblePawn())
		{
			return false;
		}
		MentalStateDef manhunter = MentalStateDefOf.Manhunter;
		if (MyDebug)
		{
			Log.Warning(p.LabelShort + " trying to go " + manhunter.defName);
		}
		if (p.mindState == null || p.mindState.mentalStateHandler == null)
		{
			if (MyDebug)
			{
				Log.Warning(p.LabelShort + " null mindstate");
			}
			return false;
		}
		if (MyDebug)
		{
			Log.Warning(p.LabelShort + " got applied " + manhunter.defName);
		}
		return p.mindState.mentalStateHandler.TryStartMentalState(manhunter);
	}

	public static void SpawnConsumptionProduct(this CorpseProduct CP, Corpse corpse, Pawn ParentPawn, IntVec3 SpawnPos, bool MyDebug = false)
	{
		Map map = ParentPawn.Map;
		string text = (MyDebug ? (ParentPawn.ThingID + " SpawnConsumptionProduct ") : null);
		if (CP == null)
		{
			if (MyDebug)
			{
				Log.Warning(text + "No CorpseProduct found");
			}
		}
		else if (CP.HasRelevantCombatPowerPerMass)
		{
			if (MyDebug)
			{
				Log.Warning(text + "CombatPowerPerMass");
			}
			int num = (int)(corpse.GetStatValue(StatDefOf.Mass) * CP.combatPowerPerMass);
			int num2 = (CP.HasRelevantCombatPowerLimit ? Math.Min(CP.combatPowerLimit, num) : num);
			if (MyDebug)
			{
				Log.Warning(text + "corpse:" + corpse.Label + "potentialCP:" + num + " NewCombatPowerLimit:" + num2);
			}
			int num3 = 0;
			int num4 = 20;
			do
			{
				num3 += CP.SpawnSinglePawn(ParentPawn, SpawnPos, map, MyDebug);
				if (MyDebug)
				{
					Log.Warning(text + "Spawned " + num3 + "/" + num2);
				}
				num4--;
			}
			while (num4 > 0 && num3 <= num2);
			if (MyDebug && num4 <= 0)
			{
				Log.Warning(text + "Got stuck in an infinite loop, had to break it");
			}
		}
		else if (CP.HasRelevantCombatPowerLimit)
		{
			if (MyDebug)
			{
				Log.Warning(text + "CombatPowerLimit");
			}
			int num5 = 0;
			int num6 = 20;
			do
			{
				num5 += CP.SpawnSinglePawn(ParentPawn, SpawnPos, map, MyDebug);
				if (MyDebug)
				{
					Log.Warning(text + "Spawned " + num5 + "/" + CP.combatPowerLimit);
				}
				num6--;
			}
			while (num6 > 0 && num5 <= CP.combatPowerLimit);
			if (MyDebug && num6 <= 0)
			{
				Log.Warning(text + "Got stuck in an infinite loop, had to break it");
			}
		}
		else
		{
			int randomInRange = CP.pawnNum.RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				CP.SpawnSinglePawn(ParentPawn, SpawnPos, map, MyDebug);
			}
		}
	}

	public static int SpawnSinglePawn(this CorpseProduct CP, Pawn ParentPawn, IntVec3 SpawnPos, Map map, bool MyDebug = false)
	{
		string text = (MyDebug ? "SpawnSinglePawn " : null);
		PawnGenOption pawnGenOption = CP.pawnKind.RandomElementByWeight((PawnGenOption p) => p.selectionWeight);
		if (pawnGenOption == null)
		{
			if (MyDebug)
			{
				Log.Warning(text + " No PawnGenOption found");
			}
			return 0;
		}
		PawnKindDef kind = pawnGenOption.kind;
		Faction faction = ((!CP.HasWeightedFaction) ? null : CP.forcedFaction?.GetFaction(ParentPawn));
		if (kind == null)
		{
			if (MyDebug)
			{
				Log.Warning(text + " No PKD found");
			}
			return 0;
		}
		if (MyDebug)
		{
			Log.Warning(text + "PKD:" + kind.label + " faction:" + faction);
		}
		float num = (Rand.Chance(CP.newBornChance) ? CP.newBornCombatPowerRatio : 1f);
		PawnGenerationRequest request = new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 0f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 0f, null, null, null, null, null, 0f, 0f, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null);
		Pawn pawn = PawnGenerator.GeneratePawn(request);
		GenSpawn.Spawn(pawn, SpawnPos, map);
		if (CP.HasRelevantManhunterChance && Rand.Chance(CP.manhunterChance))
		{
			pawn.MakeManhunter(MyDebug);
		}
		if (CP.inheritSettingsFromParent && pawn.InheritParentSettings(ParentPawn, faction) && MyDebug)
		{
			Log.Warning(text + "applied parent settings");
		}
		if (CP.setRelationsWithParent && pawn.AddParentRelations(ParentPawn) && MyDebug)
		{
			Log.Warning(text + "added relations");
		}
		return (int)(num * pawn.kindDef.combatPower);
	}

	public static Faction GetFaction(this List<WeightedFaction> myFactions, Pawn ParentPawn)
	{
		WeightedFaction weightedFaction = myFactions.RandomElementByWeightWithFallback((WeightedFaction f) => f.weight);
		if (weightedFaction == null)
		{
			return null;
		}
		FactionDef factionDef = null;
		if (weightedFaction.inheritFromParent)
		{
			return ParentPawn.Faction;
		}
		factionDef = weightedFaction.factionDef;
		return Find.FactionManager.AllFactions.Where((Faction F) => F.def == factionDef).FirstOrFallback();
	}

	public static bool InheritParentSettings(this Pawn p, Pawn hatcheeParent, Faction hatcheeFaction)
	{
		if (p.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
		{
			p.playerSettings.AreaRestrictionInPawnCurrentMap = hatcheeParent.playerSettings.AreaRestrictionInPawnCurrentMap;
			return true;
		}
		return false;
	}

	public static bool AddParentRelations(this Pawn p, Pawn hatcheeParent)
	{
		if (!p.RaceProps.IsMechanoid)
		{
			p.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
			return true;
		}
		return false;
	}
}
