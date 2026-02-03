using Verse;

namespace MoharHediffs;

public class HediffItem
{
	public HediffDef hediffDef;

	public FloatRange applyChance = new FloatRange(1f, 1f);

	public FloatRange severity = new FloatRange(0.1f, 0.2f);

	public float weight = 1f;

	public HediffCondition specificCondition;

	public bool HasSpecific => specificCondition != null;
}
