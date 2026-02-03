using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoharAiJob;

public class GraveDigWorkFlow
{
	public int workAmount = 300;

	public bool workAmountDoorOpenSpeedWeighted = true;

	public int dustPeriod = 50;

	public SoundDef sustainSound = null;

	public List<ThinkNode_JobGiver> tryToChainJobGiver;

	public bool HasCustomSustainSound => sustainSound != null;

	public bool HasRelevantWorkAmount => workAmount > 0;

	public bool HasRelevantDustPeriod => dustPeriod > 0;

	public bool HasJobGiverToChain => !tryToChainJobGiver.NullOrEmpty();
}
