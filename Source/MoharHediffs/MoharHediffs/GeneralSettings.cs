using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class GeneralSettings
{
	public List<ThingSettings> things;

	public CommonSettings defaultSettings;

	public bool HasSomethingToSpawn => !things.NullOrEmpty();

	public bool HasDefaultSettings => defaultSettings != null;

	public void LogParams(bool myDebug = false)
	{
		if (myDebug)
		{
			Log.Warning("HasSomethingToSpawn:" + HasSomethingToSpawn + "; HasDefaultSettings:" + HasDefaultSettings + "; ");
		}
	}
}
