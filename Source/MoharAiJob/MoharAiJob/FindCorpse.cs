using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoharAiJob;

public static class FindCorpse
{
	private static float GetHitPointsPerc(this Thing t)
	{
		return (float)t.HitPoints / (float)t.MaxHitPoints;
	}

	public static bool ValidateCorpse(this CorpseSpecification CS, Thing t, Pawn worker, bool myDebug = false, string callingFunc = "")
	{
		Faction pFaction = worker.Faction;
		Map map = worker.Map;
		string text = (myDebug ? (t?.ThingID + " ValidateCorpse (" + callingFunc + ")- ") : string.Empty);
		if (myDebug)
		{
			Log.Warning(text + " Trying to validate ... NullMap:" + (map == null).ToString() + " mapId" + map?.uniqueID + "; Faction:" + (pFaction != null).ToString() + " FacN:" + pFaction?.GetCallLabel());
		}
		if (!(t is Corpse))
		{
			if (myDebug)
			{
				Log.Warning(text + " is not corpse");
			}
			return false;
		}
		if (CS.HasCorpseCategoryDef && !CS.categoryDef.Any((ThingCategoryDef tc) => t.def.IsWithinCategory(tc)))
		{
			if (myDebug)
			{
				Log.Warning(text + " is not within allowed categories");
			}
			return false;
		}
		if (CS.HasRelevantHealthPerc && !CS.healthPerc.Includes(t.GetHitPointsPerc()))
		{
			if (myDebug)
			{
				Log.Warning(text + " is not within allowed health range");
			}
			return false;
		}
		if (CS.HasRelevantMassPerc && !CS.mass.Includes(t.GetStatValue(StatDefOf.Mass)))
		{
			if (myDebug)
			{
				Log.Warning(text + "is not within allowed mass range");
			}
			return false;
		}
		if (CS.HasCorpseRotStages)
		{
			CompRottable compRottable = t.TryGetComp<CompRottable>();
			if (compRottable == null)
			{
				if (myDebug)
				{
					Log.Warning(text + " has no compRottable");
				}
				return false;
			}
			if (!CS.rotStages.Contains(compRottable.Stage))
			{
				if (myDebug)
				{
					Log.Warning(text + " has no desired rotStage");
				}
				return false;
			}
		}
		if (map != null && CS.HasReservationProcess && CS.reservation.respectsThingReservation)
		{
			if (myDebug)
			{
				Log.Warning(text + " checking reservations");
			}
			LocalTargetInfo LTI = new LocalTargetInfo(t);
			if (!(from r in map.reservationManager.ReservationsReadOnly
				where r.Target == LTI
				where r.Claimant != worker
				where CS.reservation.respectsPawnKind && r.Claimant.kindDef == worker.kindDef
				where CS.reservation.respectsFaction && r.Claimant.Faction == pFaction
				select r).EnumerableNullOrEmpty())
			{
				if (myDebug)
				{
					Log.Warning(text + "is reserved");
				}
				return false;
			}
			if (myDebug)
			{
				Log.Warning(text + " found no reservation for " + t);
			}
		}
		if (myDebug)
		{
			Log.Warning(text + "is valid - OK");
		}
		return true;
	}

	public static Corpse GetClosestCompatibleCorpse(this Pawn pawn, CorpseSpecification CS, bool myDebug = false)
	{
		string meFunc = (myDebug ? "GetClosestCompatibleCorpse" : string.Empty);
		if (pawn.NegligiblePawn())
		{
			return null;
		}
		return (Corpse)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Corpse), PathEndMode.ClosestTouch, TraverseParms.For(pawn), CS.maxDistance, (Thing corpse) => CS.ValidateCorpse(corpse, pawn, myDebug, meFunc));
	}
}
