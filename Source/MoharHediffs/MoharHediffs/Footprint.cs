using UnityEngine;

namespace MoharHediffs;

public class Footprint
{
	public float intervalDistance = 0.632f;

	public Vector3 offset = new Vector3(0f, 0f, -0.3f);

	public float distanceBetweenFeet = 0.17f;

	public string Dump()
	{
		return string.Concat("intervalDistance:", intervalDistance, "; offset:", offset, "; distanceBetweenFeet:", distanceBetweenFeet);
	}
}
