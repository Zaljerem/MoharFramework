using RimWorld;
using Verse;

namespace Ubet;

public static class NoArgConditionMethods
{
	public static bool PawnIsHuman(Pawn p)
	{
		return p.def == ThingDefOf.Human;
	}

	public static bool PawnIsMale(Pawn p)
	{
		return p.gender == Gender.Male;
	}

	public static bool PawnIsFemale(Pawn p)
	{
		return p.gender == Gender.Female;
	}

	public static bool PawnIsDrafted(Pawn p)
	{
		return p.Drafted;
	}

	public static bool PawnIsUndrafted(Pawn p)
	{
		return !p.Drafted;
	}

	public static bool PawnIsInBed(Pawn p)
	{
		return p.InBed();
	}

	public static bool PawnIsInLoveBed(Pawn p)
	{
		Building_Bed building_Bed = p.CurrentBed();
		if (building_Bed == null)
		{
			return false;
		}
		for (int i = 0; i < building_Bed.OwnersForReading.Count; i++)
		{
			if (LovePartnerRelationUtility.LovePartnerRelationExists(p, building_Bed.OwnersForReading[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool PawnIsInMedicalBed(Pawn p)
	{
		return p.CurrentBed()?.Medical ?? false;
	}

	public static bool PawnIsFromPlayerFaction(Pawn p)
	{
		if (p.Faction == null)
		{
			return false;
		}
		return p.Faction.IsPlayer;
	}

	public static bool PawnIsInMentalState(this Pawn p)
	{
		return p.MentalState != null;
	}

	public static bool PawnUsesNoWeapon(this Pawn p)
	{
		if (p.equipment == null)
		{
			return false;
		}
		return p.equipment.Primary.DestroyedOrNull();
	}
}
