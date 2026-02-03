using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoharAiJob;

public class AiCorpse_Consume_JobDriver : JobDriver
{
	[DefOf]
	public class ConsumeCorpseDefaultDefOf
	{
		public static EffecterDef ButcherFlesh;

		public static SoundDef Recipe_Surgery;
	}

	private bool MyDebug = false;

	private readonly bool MyCsDebug = false;

	private readonly string DebugStr = "MoharAiJob.AiCorpse_Consume_JobDriver ";

	private const int DefaultWorkAmount = 300;

	private const int DefaultNibblingPeriod = 120;

	private const float DefaultNibblingAmount = 1f;

	public CorpseRecipeSettings RetrievedCRS = null;

	private TargetIndex CorpseInd => TargetIndex.A;

	private Corpse Corpse => (Corpse)job.GetTarget(TargetIndex.A).Thing;

	private Thing Target => Corpse;

	private bool NibblingRequired => NibblingAmount != 1f;

	public bool HasRCRCS => RetrievedCRS != null;

	public CorpseProduct RetrievedCorpseProduct => (HasRCRCS && RetrievedCRS.HasProductSpec) ? RetrievedCRS.product : null;

	public bool HasCorpseProduct => RetrievedCorpseProduct != null;

	public WorkFlow RetrievedWorkFlow => (HasRCRCS && RetrievedCRS.HasWorkFlow) ? RetrievedCRS.workFlow : null;

	public bool HasWorkFlow => RetrievedWorkFlow != null;

	public int WorkAmount
	{
		get
		{
			if (HasWorkFlow && RetrievedWorkFlow.HasWorkAmountPerHS)
			{
				return (int)((float)RetrievedWorkFlow.workAmountPerHealthScale * Corpse.InnerPawn.RaceProps.baseHealthScale);
			}
			return 300;
		}
	}

	public int NibblingPeriod
	{
		get
		{
			if (HasWorkFlow && RetrievedWorkFlow.HasNibblingPeriodPerHS)
			{
				return (int)((float)RetrievedWorkFlow.nibblingPeriodPerHealthScale * Corpse.InnerPawn.RaceProps.baseHealthScale);
			}
			return 120;
		}
	}

	public float NibblingAmount => (HasWorkFlow && RetrievedWorkFlow.HasNibblingAmount) ? RetrievedWorkFlow.nibblingAmount : 1f;

	public SoundDef MySustainSound => (HasWorkFlow && RetrievedWorkFlow.HasCustomSustainSound) ? RetrievedWorkFlow.sustainSound : ConsumeCorpseDefaultDefOf.Recipe_Surgery;

	public EffecterDef MyEffecterDef => (HasWorkFlow && RetrievedWorkFlow.HasCustomEffecterDef) ? RetrievedWorkFlow.effecterDef : ConsumeCorpseDefaultDefOf.ButcherFlesh;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		if (CheckAndFillCorpseProduct() && RetrievedCRS.HasTargetSpec && RetrievedCRS.target.HasReservationProcess && RetrievedCRS.target.reservation.reserves)
		{
			bool flag = pawn.Reserve(base.TargetA, job);
			if (MyDebug)
			{
				Log.Warning(string.Concat(pawn, " reserved ", Corpse.ThingID, ":", flag.ToString()));
			}
		}
		pawn.CurJob.count = 1;
		return true;
	}

	private bool CheckAndFillCorpseProduct()
	{
		if (HasCorpseProduct && HasWorkFlow)
		{
			return true;
		}
		if (MyCsDebug)
		{
			Log.Warning(DebugStr + " cant work without corpseProduct, Loading it for pawn:" + pawn?.ThingID + " corpse:" + Corpse?.ThingID);
		}
		CorpseJobDef cJD;
		if ((cJD = pawn.RetrieveCorpseJobDef(out MyDebug, MyCsDebug)) != null)
		{
			string meFunc = (MyCsDebug ? "CheckAndFillCorpseProduct" : string.Empty);
			IEnumerable<CorpseRecipeSettings> enumerable = from c in pawn.RetrieveCorpseRecipeSettings(cJD, MyDebug)
				where c.target.ValidateCorpse(Corpse, pawn, MyDebug, meFunc)
				select c;
			if (enumerable.EnumerableNullOrEmpty())
			{
				Log.Warning(DebugStr + "did not find CorpseRecipeSettings relative to Corpse " + Corpse.ThingID);
				return false;
			}
			if (MyDebug)
			{
				Log.Warning(meFunc + " found " + enumerable.Count() + " CRS");
			}
			RetrievedCRS = enumerable.FirstOrFallback();
			if (RetrievedCRS == null)
			{
				Log.Warning(DebugStr + "did not find CorpseRecipeSettings relative to Corpse (2)");
				return false;
			}
			if (MyDebug)
			{
				Log.Warning(DebugStr + " did it - OK");
			}
			return true;
		}
		return false;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		if (!CheckAndFillCorpseProduct())
		{
			if (MyCsDebug)
			{
				Log.Warning(DebugStr + " - Failed to initialize settings");
			}
			yield break;
		}
		this.FailOnDestroyedOrNull(CorpseInd);
		yield return Toils_Goto.GotoThing(CorpseInd, PathEndMode.Touch).FailOnDespawnedOrNull(CorpseInd);
		Toil toil = Toils_General.Wait(WorkAmount).FailOnCannotTouch(CorpseInd, PathEndMode.Touch).FailOnBurningImmobile(CorpseInd)
			.FailOnSomeonePhysicallyInteracting(CorpseInd)
			.WithEffect(MyEffecterDef, CorpseInd, null)
			.PlaySustainerOrSound(MySustainSound);
		toil.tickAction = delegate
		{
			if (NibblingRequired && pawn.IsHashIntervalTick(NibblingPeriod))
			{
				Corpse.HitPoints = (int)((float)Corpse.HitPoints * NibblingAmount);
				RetrievedWorkFlow.SpawnFilth(Corpse, pawn.Map, MyDebug);
			}
		};
		yield return toil;
		yield return RetrievedCRS.SpawnProductDespawnCorpse(pawn, Corpse, MyDebug);
	}
}
