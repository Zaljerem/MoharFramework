using System.Collections.Generic;

namespace Ubet;

public class Leaf
{
	public Condition initialCondition;

	public Operand operand = Operand.empty;

	public Condition condition;

	public List<Leaf> leaf;

	public bool HasInitialCondition => initialCondition != null;
}
