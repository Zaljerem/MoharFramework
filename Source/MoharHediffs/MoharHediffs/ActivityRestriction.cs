using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharHediffs;

public class ActivityRestriction
{
	public bool onlyWhenMoving = true;

	public List<PawnPosture> allowedPostures;

	public List<JobDef> allowedJobs;

	public List<Rot4> allowedRotation;

	public bool HasPostureRestriction => !allowedPostures.NullOrEmpty();

	public bool HasJobRestriction => !allowedJobs.NullOrEmpty();

	public bool HasAllowedRotation => !allowedRotation.NullOrEmpty();
}
