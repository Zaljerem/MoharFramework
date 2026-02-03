using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class BodyPartCondition
{
	public List<string> bodyPartLabel;

	public List<BodyPartDef> bodyPartDef;

	public List<BodyPartTagDef> bodyPartTag;

	public bool HasLabel => !bodyPartLabel.NullOrEmpty();

	public bool HasDef => !bodyPartDef.NullOrEmpty();

	public bool HasTag => !bodyPartTag.NullOrEmpty();

	public bool HasBPCondition => HasLabel || HasDef || HasTag;
}
