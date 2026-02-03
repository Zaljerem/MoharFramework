using System.Collections.Generic;
using Ubet;
using Verse;

namespace YAHA;

public class HediffAssociation
{
	public List<HediffItem> hediffPool;

	public List<RandomHediffItem> randomHediffPool;

	public List<string> bodyPart;

	public UbetDef condition;

	public ApplySpecifics specifics;

	public bool HasHediffPool => !hediffPool.NullOrEmpty();

	public bool HasRandomHediffPool => !randomHediffPool.NullOrEmpty();

	public bool HasBodyPartToApplyHediff => !bodyPart.NullOrEmpty();

	public bool HasSpecifics => specifics != null;
}
