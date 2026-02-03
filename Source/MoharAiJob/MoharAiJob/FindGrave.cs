using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoharAiJob;

public static class FindGrave
{
	private static bool ValidateGrave(Thing t, Pawn worker, GraveSpecification GS, bool myDebug = false)
	{
		string text = (myDebug ? "ValidateGrave - " : "");
		Faction pFaction = worker.Faction;
		Map map = worker.Map;
		if (t.NegligibleThing())
		{
			if (myDebug)
			{
				Log.Warning(text + "negligible thing");
			}
			return false;
		}
		if (myDebug)
		{
			text = text + t.ThingID + " ";
		}
		if (!(t is Building))
		{
			if (myDebug)
			{
				Log.Warning(text + "is not building");
			}
			return false;
		}
		if (GS.HasEligibleGraves && !GS.eligibleGraves.Contains(t.def))
		{
			if (myDebug)
			{
				Log.Warning(text + "is not within allowed categories");
			}
			return false;
		}
		if (t is Building_Casket building_Casket)
		{
			if (!building_Casket.HasAnyContents)
			{
				if (myDebug)
				{
					Log.Warning(text + "is casket but empty");
				}
				return false;
			}
			if (!(building_Casket.ContainedThing is Corpse))
			{
				if (myDebug)
				{
					Log.Warning(text + "is casket but contains no corpse");
				}
				return false;
			}
			if (map != null && GS.HasReservation && GS.reservation.respectsThingReservation)
			{
				if (myDebug)
				{
					Log.Warning(text + " checking reservations");
				}
				LocalTargetInfo LTI = new LocalTargetInfo(t);
				if (!(from r in map.reservationManager.ReservationsReadOnly
					where r.Target == LTI
					where GS.reservation.respectsPawnKind && r.Claimant.kindDef == worker.kindDef
					where GS.reservation.respectsFaction && r.Claimant.Faction == pFaction
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
			return true;
		}
		if (myDebug)
		{
			Log.Warning(text + "is not a casket");
		}
		return false;
	}

	public static bool GetClosestCompatibleGrave(this Pawn pawn, GraveSpecification GS, out Thing grave, out Thing corpse, bool myDebug = false)
	{
		grave = null;
		corpse = null;
		if (pawn.NegligiblePawn())
		{
			return false;
		}
		grave = (Building)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Grave), PathEndMode.ClosestTouch, TraverseParms.For(pawn), GS.maxDistance, (Thing graveBuilding) => ValidateGrave(graveBuilding, pawn, GS, myDebug));
		if (grave is Building_Casket { HasAnyContents: not false, ContainedThing: Corpse containedThing })
		{
			corpse = containedThing;
		}
		return grave != null && corpse != null;
	}
}
