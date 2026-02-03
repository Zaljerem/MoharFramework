using System;
using AlienRace;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public static class GfxEffects
{
	public static Color ClosestColor(Pawn pawn, bool complementary = false, bool myDebug = false)
	{
		AlienPartGenerator.AlienComp alien = Tools.GetAlien(pawn);
		Color color;
		if (alien == null)
		{
			color = pawn.DrawColor;
		}
		else
		{
			color = alien.GetChannel("skin").first;
			if (myDebug)
			{
				Log.Warning(pawn.LabelShort + " is alien, color=" + color);
			}
		}
		Color color2 = Color.blue;
		float num = 1000f;
		float num2 = Math.Abs(color.r - MyGfx.Purple.r) + Math.Abs(color.g - MyGfx.Purple.g) / 4f + Math.Abs(color.b - MyGfx.Purple.b);
		float num3 = Math.Abs(color.r - MyGfx.Blue.r) / 2f + Math.Abs(color.g - MyGfx.Blue.g) / 2f + Math.Abs(color.b - MyGfx.Blue.b);
		float num4 = Math.Abs(color.r - MyGfx.Cyan.r) / 4f + Math.Abs(color.g - MyGfx.Cyan.g) + Math.Abs(color.b - MyGfx.Cyan.b);
		float num5 = Math.Abs(color.r - MyGfx.Green.r) / 2f + Math.Abs(color.g - MyGfx.Green.g) + Math.Abs(color.b - MyGfx.Green.b) / 2f;
		float num6 = Math.Abs(color.r - MyGfx.Yellow.r) + Math.Abs(color.g - MyGfx.Yellow.g) + Math.Abs(color.b - MyGfx.Yellow.b) / 4f;
		float num7 = Math.Abs(color.r - MyGfx.Orange.r) + Math.Abs(color.g - MyGfx.Orange.g) / 1.6f + Math.Abs(color.b - MyGfx.Orange.b) / 2.5f;
		float num8 = Math.Abs(color.r - MyGfx.Red.r) + Math.Abs(color.g - MyGfx.Red.g) / 2f + Math.Abs(color.b - MyGfx.Red.b) / 2f;
		if (myDebug)
		{
			Log.Warning(pawn.LabelShort + "'s pColor: " + color);
			Log.Warning("purpleDiff: " + num2 + "; blueDiff: " + num3 + "; cyanDiff: " + num4 + "; greenDiff: " + num5 + "; yellowDiff: " + num6 + "; orangeDiff: " + num7 + "; redDiff: " + num8);
		}
		if (num2 < num)
		{
			num = num2;
			color2 = MyGfx.Purple;
		}
		if (num3 < num)
		{
			num = num3;
			color2 = MyGfx.Blue;
		}
		if (num4 < num)
		{
			num = num4;
			color2 = MyGfx.Blue;
		}
		if (num5 < num)
		{
			num = num5;
			color2 = MyGfx.Green;
		}
		if (num6 < num)
		{
			num = num6;
			color2 = MyGfx.Yellow;
		}
		if (num7 < num)
		{
			num = num7;
			color2 = MyGfx.Orange;
		}
		if (num8 < num)
		{
			num = num8;
			color2 = MyGfx.Red;
		}
		if (complementary)
		{
			if (color2 == MyGfx.Purple)
			{
				color2 = MyGfx.Yellow;
			}
			else if (color2 == MyGfx.Blue || color2 == MyGfx.Cyan)
			{
				color2 = MyGfx.Orange;
			}
			else if (color2 == MyGfx.Green)
			{
				color2 = MyGfx.Red;
			}
			else if (color2 == MyGfx.Yellow)
			{
				color2 = MyGfx.Purple;
			}
			else if (color2 == MyGfx.Orange)
			{
				color2 = MyGfx.Blue;
			}
			else if (color2 == MyGfx.Red)
			{
				color2 = MyGfx.Green;
			}
		}
		if (myDebug)
		{
			Log.Warning(complementary ? "complementary" : ("closest Color=" + color2));
		}
		return color2;
	}
}
