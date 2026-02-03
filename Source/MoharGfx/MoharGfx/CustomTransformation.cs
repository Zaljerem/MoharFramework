namespace MoharGfx;

public class CustomTransformation
{
	public Rotation rotation;

	public AnimatedMote animatedMote;

	public Scale scale;

	public ColorAltering color;

	public Misc misc;

	public bool HasAnimatedMote => animatedMote != null;

	public bool HasColor => color != null;

	public bool HasMisc => misc != null;

	public bool HasRotation => rotation != null;

	public bool HasScale => scale != null;
}
