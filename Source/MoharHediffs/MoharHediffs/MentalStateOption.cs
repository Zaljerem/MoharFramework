using Verse;

namespace MoharHediffs;

public class MentalStateOption
{
	public MentalStateDef mentalDef;

	public float weight = 1f;

	public void Dump()
	{
		Log.Warning("MentalStateDef:" + mentalDef.defName + "; weight:" + weight + "; ");
	}
}
