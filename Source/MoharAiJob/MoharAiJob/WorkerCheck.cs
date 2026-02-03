using System.Collections.Generic;
using Verse;

namespace MoharAiJob;

public static class WorkerCheck
{
	public static IEnumerable<CorpseRecipeSettings> WorkerFulfillsRequirements(this Pawn p, CorpseJobDef CJD, bool debug = false)
	{
		string DebugStr = (debug ? "WorkerFulfillsRequirements - " : string.Empty);
		if (CJD.IsEmpty)
		{
			if (debug)
			{
				Log.Warning("empy CJD");
			}
			yield break;
		}
		if (debug)
		{
			DebugStr = p.ThingID + DebugStr;
		}
		foreach (CorpseRecipeSettings CRS in CJD.corpseRecipeList)
		{
			if (!CRS.HasWorkerSpec)
			{
				if (debug)
				{
					Log.Warning(DebugStr + " no workrequirement, yield");
				}
				yield return CRS;
				continue;
			}
			WorkerRequirement WR = CRS.worker;
			if (WR.HasRelevantMinHp && !p.FulfilsHPRrequirement(WR))
			{
				if (debug)
				{
					Log.Warning(DebugStr + " HP requirement ko, continue");
				}
			}
			else if (WR.HasHediffRequirement && !p.FulfilsHediffRequirement(WR))
			{
				if (debug)
				{
					Log.Warning(DebugStr + " Hediff requirement ko, continue");
				}
			}
			else if (WR.HasFactionRequirement && !p.FulfilsFactionRequirement(WR))
			{
				if (debug)
				{
					Log.Warning(DebugStr + " Faction requirement ko, continue");
				}
			}
			else if (WR.HasLifeStageRequirement && !p.FulfilsLifeStageRequirement(WR))
			{
				if (debug)
				{
					Log.Warning(DebugStr + " lifestage requirement ko, continue");
				}
			}
			else if (WR.HasRelevantChancesToWorkDivider && !p.FulfilsChancesToWorkDivider(WR))
			{
				if (debug)
				{
					Log.Warning(DebugStr + " had not luck, continue");
				}
			}
			else
			{
				yield return CRS;
			}
		}
	}

	public static GraveDig_JobParameters WorkerFulfillsRequirements(this Pawn p, GraveDiggerDef GDD)
	{
		if (GDD.IsEmpty)
		{
			return null;
		}
		if (!GDD.jobParameters.Any((GraveDig_JobParameters jp) => jp.HasWorkerRequirement))
		{
			return GDD.jobParameters.FirstOrFallback();
		}
		foreach (GraveDig_JobParameters jobParameter in GDD.jobParameters)
		{
			if (jobParameter.HasWorkerRequirement)
			{
				WorkerRequirement workerRequirement = jobParameter.workerRequirement;
				if ((!workerRequirement.HasRelevantMinHp || p.FulfilsHPRrequirement(workerRequirement)) && (!workerRequirement.HasHediffRequirement || p.FulfilsHediffRequirement(workerRequirement)) && (!workerRequirement.HasFactionRequirement || p.FulfilsFactionRequirement(workerRequirement)) && (!workerRequirement.HasLifeStageRequirement || p.FulfilsLifeStageRequirement(workerRequirement)))
				{
					return jobParameter;
				}
			}
		}
		return null;
	}

	public static bool FulfilsLifeStageRequirement(this Pawn p, WorkerRequirement WR)
	{
		return WR.lifeStageRequirement.Contains(p.ageTracker.CurLifeStage);
	}

	public static bool FulfilsHPRrequirement(this Pawn p, WorkerRequirement WR)
	{
		return p.health.summaryHealth.SummaryHealthPercent > WR.minHealthPerc;
	}

	public static bool FulfilsHediffRequirement(this Pawn p, WorkerRequirement WR)
	{
		return p.health.hediffSet.hediffs.Any((Hediff h) => WR.hediffRequirement.Any((HediffRequirement h1) => h1.hediff == h.def && h.Severity > h1.severity));
	}

	public static bool FulfilsFactionRequirement(this Pawn p, WorkerRequirement WR)
	{
		if (p.Faction == null)
		{
			return WR.factionRequirement.Any((FactionRequirement f) => f.noFaction);
		}
		if (WR.factionRequirement.Any((FactionRequirement f) => f.belongsToFaction == p.Faction.def))
		{
			return true;
		}
		return false;
	}

	public static bool FulfilsChancesToWorkDivider(this Pawn p, WorkerRequirement WR)
	{
		return (Find.TickManager.TicksGame + p.thingIDNumber) % WR.chancesToWorkDivider == 0;
	}
}
