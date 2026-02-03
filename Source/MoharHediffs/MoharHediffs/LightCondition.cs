using Verse;

namespace MoharHediffs;

public class LightCondition
{
	public bool requiresOutside = true;

	public bool requiresInside = false;

	public FloatRange? level;

	public bool RequiresLightLevel => level.HasValue;

	public LightCondition(LightCondition copyMe)
	{
		requiresInside = copyMe.requiresInside;
		requiresOutside = copyMe.requiresOutside;
		level = copyMe.level;
	}
}
