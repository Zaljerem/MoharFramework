namespace MoharAiJob;

public class GraveDig_JobParameters
{
	public WorkerRequirement workerRequirement;

	public GraveDigWorkFlow workFlow;

	public GraveSpecification target;

	private bool HasTarget => target != null;

	public bool HasTargetSpec => HasTarget && target.HasEligibleGraves;

	public bool HasWorkFlow => workFlow != null;

	public bool HasWorkerRequirement => workerRequirement != null;
}
