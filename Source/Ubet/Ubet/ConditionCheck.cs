using System;
using System.Collections.Generic;
using Verse;

namespace Ubet;

public static class ConditionCheck
{
	public static bool MainCheck(this Thing t, Condition c, bool debug = false)
	{
		if (debug)
		{
			ParametersDump(c);
		}
		Pawn pawn;
		if ((pawn = (Pawn)t) == null)
		{
			if (debug)
			{
				Log.Warning("MainCheck - thing was not pawn, returning false");
			}
			return false;
		}
		if (c.HasNoArg)
		{
			Func<Pawn, bool> func = GenCollection.TryGetValue<ConditionType, Func<Pawn, bool>>((IReadOnlyDictionary<ConditionType, Func<Pawn, bool>>)ConditionDictionnary.noArgconditions, c.type, (Func<Pawn, bool>)null);
			if (MethodIsNullAndDebug(c, "no arg method", func == null, debug))
			{
				return false;
			}
			return func(pawn);
		}
		if (c.Has1StringArg)
		{
			Func<Pawn, List<string>, bool> func2 = GenCollection.TryGetValue<ConditionType, Func<Pawn, List<string>, bool>>((IReadOnlyDictionary<ConditionType, Func<Pawn, List<string>, bool>>)ConditionDictionnary.StringArgconditions, c.type, (Func<Pawn, List<string>, bool>)null);
			if (MethodIsNullAndDebug(c, "1 string arg method", func2 == null, debug))
			{
				return false;
			}
			return func2(pawn, c.stringArg[0]);
		}
		if (c.Has2StringArg)
		{
			Func<Pawn, List<string>, List<string>, bool> func3 = GenCollection.TryGetValue<ConditionType, Func<Pawn, List<string>, List<string>, bool>>((IReadOnlyDictionary<ConditionType, Func<Pawn, List<string>, List<string>, bool>>)ConditionDictionnary.TwoStringArgconditions, c.type, (Func<Pawn, List<string>, List<string>, bool>)null);
			if (MethodIsNullAndDebug(c, "2 string arg method", func3 == null, debug))
			{
				return false;
			}
			return func3(pawn, c.stringArg[0], c.stringArg[1]);
		}
		if (c.HasStringFloatArg)
		{
			Func<Pawn, List<string>, List<FloatRange>, bool> func4 = GenCollection.TryGetValue<ConditionType, Func<Pawn, List<string>, List<FloatRange>, bool>>((IReadOnlyDictionary<ConditionType, Func<Pawn, List<string>, List<FloatRange>, bool>>)ConditionDictionnary.StringFloatArgconditions, c.type, (Func<Pawn, List<string>, List<FloatRange>, bool>)null);
			if (MethodIsNullAndDebug(c, "strind and float range arg method", func4 == null, debug))
			{
				return false;
			}
			return func4(pawn, c.stringArg[0], c.floatArg);
		}
		if (c.HasIntArg)
		{
			Func<Pawn, List<IntRange>, bool> func5 = GenCollection.TryGetValue<ConditionType, Func<Pawn, List<IntRange>, bool>>((IReadOnlyDictionary<ConditionType, Func<Pawn, List<IntRange>, bool>>)ConditionDictionnary.IntRangeListArgconditions, c.type, (Func<Pawn, List<IntRange>, bool>)null);
			if (MethodIsNullAndDebug(c, "int range arg method", func5 == null, debug))
			{
				return false;
			}
			return func5(pawn, c.intArg);
		}
		if (c.HasFloatArg)
		{
			Func<FloatRange, bool> func6 = GenCollection.TryGetValue<ConditionType, Func<FloatRange, bool>>((IReadOnlyDictionary<ConditionType, Func<FloatRange, bool>>)ConditionDictionnary.FloatRangeArgconditions, c.type, (Func<FloatRange, bool>)null);
			if (MethodIsNullAndDebug(c, "float range arg method", func6 == null, debug))
			{
				return false;
			}
			return func6(c.floatArg[0]);
		}
		if (c.HasCurve)
		{
			Func<Pawn, SimpleCurve, bool> func7 = GenCollection.TryGetValue<ConditionType, Func<Pawn, SimpleCurve, bool>>((IReadOnlyDictionary<ConditionType, Func<Pawn, SimpleCurve, bool>>)ConditionDictionnary.CurveArgconditions, c.type, (Func<Pawn, SimpleCurve, bool>)null);
			if (MethodIsNullAndDebug(c, "curve arg method", func7 == null, debug))
			{
				return false;
			}
			return func7(pawn, c.curve);
		}
		if (debug)
		{
			Log.Warning("MainCheck - This should never be reached");
		}
		return false;
	}

	public static void ParametersDump(Condition c)
	{
		string text = "MainCheck - " + c.Description;
		if (!c.HasNoArg)
		{
			if (c.Has2StringArg || c.Has1StringArg)
			{
				text += ". string Parameters:";
				foreach (List<string> item in c.stringArg)
				{
					foreach (string item2 in item)
					{
						text = text + item2 + "; ";
					}
				}
			}
			if (c.HasIntArg)
			{
				text += ". int range Parameters:";
				foreach (IntRange item3 in c.intArg)
				{
					text = text + item3.ToString() + "; ";
				}
			}
			if (c.HasFloatArg)
			{
				text += ". float range Parameters:";
				foreach (FloatRange item4 in c.floatArg)
				{
					text = text + item4.ToString() + "; ";
				}
			}
			if (c.HasCurve)
			{
				text += ".curve param:";
				foreach (CurvePoint point in c.curve.Points)
				{
					text = text + point.ToString() + "; ";
				}
			}
		}
		Log.Warning(text);
	}

	public static void FoundMethodDebug(Condition c, string MethodType)
	{
		Log.Warning("could not find " + MethodType + " function for " + c.type.ToString() + "(" + c.Description + ")");
	}

	public static bool MethodIsNullAndDebug(Condition c, string debugStr, bool MethodIsNull, bool debug)
	{
		if (MethodIsNull)
		{
			if (debug)
			{
				FoundMethodDebug(c, debugStr);
			}
			return true;
		}
		return false;
	}
}
