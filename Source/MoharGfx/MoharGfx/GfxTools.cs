using System;
using UnityEngine;
using Verse;

namespace MoharGfx;

public static class GfxTools
{
	private static float VanillaRythm(this Thing thing)
	{
		return Time.realtimeSinceStartup + 397f * (float)(thing.thingIDNumber % 571);
	}

	public static float VanillaPulse(this Thing thing, float speed, float range)
	{
		float num = thing.VanillaRythm() * speed;
		float num2 = ((float)Math.Sin(num) + 1f) * 0.5f;
		return num2 * range;
	}
}
