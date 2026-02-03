using System;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class MoteColorChangeUtils
{
	public static int GetProgressSign(float limA, float limB, float val)
	{
		if (val <= limA && limA < limB)
		{
			return 1;
		}
		if (val >= limB && limB > limA)
		{
			return -1;
		}
		return Rand.Chance(0.5f) ? 1 : (-1);
	}

	public static Color RandomPickColor(this ColorRange colorRange, Color oldColor, bool debug = false)
	{
		float variationPerIteration = colorRange.variationPerIteration;
		float num = Rand.Range(0f, variationPerIteration);
		float num2 = Rand.Range(0f, variationPerIteration - num);
		float num3 = variationPerIteration - num - num2;
		int progressSign = GetProgressSign(colorRange.colorA.r, colorRange.colorB.r, oldColor.r);
		int progressSign2 = GetProgressSign(colorRange.colorA.g, colorRange.colorB.g, oldColor.g);
		int progressSign3 = GetProgressSign(colorRange.colorA.b, colorRange.colorB.b, oldColor.b);
		float num4 = Math.Abs(colorRange.colorA.r - colorRange.colorB.r) * num * (float)progressSign;
		float num5 = Math.Abs(colorRange.colorA.g - colorRange.colorB.g) * num2 * (float)progressSign3;
		float num6 = Math.Abs(colorRange.colorA.b - colorRange.colorB.b) * num3 * (float)progressSign2;
		return new Color((oldColor.r + num4).Clamp(colorRange.colorA.r, colorRange.colorB.r), (oldColor.g + num5).Clamp(colorRange.colorA.g, colorRange.colorB.g), (oldColor.b + num6).Clamp(colorRange.colorA.b, colorRange.colorB.b));
	}

	public static void ChangeMoteColor(this HediffComp_TrailLeaver comp, Mote mote)
	{
		if (comp.Props.HasColorRange && mote != null)
		{
			if (comp.lastColor == Color.black)
			{
				comp.lastColor = comp.Props.colorRange.colorA;
			}
			comp.lastColor = comp.Props.colorRange.RandomPickColor(comp.lastColor, comp.MyDebug);
			mote.instanceColor = comp.lastColor;
		}
	}
}
