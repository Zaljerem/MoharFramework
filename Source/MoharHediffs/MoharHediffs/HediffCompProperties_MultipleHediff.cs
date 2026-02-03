using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class HediffCompProperties_MultipleHediff : HediffCompProperties
{
	public BodyDef bodyDef;

	public List<BodyDef> bodyDefWhiteList;

	public List<BodyDef> bodyDefBlackList;

	public List<HediffAndBodyPart> hediffAndBodypart;

	public bool debug = false;

	public HediffCompProperties_MultipleHediff()
	{
		compClass = typeof(HediffComp_MultipleHediff);
	}
}
