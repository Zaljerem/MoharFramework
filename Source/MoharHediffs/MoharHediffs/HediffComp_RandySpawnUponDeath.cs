using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharHediffs;

public class HediffComp_RandySpawnUponDeath : HediffComp
{
	private bool blockSpawn = false;

	private int RandomIndex = -1;

	public Faction RandomFaction = null;

	public MentalStateDef RandomMS = null;

	private int RandomQuantity = 0;

	public readonly float minDaysB4NextErrorLimit = 0.001f;

	public readonly int spawnCountErrorLimit = 750;

	public Pawn_SkillTracker rememberSkillTracker = null;

	public int lastSkillUpdateTick = -1;

	public HediffCompProperties_RandySpawnUponDeath Props => (HediffCompProperties_RandySpawnUponDeath)props;

	public bool MyDebug => Props.debug;

	public bool ValidIndex => RandomIndex != -1 && Props.settings.HasSomethingToSpawn && RandomIndex < NumberOfItems;

	public bool ValidQuantity => RandomQuantity > 0;

	public ThingSettings ChosenItem => ValidIndex ? Props.settings.things[RandomIndex] : null;

	public bool HasRequirement => Props.HasRequirements && Props.requirements.HasAtLeastOneRequirementSetting;

	public bool HasHediffRequirement => Props.HasRequirements && Props.requirements.HasHediffRequirement;

	public bool HasThingRequirement => Props.HasRequirements && Props.requirements.HasThingRequirement;

	public bool HasCustomSpawn => HasThingRequirement && Props.requirements.thing.Any((ThingRequirementSettings t) => t.HasCustomSpawn);

	public bool HasContainerSpawn => HasThingRequirement && Props.requirements.thing.Any((ThingRequirementSettings t) => t.HasContainerSpawn);

	public bool HasChosenThing => ChosenItem != null && ChosenItem.thingToSpawn != null;

	public bool HasChosenPawn => ChosenItem != null && (ChosenItem.pawnKindToSpawn != null || IsParentPawnKindCopier);

	public bool IsParentPawnKindCopier => ChosenItem.IsCopier && ChosenItem.copyParent.pawnKind;

	public bool PrecedentIterationsExclusion => Props.excludeAlreadyPickedOptions;

	public bool HasColorCondition => Props.settings.things.Any((ThingSettings t) => t.HasColorCondition);

	public ThingDef ThingOfChoice => HasChosenThing ? ChosenItem.thingToSpawn : null;

	public List<ThingSettings> FullOptionList => Props.settings.things;

	public PawnKindDef PawnOfChoice
	{
		get
		{
			if (!HasChosenPawn)
			{
				return null;
			}
			if (IsParentPawnKindCopier)
			{
				return base.Pawn.kindDef;
			}
			return ChosenItem.pawnKindToSpawn;
		}
	}

	public bool HasFilth => FilthToSpawn != null;

	public int NumberOfItems => Props.settings.things.Count;

	public int NumberToSpawn
	{
		get
		{
			if (HasChosenThing && ChosenItem.HasStackSettings)
			{
				return ChosenItem.specificSettings.stack.spawnCount.RandomInRange;
			}
			if (Props.settings.HasDefaultSettings)
			{
				return Props.settings.defaultSettings.stack.spawnCount.RandomInRange;
			}
			return 1;
		}
	}

	public bool WeightedSpawn
	{
		get
		{
			if (HasChosenThing && ChosenItem.HasStackSettings)
			{
				return ChosenItem.specificSettings.stack.weightedSpawnCount;
			}
			if (Props.settings.HasDefaultSettings)
			{
				return Props.settings.defaultSettings.stack.weightedSpawnCount;
			}
			return false;
		}
	}

	public ThingDef FilthToSpawn
	{
		get
		{
			if (HasChosenThing && ChosenItem.HasFilthSettings)
			{
				return ChosenItem.specificSettings.filth.filthDef;
			}
			if (Props.settings.HasDefaultSettings)
			{
				return Props.settings.defaultSettings.filth.filthDef;
			}
			return null;
		}
	}

	public FloatRange FilthRadius
	{
		get
		{
			if (HasChosenThing && ChosenItem.HasFilthSettings)
			{
				return ChosenItem.specificSettings.filth.filthRadius;
			}
			if (Props.settings.HasDefaultSettings)
			{
				return Props.settings.defaultSettings.filth.filthRadius;
			}
			return new FloatRange(0f, 1f);
		}
	}

	public IntRange FilthNum
	{
		get
		{
			if (HasChosenThing && ChosenItem.HasFilthSettings)
			{
				return ChosenItem.specificSettings.filth.filthNum;
			}
			if (Props.settings.HasDefaultSettings)
			{
				return Props.settings.defaultSettings.filth.filthNum;
			}
			return new IntRange(0, 0);
		}
	}

	public override void CompPostMake()
	{
		if (MyDebug)
		{
			Log.Warning(">>> " + base.Pawn?.Label + " - " + parent.def.defName + " - CompPostMake start");
		}
	}

	public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
	{
		string text = (MyDebug ? (base.Pawn.LabelShort + " HediffComp_RandySpawnUponDeath Notify_PawnDied") : "");
		if (MyDebug)
		{
			Log.Warning(text + " Entering");
		}
		bool flag = false;
		if (base.Pawn.Corpse.Negligible())
		{
			if (MyDebug)
			{
				Log.Warning(text + " Corpse is no more, cant find its position - giving up");
			}
			flag = true;
		}
		if (blockSpawn)
		{
			if (MyDebug)
			{
				Log.Warning(text + " blockSpawn for some reason- giving up");
			}
			flag = true;
		}
		if (!this.FulfilsRequirement(out var closestThing))
		{
			if (MyDebug)
			{
				Log.Warning(text + "not Fulfiling requirements- giving up");
			}
			flag = true;
		}
		if (flag)
		{
			base.Notify_PawnDied(dinfo, culprit);
			return;
		}
		int randomInRange = Props.iterationRange.RandomInRange;
		List<int> list = new List<int>();
		if (MyDebug)
		{
			Log.Warning(text + "iterationNum: " + randomInRange);
		}
		checked
		{
			for (int i = 0; i < randomInRange; i++)
			{
				if (MyDebug)
				{
					Log.Warning(text + " Trying to spawn " + i + "/" + (randomInRange - 1));
				}
				if (!DiceThrow(list))
				{
					if (MyDebug)
					{
						Log.Warning(text + " DiceThrow wrong results");
					}
					base.Notify_PawnDied(dinfo, culprit);
					return;
				}
				if (MyDebug)
				{
					Log.Warning(text + " index: " + RandomIndex + " quantity: " + RandomQuantity + " nature: " + ChosenItem.ItemDump);
				}
				if (PrecedentIterationsExclusion)
				{
					list.Add(RandomIndex);
				}
				if (CheckShouldSpawn(closestThing) && MyDebug)
				{
					Log.Warning(text + " Spawn " + i + "/" + (randomInRange - 1) + " occured  nature: t:" + ChosenItem.ItemDump);
				}
				if (MyDebug)
				{
					Log.Warning("#################");
				}
			}
			if (CheckShouldHandleCorpse() && MyDebug)
			{
				Log.Warning(text + " Corpse handled");
			}
			base.Notify_PawnDied(dinfo, culprit);
		}
	}

	public bool DiceThrow(List<int> AlreadyPickedOptions)
	{
		RandomIndex = this.GetWeightedRandomIndex(AlreadyPickedOptions);
		if (HasChosenPawn && ChosenItem.HasFactionParams)
		{
			this.ComputeRandomFaction();
		}
		RandomQuantity = this.ComputeSpawnCount();
		if (!ValidIndex)
		{
			BlockAndDestroy(">ERROR< failed to find an index for IP, check and adjust your hediff props", MyDebug);
			return false;
		}
		if (!ValidQuantity)
		{
			if (MyDebug)
			{
				Log.Warning("random quantity: " + RandomQuantity + " - impossible to spawn anything");
			}
			return false;
		}
		return true;
	}

	public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
	{
		if (MyDebug && !ErrorLog.NullOrEmpty())
		{
			Log.Warning(ErrorLog);
		}
		blockSpawn = true;
		Tools.DestroyParentHediff(parent, myDebug);
	}

	private bool CheckShouldSpawn(Thing closestThing)
	{
		if (MyDebug)
		{
			Log.Warning(base.Pawn.LabelShort + " CheckShouldSpawn");
		}
		if (MyDebug)
		{
			Log.Warning(string.Concat(" Trying to spawn ", RandomQuantity, " ", ThingOfChoice, "/", PawnOfChoice));
		}
		Thing thing = (HasCustomSpawn ? closestThing : base.Pawn.Corpse);
		bool result = this.TryDoSpawn(thing, RandomQuantity);
		if (MyDebug)
		{
			Log.Warning("TryDoSpawn: " + result);
		}
		return result;
	}

	private bool CheckShouldHandleCorpse()
	{
		Corpse corpse = base.Pawn.Corpse;
		bool flag = false;
		flag |= this.StripCorpse(corpse);
		return flag | this.DestroyCorpse(corpse);
	}
}
