namespace MoharGfx;

public class Rotation
{
	public PeriodicRandomRotation periodicRandRot;

	public RandomRotationRate randRotRate;

	public StraightenUpRotation straightenUp;

	public bool HasPeriodicRandRot => periodicRandRot != null;

	public bool HasRandRotRate => randRotRate != null;

	public bool HasStraightenUp => straightenUp != null;
}
