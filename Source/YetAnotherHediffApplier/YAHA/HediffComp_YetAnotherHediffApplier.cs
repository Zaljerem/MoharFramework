using System.Collections.Generic;
using Verse;

namespace YAHA;

public class HediffComp_YetAnotherHediffApplier : HediffComp
{
	private Map myMap;

	public List<AssociatedHediffHistory> Registry = new List<AssociatedHediffHistory>();

	public bool ShouldSkip;

	public bool TriggeredOnlyHediffs;

	public List<TriggerEvent> WoundTriggers = new List<TriggerEvent>();

	public int UnspawnedGrace;

	public int UpdateNumthisTick;

	public HediffCompProperties_YetAnotherHediffApplier Props => (HediffCompProperties_YetAnotherHediffApplier)props;

	public bool MyDebug => Props.debug;

	public bool HasWoundTrigger => !WoundTriggers.NullOrEmpty();

	public bool HasEmptyRegistry => Registry.NullOrEmpty();

	public bool HasRegistry => !HasEmptyRegistry;

	public override string CompTipStringExtra
	{
		get
		{
			string text = string.Empty;
			if (Props.debug)
			{
				text = text + base.Pawn.PawnResumeString() + (Props.associations.NullOrEmpty() ? "empty association" : (Props.associations.Count + " hediff associations")) + (Registry.NullOrEmpty() ? "empty registry" : (Registry.Count + " registered hediff associations"));
			}
			return text;
		}
	}

	public override void CompPostMake()
	{
		Init();
	}

	public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
	{
		base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
		this.WoundTriggerManager(dinfo);
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		UpdateNumthisTick = 0;
		checked
		{
			if (UnspawnedGrace > 0)
			{
				UnspawnedGrace--;
				if (UnspawnedGrace == 0)
				{
					ShouldSkip = false;
				}
			}
			if (ShouldSkip)
			{
				return;
			}
			if (base.Pawn.Spawned)
			{
				if (myMap == null)
				{
					Init();
					return;
				}
				this.DidNothing();
				if (!TriggeredOnlyHediffs && Props.PeriodicCheck && base.Pawn.IsHashIntervalTick(Props.checkFrequency))
				{
					this.CheckAllHediffAssociations();
				}
			}
			else
			{
				if (MyDebug)
				{
					Log.Warning(string.Concat(base.Pawn.Name, " : pawn unspawned - Entering grace"));
				}
				UnspawnedGrace = Props.UnspawnedGrace;
				ShouldSkip = true;
			}
		}
	}

	public void Init()
	{
		if (MyDebug)
		{
			Log.Warning("Entering Init");
		}
		myMap = base.Pawn.Map;
		if (!base.Pawn.Spawned)
		{
			if (MyDebug)
			{
				Log.Warning("Null map");
			}
			return;
		}
		this.CreateRegistry();
		this.SetTriggerOnly();
		this.SetHasAnyWoundTrigger();
		if (MyDebug)
		{
			Log.Warning("HasWoundTrigger:" + HasWoundTrigger);
			Log.Warning("TriggeredOnlyHediffs:" + TriggeredOnlyHediffs);
		}
		this.CheckAllHediffAssociations();
	}

	public override void CompExposeData()
	{
		base.CompExposeData();
		Scribe_Values.Look(ref TriggeredOnlyHediffs, "TriggeredOnlyHediffs", defaultValue: false);
		Scribe_Collections.Look(ref WoundTriggers, "WoundTriggers", LookMode.Undefined);
		if (Scribe.mode == LoadSaveMode.PostLoadInit && WoundTriggers == null)
		{
			WoundTriggers = new List<TriggerEvent>();
		}
		Scribe_Collections.Look(ref Registry, "Registry", LookMode.Undefined);
		if (Scribe.mode == LoadSaveMode.PostLoadInit && Registry == null)
		{
			Registry = new List<AssociatedHediffHistory>();
		}
	}
}
