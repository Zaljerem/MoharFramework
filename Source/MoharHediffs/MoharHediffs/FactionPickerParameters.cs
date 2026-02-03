using RimWorld;
using Verse;

namespace MoharHediffs;

public class FactionPickerParameters
{
	public bool inheritedFaction = false;

	public FactionDef forcedFaction = null;

	public bool playerFaction = false;

	public bool defaultPawnKindFaction = false;

	public bool noFaction = false;

	public float weight = 1f;

	public bool HasInheritedFaction => inheritedFaction;

	public bool HasForcedFaction => forcedFaction != null;

	public bool HasPlayerFaction => playerFaction;

	public bool HasNoFaction => noFaction;

	public bool HasDefaultPawnKindFaction => defaultPawnKindFaction;

	public bool IsLegitRandomFactionParameter()
	{
		int num = 0;
		checked
		{
			if (HasInheritedFaction)
			{
				num++;
			}
			if (HasForcedFaction)
			{
				num++;
			}
			if (HasPlayerFaction)
			{
				num++;
			}
			if (HasNoFaction)
			{
				num++;
			}
			if (HasDefaultPawnKindFaction)
			{
				num++;
			}
			return num == 1;
		}
	}

	public void Dump()
	{
		Log.Warning("inherited:" + HasInheritedFaction + "; forced:" + HasForcedFaction + "; player:" + HasPlayerFaction + "; noFaction:" + HasNoFaction + "; defaultPawnKindFaction:" + HasDefaultPawnKindFaction + "; ");
	}
}
