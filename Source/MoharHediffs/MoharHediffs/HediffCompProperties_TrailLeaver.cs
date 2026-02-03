using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public class HediffCompProperties_TrailLeaver : HediffCompProperties
{
	public IntRange period = new IntRange(15, 25);

	public List<ThingDef> motePool;

	public List<BodyTypeOffset> offSetPerBodyType;

	public Vector3 defaultOffset = new Vector3(0f, 0f, -0.32f);

	public Restriction restriction;

	public Footprint footprint;

	public ColorRange colorRange;

	public float rotationOffset = 0f;

	public bool dynamicRotation = true;

	public FloatRange randomScale = new FloatRange(0.5f, 0.8f);

	public bool debug = false;

	public bool HasRestriction => restriction != null;

	public bool HasColorRange => colorRange != null;

	public bool UsesFootPrints => footprint != null;

	public bool HasMotePool => !motePool.NullOrEmpty();

	public bool HasOffset => !offSetPerBodyType.NullOrEmpty();

	public bool HasRotationOffset => rotationOffset != 0f;

	public HediffCompProperties_TrailLeaver()
	{
		compClass = typeof(HediffComp_TrailLeaver);
	}
}
