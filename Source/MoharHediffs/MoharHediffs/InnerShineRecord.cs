using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoharHediffs;

public class InnerShineRecord
{
	public string label;

	public List<Thing> spawned;

	public int ticksLeft;

	public Color lastColor;

	public string Dump => $"label:{label} spawned:{spawned?.CountAllowNull()} ticksLeft:{ticksLeft} lastColor:{lastColor}";

	public InnerShineRecord(InnerShineItem ISI)
	{
		label = ISI.label;
		spawned = new List<Thing>();
		ticksLeft = ISI.NewPeriod();
		lastColor = Color.black;
	}
}
