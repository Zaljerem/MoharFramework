using System;
using UnityEngine;
using Verse;

namespace MoharGfx;

[StaticConstructorOnStartup]
public class Graphic_AnimatedMote : Graphic_Collection
{
	protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	public int TicksPerFrame = 7;

	public int FrameOffset = 0;

	public IndexEngine.TickEngine Engine = IndexEngine.TickEngine.synced;

	public bool Flipped = false;

	public Vector2 PulsingScaleRange = new Vector2(0f, 0f);

	public float PulsingScaleSpeed = 0.5f;

	public bool MyDebug = false;

	protected virtual bool ForcePropertyBlock => false;

	public bool HasPulsingScale => PulsingScaleRange != Vector2.zero;

	public int GetIndex
	{
		get
		{
			int num = (int)Math.Floor((double)(Find.TickManager.TicksGame / TicksPerFrame));
			return num % subGraphics.Length;
		}
	}

	public int TickEngine(Mote mote)
	{
		if (Engine == IndexEngine.TickEngine.moteLifespan)
		{
			return Find.TickManager.TicksGame - mote.spawnTick;
		}
		if (Engine == IndexEngine.TickEngine.relativeMoteLifespan)
		{
			return (int)((float)(Find.TickManager.TicksGame - mote.spawnTick) * (mote.AgeSecs / mote.def.mote.Lifespan));
		}
		return Find.TickManager.TicksGame;
	}

	public int GetAnotherIndex(Mote mote)
	{
		int num = (int)Math.Floor((double)(TickEngine(mote) / TicksPerFrame));
		return num % subGraphics.Length;
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		DrawMoteInternal(loc, rot, thingDef, thing, 0);
	}

	public void ResolveScale(Mote mote, Thing thing, out Vector3 finalePos, out Vector3 exactScale)
	{
		exactScale = mote.linearScale;
		exactScale.x *= data.drawSize.x;
		exactScale.z *= data.drawSize.y;
		if (HasPulsingScale)
		{
			float num = thing.VanillaPulse(PulsingScaleSpeed, PulsingScaleRange.x);
			float num2 = thing.VanillaPulse(PulsingScaleSpeed, PulsingScaleRange.y);
			exactScale.x += num;
			exactScale.z += num2;
		}
		finalePos = new Vector3
		{
			x = mote.DrawPos.x + mote.def.graphicData.drawOffset.x,
			y = mote.DrawPos.y + mote.def.graphicData.drawOffset.y,
			z = mote.DrawPos.z + mote.def.graphicData.drawOffset.z
		};
	}

	public void ResolveAlpha(Mote mote, out Color color)
	{
		float alpha = mote.Alpha;
		color = base.Color * mote.instanceColor;
		if (!(alpha <= 0f))
		{
			color.a *= alpha;
		}
	}

	public void ResolveAnimationFram(Mote mote, out Material myMaterial)
	{
		int num = (GetAnotherIndex(mote) + FrameOffset) % subGraphics.Length;
		Graphic graphic = subGraphics[num];
		myMaterial = graphic.MatSingle;
	}

	public void DrawMoteInternal(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer)
	{
		Mote mote = (Mote)thing;
		ResolveAlpha(mote, out var color);
		ResolveScale(mote, thing, out var finalePos, out var exactScale);
		Matrix4x4 matrix = default(Matrix4x4);
		matrix.SetTRS(finalePos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);
		ResolveAnimationFram(mote, out var myMaterial);
		Mesh mesh = (Flipped ? MeshPool.plane10Flip : MeshPool.plane10);
		if (!ForcePropertyBlock && color.IndistinguishableFrom(myMaterial.color))
		{
			Graphics.DrawMesh(mesh, matrix, myMaterial, layer, null, 0);
			return;
		}
		propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
		Graphics.DrawMesh(mesh, matrix, myMaterial, layer, null, 0, propertyBlock);
	}
}
