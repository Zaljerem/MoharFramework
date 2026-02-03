using System.Collections.Generic;
using Verse;

namespace MoharGfx;

public class StraightenUpRotation
{
	public float aimedRotation;

	public float goalLifeSpanRatio;

	public List<FloatRange> gracePeriod;

	public float tolerance;

	public bool HasGracePeriod => !gracePeriod.NullOrEmpty();

	public bool IsWithinGracePeriod(float curLifeSpanRatio)
	{
		if (!HasGracePeriod)
		{
			return false;
		}
		return gracePeriod.Any((FloatRange fr) => fr.Includes(curLifeSpanRatio));
	}
}
