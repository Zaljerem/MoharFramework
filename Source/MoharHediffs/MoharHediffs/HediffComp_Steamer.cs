using RimWorld;
using Verse;

namespace MoharHediffs;

public class HediffComp_Steamer : HediffComp
{
	private int ticksUntilSpray = 500;

	private int sprayTicksLeft;

	private bool MyDebug => Props.debug;

	private Map MyMap => base.Pawn.Map;

	public HediffCompProperties_Steamer Props => (HediffCompProperties_Steamer)props;

	public override string CompTipStringExtra
	{
		get
		{
			string empty = string.Empty;
			return empty + "Puff in " + sprayTicksLeft.ToStringTicksToPeriod();
		}
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		checked
		{
			if (base.Pawn.Negligible())
			{
				if (MyDebug)
				{
					Log.Warning("null pawn");
				}
			}
			else if (sprayTicksLeft <= 0)
			{
				if (Rand.Chance(Props.puffingChance))
				{
					FleckMaker.ThrowAirPuffUp(base.Pawn.TrueCenter(), MyMap);
					GenTemperature.PushHeat(base.Pawn.Position, MyMap, Props.temperatureIncreasePerPuff);
				}
				sprayTicksLeft = (ticksUntilSpray = Rand.RangeInclusive(Props.MinTicksBetweenSprays, Props.MaxTicksBetweenSprays));
			}
			else
			{
				sprayTicksLeft--;
			}
		}
	}
}
