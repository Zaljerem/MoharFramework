using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Ubet;

public static class TwoStringArgConditionMethods
{
	public static bool PawnUsesSpecificWeaponMadeOf(Pawn p, List<string> WeaponDef, List<string> Stuff)
	{
		if (p.equipment == null || p.equipment.Primary.DestroyedOrNull())
		{
			return false;
		}
		ThingWithComps primary = p.equipment.Primary;
		return WeaponDef.Contains(primary.def.defName) && p.ThingIsMadeOfStuff(Stuff);
	}

	public static bool PawnWearsSpecificApparelMadeOf(Pawn p, List<string> ApparelDef, List<string> Stuff)
	{
		if (p.apparel == null || p.apparel.WornApparelCount == 0)
		{
			return false;
		}
		return p.apparel.WornApparel.Any((Apparel a) => ApparelDef.Contains(a.def.defName) && a.ThingIsMadeOfStuff(Stuff));
	}

	public static bool PawnDoingBill(Pawn p, List<string> BuildingDef, List<string> RecipeDef)
	{
		if (p.CurJob == null)
		{
			return false;
		}
		if (p.CurJobDef != JobDefOf.DoBill)
		{
			return false;
		}
		Thing thing;
		if ((thing = p.CurJob.targetA.Thing) == null)
		{
			return false;
		}
		Building building;
		if ((building = (Building)thing) == null)
		{
			return false;
		}
		if (!BuildingDef.Contains(building.def.defName))
		{
			return false;
		}
		if (p.Position != building.InteractionCell)
		{
			return false;
		}
		if (!RecipeDef.NullOrEmpty() && !RecipeDef.Contains(p.CurJob.RecipeDef?.defName))
		{
			return false;
		}
		return true;
	}

	public static bool PawnIsPerformingTouchJob(Pawn p, List<string> JobDef, List<string> BuildingDef)
	{
		if (p.CurJob == null)
		{
			return false;
		}
		if (!JobDef.Contains(p.CurJob.def.defName))
		{
			return false;
		}
		if (p.CurJob.targetA == null)
		{
			return false;
		}
		Thing thing;
		if ((thing = p.CurJob.targetA.Thing) == null)
		{
			return false;
		}
		Building building;
		if ((building = (Building)thing) == null)
		{
			return false;
		}
		if (!BuildingDef.Contains(building.def.defName))
		{
			return false;
		}
		if (p.Position.DistanceTo(building.Position) >= 1.5f)
		{
			return false;
		}
		return true;
	}
}
