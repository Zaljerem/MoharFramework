using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class TrailUtils
{
	public static float GetMoteRotation(this HediffComp_TrailLeaver comp, Vector3 drawPos, out Vector3 normalized)
	{
		normalized = Vector3.zero;
		if (!comp.Props.dynamicRotation && !comp.Props.UsesFootPrints)
		{
			return 0f;
		}
		float dynamicRotation = comp.GetDynamicRotation(drawPos, out normalized);
		float num = (comp.Props.dynamicRotation ? dynamicRotation : 0f);
		num += (comp.Props.HasRotationOffset ? comp.Props.rotationOffset : 0f);
		return num % 360f;
	}

	public static float GetDynamicRotation(this HediffComp_TrailLeaver comp, Vector3 drawPos, out Vector3 normalized)
	{
		normalized = (drawPos - comp.lastMotePos).normalized;
		return normalized.AngleFlat();
	}

	public static Vector3 GetFootPrintOffset(this HediffComp_TrailLeaver comp, Vector3 normalized)
	{
		if (!comp.Props.UsesFootPrints)
		{
			return Vector3.zero;
		}
		float angle = (comp.lastFootprintRight ? 90 : (-90));
		Vector3 vector = normalized.RotatedBy(angle) * comp.Props.footprint.distanceBetweenFeet * Mathf.Sqrt(comp.Pawn.BodySize);
		comp.lastFootprintRight = !comp.lastFootprintRight;
		return comp.Props.footprint.offset + vector;
	}

	public static void RecordMotePos(this HediffComp_TrailLeaver comp, Vector3 drawPos)
	{
		if (comp.Props.dynamicRotation)
		{
			comp.lastMotePos = drawPos;
		}
	}
}
