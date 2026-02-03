namespace MoharHediffs;

public class HediffCondition
{
	public PawnCondition pawn;

	public BodyPartCondition bodyPart;

	public bool HasPawnCondition => pawn != null;

	public bool HasBodypartCondition => bodyPart != null && bodyPart.HasBPCondition;
}
