using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharHediffs;

public static class SpawnerUtils
{
	public static bool TrySpawnPawn(this HediffComp_RandySpawnUponDeath comp, Thing refThing, int randomQuantity)
	{
		string text = (comp.MyDebug ? (comp.Pawn.LabelShort + " TrySpawnPawn ") : "");
		ThingSettings chosenItem = comp.ChosenItem;
		IntVec3 position = refThing.Position;
		Map map = refThing.Map;
		PawnKindDef pawnOfChoice = comp.PawnOfChoice;
		PawnGenerationRequest request = (chosenItem.newBorn ? new PawnGenerationRequest(pawnOfChoice, comp.RandomFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 0f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 0f, null, null, null, null, null, 0f, 0f, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null) : new PawnGenerationRequest(pawnOfChoice, comp.RandomFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 0f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 0f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null));
		for (int i = 0; i < randomQuantity; i = checked(i + 1))
		{
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			comp.SetAge(pawn);
			if (chosenItem.IsCopier)
			{
				comp.SetName(pawn);
				comp.SetGender(pawn);
				comp.SetMelanin(pawn);
				comp.SetAlienSkinColor(pawn);
				comp.SetAlienBodyAndHeadType(pawn);
				comp.SetHair(pawn);
				comp.SetHairColor(pawn);
				comp.SetHediff(pawn);
				PawnCopyUtils.InitRememberBackstories(out var childBS, out var adultBS);
				if (comp.ChosenItem.copyParent.passions || comp.ChosenItem.copyParent.traits)
				{
					comp.RememberBackstories(pawn, out childBS, out adultBS);
					comp.ResetBackstories(pawn);
					comp.SetPassions(pawn);
					comp.SetSkills(pawn);
					comp.SetTraits(pawn);
				}
				if (childBS != null || adultBS != null)
				{
					comp.ReinjectBackstories(pawn, childBS, adultBS);
				}
				comp.SetBackstories(pawn);
				comp.UpdateDisabilities(pawn);
			}
			if (chosenItem.IsRedresser)
			{
				comp.DestroyApparel(pawn);
				comp.DestroyEquipment(pawn);
				comp.DestroyInventory(pawn);
			}
			if (comp.HasContainerSpawn)
			{
				if (refThing is Building_Casket building_Casket && !building_Casket.TryAcceptThing(pawn) && comp.MyDebug)
				{
					Log.Warning(text + " tried to add " + pawn.LabelShort + " to " + refThing.Label + ", but failed");
				}
			}
			else
			{
				GenSpawn.Spawn(pawn, position, map);
			}
			if (comp.ChosenItem.HasMentalStateParams)
			{
				comp.ComputeRandomMentalState();
				if (comp.RandomMS != null)
				{
					pawn.mindState.mentalStateHandler.TryStartMentalState(comp.RandomMS, null, forced: false, forceWake: false, causedByMood: false, null, transitionSilently: true);
				}
			}
			comp.TrySpawnAllFilth(refThing);
			if (comp.MyDebug)
			{
				Log.Warning("------------------");
			}
		}
		return true;
	}

	public static void TrySpawnAllFilth(this HediffComp_RandySpawnUponDeath comp, Thing refThing, bool debug = false)
	{
		if (debug)
		{
			Log.Warning(comp.Pawn.LabelShort + " - TrySpawnAllFilth");
		}
		if (!comp.HasFilth)
		{
			if (debug)
			{
				Log.Warning("no filth found");
			}
			return;
		}
		int randomInRange = comp.FilthNum.RandomInRange;
		for (int i = 0; i < randomInRange; i = checked(i + 1))
		{
			if (debug)
			{
				Log.Warning(string.Concat("filth ", i, "/", randomInRange, " - fDef:", comp.FilthToSpawn, " - pos:", refThing.Position, " - map null?", (refThing.Map == null).ToString()));
			}
			TrySpawnFilth(refThing, comp.FilthRadius.RandomInRange, comp.FilthToSpawn);
		}
	}

	public static void TrySpawnFilth(Thing refT, float filthRadius, ThingDef filthDef)
	{
		if (refT.Map != null && CellFinder.TryFindRandomReachableNearbyCell(refT.Position, refT.Map, filthRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors), (IntVec3 x) => x.Standable(refT.Map), (Region x) => true, out var result))
		{
			FilthMaker.TryMakeFilth(result, refT.Map, filthDef);
		}
	}

	public static bool TrySpawnThing(this HediffComp_RandySpawnUponDeath comp, Thing thing, int randomQuantity)
	{
		Map map = thing.Map;
		checked
		{
			if (comp.Props.spawnMaxAdjacent >= 0)
			{
				int num = 0;
				for (int i = 0; i < 9; i++)
				{
					IntVec3 c = thing.Position + GenAdj.AdjacentCellsAndInside[i];
					if (!c.InBounds(map))
					{
						continue;
					}
					List<Thing> thingList = c.GetThingList(map);
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j].def == comp.ChosenItem.thingToSpawn)
						{
							num += thingList[j].stackCount;
							if (num >= comp.Props.spawnMaxAdjacent)
							{
								return false;
							}
						}
					}
				}
			}
			int num2 = 0;
			int num3 = randomQuantity;
			int num4 = 0;
			while (num2 < randomQuantity)
			{
				if (comp.TryFindSpawnCell(thing, randomQuantity, map, out var result))
				{
					Thing thing2 = ThingMaker.MakeThing(comp.ChosenItem.thingToSpawn);
					thing2.stackCount = num3;
					if (thing2.def.stackLimit > 0 && thing2.stackCount > thing2.def.stackLimit)
					{
						thing2.stackCount = thing2.def.stackLimit;
					}
					num2 += thing2.stackCount;
					num3 -= thing2.stackCount;
					GenPlace.TryPlaceThing(thing2, result, map, ThingPlaceMode.Direct, out var lastResultingThing);
					if (comp.Props.spawnForbidden)
					{
						lastResultingThing.SetForbidden(value: true);
					}
				}
				if (num4++ > 10)
				{
					if (comp.MyDebug)
					{
						Log.Warning("Had to break the loop");
					}
					return false;
				}
			}
			if (num3 <= 0)
			{
				return true;
			}
			return false;
		}
	}

	public static bool TryDoSpawn(this HediffComp_RandySpawnUponDeath comp, Thing thing, int randomQuantity)
	{
		if (thing.Negligible())
		{
			if (comp.MyDebug)
			{
				Log.Warning("TryDoSpawn - negligeable");
			}
			return false;
		}
		if (comp.HasChosenPawn)
		{
			if (comp.MyDebug)
			{
				Log.Warning("TryDoSpawn -> TrySpawnPawn");
			}
			return comp.TrySpawnPawn(thing, randomQuantity);
		}
		if (comp.HasChosenThing)
		{
			if (comp.MyDebug)
			{
				Log.Warning("TryDoSpawn -> TrySpawnPawn");
			}
			return comp.TrySpawnThing(thing, randomQuantity);
		}
		return false;
	}

	public static bool TryFindSpawnCell(this HediffComp_RandySpawnUponDeath comp, Thing refThing, int randomQuantity, Map map, out IntVec3 result)
	{
		ThingDef thingToSpawn = comp.ChosenItem.thingToSpawn;
		if (refThing.Negligible())
		{
			result = IntVec3.Invalid;
			if (comp.MyDebug)
			{
				Log.Warning("TryFindSpawnCell Null - pawn null");
			}
			return false;
		}
		checked
		{
			foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(refThing).InRandomOrder())
			{
				if (!item.Walkable(map))
				{
					continue;
				}
				Building edifice = item.GetEdifice(map);
				if ((edifice != null && thingToSpawn.IsEdifice()) || edifice is Building_Door { FreePassage: false } || !GenSight.LineOfSight(refThing.Position, item, map, skipFirstCell: false, null, 0, 0))
				{
					continue;
				}
				bool flag = false;
				List<Thing> thingList = item.GetThingList(map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (thing.def.category == ThingCategory.Item && (thing.def != thingToSpawn || thing.stackCount > thingToSpawn.stackLimit - randomQuantity))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					result = item;
					return true;
				}
			}
			if (comp.MyDebug)
			{
				Log.Warning("TryFindSpawnCell Null - no spawn cell found");
			}
			result = IntVec3.Invalid;
			return false;
		}
	}
}
