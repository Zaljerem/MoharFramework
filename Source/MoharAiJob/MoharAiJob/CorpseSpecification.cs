using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharAiJob;

public class CorpseSpecification
{
	public List<ThingCategoryDef> categoryDef;

	public List<RotStage> rotStages;

	public FloatRange healthPerc = new FloatRange(0f, 1f);

	public FloatRange mass = new FloatRange(0f, 9999f);

	public float maxDistance = 10f;

	public ReservationProcess reservation;

	public bool HasCorpseCategoryDef => !categoryDef.NullOrEmpty();

	public bool HasCorpseRotStages => !rotStages.NullOrEmpty();

	public bool HasRelevantHealthPerc => healthPerc.min != 0f || healthPerc.max != 1f;

	public bool HasRelevantMassPerc => mass.min != 0f || mass.max != 9999f;

	public bool HasReservationProcess => reservation != null;
}
