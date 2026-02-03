using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class HediffCompProperties_RainbowTrail : HediffCompProperties
{
	public int period = 15;

	public List<ThingDef> motePurpleDef;

	public List<ThingDef> moteBlueDef;

	public List<ThingDef> moteGreenDef;

	public List<ThingDef> moteYellowDef;

	public List<ThingDef> moteOrangeDef;

	public List<ThingDef> moteRedDef;

	public float staySameColorChance = 0.5f;

	public int maxTimesSameColor = 3;

	public int minTimesSameColor = 1;

	public FloatRange scale = new FloatRange(0.5f, 0.8f);

	public bool threeColorsGradient = false;

	public HediffComp_RainbowTrail.ColorChoice colorChoice = HediffComp_RainbowTrail.ColorChoice.random;

	public HediffComp_RainbowTrail.CycleKind cycleKind = HediffComp_RainbowTrail.CycleKind.circular;

	public bool debug = false;

	public bool hideBySeverity = true;

	public HediffCompProperties_RainbowTrail()
	{
		compClass = typeof(HediffComp_RainbowTrail);
	}
}
