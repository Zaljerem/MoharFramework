using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class HediffKeepingCondition
{
	public FloatRange? temperature = null;

	public LightCondition light;

	public List<NeedCondition> needs;

	public List<HediffSeverityCondition> destroyingHediffs;

	public bool HasTemperatureCondition => temperature.HasValue;

	public bool HasLightCondition => light != null;

	public bool HasNeedCondition => !needs.NullOrEmpty();

	public bool HasDestroyingHediffs => !destroyingHediffs.NullOrEmpty() && destroyingHediffs.Any((HediffSeverityCondition dh) => !dh.HasHediffDef);
}
