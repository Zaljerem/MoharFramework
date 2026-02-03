using Verse;

namespace MoharGfx;

public class AlphaScaleLayer_MoteDef : ThingDef
{
	public AlphaScaleLayer alphaScaleLayer;

	public bool debug = false;

	public bool HasASL => alphaScaleLayer != null;

	public bool HasAlpha => HasASL && alphaScaleLayer.HasAlpha;

	public bool HasScale => HasASL && alphaScaleLayer.HasScale;

	public bool HasLayer => HasASL && alphaScaleLayer.HasLayer;

	public bool HasWeigthedAlpha => HasASL && alphaScaleLayer.HasWeigthedAlpha;

	public bool HasWeigthedScale => HasASL && alphaScaleLayer.HasWeigthedScale;

	public static AlphaScaleLayer_MoteDef MyNamed(string defName)
	{
		return DefDatabase<AlphaScaleLayer_MoteDef>.GetNamed(defName);
	}
}
