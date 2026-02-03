using Verse;

namespace DUDOD;

public class HediffComp_DestroyUponDeathOrDowned : HediffComp
{
	private Thing RememberWeapon = null;

	private bool myDebug = false;

	public HediffCompProperties_DestroyUponDeathOrDowned Props => (HediffCompProperties_DestroyUponDeathOrDowned)props;

	public override void CompPostMake()
	{
		myDebug = Props.debug;
	}

	public void MemorizeWeapon()
	{
		RememberWeapon = base.Pawn.equipment.Primary ?? null;
	}

	private bool PawnDestroy()
	{
		if (base.Pawn.Dead)
		{
			if (myDebug)
			{
				Log.Warning(base.Pawn.LabelShort + " is dead and will get destroyed");
			}
			if (base.Pawn.Corpse == null)
			{
				if (myDebug)
				{
					Log.Warning(base.Pawn.LabelShort + " found no corpse to work with, wont do anything");
				}
				return false;
			}
			Corpse corpse = base.Pawn.Corpse;
			if (Props.StripBeforeDeath && corpse.AnythingToStrip())
			{
				corpse.Strip();
			}
			corpse.DeSpawn();
		}
		else if (base.Pawn.Downed)
		{
			if (myDebug)
			{
				Log.Warning(base.Pawn.LabelShort + " is downed and will get destroyed");
			}
			if (Props.StripBeforeDeath && base.Pawn.AnythingToStrip())
			{
				base.Pawn.Strip();
			}
			base.Pawn.Destroy();
		}
		else if (myDebug)
		{
			Log.Warning(base.Pawn.LabelShort + " How?");
		}
		if (Props.DestroyWeapon && RememberWeapon != null && RememberWeapon.Spawned)
		{
			RememberWeapon.Destroy();
		}
		return true;
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (Find.TickManager.TicksGame % Props.WeaponRefreshRate == 0)
		{
			MemorizeWeapon();
		}
	}

	public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
	{
		base.Notify_PawnDied(dinfo, culprit);
		if (Props.DestroyUponDeath)
		{
			PawnDestroy();
		}
	}

	public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
	{
		base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
		if (Props.DestroyUponDown && base.Pawn.Downed)
		{
			PawnDestroy();
		}
	}
}
