namespace YAHA;

public class Grace
{
	public GraceSettings uponApply;

	public GraceSettings uponRemove;

	public bool HasUponApplyGrace => uponApply != null;

	public bool HasUponRemoveGrace => uponRemove != null;
}
