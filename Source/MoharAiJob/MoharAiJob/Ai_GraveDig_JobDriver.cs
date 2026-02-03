using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoharAiJob;

public class Ai_GraveDig_JobDriver : JobDriver
{
	[DefOf]
	public class GraveDigDefaultDefOf
	{
		public static SoundDef Tunnel;
	}

	private bool MyDebug = false;

	private const TargetIndex GraveInd = TargetIndex.A;

	private const TargetIndex CorpseInd = TargetIndex.B;

	private const TargetIndex CellInd = TargetIndex.C;

	private const int DefaultWorkAmount = 300;

	private const int DefaultDustPeriod = 50;

	public GraveDig_JobParameters RetrievedGDJP = null;

	public bool MyCsDebug => false;

	private Building GraveBuilding => (Building)job.GetTarget(TargetIndex.A).Thing;

	private Corpse CorpseThing => (Corpse)job.GetTarget(TargetIndex.B).Thing;

	private Thing Target => GraveBuilding;

	public bool HasGDJP => RetrievedGDJP != null;

	public GraveDigWorkFlow RetrievedWorkFlow => (HasGDJP && RetrievedGDJP.HasWorkFlow) ? RetrievedGDJP.workFlow : null;

	public bool HasWorkFlow => RetrievedWorkFlow != null;

	public float SpeedFactor => (GraveBuilding.Stuff == null) ? 1f : GraveBuilding.Stuff.GetStatValueAbstract(StatDefOf.DoorOpenSpeed);

	public int WorkAmount
	{
		get
		{
			if (HasWorkFlow && RetrievedWorkFlow.HasRelevantWorkAmount)
			{
				return RetrievedWorkFlow.workAmountDoorOpenSpeedWeighted ? ((int)((float)RetrievedWorkFlow.workAmount * SpeedFactor)) : RetrievedWorkFlow.workAmount;
			}
			return 300;
		}
	}

	public SoundDef MySustainSound => (HasWorkFlow && RetrievedWorkFlow.HasCustomSustainSound) ? RetrievedWorkFlow.sustainSound : GraveDigDefaultDefOf.Tunnel;

	public int MyDustPeriod => (HasWorkFlow && RetrievedWorkFlow.HasRelevantDustPeriod) ? RetrievedWorkFlow.dustPeriod : 50;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		if (CheckAndFillWorkFlow() && RetrievedGDJP.HasTargetSpec && RetrievedGDJP.target.HasReservation && RetrievedGDJP.target.reservation.reserves)
		{
			bool flag = pawn.Reserve(base.TargetA, job);
			if (MyDebug)
			{
				Log.Warning(string.Concat(pawn, " reserved ", GraveBuilding.ThingID, ":", flag.ToString()));
			}
		}
		pawn.CurJob.count = 1;
		return true;
	}

	private bool CheckAndFillWorkFlow()
	{
		if (!HasGDJP)
		{
			if (MyCsDebug || MyDebug)
			{
				Log.Warning("Ai_GraveDig_JobDriver cant work without workflow, Loading it");
			}
			GraveDiggerDef gDD;
			GraveDig_JobParameters retrievedGDJP;
			if ((gDD = pawn.RetrieveGDD(out MyDebug, MyCsDebug)) != null && (retrievedGDJP = pawn.RetrieveGDJP(gDD, MyDebug)) != null)
			{
				RetrievedGDJP = retrievedGDJP;
				return true;
			}
			return false;
		}
		return true;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		if (!CheckAndFillWorkFlow())
		{
			if (MyCsDebug)
			{
				Log.Warning("Ai_GraveDig_JobDriver - Failed to initialize settings");
			}
			yield break;
		}
		this.FailOnDestroyedOrNull(TargetIndex.A);
		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
		this.FailOnDestroyedOrNull(TargetIndex.A);
		Toil toil = Toils_General.Wait(WorkAmount).FailOnDespawnedOrNull(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
			.PlaySustainerOrSound(MySustainSound);
		toil.tickAction = delegate
		{
			if (pawn.IsHashIntervalTick(MyDustPeriod))
			{
				if (MyDebug)
				{
					Log.Warning("Ai_GraveDig_JobDriver - time to puff");
				}
				ThrowDigMote(pawn.Position.BetweenTouchingCells(Target.Position), pawn.Map);
			}
		};
		yield return toil;
		yield return Toils_General.Open(TargetIndex.A);
		this.FailOnDestroyedOrNull(TargetIndex.B);
		Toil gotoCorpse = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B);
		yield return Toils_Reserve.Reserve(TargetIndex.B);
		yield return gotoCorpse;
		yield return Toils_Haul.StartCarryThing(TargetIndex.B);
		yield return FindCellToDropCorpseToil();
		yield return Toils_Reserve.Reserve(TargetIndex.C);
		yield return Toils_Goto.GotoCell(TargetIndex.C, PathEndMode.Touch);
		yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, null, storageMode: false);
		yield return Forbid();
		if (RetrievedWorkFlow.HasJobGiverToChain)
		{
			yield return ChainJob();
		}
	}

	private Toil ChainJob()
	{
		return new Toil
		{
			initAction = delegate
			{
				ThinkNode_JobGiver thinkNode_JobGiver;
				if ((thinkNode_JobGiver = RetrievedWorkFlow.tryToChainJobGiver.FirstOrFallback()) != null)
				{
					if (MyDebug)
					{
						Log.Warning("trying to thinknode: " + thinkNode_JobGiver.ToString());
					}
					ThinkResult thinkResult = thinkNode_JobGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
					if (MyDebug)
					{
						Log.Warning(thinkResult.ToString());
					}
					if (thinkResult != ThinkResult.NoJob)
					{
						pawn.jobs.jobQueue.EnqueueFirst(thinkResult.Job, null);
					}
					else if (MyDebug)
					{
						Log.Warning("failed to chain job");
					}
				}
				else if (MyDebug)
				{
					Log.Warning("no thinknode");
				}
			}
		};
	}

	private Toil FindCellToDropCorpseToil()
	{
		return new Toil
		{
			initAction = delegate
			{
				IntVec3 invalid = IntVec3.Invalid;
				invalid = CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 10, (IntVec3 x) => pawn.CanReserve(x) && x.GetFirstItem(pawn.Map) == null);
				job.SetTarget(TargetIndex.C, invalid);
			},
			atomicWithPrevious = true
		};
	}

	private Toil Forbid()
	{
		return new Toil
		{
			initAction = delegate
			{
				CorpseThing?.SetForbidden(value: true);
			},
			atomicWithPrevious = true
		};
	}

	public static void ThrowDigMote(Vector3 loc, Map map)
	{
		if (map.AllowedMoteSpawn(loc))
		{
			FleckMaker.ThrowDustPuffThick(loc, map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
		}
	}
}
