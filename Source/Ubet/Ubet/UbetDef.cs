using Verse;

namespace Ubet;

public class UbetDef : Def
{
	public Leaf trunk;

	public bool debug = false;

	public override string ToString()
	{
		return defName;
	}

	public UbetDef Named(string searchedDN)
	{
		return DefDatabase<UbetDef>.GetNamed(searchedDN);
	}

	public override int GetHashCode()
	{
		return defName.GetHashCode();
	}
}
