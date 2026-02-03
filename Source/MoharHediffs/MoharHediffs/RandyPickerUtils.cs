using System;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class RandyPickerUtils
{
	public static float ThingsTotalWeight(this HediffComp_RandySpawnUponDeath comp, List<ThingSettings> TSList)
	{
		string text = (comp.MyDebug ? (comp.Pawn.LabelShort + " ThingsTotalWeight ") : "");
		if (comp.MyDebug)
		{
			Log.Warning(text + " searching total weights thing list");
		}
		float num = 0f;
		for (int i = 0; i < TSList.Count; i = checked(i + 1))
		{
			num += TSList[i].weight;
		}
		if (comp.MyDebug)
		{
			Log.Warning(text + " found: " + num);
		}
		return num;
	}

	public static bool SameColorAs(this Color colorA, Color colorB)
	{
		bool flag = (double)Math.Abs(colorA.r - colorB.r) < 0.01;
		bool flag2 = (double)Math.Abs(colorA.g - colorB.g) < 0.01;
		bool flag3 = (double)Math.Abs(colorA.b - colorB.b) < 0.01;
		bool flag4 = (double)Math.Abs(colorA.a - colorB.a) < 0.01;
		return flag && flag2 && flag3 && flag4;
	}

	public static Color PickAlienColor(this AlienPartGenerator.AlienComp a, string channelName, int channelNum)
	{
		return channelNum switch
		{
			2 => a.GetChannel(channelName).second, 
			1 => a.GetChannel(channelName).first, 
			_ => Color.white, 
		};
	}

	public static Color PickStuffColor(this ThingDef tDef)
	{
		StuffProperties stuffProps = tDef.stuffProps;
		if (stuffProps != null)
		{
			_ = stuffProps.color;
			if (true)
			{
				return tDef.stuffProps.color;
			}
		}
		ThingDefCountClass thingDefCountClass = tDef.butcherProducts.FirstOrDefault() ?? null;
		ThingDef thingDef = null;
		if (thingDefCountClass == null)
		{
			return Color.black;
		}
		thingDef = thingDefCountClass.thingDef;
		if (thingDef == null || !(thingDef.stuffProps?.color).HasValue)
		{
			return Color.black;
		}
		return thingDef.stuffProps.color;
	}

	public static List<ThingSettings> ThingSettingsWithColor(this HediffComp_RandySpawnUponDeath comp)
	{
		string text = (comp.MyDebug ? (comp.Pawn.LabelShort + " ThingSettingsWithColor -") : "");
		if (comp.MyDebug)
		{
			Log.Warning(text + " creating thing list with color");
		}
		if (!comp.HasColorCondition || !comp.Pawn.IsAlien())
		{
			if (comp.MyDebug)
			{
				Log.Warning(text + "Found no color condition or pawn is not alien");
			}
			return null;
		}
		AlienPartGenerator.AlienComp alien = Tools.GetAlien(comp.Pawn);
		if (alien == null)
		{
			if (comp.MyDebug)
			{
				Log.Warning(text + "Found no AlienPartGenerator.AlienComp");
			}
			return null;
		}
		if (comp.MyDebug)
		{
			Log.Warning(string.Concat(text, "colors=> skin.first:", alien.GetChannel("skin").first, " skin.second:", alien.GetChannel("skin").second));
		}
		List<ThingSettings> list = comp.FullOptionList.Where((ThingSettings t) => t.IsThingSpawner && t.HasColorCondition).ToList();
		if (comp.MyDebug)
		{
			Log.Warning("Option num:" + list.Count);
		}
		Color PawnColor = alien.GetChannel("skin").first;
		foreach (ThingSettings item in list)
		{
			if (comp.MyDebug)
			{
				Log.Warning(string.Concat(" TS.Def: ", item.thingToSpawn.defName, "; TS.color: ", item.thingToSpawn.PickStuffColor(), "; P.color: ", PawnColor, "; equals: ", PawnColor.SameColorAs(item.thingToSpawn.PickStuffColor()).ToString()));
			}
		}
		List<ThingSettings> list2 = new List<ThingSettings>();
		list2 = comp.FullOptionList.Where((ThingSettings t) => t.IsThingSpawner && t.HasColorCondition && PawnColor.SameColorAs(t.thingToSpawn.PickStuffColor())).ToList();
		if (comp.MyDebug)
		{
			Log.Warning(text + "Found " + list2.Count + " things with color");
		}
		return list2;
	}

	public static List<ThingSettings> ThingSettingsWithExclusion(this HediffComp_RandySpawnUponDeath comp, List<ThingSettings> TSList, List<int> AlreadyPickedOptions)
	{
		List<ThingSettings> list = new List<ThingSettings>();
		list = comp.Props.settings.things.ListFullCopy();
		foreach (int AlreadyPickedOption in AlreadyPickedOptions)
		{
			list.RemoveAt(AlreadyPickedOption);
		}
		return list;
	}

	public static int GetWeightedRandomIndex(this HediffComp_RandySpawnUponDeath comp, List<int> AlreadyPickedOptions)
	{
		if (!comp.Props.settings.HasSomethingToSpawn)
		{
			return -1;
		}
		List<ThingSettings> list = ((!comp.HasColorCondition) ? comp.FullOptionList : comp.ThingSettingsWithColor());
		if (!AlreadyPickedOptions.NullOrEmpty())
		{
			list = comp.ThingSettingsWithExclusion(list, AlreadyPickedOptions);
		}
		float num = Rand.Range(0f, comp.ThingsTotalWeight(list));
		for (int i = 0; i < list.Count; i = checked(i + 1))
		{
			if ((num -= list[i].weight) < 0f)
			{
				if (comp.MyDebug)
				{
					Log.Warning("GetWeightedRandomIndex : returning thing " + i);
				}
				if (AlreadyPickedOptions.NullOrEmpty() && !comp.HasColorCondition)
				{
					return i;
				}
				int num2 = comp.Props.settings.things.IndexOf(list[i]);
				if (comp.MyDebug)
				{
					Log.Warning("GetWeightedRandomIndex : returning thing " + i + " normalized:" + num2);
				}
				return num2;
			}
		}
		if (comp.MyDebug)
		{
			Log.Warning("GetWeightedRandomIndex : failed to return proper index, returning -1");
		}
		return -1;
	}
}
