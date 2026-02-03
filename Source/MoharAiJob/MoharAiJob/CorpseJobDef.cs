using System.Collections.Generic;
using Verse;

namespace MoharAiJob;

public class CorpseJobDef : Def
{
	public List<PawnKindDef> workerPawnKind;

	public List<CorpseRecipeSettings> corpseRecipeList;

	public JobDef jobDef;

	public bool debug = false;

	public bool IsEmpty => corpseRecipeList.NullOrEmpty();

	public override string ToString()
	{
		return defName;
	}

	public CorpseJobDef Named(string searchedDN)
	{
		return DefDatabase<CorpseJobDef>.GetNamed(searchedDN);
	}

	public override int GetHashCode()
	{
		return defName.GetHashCode();
	}
}
