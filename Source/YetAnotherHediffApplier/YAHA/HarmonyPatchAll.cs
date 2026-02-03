using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace YAHA;

[StaticConstructorOnStartup]
internal static class HarmonyPatchAll
{
	static HarmonyPatchAll()
	{
		Harmony arg = new Harmony("goudaQuiche.MoharFramework.YAHA");
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, Func<Harmony, bool>> item in PatchDictionary.harmonyDict)
		{
			if (item.Value(arg))
			{
				list.Add(item.Key);
			}
			else
			{
				list2.Add(item.Key);
			}
		}
		//Log.Message(SmallReport(list, list2));
	}

	private static string SmallReport(List<string> successStr, List<string> failureStr)
	{
		string text = "MoharFW YAHA - ";
		if (!successStr.NullOrEmpty())
		{
			text = text + "successfuly completed " + successStr.Count + " harmony patches. ";
		}
		if (!failureStr.NullOrEmpty())
		{
			text = text + "Patch failures:" + failureStr.Count + ".";
		}
		return text;
	}

	private static string ReportDump(List<string> successStr, List<string> failureStr)
	{
		string text = "MoharFW YAHA - ";
		checked
		{
			if (!successStr.NullOrEmpty())
			{
				text += "Patch success:";
				for (int i = 0; i < successStr.Count; i++)
				{
					text = text + ((i == 0) ? "" : "/") + successStr[i];
				}
				text += "; ";
			}
			if (!failureStr.NullOrEmpty())
			{
				text += "Patch failure:";
				for (int j = 0; j < failureStr.Count; j++)
				{
					text = text + failureStr[j] + ((j == 0) ? "" : "/");
				}
				text += ";";
			}
			return text;
		}
	}
}
