using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace MoharGfx;

public class HarmonyPatch_Mote
{
	private static class Mote_HarmonyPatch
	{
		private static void Mote_SpawnSetup_Postfix(Mote __instance)
		{
			CustomTransformation_MoteDef customTransformation_MoteDef;
			if (__instance.Graphic is Graphic_AnimatedMote graphic_AnimatedMote && (customTransformation_MoteDef = CustomTransformation_MoteDef.MyNamed(__instance.def.defName)) != null)
			{
				graphic_AnimatedMote.MyDebug = customTransformation_MoteDef.debug;
				if (customTransformation_MoteDef.HasMisc)
				{
					graphic_AnimatedMote.Flipped = customTransformation_MoteDef.transformation.misc.flipped;
				}
				if (customTransformation_MoteDef.HasAnimatedMote)
				{
					graphic_AnimatedMote.FrameOffset = customTransformation_MoteDef.transformation.animatedMote.frameOffset;
					graphic_AnimatedMote.TicksPerFrame = customTransformation_MoteDef.transformation.animatedMote.ticksPerFrame;
					graphic_AnimatedMote.Engine = customTransformation_MoteDef.transformation.animatedMote.engine;
				}
				if (customTransformation_MoteDef.HasRandomRotationRate)
				{
					__instance.rotationRate = customTransformation_MoteDef.transformation.rotation.randRotRate.range.RandomInRange;
				}
				if (customTransformation_MoteDef.HasPulsingScale)
				{
					graphic_AnimatedMote.PulsingScaleRange = customTransformation_MoteDef.transformation.scale.pulsingScale.range;
					graphic_AnimatedMote.PulsingScaleSpeed = customTransformation_MoteDef.transformation.scale.pulsingScale.speed;
				}
			}
		}
	}

	public static bool Try_MoteSpawnSetup_PostfixPatch(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Mote), "SpawnSetup");
			HarmonyMethod prefix = null;
			HarmonyMethod postfix = new HarmonyMethod(typeof(Mote_HarmonyPatch), "Mote_SpawnSetup_Postfix");
			myPatch.Patch(original, prefix, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MoharFramework.MoharGfx failed Try_MoteSpawnSetup_PostfixPatch" + ex);
			return false;
		}
		return true;
	}
}
