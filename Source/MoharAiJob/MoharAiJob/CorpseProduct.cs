using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoharAiJob;

public class CorpseProduct
{
	public List<PawnGenOption> pawnKind = new List<PawnGenOption>();

	public List<WeightedFaction> forcedFaction = null;

	public float manhunterChance = 0f;

	public float newBornChance = 0f;

	public IntRange pawnNum = new IntRange(1, 1);

	public int combatPowerLimit = -1;

	public float combatPowerPerMass = -1f;

	public bool inheritSettingsFromParent = true;

	public bool setRelationsWithParent = true;

	public float newBornCombatPowerRatio = 0.3f;

	public bool HasPawnKindProduct => !pawnKind.NullOrEmpty();

	public bool HasWeightedFaction => !forcedFaction.NullOrEmpty();

	public bool HasRelevantManhunterChance => manhunterChance != 0f;

	public bool HasRelevantNewBornChance => newBornChance != 0f;

	public bool HasRelevantCombatPowerLimit => combatPowerLimit > 0;

	public bool HasRelevantCombatPowerPerMass => combatPowerPerMass > 0f;
}
