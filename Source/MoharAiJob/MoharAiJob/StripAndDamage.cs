using Verse;

namespace MoharAiJob;

public class StripAndDamage
{
	public bool mustStrip = true;

	public bool mustDamage = true;

	public FloatRange apparelsDamagingRatio = new FloatRange(0.35f, 0.85f);

	public FloatRange primaryDamagingRatio = new FloatRange(0.65f, 0.85f);

	public FloatRange inventoryDamagingRatio = new FloatRange(0.15f, 0.75f);
}
