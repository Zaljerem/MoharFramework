using System;
using System.Collections.Generic;
using HarmonyLib;

namespace YAHA;

public static class PatchDictionary
{
	public static readonly Dictionary<string, Func<Harmony, bool>> harmonyDict = new Dictionary<string, Func<Harmony, bool>>
	{
		{
			"ClearQueuedJobs",
			DraftPatch.TryPatch_ClearQueuedJobs
		},
		{
			"Notify_ApparelAdded",
			WearPatch.TryPatch_ApparelWorn
		},
		{
			"Notify_ApparelRemoved",
			UndressPatch.TryPatch_ApparelUndressed
		},
		{
			"Notify_EquipmentAdded",
			EquipPatch.TryPatch_WeaponEquiped
		},
		{
			"Notify_EquipmentRemoved",
			RemovePatch.TryPatch_WeaponRemoved
		},
		{
			"PostAdd",
			HediffAddedPatch.TryPatch_HediffAdded
		},
		{
			"PostRemove",
			HediffRemovedPatch.TryPatch_HediffRemoved
		}
	};
}
