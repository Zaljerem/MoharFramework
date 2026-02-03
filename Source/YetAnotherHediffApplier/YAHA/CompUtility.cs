using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace YAHA;

public static class CompUtility
{
	public static void SetTriggerOnly(this HediffComp_YetAnotherHediffApplier c)
	{
		c.TriggeredOnlyHediffs = c.Props.associations.All((HediffAssociation a) => a.specifics.IsTriggered);
	}

	public static void RememberTrigger(List<TriggerEvent> haTriggerEvents, List<TriggerEvent> triggerEventsRegistry, TriggerEvent te)
	{
		if (haTriggerEvents.Contains(te) && !triggerEventsRegistry.Contains(te))
		{
			triggerEventsRegistry.Add(te);
		}
	}

	public static void SetHasAnyWoundTrigger(this HediffComp_YetAnotherHediffApplier c)
	{
		foreach (HediffAssociation association in c.Props.associations)
		{
			if (association.HasSpecifics && association.specifics.IsTriggered)
			{
				RememberTrigger(association.specifics.triggerEvent, c.WoundTriggers, TriggerEvent.anyWound);
				RememberTrigger(association.specifics.triggerEvent, c.WoundTriggers, TriggerEvent.friendlyFire);
				RememberTrigger(association.specifics.triggerEvent, c.WoundTriggers, TriggerEvent.enemyWound);
			}
		}
	}

	public static void CreateRegistry(this HediffComp_YetAnotherHediffApplier c)
	{
		if (c.HasRegistry)
		{
			return;
		}
		foreach (HediffAssociation association in c.Props.associations)
		{
			_ = association;
			if (c.Props.debug)
			{
				Log.Warning("Added 1 HediffAssociation");
			}
			c.Registry.Add(new AssociatedHediffHistory());
		}
	}

	public static void DidNothing(this HediffComp_YetAnotherHediffApplier c)
	{
		if (c.HasEmptyRegistry)
		{
			return;
		}
		foreach (AssociatedHediffHistory item in c.Registry)
		{
			item.DidSomethingThisTick = false;
		}
	}

	public static void WoundTriggerManager(this HediffComp_YetAnotherHediffApplier c, DamageInfo dinfo)
	{
		if (!c.HasWoundTrigger)
		{
			return;
		}
		if (c.WoundTriggers.Contains(TriggerEvent.enemyWound) || c.WoundTriggers.Contains(TriggerEvent.friendlyFire))
		{
			Faction faction;
			Faction faction2;
			if ((faction = dinfo.Instigator.Faction) != null && (faction2 = c.Pawn.Faction) != null)
			{
				TriggerEvent triggerEvent = TriggerEvent.empty;
				triggerEvent = (faction2.AllyOrNeutralTo(faction) ? TriggerEvent.friendlyFire : TriggerEvent.enemyWound);
				if (triggerEvent != TriggerEvent.empty)
				{
					YahaUtility.UpdateDependingOnTriggerEvent(c.Pawn, triggerEvent);
				}
			}
		}
		else
		{
			YahaUtility.UpdateDependingOnTriggerEvent(c.Pawn, TriggerEvent.anyWound);
		}
	}
}
