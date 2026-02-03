using Verse;

namespace MoharAiJob;

public class WorkFlow
{
	public int workAmount = 300;

	public int workAmountPerHealthScale = -1;

	public int nibblingPeriod = 120;

	public int nibblingPeriodPerHealthScale = -1;

	public float nibblingAmount = -1f;

	public SoundDef sustainSound = null;

	public EffecterDef effecterDef = null;

	public bool bloodFilth = true;

	public ThingDef filthDef = null;

	public FloatRange filthPerHealthScale = new FloatRange(0f, 0f);

	public float filthRadius = 1.5f;

	public StripAndDamage strip;

	public bool HasWorkAmountPerHS => workAmountPerHealthScale > 0;

	public bool HasNibblingAmount => nibblingAmount > 0f;

	public bool HasNibblingPeriodPerHS => nibblingPeriodPerHealthScale > 0 && HasNibblingAmount;

	public bool HasCustomSustainSound => sustainSound != null;

	public bool HasCustomEffecterDef => effecterDef != null;

	public bool SpawnsFilth => (filthPerHealthScale.min != 0f || filthPerHealthScale.min != 0f) && (bloodFilth || filthDef != null);

	public bool MustStrip => strip != null && strip.mustStrip;
}
