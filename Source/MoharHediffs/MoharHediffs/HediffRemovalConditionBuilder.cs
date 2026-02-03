using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoharHediffs;

public static class HediffRemovalConditionBuilder
{
	public static void CopyHediffKeepingCondition(HediffKeepingCondition source, HediffKeepingCondition dest, bool debug = false)
	{
		string text = (debug ? "CopyHediffCondition - " : "");
		if (source.HasTemperatureCondition)
		{
			Tools.Warn(text + "found HasTemperatureCondition, copying", debug);
			dest.temperature = source.temperature;
		}
		if (source.HasLightCondition)
		{
			Tools.Warn(text + "found HasLightCondition, copying", debug);
			dest.light = new LightCondition(source.light);
		}
		if (source.HasNeedCondition)
		{
			Tools.Warn(text + "found HasNeedCondition, copying", debug);
			foreach (NeedCondition nc in source.needs)
			{
				if (dest.needs.Any((NeedCondition n) => n.needDef == nc.needDef))
				{
					dest.needs.Where((NeedCondition n) => n.needDef == nc.needDef).First().level = nc.level;
				}
				else
				{
					dest.needs.Add(new NeedCondition(nc));
				}
			}
		}
		if (!source.HasDestroyingHediffs)
		{
			return;
		}
		Tools.Warn(text + "found HasDestroyingHediffs, copying", debug);
		foreach (HediffSeverityCondition hsc in source.destroyingHediffs)
		{
			if (dest.destroyingHediffs.Any((HediffSeverityCondition dh) => dh.hediffDef == hsc.hediffDef))
			{
				dest.destroyingHediffs.Where((HediffSeverityCondition dh) => dh.hediffDef == hsc.hediffDef).First().acceptableSeverity = hsc.acceptableSeverity;
			}
			else
			{
				dest.destroyingHediffs.Add(new HediffSeverityCondition(hsc));
			}
		}
	}

	public static HediffKeepingCondition GetDefaultPlusSpecificHediffCondition(HediffKeepingCondition defaultHKC, HediffKeepingCondition specificHKC, bool debug = false)
	{
		string text = (debug ? "GetDefaultPlusSpecificHediffCondition - " : "");
		Tools.Warn(text + "allocating answerHC", debug);
		HediffKeepingCondition hediffKeepingCondition = new HediffKeepingCondition
		{
			needs = new List<NeedCondition>()
		};
		if (defaultHKC != null)
		{
			Tools.Warn(text + "found defaultHKC, copying", debug);
			CopyHediffKeepingCondition(defaultHKC, hediffKeepingCondition, debug);
		}
		if (specificHKC != null)
		{
			Tools.Warn(text + "found specificHKC, copying", debug);
			CopyHediffKeepingCondition(specificHKC, hediffKeepingCondition, debug);
		}
		Tools.Warn(text + $"HasDestroyingHediffs:{hediffKeepingCondition.HasDestroyingHediffs} - " + (hediffKeepingCondition.HasDestroyingHediffs ? hediffKeepingCondition.destroyingHediffs.Count() : 0) + $"HasLightCondition:{hediffKeepingCondition.HasLightCondition} - " + (hediffKeepingCondition.HasLightCondition ? ("reqIn:" + hediffKeepingCondition.light.requiresInside + " reqOut:" + hediffKeepingCondition.light.requiresOutside) : "") + $"HasNeedCondition:{hediffKeepingCondition.HasNeedCondition}" + (hediffKeepingCondition.HasNeedCondition ? hediffKeepingCondition.needs.Count() : 0) + $"HasTemperatureCondition:{hediffKeepingCondition.HasTemperatureCondition}", debug);
		return hediffKeepingCondition;
	}
}
