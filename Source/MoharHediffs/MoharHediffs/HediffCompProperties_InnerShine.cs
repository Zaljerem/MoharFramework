using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class HediffCompProperties_InnerShine : HediffCompProperties
{
	public List<InnerShineItem> innerShinePool;

	public List<InnerShineDef> innerShineDefPool;

	public bool debug;

	public bool HasShinePool => HasRawShinePool || HasShineDefPool;

	public bool HasRawShinePool => !innerShinePool.NullOrEmpty();

	public bool HasShineDefPool => !innerShineDefPool.NullOrEmpty();

	public HediffCompProperties_InnerShine()
	{
		compClass = typeof(HediffComp_InnerShine);
	}
}
