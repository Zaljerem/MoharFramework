using UnityEngine;
using Verse;

namespace MoharHediffs;

public class HediffComp_TrailLeaver : HediffComp
{
	public int currentPeriod = 0;

	public Vector3 lastMotePos;

	public Color lastColor = Color.black;

	public bool lastFootprintRight;

	public Map MyMap => base.Pawn.Map;

	public bool NullMap => MyMap == null;

	public bool MyDebug => Props.debug;

	public HediffCompProperties_TrailLeaver Props => (HediffCompProperties_TrailLeaver)props;

	public bool HasMotePool => Props.HasMotePool;

	public TerrainRestriction TerrainRestriction => (!Props.HasRestriction || !Props.restriction.HasTerrainRestriction) ? null : Props.restriction.terrain;

	public bool HasTerrainRestriction => TerrainRestriction != null;

	public Restriction PawnRestriction => (!Props.HasRestriction) ? null : Props.restriction;

	public bool HasPawnRestriction => TerrainRestriction != null;

	public override void CompPostMake()
	{
		PropsCheck();
	}

	public void NewPeriod()
	{
		currentPeriod = Props.period.RandomInRange;
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (base.Pawn.Negligible())
		{
			if (MyDebug)
			{
				Log.Warning("null pawn");
			}
			return;
		}
		if (currentPeriod == 0)
		{
			NewPeriod();
		}
		if (!base.Pawn.IsHashIntervalTick(currentPeriod))
		{
			return;
		}
		if (!this.IsTerrainAllowed(base.Pawn.Position.GetTerrain(MyMap)))
		{
			if (MyDebug)
			{
				Log.Warning("terrain does not allow motes");
			}
		}
		else if (!this.IsPawnActivityCompatible())
		{
			if (MyDebug)
			{
				Log.Warning("pawn activity does not allow motes");
			}
		}
		else
		{
			TryPlaceMote();
			NewPeriod();
		}
	}

	public void PropsCheck()
	{
		if (!MyDebug)
		{
			return;
		}
		if (!HasMotePool)
		{
			Log.Warning("Mote pool is empty, trailLeaver needs at least 1 mote");
		}
		if (!Props.HasOffset)
		{
			Log.Warning("no offset per body type found, will use default:" + Props.defaultOffset);
		}
		else
		{
			string text = "BodyTypeOffsets dump => ";
			foreach (BodyTypeOffset item in Props.offSetPerBodyType)
			{
				text = string.Concat(text, item.bodyType.defName, ":", item.offset, "; ");
			}
			Log.Warning(text);
		}
		if (Props.UsesFootPrints)
		{
			Log.Warning("footprints => " + Props.footprint.Dump());
		}
	}

	private void TryPlaceMote()
	{
		if (!HasMotePool)
		{
			return;
		}
		Vector3 drawPos = base.Pawn.DrawPos;
		if (base.Pawn.Position.InBounds(MyMap))
		{
			Vector3 normalized;
			float moteRotation = this.GetMoteRotation(drawPos, out normalized);
			Vector3 vector = drawPos + this.GetBodyTypeOffset() + this.GetFootPrintOffset(normalized);
			if (MyDebug)
			{
				Log.Warning(string.Concat(base.Pawn.ThingID, " ", parent.def.defName, " TryPlaceMote -  dynRot:", Props.dynamicRotation.ToString(), " footprint:", Props.UsesFootPrints.ToString(), " drawPos:", drawPos, " offset:", vector, " rot:", moteRotation, " normalized:", normalized));
			}
			float randomInRange = Props.randomScale.RandomInRange;
			ThingDef moteDef = Props.motePool.RandomElementWithFallback();
			if (vector.TryAnyMoteSpawn(MyMap, moteRotation, randomInRange, moteDef, MyDebug) is Mote mote)
			{
				this.ChangeMoteColor(mote);
			}
		}
		this.RecordMotePos(drawPos);
	}
}
