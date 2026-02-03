using Verse;

namespace Ubet;

public enum ConditionType
{
	isNegligible,
	[Description("is Human")]
	isHuman,
	[Description("is Male")]
	isMale,
	[Description("is Female")]
	isFemale,
	[Description("is Drafted")]
	isDrafted,
	[Description("is Undrafted")]
	isUndrafted,
	[Description("is performing job")]
	isPerformingJob,
	[Description("is performing job with PathEndMode.Touch")]
	isPerformingTouchJob,
	[Description("is doing bill job")]
	isDoingBill,
	[Description("is in mental state")]
	isInMentalState,
	[Description("is in specific mental state")]
	isInSpecificMentalState,
	[Description("belongs to race")]
	belongsToRace,
	[Description("belongs to pawn kind")]
	isPawnKind,
	[Description("belongs to life stage")]
	belongsToLifeStage,
	[Description("biological age is within race def life stages")]
	biologicalAgeWithinLifeStage,
	[Description("belongs to player faction")]
	belongsToPlayerFaction,
	[Description("has trait")]
	hasTrait,
	[Description("has backstory")]
	hasBackstory,
	[Description("has need in range")]
	hasNeedInRange,
	[Description("is on a map with weather")]
	isOnMapWithWeather,
	[Description("is on map with season")]
	isOnMapWithSeason,
	[Description("is within day of year range")]
	isWithinDayOfYearRange,
	[Description("is within day of season range")]
	isWithinDayOfSeasonRange,
	[Description("is within day of quadrum range")]
	isWithinDayOfQuadrumRange,
	[Description("is within day of twelfth range")]
	isWithinDayOfTwelfthRange,
	[Description("is within hour of day range")]
	isWithinHourOfDayRange,
	[Description("is within twelfth range")]
	isWithinTwelfthRange,
	[Description("is lying in a bed")]
	lyingInBed,
	[Description("is lying in a love bed")]
	lyingInLoveBed,
	[Description("is lying in a medical bed")]
	lyingInMedicalBed,
	[Description("has alive relation")]
	hasAliveRelation,
	[Description("has dead relation")]
	hasDeadRelation,
	[Description("has body part")]
	hasBodyPart,
	[Description("has hediff")]
	hasAnyHediff,
	[Description("has all hediffs")]
	hasAllHediff,
	[Description("has all hediffs on bodyparts")]
	hasAllHediffOnBodyPart,
	[Description("wears any apparel made of")]
	wearsApparelMadeOf,
	[Description("uses any weapon made of")]
	usesWeaponMadeOf,
	[Description("wears specific apparel")]
	wearsApparel,
	[Description("uses specific weapon")]
	usesWeapon,
	[Description("wears specific apparel made of")]
	wearsSpecificApparelMadeOf,
	[Description("uses specific weapon made of")]
	usesSpecificWeaponMadeOf,
	[Description("uses no weapon")]
	usesNoWeapon,
	[Description("random dice roll")]
	floatRangeRandom,
	[Description("random dice based on age curve roll")]
	ageCurveRandom,
	[Description("random dice based on health curve roll")]
	healthCurveRandom,
	[Description("empty condition")]
	empty
}
