using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs;

public class HediffComp_Spawner : HediffComp
{
	private int ticksUntilSpawn;

	private int initialTicksUntilSpawn = 0;

	private int hungerReset = 0;

	private int healthReset = 0;

	private int graceTicks = 0;

	private Pawn pawn = null;

	private float calculatedMaxDaysB4Next = 2f;

	private float calculatedMinDaysB4Next = 1f;

	private int calculatedQuantity = 1;

	private bool blockSpawn = false;

	private bool myDebug = false;

	private readonly float errorMinDaysB4Next = 0.001f;

	private readonly int errorExponentialLimit = 20;

	private readonly int errorSpawnCount = 750;

	public HediffCompProperties_Spawner Props => (HediffCompProperties_Spawner)props;

	public override string CompTipStringExtra
	{
		get
		{
			string empty = string.Empty;
			if (graceTicks > 0)
			{
				empty = ((!Props.animalThing) ? (" No " + Props.thingToSpawn.label + " for " + graceTicks.ToStringTicksToPeriod()) : (" No " + Props.animalToSpawn.defName + " for " + graceTicks.ToStringTicksToPeriod()));
				if (hungerReset > 0)
				{
					return empty + "(hunger)";
				}
				if (healthReset > 0)
				{
					return empty + "(injury)";
				}
				return empty + "(grace period)";
			}
			empty = ticksUntilSpawn.ToStringTicksToPeriod() + " before ";
			empty = ((!Props.animalThing) ? (empty + Props.thingToSpawn.label) : (empty + Props.animalToSpawn.defName));
			return empty + " " + Props.spawnVerb + "(" + calculatedQuantity + "x)";
		}
	}

	public override void CompExposeData()
	{
		Scribe_Values.Look(ref ticksUntilSpawn, "ticksUntilSpawn", 0);
		Scribe_Values.Look(ref initialTicksUntilSpawn, "initialTicksUntilSpawn", 0);
		Scribe_Values.Look(ref calculatedMinDaysB4Next, "calculatedMinDaysB4Next", 0f);
		Scribe_Values.Look(ref calculatedMaxDaysB4Next, "calculatedMaxDaysB4Next", 0f);
		Scribe_Values.Look(ref calculatedQuantity, "calculatedQuantity", 0);
		Scribe_Values.Look(ref graceTicks, "graceTicks", 0);
	}

	public override void CompPostMake()
	{
		myDebug = Props.debug;
		Tools.Warn(">>> " + parent.pawn.Label + " - " + parent.def.defName + " - CompPostMake start", myDebug);
		TraceProps();
		CheckProps();
		CalculateValues();
		CheckCalculatedValues();
		TraceCalculatedValues();
		if (initialTicksUntilSpawn == 0)
		{
			Tools.Warn("Reseting countdown bc initialTicksUntilSpawn == 0 (comppostmake)", myDebug);
			ResetCountdown();
		}
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		pawn = parent.pawn;
		if (!Tools.OkPawn(pawn) || blockSpawn)
		{
			return;
		}
		checked
		{
			if (graceTicks > 0)
			{
				graceTicks--;
				return;
			}
			if (Props.hungerRelative && pawn.IsHungry(myDebug))
			{
				int num = (int)(RandomGraceDays() * 60000f);
				hungerReset++;
				graceTicks = num;
				return;
			}
			if (Props.healthRelative && pawn.IsInjured(myDebug))
			{
				int num2 = (int)(RandomGraceDays() * 60000f);
				healthReset++;
				graceTicks = num2;
				return;
			}
			hungerReset = (healthReset = 0);
			if (CheckShouldSpawn())
			{
				Tools.Warn("Reseting countdown bc spawned thing", myDebug);
				CalculateValues();
				CheckCalculatedValues();
				ResetCountdown();
				if (Rand.Chance(Props.randomGrace))
				{
					int num3 = (int)(RandomGraceDays() * 60000f);
					graceTicks = num3;
				}
			}
		}
	}

	private void TraceProps()
	{
		Tools.Warn("Props => minDaysB4Next: " + Props.minDaysB4Next + "; maxDaysB4Next: " + Props.maxDaysB4Next + "; randomGrace: " + Props.randomGrace + "; graceDays: " + Props.graceDays + "; hungerRelative: " + Props.hungerRelative.ToString() + "; healthRelative: " + Props.healthRelative.ToString() + "; ", myDebug);
		if (Props.animalThing)
		{
			Tools.Warn("animalThing: " + Props.animalThing + "; animalName: " + Props.animalToSpawn.defName + "; factionOfPlayerAnimal: " + Props.factionOfPlayerAnimal + "; ", myDebug);
		}
		if (Props.ageWeightedQuantity)
		{
			Tools.Warn("ageWeightedQuantity:" + Props.ageWeightedQuantity + "; olderBiggerQuantity:" + Props.olderBiggerQuantity + "; " + myDebug);
			if (Props.exponentialQuantity)
			{
				Tools.Warn("exponentialQuantity:" + Props.exponentialQuantity.ToString() + "; exponentialRatioLimit:" + Props.exponentialRatioLimit + "; ", myDebug);
			}
		}
		if (Props.ageWeightedPeriod)
		{
		}
		Tools.Warn("ageWeightedPeriod:" + Props.ageWeightedPeriod + "; olderSmallerPeriod:" + Props.olderSmallerPeriod + "; " + myDebug);
	}

	private void CalculateValues()
	{
		float pawnAgeOverlifeExpectancyRatio = Tools.GetPawnAgeOverlifeExpectancyRatio(parent.pawn, myDebug);
		pawnAgeOverlifeExpectancyRatio = ((pawnAgeOverlifeExpectancyRatio > 1f) ? 1f : pawnAgeOverlifeExpectancyRatio);
		calculatedMinDaysB4Next = Props.minDaysB4Next;
		calculatedMaxDaysB4Next = Props.maxDaysB4Next;
		if (Props.ageWeightedPeriod)
		{
			float num = (Props.olderSmallerPeriod ? (0f - pawnAgeOverlifeExpectancyRatio) : pawnAgeOverlifeExpectancyRatio);
			calculatedMinDaysB4Next = Props.minDaysB4Next * (1f + num);
			calculatedMaxDaysB4Next = Props.maxDaysB4Next * (1f + num);
			Tools.Warn(" ageWeightedPeriod: " + Props.ageWeightedPeriod.ToString() + " ageRatio: " + pawnAgeOverlifeExpectancyRatio + " minDaysB4Next: " + Props.minDaysB4Next + " maxDaysB4Next: " + Props.maxDaysB4Next + " daysAgeRatio: " + num + " calculatedMinDaysB4Next: " + calculatedMinDaysB4Next + ";  calculatedMaxDaysB4Next: " + calculatedMaxDaysB4Next + "; ", myDebug);
		}
		calculatedQuantity = Props.spawnCount;
		if (!Props.ageWeightedQuantity)
		{
			return;
		}
		float num2 = (Props.olderBiggerQuantity ? pawnAgeOverlifeExpectancyRatio : (0f - pawnAgeOverlifeExpectancyRatio));
		Tools.Warn("quantityAgeRatio: " + num2, myDebug);
		checked
		{
			calculatedQuantity = (int)Math.Round((double)Props.spawnCount * (double)(1f + num2));
			if (Props.exponentialQuantity)
			{
				num2 = 1f - pawnAgeOverlifeExpectancyRatio;
				if (num2 == 0f)
				{
					Tools.Warn(">ERROR< quantityAgeRatio is f* up : " + num2, myDebug);
					blockSpawn = true;
					Tools.DestroyParentHediff(parent, myDebug);
					return;
				}
				float num3 = (Props.olderBiggerQuantity ? (1f / num2) : (num2 * num2));
				bool flag = false;
				bool flag2 = false;
				if (num3 > (float)Props.exponentialRatioLimit)
				{
					num3 = Props.exponentialRatioLimit;
					flag = true;
				}
				calculatedQuantity = (int)Math.Round((double)Props.spawnCount * (double)num3);
				if (calculatedQuantity < 1)
				{
					calculatedQuantity = 1;
					flag2 = true;
				}
				Tools.Warn(" exponentialQuantity: " + Props.exponentialQuantity.ToString() + "; expoFactor: " + num3 + "; gotLimited: " + flag.ToString() + "; gotAugmented: " + flag2.ToString(), myDebug);
			}
			Tools.Warn("; Props.spawnCount: " + Props.spawnCount + "; calculatedQuantity: " + calculatedQuantity, myDebug);
		}
	}

	private void CheckCalculatedValues()
	{
		if (calculatedQuantity > errorSpawnCount)
		{
			Tools.Warn(">ERROR< calculatedQuantity is too high: " + calculatedQuantity + "(>" + errorSpawnCount + "), check and adjust your hediff props", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
		}
		else if (calculatedMinDaysB4Next >= calculatedMaxDaysB4Next)
		{
			Tools.Warn(">ERROR< calculatedMinDaysB4Next should be lower than calculatedMaxDaysB4Next", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
		}
		else if (calculatedMinDaysB4Next < errorMinDaysB4Next)
		{
			Tools.Warn(">ERROR< calculatedMinDaysB4Next is too low: " + Props.minDaysB4Next + "(<" + errorMinDaysB4Next + "), check and adjust your hediff props", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
		}
	}

	private void TraceCalculatedValues()
	{
		Tools.Warn("<<< " + (Props.ageWeightedPeriod ? ("Props.olderMoreOften: " + Props.olderSmallerPeriod + "; ") : "") + (Props.ageWeightedQuantity ? ("Props.olderBiggerquantities: " + Props.olderBiggerQuantity + "; ") : "") + " Props.minDaysB4Next: " + Props.minDaysB4Next + "; Props.maxDaysB4Next: " + Props.maxDaysB4Next + ";  calculatedMinDaysB4Next: " + calculatedMinDaysB4Next + "; calculatedMaxDaysB4Next: " + calculatedMaxDaysB4Next + ";  Props.spawnCount: " + Props.spawnCount + "; CalculatedQuantity: " + calculatedQuantity + "; ", myDebug);
	}

	private void CheckProps()
	{
		if (Props.spawnCount > errorSpawnCount)
		{
			Tools.Warn("SpawnCount is too high: " + Props.spawnCount + "(>" + errorSpawnCount + "),  some people just want to see the world burn", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
			return;
		}
		if (Props.minDaysB4Next >= Props.maxDaysB4Next)
		{
			Tools.Warn("minDaysB4Next should be lower than maxDaysB4Next", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
			return;
		}
		if (Props.minDaysB4Next < errorMinDaysB4Next)
		{
			Tools.Warn("minDaysB4Next is too low: " + Props.minDaysB4Next + "(<" + errorMinDaysB4Next + "), some people just want to see the world burn", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
			return;
		}
		if (Props.animalThing)
		{
			if (Props.animalToSpawn == null || Props.animalToSpawn.defName.NullOrEmpty())
			{
				Tools.Warn("Props.animalThing=" + Props.animalThing + "; but no Props.animalName", myDebug);
				blockSpawn = true;
				Tools.DestroyParentHediff(parent, myDebug);
				return;
			}
			Tools.Warn("Found animal PawnKindDef.defName=" + Props.animalToSpawn.defName, myDebug);
		}
		else
		{
			ThingDef thingDef = DefDatabase<ThingDef>.AllDefs.Where((ThingDef b) => b == Props.thingToSpawn).RandomElement();
			if (thingDef == null)
			{
				Tools.Warn("Could not find Props.thingToSpawn in DefDatabase", myDebug);
				blockSpawn = true;
				Tools.DestroyParentHediff(parent, myDebug);
				return;
			}
			Tools.Warn("Found ThingDef for " + Props.thingToSpawn.defName + "in DefDatabase", myDebug);
		}
		if (!Props.ageWeightedPeriod && Props.olderSmallerPeriod)
		{
			Tools.Warn("olderSmallerPeriod ignored since ageWeightedPeriod is false ", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
			return;
		}
		if (!Props.ageWeightedQuantity)
		{
			if (Props.olderBiggerQuantity)
			{
				Tools.Warn("olderBiggerQuantity ignored since ageWeightedQuantity is false ", myDebug);
			}
			if (Props.exponentialQuantity)
			{
				Tools.Warn("exponentialQuantity ignored since ageWeightedQuantity is false ", myDebug);
			}
			if (Props.olderBiggerQuantity || Props.exponentialQuantity)
			{
				blockSpawn = true;
				Tools.DestroyParentHediff(parent, myDebug);
				return;
			}
		}
		if (Props.exponentialQuantity && Props.exponentialRatioLimit > errorExponentialLimit)
		{
			Tools.Warn("expoRatioLimit too low while expoQuantity is set: " + Props.exponentialRatioLimit + "(>" + errorExponentialLimit + "), some people just want to see the world burn", myDebug);
			blockSpawn = true;
			Tools.DestroyParentHediff(parent, myDebug);
		}
	}

	private bool CheckShouldSpawn()
	{
		pawn = parent.pawn;
		if (!Tools.OkPawn(pawn))
		{
			Tools.Warn("CheckShouldSpawn pawn Null", myDebug);
			return false;
		}
		checked
		{
			ticksUntilSpawn--;
			if (ticksUntilSpawn <= 0)
			{
				Tools.Warn("TryDoSpawn: " + TryDoSpawn(), myDebug);
				return true;
			}
			return false;
		}
	}

	private PawnKindDef MyPawnKindDefNamed(string myDefName)
	{
		PawnKindDef result = null;
		foreach (PawnKindDef allDef in DefDatabase<PawnKindDef>.AllDefs)
		{
			if (allDef.defName == myDefName)
			{
				return allDef;
			}
		}
		return result;
	}

	public bool TryDoSpawn()
	{
		pawn = parent.pawn;
		if (!Tools.OkPawn(pawn))
		{
			Tools.Warn("TryDoSpawn - pawn null", myDebug);
			return false;
		}
		checked
		{
			if (Props.animalThing)
			{
				Faction faction = (Props.factionOfPlayerAnimal ? Faction.OfPlayer : null);
				PawnGenerationRequest request = new PawnGenerationRequest(Props.animalToSpawn, faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: true, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null);
				for (int i = 0; i < calculatedQuantity; i++)
				{
					Pawn newThing = PawnGenerator.GeneratePawn(request);
					GenSpawn.Spawn(newThing, parent.pawn.Position, parent.pawn.Map);
					FilthMaker.TryMakeFilth(parent.pawn.Position, parent.pawn.Map, ThingDefOf.Filth_AmnioticFluid);
				}
				return true;
			}
			if (Props.spawnMaxAdjacent >= 0)
			{
				int num = 0;
				for (int j = 0; j < 9; j++)
				{
					IntVec3 c = pawn.Position + GenAdj.AdjacentCellsAndInside[j];
					if (!c.InBounds(pawn.Map))
					{
						continue;
					}
					List<Thing> thingList = c.GetThingList(pawn.Map);
					for (int k = 0; k < thingList.Count; k++)
					{
						if (thingList[k].def == Props.thingToSpawn)
						{
							num += thingList[k].stackCount;
							if (num >= Props.spawnMaxAdjacent)
							{
								return false;
							}
						}
					}
				}
			}
			int num2 = 0;
			int num3 = calculatedQuantity;
			int num4 = 0;
			while (num2 < calculatedQuantity)
			{
				if (TryFindSpawnCell(out var result))
				{
					Thing thing = ThingMaker.MakeThing(Props.thingToSpawn);
					thing.stackCount = num3;
					if (thing.def.stackLimit > 0 && thing.stackCount > thing.def.stackLimit)
					{
						thing.stackCount = thing.def.stackLimit;
					}
					num2 += thing.stackCount;
					num3 -= thing.stackCount;
					GenPlace.TryPlaceThing(thing, result, pawn.Map, ThingPlaceMode.Direct, out var lastResultingThing);
					if (Props.spawnForbidden)
					{
						lastResultingThing.SetForbidden(value: true);
					}
				}
				if (num4++ > 10)
				{
					Tools.Warn("Had to break the loop", myDebug);
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

	private bool TryFindSpawnCell(out IntVec3 result)
	{
		pawn = parent.pawn;
		if (!Tools.OkPawn(pawn))
		{
			result = IntVec3.Invalid;
			Tools.Warn("TryFindSpawnCell Null - pawn null", myDebug);
			return false;
		}
		checked
		{
			foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(pawn).InRandomOrder())
			{
				if (!item.Walkable(pawn.Map))
				{
					continue;
				}
				Building edifice = item.GetEdifice(pawn.Map);
				if ((edifice != null && Props.thingToSpawn.IsEdifice()) || edifice is Building_Door { FreePassage: false } || !GenSight.LineOfSight(pawn.Position, item, pawn.Map, skipFirstCell: false, null, 0, 0))
				{
					continue;
				}
				bool flag = false;
				List<Thing> thingList = item.GetThingList(pawn.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (thing.def.category == ThingCategory.Item && (thing.def != Props.thingToSpawn || thing.stackCount > Props.thingToSpawn.stackLimit - calculatedQuantity))
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
			Tools.Warn("TryFindSpawnCell Null - no spawn cell found", myDebug);
			result = IntVec3.Invalid;
			return false;
		}
	}

	private void ResetCountdown()
	{
		ticksUntilSpawn = (initialTicksUntilSpawn = checked((int)(RandomDays2wait() * 60000f)));
	}

	private float RandomDays2wait()
	{
		return Rand.Range(calculatedMinDaysB4Next, calculatedMaxDaysB4Next);
	}

	private float RandomGraceDays()
	{
		return Props.graceDays * Rand.Range(0f, 1f);
	}
}
