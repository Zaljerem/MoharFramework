using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoharHediffs;

public class HediffComp_InnerShine : HediffComp
{
	public List<InnerShineRecord> Tracer;

	public Map MyMap => base.Pawn.Map;

	public bool NullMap => MyMap == null;

	public bool MyDebug => Props.debug;

	public bool HasEmptyTracer => Tracer.NullOrEmpty();

	public bool HasShinePool => Props.HasShinePool;

	public HediffCompProperties_InnerShine Props => (HediffCompProperties_InnerShine)props;

	public override void CompPostMake()
	{
		PropsCheck();
		this.CreateTracer();
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (base.Pawn.Negligible())
		{
			if (MyDebug)
			{
				Log.Warning("null pawn");
			}
			return;
		}
		if (HasEmptyTracer)
		{
			this.CreateTracer();
		}
		checked
		{
			foreach (InnerShineRecord item in Tracer)
			{
				InnerShineItem innerShineItem = this.RetrieveISI(item.label);
				if (innerShineItem == null)
				{
					if (MyDebug)
					{
						Log.Warning("Did not find ISI with label:" + item.label);
					}
					continue;
				}
				if (item.ticksLeft <= 0)
				{
					if (innerShineItem.ShouldSpawnMote(item, base.Pawn))
					{
						innerShineItem.TryPlaceMote(item, base.Pawn);
					}
					innerShineItem.ResetTicks(item);
				}
				else
				{
					item.ticksLeft--;
				}
				innerShineItem.UpdateMotes(item, base.Pawn, MyDebug);
			}
		}
	}

	public void PropsCheck()
	{
		checked
		{
			if (!HasShinePool)
			{
				if (MyDebug)
				{
					Log.Warning("no shine pool, giving up");
				}
				this.SelfDestroy();
			}
			else
			{
				if (!MyDebug)
				{
					return;
				}
				if (Props.HasRawShinePool)
				{
					IEnumerable<InnerShineItem> enumerable;
					if ((enumerable = Props.innerShinePool.Where((InnerShineItem s) => !s.HasMotePool)) != null)
					{
						foreach (InnerShineItem item in enumerable)
						{
							Log.Warning(item.label + " has no mote pool");
						}
					}
					IEnumerable<InnerShineItem> enumerable2;
					if ((enumerable2 = Props.innerShinePool.Where((InnerShineItem s) => !s.HasDefaultDrawRules && !s.HasBodyTypeDrawRules)) != null)
					{
						foreach (InnerShineItem item2 in enumerable2)
						{
							Log.Warning(item2.label + " has no default nor bodytypedef draw rules, at least one is required");
						}
					}
					int num = 0;
					foreach (InnerShineItem item3 in Props.innerShinePool)
					{
						Log.Warning("Raw" + num.ToString("00") + " => " + item3.Dump());
						num++;
					}
				}
				if (!Props.HasShineDefPool)
				{
					return;
				}
				IEnumerable<InnerShineDef> enumerable3;
				if ((enumerable3 = Props.innerShineDefPool.Where((InnerShineDef s) => !s.item.HasMotePool)) != null)
				{
					foreach (InnerShineDef item4 in enumerable3)
					{
						Log.Warning(item4.item.label + " has no mote pool");
					}
				}
				IEnumerable<InnerShineDef> enumerable4;
				if ((enumerable4 = Props.innerShineDefPool.Where((InnerShineDef s) => !s.item.HasDefaultDrawRules && !s.item.HasBodyTypeDrawRules)) != null)
				{
					foreach (InnerShineDef item5 in enumerable4)
					{
						Log.Warning(item5.item.label + " has no default nor bodytypedef draw rules, at least one is required");
					}
				}
				int num2 = 0;
				foreach (InnerShineDef item6 in Props.innerShineDefPool)
				{
					Log.Warning("Def" + num2.ToString("00") + " => " + item6.item.Dump());
					num2++;
				}
			}
		}
	}
}
