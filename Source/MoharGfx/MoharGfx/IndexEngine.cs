using Verse;

namespace MoharGfx;

public class IndexEngine
{
	public enum TickEngine
	{
		[Description("synced")]
		synced,
		[Description("moteLifespan")]
		moteLifespan,
		[Description("relativeMoteLifespan")]
		relativeMoteLifespan,
		[Description("anotherLifespan")]
		anotherLifespan
	}
}
