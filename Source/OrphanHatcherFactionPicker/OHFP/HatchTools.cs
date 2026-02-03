using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OHFP;

public static class HatchTools
{
	public static bool MyTrySpawnHatchedOrBornPawn(this Thing motherOrEgg, Pawn pawn)
	{
		if (motherOrEgg.SpawnedOrAnyParentSpawned)
		{
			return GenSpawn.Spawn(pawn, motherOrEgg.PositionHeld, motherOrEgg.MapHeld) != null;
		}
		if (motherOrEgg is Pawn pawn2)
		{
			if (pawn2.IsCaravanMember())
			{
				pawn2.GetCaravan().AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny: true);
				Find.WorldPawns.PassToWorld(pawn);
				return true;
			}
			if (pawn2.IsWorldPawn())
			{
				Find.WorldPawns.PassToWorld(pawn);
				return true;
			}
		}
		else if (motherOrEgg.ParentHolder != null && motherOrEgg.ParentHolder is Pawn_InventoryTracker pawn_InventoryTracker)
		{
			if (pawn_InventoryTracker.pawn.IsCaravanMember())
			{
				pawn_InventoryTracker.pawn.GetCaravan().AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny: true);
				Find.WorldPawns.PassToWorld(pawn);
				return true;
			}
			if (pawn_InventoryTracker.pawn.IsWorldPawn())
			{
				Find.WorldPawns.PassToWorld(pawn);
				return true;
			}
		}
		return false;
	}

	public static bool MakeManhunter(this Pawn p, bool MyDebug = false)
	{
		if (p.NegligiblePawn())
		{
			return false;
		}
		MentalStateDef mentalStateDef = null;
		mentalStateDef = MentalStateDefOf.Manhunter;
		Tools.Warn(p.LabelShort + " trying to go " + mentalStateDef.defName, MyDebug);
		string reason = "because ";
		if (p.mindState == null || p.mindState.mentalStateHandler == null)
		{
			Tools.Warn(p.LabelShort + " null mindstate", MyDebug);
			return false;
		}
		Tools.Warn(p.LabelShort + " got applied " + mentalStateDef.defName, MyDebug);
		p.mindState.mentalStateHandler.TryStartMentalState(mentalStateDef, reason, forced: true);
		return true;
	}

	public static void InheritParentSettings(this Pawn p, Pawn hatcheeParent, Faction hatcheeFaction)
	{
		if (p.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
		{
			p.playerSettings.AreaRestrictionInPawnCurrentMap = hatcheeParent.playerSettings.AreaRestrictionInPawnCurrentMap;
		}
	}

	public static void AddParentRelations(this Pawn p, Pawn hatcheeParent)
	{
		if (!p.RaceProps.IsMechanoid)
		{
			p.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
		}
	}

	public static void AddOtherParentRelations(this Pawn p, Pawn hatcheeParent, Pawn otherParent)
	{
		if (otherParent != null && (hatcheeParent == null || hatcheeParent.gender != otherParent.gender) && !p.RaceProps.IsMechanoid)
		{
			p.relations.AddDirectRelation(PawnRelationDefOf.Parent, otherParent);
		}
	}
}
