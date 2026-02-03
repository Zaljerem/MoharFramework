using RimWorld;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class MoteLink
{
	public enum Nature
	{
		head,
		body
	}

	public static Vector3 GetLinkOffset(this Pawn p, Nature linkType)
	{
		return linkType switch
		{
			Nature.head => p.Drawer.renderer.BaseHeadOffsetAt((p.GetPosture() == PawnPosture.Standing) ? Rot4.North : p.Drawer.renderer.LayingFacing()).RotatedBy(p.Drawer.renderer.BodyAngle(PawnRenderFlags.Cache)), 
			_ => Vector3.zero, 
		};
	}
}
