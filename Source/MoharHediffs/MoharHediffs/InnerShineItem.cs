using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class InnerShineItem
{
	public string label;

	public SpawnRules spawningRules;

	public List<ThingDef> motePool;

	public MoteLink.Nature linkType;

	public List<BodyTypeSpecificities> bodyTypeDrawRules;

	public DrawingSpecificities defaultDrawRules;

	public ActivityRestriction restriction;

	public ColorRange colorRange;

	public bool debug = false;

	public bool HasSpawningRules => spawningRules != null;

	public bool HasRestriction => restriction != null;

	public bool HasMotePool => !motePool.NullOrEmpty();

	public bool HasBodyTypeDrawRules => !bodyTypeDrawRules.NullOrEmpty();

	public bool HasDefaultDrawRules => defaultDrawRules != null;

	public bool HasColorRange => colorRange != null;

	public string Dump()
	{
		return "label:" + label + $" HasSpawningRules:{HasSpawningRules} HasRestriction:{HasRestriction}" + $" HasMotePool:{HasMotePool} HasBodyTypeDrawRules:{HasBodyTypeDrawRules} HasDefaultDrawRules:{HasDefaultDrawRules}" + $" HasColorRange:{HasColorRange}" + $" debug:{debug}";
	}
}
