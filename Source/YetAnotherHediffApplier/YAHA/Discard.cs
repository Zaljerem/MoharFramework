namespace YAHA;

public class Discard
{
	public DiscardSettings uponApply;

	public DiscardSettings uponRemove;

	public bool HasUponApplyDiscard => uponApply != null;

	public bool HasUponRemoveDiscard => uponRemove != null;
}
