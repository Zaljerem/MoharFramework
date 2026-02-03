using System.Collections.Generic;
using Verse;

namespace MoharGfx;

public class AlphaScaleLayer
{
	public SimpleCurve alpha;

	public SimpleCurve scale;

	public FloatRange weightedScaleRange;

	public FloatRange weightedAlphaRange;

	public List<LayerSet> layerSets;

	public bool HasAlpha => !alpha.EnumerableNullOrEmpty();

	public bool HasScale => !scale.EnumerableNullOrEmpty();

	public bool HasLayer => !layerSets.NullOrEmpty();

	public bool HasWeigthedAlpha
	{
		get
		{
			_ = weightedAlphaRange;
			return true;
		}
	}

	public bool HasWeigthedScale
	{
		get
		{
			_ = weightedScaleRange;
			return true;
		}
	}
}
