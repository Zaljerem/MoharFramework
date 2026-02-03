using System.Collections.Generic;
using Verse;

namespace YAHA;

public class ApplySpecifics
{
	public int applyNumLimit = 1;

	public bool removeIfFalse;

	public Grace grace;

	public Discard discard;

	public List<TriggerEvent> triggerEvent;

	public bool IsTriggered => !triggerEvent.NullOrEmpty();

	public bool HasLimit => applyNumLimit > 0;

	public bool HasDiscard => discard != null;

	public bool HasGrace => grace != null;
}
