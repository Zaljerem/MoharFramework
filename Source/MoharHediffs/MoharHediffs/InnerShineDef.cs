using Verse;

namespace MoharHediffs;

public class InnerShineDef : Def
{
	public InnerShineItem item;

	public override string ToString()
	{
		return defName;
	}

	public InnerShineDef Named(string name)
	{
		return DefDatabase<InnerShineDef>.GetNamed(name);
	}

	public override int GetHashCode()
	{
		return defName.GetHashCode();
	}
}
