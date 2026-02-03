using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public class HediffComp_RainbowTrail : HediffComp
{
	public enum RainbowColor
	{
		[Description("purple")]
		purple,
		[Description("blue")]
		blue,
		[Description("green")]
		green,
		[Description("yellow")]
		yellow,
		[Description("orange")]
		orange,
		[Description("red")]
		red
	}

	public enum ColorChoice
	{
		[Description("pawnColor")]
		pawnColor,
		[Description("complementary")]
		complementary,
		[Description("random")]
		random
	}

	public enum CycleKind
	{
		[Description("circular")]
		circular,
		[Description("bouncing")]
		bouncing,
		[Description("random")]
		random
	}

	private Pawn myPawn = null;

	private Map myMap = null;

	private Vector3 lastFootprintPlacePos;

	private static readonly Vector3 PuddleOffset = new Vector3(0f, 0f, -0.3f);

	private int ticksLeft;

	private List<ThingDef> moteDef = null;

	private RainbowColor curColor = RainbowColor.purple;

	private int sameColorInRow = 0;

	public RainbowColor bottomColor;

	public RainbowColor pivotColor;

	public RainbowColor topColor;

	private int variation = 1;

	private bool blockAction = false;

	private bool myDebug = false;

	public HediffCompProperties_RainbowTrail Props => (HediffCompProperties_RainbowTrail)props;

	public override void CompPostMake()
	{
		if (Props.hideBySeverity)
		{
			parent.Severity = 0.05f;
		}
		myDebug = Props.debug;
		Init(myDebug);
	}

	public override void CompExposeData()
	{
		Scribe_Values.Look(ref ticksLeft, "ticksLeft", 0);
		Scribe_Values.Look(ref curColor, "curColor", RainbowColor.purple);
		Scribe_Values.Look(ref variation, "variation", 0);
		Scribe_Values.Look(ref myDebug, "debug", defaultValue: false);
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		SetPawnAndMap();
		if (myPawn == null || myMap == null || blockAction)
		{
			Tools.Warn("null tick - pawn: " + (myPawn == null) + "myMap: " + (myMap == null) + "blockAction: " + blockAction, myDebug);
			return;
		}
		if (moteDef.NullOrEmpty())
		{
			Tools.Warn("empty moteDef", myDebug);
			Init(myDebug);
		}
		TerrainDef terrain = myPawn.Position.GetTerrain(myPawn.Map);
		checked
		{
			if (terrain == null || terrain.IsWater || myPawn.Map.snowGrid.GetDepth(myPawn.Position) >= 0.4f || !myPawn.Position.InBounds(myMap))
			{
				Tools.Warn(string.Concat(myPawn, "'s ", parent.def.defName, " is blocked by terrain"), myDebug);
			}
			else if (ticksLeft <= 0)
			{
				if (TryPlaceMote())
				{
					sameColorInRow++;
					if (sameColorInRow >= Props.minTimesSameColor || sameColorInRow > Props.maxTimesSameColor || !Rand.Chance(Props.staySameColorChance))
					{
						if (Props.threeColorsGradient)
						{
							NextColorThreeColors();
						}
						else
						{
							NextColorRainbow();
						}
						Tools.Warn(string.Concat(myPawn, "'s ", parent.def.defName, " now uses ", curColor, "after ", sameColorInRow, "puddles"), myDebug);
						SetMoteDef();
						sameColorInRow = 0;
					}
					Reset();
				}
				else
				{
					Tools.Warn(string.Concat(myPawn, "'s ", parent.def.defName, "failed to TryPlaceMote"), myDebug);
				}
			}
			else
			{
				ticksLeft--;
			}
		}
	}

	public void SetMoteDef()
	{
		switch (curColor)
		{
		case RainbowColor.purple:
			moteDef = Props.motePurpleDef;
			break;
		case RainbowColor.blue:
			moteDef = Props.moteBlueDef;
			break;
		case RainbowColor.green:
			moteDef = Props.moteGreenDef;
			break;
		case RainbowColor.yellow:
			moteDef = Props.moteYellowDef;
			break;
		case RainbowColor.orange:
			moteDef = Props.moteOrangeDef;
			break;
		case RainbowColor.red:
			moteDef = Props.moteRedDef;
			break;
		}
	}

	public void NextColorRainbow()
	{
		checked
		{
			curColor += variation;
		}
		if (Props.cycleKind == CycleKind.circular)
		{
			if (curColor > RainbowColor.red)
			{
				curColor = RainbowColor.purple;
			}
			else if (curColor < RainbowColor.purple)
			{
				curColor = RainbowColor.red;
			}
		}
		else if (Props.cycleKind == CycleKind.bouncing)
		{
			if (curColor > RainbowColor.red)
			{
				variation = -1;
				curColor = RainbowColor.orange;
			}
			else if (curColor < RainbowColor.purple)
			{
				variation = 1;
				curColor = RainbowColor.blue;
			}
		}
		else if (Props.cycleKind == CycleKind.random)
		{
			curColor = (RainbowColor)Rand.Range(0, 6);
		}
	}

	public void NextColorThreeColors()
	{
		checked
		{
			curColor += variation;
			if (Props.cycleKind == CycleKind.circular)
			{
				if (curColor > topColor)
				{
					curColor = bottomColor;
				}
				else if (curColor < bottomColor)
				{
					curColor = topColor;
				}
			}
			else if (Props.cycleKind == CycleKind.bouncing)
			{
				if (curColor > topColor)
				{
					variation = -1;
					curColor = pivotColor;
				}
				else if (curColor < RainbowColor.purple)
				{
					variation = 1;
					curColor = pivotColor;
				}
			}
			else if (Props.cycleKind == CycleKind.random)
			{
				if (Rand.Chance(0.33f))
				{
					curColor = pivotColor;
				}
				else if (Rand.Chance(0.5f))
				{
					curColor = topColor;
				}
				else
				{
					curColor = bottomColor;
				}
			}
		}
	}

	private void SetPawnAndMap()
	{
		myPawn = parent.pawn;
		myMap = myPawn.Map;
	}

	public RainbowColor ColorTranslation(Color myColor)
	{
		if (myColor == MyGfx.Purple)
		{
			return RainbowColor.purple;
		}
		if (myColor == MyGfx.Blue)
		{
			return RainbowColor.blue;
		}
		if (myColor == MyGfx.Green)
		{
			return RainbowColor.green;
		}
		if (myColor == MyGfx.Yellow)
		{
			return RainbowColor.yellow;
		}
		if (myColor == MyGfx.Orange)
		{
			return RainbowColor.orange;
		}
		if (myColor == MyGfx.Red)
		{
			return RainbowColor.red;
		}
		Tools.Warn(myPawn.LabelShort + " could not ColorTranslation, default value", myDebug);
		return RainbowColor.blue;
	}

	public void Init(bool myDebug = false)
	{
		SetPawnAndMap();
		Tools.Warn(string.Concat(myPawn, "'s ", parent.def.defName, " Init"), myDebug);
		if (Props.motePurpleDef.NullOrEmpty() || Props.moteBlueDef.NullOrEmpty() || Props.moteGreenDef.NullOrEmpty() || Props.moteYellowDef.NullOrEmpty() || Props.moteOrangeDef.NullOrEmpty() || Props.moteRedDef.NullOrEmpty())
		{
			blockAction = true;
			parent.Severity = 0f;
		}
		switch (Props.colorChoice)
		{
		case ColorChoice.pawnColor:
		{
			Color myColor2 = GfxEffects.ClosestColor(myPawn, complementary: false, myDebug);
			pivotColor = (curColor = ColorTranslation(myColor2));
			break;
		}
		case ColorChoice.complementary:
		{
			Color myColor = GfxEffects.ClosestColor(myPawn, complementary: true, myDebug);
			pivotColor = (curColor = ColorTranslation(myColor));
			break;
		}
		case ColorChoice.random:
			pivotColor = (curColor = (RainbowColor)Rand.Range(0, 6));
			break;
		}
		checked
		{
			bottomColor = pivotColor - 1;
			topColor = pivotColor + 1;
			if (topColor > RainbowColor.red)
			{
				topColor = RainbowColor.purple;
			}
			if (bottomColor < RainbowColor.purple)
			{
				bottomColor = RainbowColor.red;
			}
			Tools.Warn(string.Concat(myPawn, "'s ", parent.def.defName, " bottom: ", bottomColor, " pivot: ", pivotColor, " top: ", topColor), myDebug);
			SetMoteDef();
		}
	}

	public static void PlacePuddle(Vector3 loc, Map map, float rot, float scale, ThingDef Mote_FootprintDef)
	{
		if (loc.ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority)
		{
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(Mote_FootprintDef);
			moteThrown.Scale = scale;
			moteThrown.exactRotation = rot;
			moteThrown.exactPosition = loc;
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
		}
	}

	private bool TryPlaceMote()
	{
		Vector3 drawPos = myPawn.Drawer.DrawPos;
		Vector3 normalized = (drawPos - lastFootprintPlacePos).normalized;
		float rot = normalized.AngleFlat();
		Vector3 vector = myPawn.TrueCenter() + PuddleOffset;
		IntVec3 c = vector.ToIntVec3();
		if (c.InBounds(myMap))
		{
			TerrainDef terrain = c.GetTerrain(myPawn.Map);
			if (terrain != null)
			{
				PlacePuddle(vector, myMap, rot, Props.scale.RandomInRange, moteDef.RandomElement());
				return true;
			}
		}
		lastFootprintPlacePos = drawPos;
		return false;
	}

	private void Reset()
	{
		ticksLeft = Props.period;
	}
}
