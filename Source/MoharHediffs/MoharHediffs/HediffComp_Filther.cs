using RimWorld;
using Verse;

namespace MoharHediffs;

public class HediffComp_Filther : HediffComp
{
	private Pawn myPawn = null;

	private int ticksUntilFilth = 500;

	private int filthTicksLeft;

	private bool myDebug = false;

	public HediffCompProperties_Filther Props => (HediffCompProperties_Filther)props;

	public override void CompPostMake()
	{
		myDebug = Props.debug;
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		myPawn = parent.pawn;
		checked
		{
			if (myPawn == null)
			{
				Tools.Warn("pawn null", myDebug);
			}
			else if (myPawn.Map != null && Props.filthDef != null)
			{
				if (filthTicksLeft <= 0)
				{
					FilthMaker.TryMakeFilth(myPawn.Position, myPawn.Map, Props.filthDef);
					filthTicksLeft = (ticksUntilFilth = Rand.RangeInclusive(Props.MinTicksBetweenSprays, Props.MaxTicksBetweenSprays));
				}
				else
				{
					filthTicksLeft--;
				}
			}
		}
	}
}
