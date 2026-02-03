using System;
using UnityEngine;
using Verse;

namespace MoharGfx;

public class CustomTransformation_Mote : MoteThrown
{
	public int randRot_NextPeriod;

	public bool SUReached = false;

	public CustomTransformation_MoteDef Def => def as CustomTransformation_MoteDef;

	public bool MyDebug => Def.debug;

	public string MainDebugStr => MyDebug ? (Def.defName + " CustomTransformation_Mote - ") : string.Empty;

	public bool HasArbitraryAlpha => Def.HasArbitraryAlpha;

	public bool HasAlphaCurve => Def.HasAlphaCurve;

	public bool IsPeriodicRandomRotationTime => Def.HasPeriodicRandomRotation && this.IsHashIntervalTick(randRot_NextPeriod);

	private int TickAge => Find.TickManager.TicksGame - spawnTick;

	private int TickLifeSpan => (int)(Def.mote.Lifespan * 60f);

	private float LifeSpentRatio => (float)TickAge / (float)TickLifeSpan;

	public StraightenUpRotation SUDef => Def.transformation.rotation.straightenUp;

	public float SUAim => SUDef.aimedRotation;

	public float SUTolerance => SUDef.tolerance;

	public float SUGoalRatio => SUDef.goalLifeSpanRatio;

	public bool HasStraightenUp => Def.HasStraightenUp;

	public float SUWorkTodo => Math.Abs(SUAim - exactRotation);

	public bool SUReachedGoal => SUWorkTodo < SUTolerance;

	public bool NeedsStraightenUp => HasStraightenUp && !SUReached;

	public override float Alpha
	{
		get
		{
			if (HasAlphaCurve)
			{
				return Def.transformation.color.alphaCurve.Evaluate(LifeSpentRatio);
			}
			if (HasArbitraryAlpha)
			{
				return Def.transformation.color.arbitraryAlpha * base.Alpha;
			}
			return base.Alpha;
		}
	}

	private float GetSUExactRotation
	{
		get
		{
			if (exactRotation > 180f)
			{
				return exactRotation - 360f;
			}
			if (exactRotation < -180f)
			{
				return exactRotation + 360f;
			}
			return exactRotation;
		}
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		base.SpawnSetup(map, respawningAfterLoad);
		if (Def.HasPeriodicRandomRotation)
		{
			SetNextPeriod();
		}
	}

	private void SetNextPeriod()
	{
		randRot_NextPeriod = Def.transformation.rotation.periodicRandRot.period.RandomInRange;
	}

	public bool TryStraightenUp()
	{
		if (!NeedsStraightenUp)
		{
			return false;
		}
		if (SUDef.IsWithinGracePeriod(LifeSpentRatio))
		{
			return false;
		}
		float getSUExactRotation = GetSUExactRotation;
		float num = SUAim - getSUExactRotation;
		int num2 = (int)(SUGoalRatio * (float)TickLifeSpan - (float)TickAge);
		if (num2 != 0)
		{
			float num3 = num / (float)num2;
			exactRotation += num3;
		}
		SUReached = SUReachedGoal;
		return true;
	}

	public void TryPeriodicRandomRotation()
	{
		if (IsPeriodicRandomRotationTime)
		{
			if (Rand.Chance(Def.transformation.rotation.periodicRandRot.chance))
			{
				exactRotation += (float)((!Rand.Chance(0.5f)) ? 1 : (-1)) * Def.transformation.rotation.periodicRandRot.randomAngle.RandomInRange;
			}
			SetNextPeriod();
		}
	}

	protected override void Tick()
	{
		base.Tick();
		TryStraightenUp();
		TryPeriodicRandomRotation();
		if (Def.HasScaleCurve)
		{
			float num = Def.transformation.scale.scaleCurve.Evaluate(LifeSpentRatio);
			linearScale = new Vector3(num, 0f, num);
		}
	}
}
