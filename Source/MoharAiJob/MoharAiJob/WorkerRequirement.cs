using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharAiJob;

public class WorkerRequirement
{
	public float minHealthPerc = 0f;

	public int chancesToWorkDivider = -1;

	public List<FactionRequirement> factionRequirement;

	public List<HediffRequirement> hediffRequirement;

	public List<LifeStageDef> lifeStageRequirement;

	public bool HasHediffRequirement => !hediffRequirement.NullOrEmpty();

	public bool HasFactionRequirement => !factionRequirement.NullOrEmpty();

	public bool HasLifeStageRequirement => !lifeStageRequirement.NullOrEmpty();

	public bool HasRelevantMinHp => minHealthPerc > 0f;

	public bool HasRelevantChancesToWorkDivider => chancesToWorkDivider > 0;
}
