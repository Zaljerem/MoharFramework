using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class InnerShinerUtils
{
	public static InnerShineItem RetrieveISI(this HediffComp_InnerShine comp, string label)
	{
		InnerShineItem result;
		if (comp.Props.HasRawShinePool && (result = comp.Props.innerShinePool.Where((InnerShineItem i) => i.label == label).FirstOrFallback()) != null)
		{
			return result;
		}
		InnerShineDef innerShineDef;
		if (comp.Props.HasShineDefPool && (innerShineDef = comp.Props.innerShineDefPool.Where((InnerShineDef i) => i.item.label == label).FirstOrFallback()) != null)
		{
			return innerShineDef.item;
		}
		return null;
	}

	public static void SelfDestroy(this HediffComp_InnerShine comp)
	{
		comp.parent.Severity = 0f;
		comp.Pawn.health.RemoveHediff(comp.parent);
	}

	public static void ChangeMoteColor(this InnerShineItem ISI, InnerShineRecord ISR, Mote mote)
	{
		if (ISI.HasColorRange && mote != null)
		{
			if (ISR.lastColor == Color.black)
			{
				ISR.lastColor = ISI.colorRange.colorA;
			}
			ISR.lastColor = ISI.colorRange.RandomPickColor(ISR.lastColor, ISI.debug);
			mote.instanceColor = ISR.lastColor;
		}
	}

	public static void GetSpecifities(this InnerShineItem ISI, Pawn p, out Vector3 offset, out float scale)
	{
		offset = Vector3.zero;
		scale = 1f;
		if (p.story?.bodyType == null || !ISI.HasBodyTypeDrawRules)
		{
			if (ISI.HasDefaultDrawRules)
			{
				offset = ISI.defaultDrawRules.GetRotationOffset(p);
				scale = ISI.defaultDrawRules.randomScale.RandomInRange;
			}
			return;
		}
		BodyTypeSpecificities bodyTypeSpecificities = ISI.bodyTypeDrawRules.Where((BodyTypeSpecificities b) => b.bodyTypeDef == p.story.bodyType).FirstOrFallback();
		if (bodyTypeSpecificities == null)
		{
			if (ISI.HasDefaultDrawRules)
			{
				offset = ISI.defaultDrawRules.GetRotationOffset(p);
				scale = ISI.defaultDrawRules.randomScale.RandomInRange;
			}
		}
		else
		{
			offset = bodyTypeSpecificities.drawRules.GetRotationOffset(p);
			scale = bodyTypeSpecificities.drawRules.randomScale.RandomInRange;
		}
	}

	public static bool ShouldSpawnMote(this InnerShineItem ISI, InnerShineRecord ISR, Pawn p)
	{
		if (!ISI.HasCompatibleActivity(p))
		{
			return false;
		}
		if (ISI.HasMoteNumLimit())
		{
			return !ISR.AlreadyReachedMax(ISI.spawningRules.spawnedMax);
		}
		return true;
	}

	public static Vector3 GetDrawOffset(this InnerShineItem ISI, Pawn p)
	{
		if (p.story?.bodyType == null || !ISI.HasBodyTypeDrawRules)
		{
			if (ISI.HasDefaultDrawRules)
			{
				return ISI.defaultDrawRules.GetRotationOffset(p);
			}
			return Vector3.zero;
		}
		return ISI.bodyTypeDrawRules.Where((BodyTypeSpecificities b) => b.bodyTypeDef == p.story.bodyType).FirstOrFallback()?.drawRules.GetRotationOffset(p) ?? (ISI.HasDefaultDrawRules ? ISI.defaultDrawRules.GetRotationOffset(p) : Vector3.zero);
	}

	public static bool AlreadyReachedMax(this InnerShineRecord ISR, int max)
	{
		if (ISR.spawned.NullOrEmpty())
		{
			return false;
		}
		return ISR.spawned.Count() >= max;
	}

	public static bool HasCompatibleActivity(this InnerShineItem ISI, Pawn p)
	{
		if (!ISI.HasRestriction)
		{
			return true;
		}
		ActivityRestriction restriction = ISI.restriction;
		if (restriction.HasPostureRestriction && !restriction.allowedPostures.Contains(p.GetPosture()))
		{
			return false;
		}
		if (restriction.HasJobRestriction && p.CurJob != null && !restriction.allowedJobs.Contains(p.CurJob.def))
		{
			return false;
		}
		if (restriction.HasAllowedRotation && !restriction.allowedRotation.Contains(p.Rotation))
		{
			return false;
		}
		return true;
	}
}
