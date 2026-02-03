using Verse;

namespace MoharGfx;

public class CustomTransformation_MoteDef : ThingDef
{
	public CustomTransformation transformation;

	public bool debug = false;

	public bool HasTransformation => transformation != null;

	public bool HasAnimatedMote => HasTransformation && transformation.HasAnimatedMote;

	public bool HasColor => HasTransformation && transformation.HasColor;

	public bool HasArbitraryAlpha => HasColor && transformation.color.HasArbitraryAlpha;

	public bool HasAlphaCurve => HasColor && transformation.color.HasAlphaCurve;

	public bool HasMisc => HasTransformation && transformation.HasMisc;

	public bool HasRotation => HasTransformation && transformation.HasRotation;

	public bool HasPeriodicRandomRotation => HasRotation && transformation.rotation.HasPeriodicRandRot;

	public bool HasRandomRotationRate => HasRotation && transformation.rotation.HasRandRotRate;

	public bool HasStraightenUp => HasRotation && transformation.rotation.HasStraightenUp;

	public bool HasScale => HasTransformation && transformation.HasScale;

	public bool HasPulsingScale => HasScale && transformation.scale.HasPulsingScale;

	public bool HasScaleCurve => HasScale && transformation.scale.HasScaleCurve;

	public static CustomTransformation_MoteDef MyNamed(string defName)
	{
		return DefDatabase<CustomTransformation_MoteDef>.GetNamed(defName, errorOnFail: false);
	}
}
