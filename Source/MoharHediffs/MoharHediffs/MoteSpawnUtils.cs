using System;
using MoharGfx;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class MoteSpawnUtils
{
	public static Thing TryMoteSpawn(this Vector3 loc, Map map, float rot, float scale, ThingDef moteDef, bool debug = false)
	{
		if (loc.ForbiddenMote(map))
		{
			return null;
		}
		if (moteDef == null)
		{
			if (debug)
			{
				Log.Warning("null mote");
			}
			return null;
		}
		MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);
		if (moteThrown == null)
		{
			return null;
		}
		moteThrown.Scale = scale;
		moteThrown.exactRotation = rot;
		moteThrown.exactPosition = loc;
		return GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
	}

	public static Thing TryAnyMoteSpawn(this Vector3 loc, Map map, float rot, float scale, ThingDef moteDef, bool debug = false)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		if (loc.ForbiddenMote(map))
		{
			return null;
		}
		if (moteDef == null)
		{
			if (debug)
			{
				Log.Warning("null mote");
			}
			return null;
		}
		Type thingClass = moteDef.thingClass;
		if (thingClass == typeof(CustomTransformation_Mote))
		{
			CustomTransformation_Mote mote = (CustomTransformation_Mote)ThingMaker.MakeThing(moteDef);
			return mote.FinalizeMoteSpawn(loc, map, rot, scale);
		}
		if (thingClass == typeof(MoteThrown))
		{
			MoteThrown mote2 = (MoteThrown)ThingMaker.MakeThing(moteDef);
			return mote2.FinalizeMoteSpawn(loc, map, rot, scale);
		}
		return null;
	}

	public static Thing FinalizeMoteSpawn(this CustomTransformation_Mote mote, Vector3 loc, Map map, float rot, float scale)
	{
		((Mote)(object)mote).Scale = scale;
		((Mote)(object)mote).exactRotation = rot;
		((Mote)(object)mote).exactPosition = loc;
		return GenSpawn.Spawn((Thing)(object)mote, loc.ToIntVec3(), map);
	}

	public static Thing FinalizeMoteSpawn(this MoteThrown mote, Vector3 loc, Map map, float rot, float scale)
	{
		mote.Scale = scale;
		mote.exactRotation = rot;
		mote.exactPosition = loc;
		return GenSpawn.Spawn(mote, loc.ToIntVec3(), map);
	}
}
