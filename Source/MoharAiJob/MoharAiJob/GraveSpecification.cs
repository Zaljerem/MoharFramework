using System.Collections.Generic;
using Verse;

namespace MoharAiJob;

public class GraveSpecification
{
	public List<ThingDef> eligibleGraves;

	public float maxDistance = 10f;

	public ReservationProcess reservation;

	public bool HasEligibleGraves => !eligibleGraves.NullOrEmpty();

	public bool HasReservation => reservation != null;
}
