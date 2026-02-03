using Verse;

namespace DUDOD;

public class HediffCompProperties_DestroyUponDeathOrDowned : HediffCompProperties
{
	public bool DestroyUponDeath = true;

	public bool DestroyUponDown = true;

	public bool DestroyWeapon = true;

	public bool StripBeforeDeath = false;

	public int WeaponRefreshRate = 60;

	public bool debug = false;

	public HediffCompProperties_DestroyUponDeathOrDowned()
	{
		compClass = typeof(HediffComp_DestroyUponDeathOrDowned);
	}
}
