using Verse;

namespace MoharHediffs;

public class HediffAndBodyPart
{
	public HediffDef hediff;

	public BodyPartDef bodyPart;

	public string bodyPartLabel;

	public bool prioritizeMissing = false;

	public bool allowMissing = true;

	public bool regenIfMissing = true;

	public bool allowAddedPart = true;

	public bool wholeBodyFallback = true;
}
