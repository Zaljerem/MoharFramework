using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class PawnCondition
{
	public List<ThingDef> race;

	public FloatRange ageRange = new FloatRange(0f, 999f);

	public List<Gender> gender;

	public bool HasRace => !race.NullOrEmpty();

	public bool HasGender => !gender.NullOrEmpty();
}
