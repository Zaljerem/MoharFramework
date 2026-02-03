using Verse;

namespace Ubet;

public static class NodeCompute
{
	public static bool TrunkNodeComputation(this Thing t, Leaf trunk, bool debug = false)
	{
		bool result = t.RecursiveNodeComputation(trunk, 0, debug);
		if (debug)
		{
			Log.Warning("final result:" + result);
		}
		return result;
	}

	public static bool RecursiveNodeComputation(this Thing t, Leaf branch, int depth = 0, bool debug = false)
	{
		if (depth == 0)
		{
			if (debug)
			{
				Log.Warning("RecursiveNodeComputation - trunk detected");
			}
			if (branch.HasInitialCondition)
			{
				if (!t.MainCheck(branch.initialCondition, debug))
				{
					if (debug)
					{
						Log.Warning("Initial condition " + branch.initialCondition.Description + ", result=false");
					}
					return false;
				}
				if (branch.leaf == null)
				{
					if (debug)
					{
						Log.Warning("Initial condition " + branch.initialCondition.Description + ", result=true");
					}
					return true;
				}
			}
		}
		else if (debug)
		{
			Log.Warning("RecursiveNodeComputation - leaf depth: " + depth);
		}
		bool flag = true;
		if (branch.operand == Operand.and)
		{
			flag = true;
		}
		if (branch.operand == Operand.or)
		{
			flag = false;
		}
		foreach (Leaf item in branch.leaf)
		{
			if (debug)
			{
				string text = "Browsing leaves; depth=" + depth;
				text = text + "; operand:" + branch.operand.DescriptionAttr();
				text += ((branch.condition == null) ? "; no condition" : ("; condition:" + branch.condition.Description));
				Log.Warning(text);
			}
			bool flag2 = ((!item.leaf.NullOrEmpty() || item.condition == null) ? t.RecursiveNodeComputation(item, depth + 1, debug) : t.MainCheck(item.condition, debug));
			if (branch.operand == Operand.and)
			{
				if (!flag2)
				{
					if (debug)
					{
						Log.Warning("depth=" + depth + " - AND && false : fast exit with false");
					}
					return false;
				}
				flag = flag && flag2;
				continue;
			}
			if (branch.operand == Operand.or)
			{
				if (flag2)
				{
					if (debug)
					{
						Log.Warning("depth=" + depth + " - OR && true : fast exit with true");
					}
					return true;
				}
				flag = flag || flag2;
				continue;
			}
			if (debug && branch.operand == Operand.not)
			{
				Log.Warning("depth=" + depth + " - NOT");
			}
			return (item.operand == Operand.not) ? (!flag2) : flag2;
		}
		return flag;
	}
}
