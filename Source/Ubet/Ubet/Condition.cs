using System.Collections.Generic;
using Verse;

namespace Ubet;

public class Condition
{
	public ConditionType type = ConditionType.empty;

	public List<List<string>> stringArg;

	public List<IntRange> intArg;

	public List<FloatRange> floatArg;

	public SimpleCurve curve;

	public bool HasStringArg => !stringArg.NullOrEmpty();

	public bool Has1StringArg => HasStringArg && stringArg.Count == 1;

	public bool Has2StringArg => HasStringArg && stringArg.Count == 2;

	public bool HasIntArg => !intArg.NullOrEmpty();

	public bool HasFloatArg => !floatArg.NullOrEmpty();

	public bool HasStringFloatArg => HasStringArg && HasFloatArg;

	public bool HasCurve => !curve.EnumerableNullOrEmpty();

	public bool HasNoArg => stringArg.NullOrEmpty() && intArg.NullOrEmpty();

	public string Description => type.DescriptionAttr();
}
