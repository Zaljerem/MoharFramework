using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharHediffs;

public class HediffComp_RandySpawner : HediffComp
{
	private int ticksUntilSpawn;

	private int initialTicksUntilSpawn = 0;

	public int graceTicks = 0;

	public int calculatedQuantity;

	public int hungerReset = 0;

	public int healthReset = 0;

	private bool blockSpawn = false;

	private int pickedItem = -1;

	public Faction Itemfaction = null;

	public bool newBorn = false;

	public readonly float minDaysB4NextErrorLimit = 0.001f;

	public readonly int spawnCountErrorLimit = 750;

	public HediffCompProperties_RandySpawner Props => (HediffCompProperties_RandySpawner)props;

	public bool MyDebug => Props.debug;

	public bool HasGraceDelay => graceTicks > 0;

	private float CalculatedDaysB4Next => (float)ticksUntilSpawn / 60000f;

	public ItemParameter CurIP => (pickedItem != -1 && !Props.itemParameters.NullOrEmpty() && pickedItem < Props.itemParameters.Count) ? Props.itemParameters[pickedItem] : null;

	public bool HasValidIP => CurIP != null;

	public bool RequiresFood => Props.hungerRelative && base.Pawn.IsHungry(MyDebug);

	public bool RequiresHealth => Props.healthRelative && base.Pawn.IsInjured(MyDebug);

	public override string CompTipStringExtra
	{
		get
		{
			string text = string.Empty;
			if (!HasValidIP || !Props.logNextSpawn)
			{
				return text;
			}
			if (HasGraceDelay)
			{
				if (CurIP.PawnSpawner)
				{
					text = " No " + CurIP.pawnKindToSpawn.label + " for " + graceTicks.ToStringTicksToPeriod();
				}
				else if (CurIP.ThingSpawner)
				{
					text = " No " + CurIP.thingToSpawn.label + " for " + graceTicks.ToStringTicksToPeriod();
				}
				text = ((hungerReset > 0) ? (text + "(hunger)") : ((healthReset <= 0) ? (text + "(grace period)") : (text + "(injury)")));
			}
			else
			{
				text = ticksUntilSpawn.ToStringTicksToPeriod() + " before ";
				if (CurIP.PawnSpawner)
				{
					text += CurIP.pawnKindToSpawn.label;
				}
				else if (CurIP.ThingSpawner)
				{
					text += CurIP.thingToSpawn.label;
				}
				text = text + " " + CurIP.spawnVerb + "(" + calculatedQuantity + "x)";
			}
			return text;
		}
	}

	public override void CompExposeData()
	{
		Scribe_Values.Look(ref ticksUntilSpawn, "ticksUntilSpawn", 0);
		Scribe_Values.Look(ref initialTicksUntilSpawn, "initialTicksUntilSpawn", 0);
		Scribe_Values.Look(ref calculatedQuantity, "calculatedQuantity", 0);
		Scribe_Values.Look(ref hungerReset, "LTF_hungerReset", 0);
		Scribe_Values.Look(ref healthReset, "LTF_healthReset", 0);
		Scribe_Values.Look(ref graceTicks, "graceTicks", 0);
		Scribe_Values.Look(ref pickedItem, "pickedItem", 0);
	}

	public override void CompPostMake()
	{
		Tools.Warn(">>> " + base.Pawn?.Label + " - " + parent.def.defName + " - CompPostMake start", MyDebug);
		this.DumpProps();
		this.CheckProps();
		CalculateValues();
		CheckCalculatedValues();
		DumpCalculatedValues();
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		checked
		{
			if (!base.Pawn.Negligible() && !blockSpawn)
			{
				if (HasGraceDelay)
				{
					graceTicks--;
				}
				else if (!this.SetRequirementGraceTicks() && CheckShouldSpawn())
				{
					Tools.Warn("Reseting countdown bc spawned thing", MyDebug);
					CalculateValues();
					CheckCalculatedValues();
				}
			}
		}
	}

	private void CalculateValues()
	{
		pickedItem = this.GetWeightedRandomIndex();
		if (HasValidIP)
		{
			CurIP.ComputeRandomParameters(out ticksUntilSpawn, out graceTicks, out calculatedQuantity);
			if (CurIP.HasFactionParams)
			{
				this.ComputeRandomFaction();
			}
		}
		else
		{
			BlockAndDestroy(">ERROR< failed to find an index for IP, check and adjust your hediff props", MyDebug);
		}
	}

	private void CheckCalculatedValues()
	{
		if (calculatedQuantity > spawnCountErrorLimit)
		{
			BlockAndDestroy(">ERROR< calculatedQuantity is too high: " + calculatedQuantity + "(>" + spawnCountErrorLimit + "), check and adjust your hediff props", MyDebug);
		}
		else if (CalculatedDaysB4Next < minDaysB4NextErrorLimit)
		{
			BlockAndDestroy(">ERROR< calculatedMinDaysB4Next is too low: " + CalculatedDaysB4Next + "(<" + minDaysB4NextErrorLimit + "), check and adjust your hediff props", MyDebug);
		}
	}

	private void DumpCalculatedValues()
	{
		Tools.Warn("<<<  calculatedDaysB4Next: " + CalculatedDaysB4Next + "; CalculatedQuantity: " + calculatedQuantity + "; ", MyDebug);
	}

	public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
	{
		Tools.Warn(ErrorLog, myDebug);
		blockSpawn = true;
		Tools.DestroyParentHediff(parent, myDebug);
	}

	private bool CheckShouldSpawn()
	{
		checked
		{
			ticksUntilSpawn--;
			if (ticksUntilSpawn <= 0)
			{
				bool flag = TryDoSpawn();
				Tools.Warn("TryDoSpawn: " + flag, MyDebug);
				if (flag)
				{
					pickedItem = -1;
				}
				return flag;
			}
			return false;
		}
	}

	public bool TrySpawnPawn()
	{
		PawnGenerationRequest request = new PawnGenerationRequest(CurIP.pawnKindToSpawn, Itemfaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newBorn, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null);
		for (int i = 0; i < calculatedQuantity; i = checked(i + 1))
		{
			Pawn newThing = PawnGenerator.GeneratePawn(request);
			GenSpawn.Spawn(newThing, base.Pawn.Position, base.Pawn.Map);
			if (CurIP.HasFilth)
			{
				FilthMaker.TryMakeFilth(parent.pawn.Position, parent.pawn.Map, CurIP.filthDef);
			}
		}
		return true;
	}

	public bool TryDoSpawn()
	{
		if (base.Pawn.Negligible())
		{
			Tools.Warn("TryDoSpawn - pawn null", MyDebug);
			return false;
		}
		if (CurIP.PawnSpawner)
		{
			return TrySpawnPawn();
		}
		checked
		{
			if (CurIP.ThingSpawner && Props.spawnMaxAdjacent >= 0)
			{
				int num = 0;
				for (int i = 0; i < 9; i++)
				{
					IntVec3 c = base.Pawn.Position + GenAdj.AdjacentCellsAndInside[i];
					if (!c.InBounds(base.Pawn.Map))
					{
						continue;
					}
					List<Thing> thingList = c.GetThingList(base.Pawn.Map);
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j].def == CurIP.thingToSpawn)
						{
							num += thingList[j].stackCount;
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
				if (this.TryFindSpawnCell(out var result))
				{
					Thing thing = ThingMaker.MakeThing(CurIP.thingToSpawn);
					thing.stackCount = num3;
					if (thing.def.stackLimit > 0 && thing.stackCount > thing.def.stackLimit)
					{
						thing.stackCount = thing.def.stackLimit;
					}
					num2 += thing.stackCount;
					num3 -= thing.stackCount;
					GenPlace.TryPlaceThing(thing, result, base.Pawn.Map, ThingPlaceMode.Direct, out var lastResultingThing);
					if (Props.spawnForbidden)
					{
						lastResultingThing.SetForbidden(value: true);
					}
				}
				if (num4++ > 10)
				{
					Tools.Warn("Had to break the loop", MyDebug);
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
}
