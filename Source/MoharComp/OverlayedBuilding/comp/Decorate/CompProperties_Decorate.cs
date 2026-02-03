using System.Collections.Generic;
using Verse;

namespace OLB
{
	public class CompProperties_Decorate : CompProperties
	{
        public List<MoteDecoration> mote;
        public int workerReservationUpdateFrequency = 60;

        public bool debug = false;
        public int verboseLevel = 1;

        public CompProperties_Decorate()
		{
			compClass = typeof(CompDecorate);
		}
	}
}