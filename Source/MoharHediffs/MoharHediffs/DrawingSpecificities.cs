using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public class DrawingSpecificities
{
	public FloatRange randomScale = new FloatRange(0.5f, 0.8f);

	public List<RotationOffset> rotationOffset;

	public Vector3 GetRotationOffset(Pawn p)
	{
		RotationOffset rotationOffset;
		if ((rotationOffset = this.rotationOffset.Where((RotationOffset ro) => ro.rot == p.Rotation).FirstOrFallback()) != null)
		{
			return rotationOffset.offset;
		}
		return Vector3.zero;
	}
}
