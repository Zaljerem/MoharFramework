using UnityEngine;
using Verse;

namespace MoharGfx;

public class AlphaScaleLayer_Mote : MoteThrown
{
	public float LifeSpentRatio;

	public Vector3 InitialScale;

	public AltitudeLayer? NullableAltitudeLayer = null;

	public float? WeightedScale = null;

	public float? WeightedAlpha = null;

	public AlphaScaleLayer_MoteDef Def => def as AlphaScaleLayer_MoteDef;

	public bool MyDebug => Def.debug;

	public string MainDebugStr => MyDebug ? (Def.defName + " AlphaScaleLayer_Mote - ") : string.Empty;

	public bool HasASL => Def.HasASL;

	public bool HasAlpha => Def.HasAlpha;

	public bool HasScale => Def.HasScale;

	public bool HasLayer => Def.HasLayer;

	public bool HasWeightedAlpha => Def.HasWeigthedAlpha;

	public bool HasWeigthedScale => Def.HasWeigthedScale;

	public int TickAge => Find.TickManager.TicksGame - spawnTick;

	public override float Alpha
	{
		get
		{
			float num = base.Alpha;
			if (HasAlpha)
			{
				num *= Def.alphaScaleLayer.alpha.Evaluate(LifeSpentRatio);
			}
			if (HasWeightedAlpha && WeightedAlpha.HasValue)
			{
				num *= WeightedAlpha.Value;
			}
			return num;
		}
	}

	public AltitudeLayer GetLayer
	{
		get
		{
			if (NullableAltitudeLayer.HasValue)
			{
				return NullableAltitudeLayer.Value;
			}
			if (HasLayer)
			{
				for (int i = 0; i < Def.alphaScaleLayer.layerSets.Count; i++)
				{
					LayerSet layerSet = Def.alphaScaleLayer.layerSets[i];
					if (LifeSpentRatio <= layerSet.lifeRange)
					{
						return layerSet.layer;
					}
				}
			}
			return Def.altitudeLayer;
		}
	}

	public float GetLifeSpentRatio
	{
		get
		{
			int num = (int)(Def.mote.Lifespan * 60f);
			return (float)TickAge / (float)num;
		}
	}

	public void SetWeightedScale(float ratio)
	{
		if (!HasWeigthedScale)
		{
			WeightedScale = null;
		}
		WeightedScale = Def.alphaScaleLayer.weightedScaleRange.LerpThroughRange(ratio);
	}

	public void SetWeightedAlpha(float ratio)
	{
		if (!HasWeightedAlpha)
		{
			WeightedAlpha = null;
		}
		WeightedAlpha = Def.alphaScaleLayer.weightedAlphaRange.LerpThroughRange(ratio);
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		base.SpawnSetup(map, respawningAfterLoad);
		InitialScale = new Vector3(base.ExactScale.x, 0f, base.ExactScale.z);
	}

	protected override void TimeInterval(float deltaTime)
	{
		base.TimeInterval(deltaTime);
		if (HasScale)
		{
			linearScale = new Vector3(InitialScale.x, 0f, InitialScale.z);
			if (HasScale)
			{
				float num = Def.alphaScaleLayer.scale.Evaluate(LifeSpentRatio);
				linearScale.x *= num;
				linearScale.z *= num;
			}
			if (HasWeigthedScale && WeightedScale.HasValue)
			{
				linearScale.x *= WeightedScale.Value;
				linearScale.z *= WeightedScale.Value;
			}
			linearScale.x = Mathf.Max(base.ExactScale.x, 0.0001f);
			linearScale.z = Mathf.Max(base.ExactScale.z, 0.0001f);
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		if (HasLayer || NullableAltitudeLayer.HasValue)
		{
			DrawMote(GetLayer.AltitudeFor());
		}
		else
		{
			DrawMote(def.altitudeLayer.AltitudeFor());
		}
	}

	protected override void Tick()
	{
		if (HasASL)
		{
			LifeSpentRatio = GetLifeSpentRatio;
		}
		base.Tick();
	}
}
