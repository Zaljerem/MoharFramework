using Verse;

namespace MoharHediffs;

public class HediffRequirementSettings
{
	public HediffDef hediffDef;

	public FloatRange severity = new FloatRange(0f, 1f);

	public bool HasHediffDef => hediffDef != null;
}
