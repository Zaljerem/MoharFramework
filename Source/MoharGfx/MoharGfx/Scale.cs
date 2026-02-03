using Verse;

namespace MoharGfx;

public class Scale
{
	public PulsingScale pulsingScale;

	public SimpleCurve scaleCurve = null;

	public bool HasPulsingScale => pulsingScale != null;

	public bool HasScaleCurve => scaleCurve != null;
}
