namespace MoharAiJob;

public class CorpseRecipeSettings
{
	public WorkerRequirement worker;

	public CorpseSpecification target;

	public CorpseProduct product;

	public WorkFlow workFlow;

	public bool HasWorkerSpec => worker != null;

	private bool HasTarget => target != null;

	public bool HasTargetSpec => HasTarget && target.HasCorpseCategoryDef;

	public bool HasRottenSpec => HasTargetSpec && target.HasCorpseRotStages;

	private bool HasProduct => product != null;

	public bool HasProductSpec => HasProduct && product.HasPawnKindProduct;

	public bool HasWorkFlow => workFlow != null;
}
