using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace OHFP;

public class Comp_OHFP_Hatcher : ThingComp
{
	private float gestateProgress;

	public Pawn hatcheeParent;

	public Pawn otherParent;

	public Faction hatcheeFaction;

	public PawnKindDef hatcheePawnKind;

	public bool HasForcedFaction => Props.HasForcedFaction;

	public bool IsRandomlyAdopted => Props.IsRandomlyAdopted;

	public bool MyDebug => Props.debug;

	public CompProperties_OHFP_Hatcher Props => (CompProperties_OHFP_Hatcher)props;

	private CompTemperatureRuinable FreezerComp => parent.GetComp<CompTemperatureRuinable>();

	public bool TemperatureDamaged
	{
		get
		{
			if (FreezerComp != null)
			{
				return FreezerComp.Ruined;
			}
			return false;
		}
	}

	public override void PostExposeData()
	{
		base.PostExposeData();
		Scribe_Values.Look(ref gestateProgress, "gestateProgress", 0f);
		Scribe_References.Look(ref hatcheeParent, "hatcheeParent");
		Scribe_References.Look(ref otherParent, "otherParent");
		Scribe_References.Look(ref hatcheeFaction, "hatcheeFaction");
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		if (HasForcedFaction)
		{
			SetForcedFaction();
			Tools.Warn("Faction forced ok", MyDebug);
		}
		else if (IsRandomlyAdopted)
		{
			SetRandomFaction();
			Tools.Warn("SetFactionAndParent ok", MyDebug);
		}
		else if (hatcheeParent != null)
		{
			hatcheeFaction = hatcheeParent.Faction;
		}
		if (Props.findRandomMotherIfNull && hatcheeParent == null)
		{
			hatcheeParent = GetMother(hatcheeFaction);
		}
		if (Props.findRandomFatherIfNull && otherParent == null)
		{
			otherParent = GetFather(hatcheeFaction, hatcheeParent);
		}
	}

	private bool SetPawnKind()
	{
		if (Props.hatcherPawnList.NullOrEmpty())
		{
			return false;
		}
		hatcheePawnKind = Props.hatcherPawnList.RandomElement();
		return hatcheePawnKind != null;
	}

	private void SetForcedFaction()
	{
		hatcheeFaction = Find.FactionManager.AllFactions.Where((Faction F) => F.def == Props.forcedFaction).FirstOrFallback();
	}

	private Pawn GetMother(Faction faction)
	{
		return (from p in Find.WorldPawns.AllPawnsAlive
			where p.Faction != null && p.Faction == faction
			where p.kindDef == Props.hatcherPawnList.RandomElement()
			where p.ageTracker.CurLifeStage.reproductive
			where p.gender == Gender.Female
			select p).RandomElementWithFallback();
	}

	private Pawn GetFather(Faction faction, Pawn Mother)
	{
		return (from p in Find.WorldPawns.AllPawnsAlive
			where p.Faction != null && p.Faction == hatcheeFaction
			where hatcheeParent == null || (p.kindDef == hatcheeParent.kindDef && p != hatcheeParent)
			where p.ageTracker.CurLifeStage.reproductive
			where p.gender == Gender.Male
			select p).RandomElementWithFallback();
	}

	private void SetRandomFaction()
	{
		RandomAdoption randomAdoption = Props.randomAdoption.RandomElementByWeightWithFallback((RandomAdoption ra) => ra.weight);
		switch (randomAdoption.factionType)
		{
		case AdoptionType.enemy:
			hatcheeFaction = Find.FactionManager.AllFactions.Where((Faction f) => !f.IsPlayer && !f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();
			break;
		case AdoptionType.neutral:
			hatcheeFaction = Find.FactionManager.AllFactions.Where((Faction f) => !f.IsPlayer && f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();
			break;
		default:
			hatcheeFaction = Faction.OfPlayer;
			break;
		}
	}

	public override void CompTick()
	{
		if (!TemperatureDamaged)
		{
			float num = 1f / (Props.hatcherDaystoHatch * 60000f);
			gestateProgress += num;
			if (gestateProgress > 1f)
			{
				Hatch();
			}
		}
	}

	public void Hatch()
	{
		Tools.Warn("hatcheeFaction == null", hatcheeFaction == null && MyDebug);
		Tools.Warn("hatcheeParent == null", hatcheeParent == null && MyDebug);
		Tools.Warn("otherParent == null", otherParent == null && MyDebug);
		PawnGenerationContext context = PawnGenerationContext.NonPlayer;
		try
		{
			for (int i = 0; i < parent.stackCount; i = checked(i + 1))
			{
				bool flag = Rand.Chance(Props.newBornChance);
				if (!SetPawnKind())
				{
					continue;
				}
				Tools.Warn("SetPawnKind: " + hatcheePawnKind.label, MyDebug);
				PawnGenerationRequest request = ((!flag) ? new PawnGenerationRequest(hatcheePawnKind, hatcheeFaction, context, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null) : new PawnGenerationRequest(hatcheePawnKind, hatcheeFaction, context, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, 0f, 0f, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null));
				Pawn pawn = PawnGenerator.GeneratePawn(request);
				if (parent.MyTrySpawnHatchedOrBornPawn(pawn))
				{
					if (pawn != null)
					{
						if (hatcheeParent != null)
						{
							pawn.InheritParentSettings(hatcheeParent, hatcheeFaction);
							pawn.AddParentRelations(hatcheeParent);
						}
						pawn.AddOtherParentRelations(hatcheeParent, otherParent);
						if (parent.Spawned)
						{
							FilthMaker.TryMakeFilth(parent.Position, parent.Map, ThingDefOf.Filth_AmnioticFluid);
						}
						if (Rand.Chance(Props.manhunterChance))
						{
							pawn.MakeManhunter(MyDebug);
						}
					}
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
				}
			}
		}
		finally
		{
			parent.Destroy();
		}
	}

	public override void PreAbsorbStack(Thing otherStack, int count)
	{
		float t = (float)count / (float)checked(parent.stackCount + count);
		float b = ((ThingWithComps)otherStack).GetComp<Comp_OHFP_Hatcher>().gestateProgress;
		gestateProgress = Mathf.Lerp(gestateProgress, b, t);
	}

	public override void PostSplitOff(Thing piece)
	{
		Comp_OHFP_Hatcher comp = ((ThingWithComps)piece).GetComp<Comp_OHFP_Hatcher>();
		comp.gestateProgress = gestateProgress;
		comp.hatcheeParent = hatcheeParent;
		comp.otherParent = otherParent;
		comp.hatcheeFaction = hatcheeFaction;
	}

	public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
	{
		base.PrePreTraded(action, playerNegotiator, trader);
		switch (action)
		{
		case TradeAction.PlayerBuys:
			hatcheeFaction = Faction.OfPlayer;
			break;
		case TradeAction.PlayerSells:
			hatcheeFaction = trader.Faction;
			break;
		}
	}

	public override void PostPostGeneratedForTrader(TraderKindDef trader, PlanetTile forTile, Faction forFaction)
	{
		base.PostPostGeneratedForTrader(trader, forTile, forFaction);
		hatcheeFaction = forFaction;
	}

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		if (Prefs.DevMode)
		{
			yield return new Command_Action
			{
				defaultLabel = "force hatch",
				defaultDesc = "debug egg",
				action = delegate
				{
					gestateProgress = Props.hatcherDaystoHatch * 60000f;
				}
			};
		}
	}

	public override string CompInspectStringExtra()
	{
		string text = string.Empty;
		if (Prefs.DevMode)
		{
			if (hatcheeFaction != null)
			{
				text = text + "Faction:" + hatcheeFaction;
			}
			if (hatcheeParent != null)
			{
				text = text + "; Mother:" + hatcheeParent;
			}
			if (otherParent != null)
			{
				text = text + "; Father:" + otherParent;
			}
		}
		if (!TemperatureDamaged)
		{
			text += "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent();
		}
		if (text.NullOrEmpty())
		{
			return null;
		}
		return text;
	}
}
