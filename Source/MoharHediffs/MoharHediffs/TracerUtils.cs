using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class TracerUtils
{
	public static void CreateTracer(this HediffComp_InnerShine comp)
	{
		comp.Tracer = new List<InnerShineRecord>();
		if (comp.Props.HasRawShinePool)
		{
			foreach (InnerShineItem item in comp.Props.innerShinePool)
			{
				comp.Tracer.Add(new InnerShineRecord(item));
			}
		}
		if (comp.Props.HasShineDefPool)
		{
			foreach (InnerShineDef item2 in comp.Props.innerShineDefPool)
			{
				comp.Tracer.Add(new InnerShineRecord(item2.item));
			}
		}
		if (!comp.MyDebug)
		{
			return;
		}
		int num = 0;
		foreach (InnerShineRecord item3 in comp.Tracer)
		{
			Log.Warning(num.ToString("00") + "=>" + item3.Dump);
			num = checked(num + 1);
		}
	}

	public static int NewPeriod(this InnerShineItem ISI)
	{
		return ISI.spawningRules.period.RandomInRange;
	}

	public static void ResetTicks(this InnerShineItem ISI, InnerShineRecord ISR)
	{
		ISR.ticksLeft = ISI.NewPeriod();
	}

	public static bool HasMoteNumLimit(this InnerShineItem ISI)
	{
		return !ISI.spawningRules.IsUnlimited;
	}

	public static void TryPlaceMote(this InnerShineItem ISI, InnerShineRecord ISR, Pawn pawn)
	{
		if (!pawn.Position.InBounds(pawn.Map))
		{
			return;
		}
		float rot = 0f;
		ISI.InitSpecs(ISR, pawn, out var drawPosWithOffset, out var scale);
		if (drawPosWithOffset.ToIntVec3().InBounds(pawn.Map))
		{
			ThingDef moteDef = ISI.motePool.RandomElementWithFallback();
			if (drawPosWithOffset.TryAnyMoteSpawn(pawn.Map, rot, scale, moteDef, ISI.debug) is Mote mote)
			{
				ISI.ChangeMoteColor(ISR, mote);
				ISR.spawned.Add(mote);
				ISI.NewPeriod();
			}
		}
	}

	public static void UpdateMotes(this InnerShineItem ISI, InnerShineRecord ISR, Pawn pawn, bool debug = false)
	{
		if (ISR.spawned.NullOrEmpty())
		{
			return;
		}
		checked
		{
			for (int num = ISR.spawned.Count - 1; num >= 0; num--)
			{
				Thing thing = ISR.spawned[num];
				if (thing.DestroyedOrNull())
				{
					ISR.spawned.RemoveAt(num);
				}
				else if (!ISI.HasCompatibleActivity(pawn))
				{
					thing.Destroy();
					ISR.spawned.RemoveAt(num);
				}
				else if (thing is Mote mote)
				{
					mote.exactPosition = pawn.DrawPos + pawn.GetLinkOffset(ISI.linkType) + ISI.GetDrawOffset(pawn);
				}
			}
		}
	}

	public static void InitSpecs(this InnerShineItem ISI, InnerShineRecord ISR, Pawn pawn, out Vector3 drawPosWithOffset, out float scale)
	{
		Vector3 drawPos = pawn.DrawPos;
		ISI.GetSpecifities(pawn, out var offset, out scale);
		Vector3 linkOffset = pawn.GetLinkOffset(ISI.linkType);
		drawPosWithOffset = drawPos + linkOffset + offset;
		if (ISI.debug)
		{
			Log.Warning(string.Concat(pawn.ThingID, " ", ISI.label, " TryPlaceMote - drawPos: ", drawPos, " linkOffset:", linkOffset, " bodyTypeOffset:", offset, "scale: ", scale));
		}
	}
}
