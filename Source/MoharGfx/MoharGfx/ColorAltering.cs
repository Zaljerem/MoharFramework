using Verse;

namespace MoharGfx;

public class ColorAltering
{
	public float arbitraryAlpha = 1f;

	public SimpleCurve alphaCurve = null;

	public bool HasArbitraryAlpha => arbitraryAlpha != 1f;

	public bool HasAlphaCurve => alphaCurve != null;
}
