using System;
using System.Collections.Generic;
using Verse;

namespace Ubet;

public static class ConditionDictionnary
{
	public static readonly Dictionary<ConditionType, Func<Pawn, bool>> noArgconditions = new Dictionary<ConditionType, Func<Pawn, bool>>
	{
		{
			ConditionType.isMale,
			NoArgConditionMethods.PawnIsMale
		},
		{
			ConditionType.isFemale,
			NoArgConditionMethods.PawnIsFemale
		},
		{
			ConditionType.isHuman,
			NoArgConditionMethods.PawnIsHuman
		},
		{
			ConditionType.belongsToPlayerFaction,
			NoArgConditionMethods.PawnIsFromPlayerFaction
		},
		{
			ConditionType.isDrafted,
			NoArgConditionMethods.PawnIsDrafted
		},
		{
			ConditionType.isUndrafted,
			NoArgConditionMethods.PawnIsUndrafted
		},
		{
			ConditionType.isInMentalState,
			NoArgConditionMethods.PawnIsInMentalState
		},
		{
			ConditionType.lyingInBed,
			NoArgConditionMethods.PawnIsInBed
		},
		{
			ConditionType.lyingInLoveBed,
			NoArgConditionMethods.PawnIsInLoveBed
		},
		{
			ConditionType.lyingInMedicalBed,
			NoArgConditionMethods.PawnIsInMedicalBed
		},
		{
			ConditionType.usesNoWeapon,
			NoArgConditionMethods.PawnUsesNoWeapon
		}
	};

	public static readonly Dictionary<ConditionType, Func<Pawn, List<string>, bool>> StringArgconditions = new Dictionary<ConditionType, Func<Pawn, List<string>, bool>>
	{
		{
			ConditionType.isPerformingJob,
			StringArgConditionMethods.PawnIsPerformingJob
		},
		{
			ConditionType.isInSpecificMentalState,
			StringArgConditionMethods.PawnIsInSpecificMentalState
		},
		{
			ConditionType.belongsToLifeStage,
			StringArgConditionMethods.PawnBelongsToLifeStage
		},
		{
			ConditionType.biologicalAgeWithinLifeStage,
			StringArgConditionMethods.RacePawnBelongsToLifeStage
		},
		{
			ConditionType.isPawnKind,
			StringArgConditionMethods.PawnIsPawnKind
		},
		{
			ConditionType.belongsToRace,
			StringArgConditionMethods.PawnIsFromRace
		},
		{
			ConditionType.hasTrait,
			StringArgConditionMethods.PawnHasTrait
		},
		{
			ConditionType.hasBackstory,
			StringArgConditionMethods.PawnHasBackstory
		},
		{
			ConditionType.hasBodyPart,
			StringArgConditionMethods.PawnHasBodyPart
		},
		{
			ConditionType.hasAnyHediff,
			StringArgConditionMethods.PawnHasAnyHediff
		},
		{
			ConditionType.hasAllHediff,
			StringArgConditionMethods.PawnHasAllHediffs
		},
		{
			ConditionType.isOnMapWithWeather,
			StringArgConditionMethods.PawnMapWeather
		},
		{
			ConditionType.isOnMapWithSeason,
			StringArgConditionMethods.PawnMapSeason
		},
		{
			ConditionType.hasAliveRelation,
			StringArgConditionMethods.PawnHasAliveRelation
		},
		{
			ConditionType.hasDeadRelation,
			StringArgConditionMethods.PawnHasDeadRelation
		},
		{
			ConditionType.wearsApparelMadeOf,
			StringArgConditionMethods.PawnWearsApparelMadeOf
		},
		{
			ConditionType.usesWeaponMadeOf,
			StringArgConditionMethods.PawnUsesWeaponMadeOf
		},
		{
			ConditionType.wearsApparel,
			StringArgConditionMethods.PawnUsesApparel
		},
		{
			ConditionType.usesWeapon,
			StringArgConditionMethods.PawnUsesWeapon
		}
	};

	public static readonly Dictionary<ConditionType, Func<Pawn, List<string>, List<string>, bool>> TwoStringArgconditions = new Dictionary<ConditionType, Func<Pawn, List<string>, List<string>, bool>>
	{
		{
			ConditionType.wearsSpecificApparelMadeOf,
			TwoStringArgConditionMethods.PawnWearsSpecificApparelMadeOf
		},
		{
			ConditionType.usesSpecificWeaponMadeOf,
			TwoStringArgConditionMethods.PawnUsesSpecificWeaponMadeOf
		},
		{
			ConditionType.isDoingBill,
			TwoStringArgConditionMethods.PawnDoingBill
		},
		{
			ConditionType.isPerformingTouchJob,
			TwoStringArgConditionMethods.PawnIsPerformingTouchJob
		},
		{
			ConditionType.hasAllHediffOnBodyPart,
			StringArgConditionMethods.PawnHasAllHediffsOnBodyParts
		}
	};

	public static readonly Dictionary<ConditionType, Func<Pawn, List<string>, List<FloatRange>, bool>> StringFloatArgconditions = new Dictionary<ConditionType, Func<Pawn, List<string>, List<FloatRange>, bool>> { 
	{
		ConditionType.hasNeedInRange,
		StringFloatArgConditionMethods.PawnHasNeedInRange
	} };

	public static readonly Dictionary<ConditionType, Func<FloatRange, bool>> FloatRangeArgconditions = new Dictionary<ConditionType, Func<FloatRange, bool>> { 
	{
		ConditionType.floatRangeRandom,
		FloatRangeArgConditionMethods.RandomRoll
	} };

	public static readonly Dictionary<ConditionType, Func<Pawn, SimpleCurve, bool>> CurveArgconditions = new Dictionary<ConditionType, Func<Pawn, SimpleCurve, bool>>
	{
		{
			ConditionType.ageCurveRandom,
			CurveArgConditionMethods.PawnAgeCurveRandomRoll
		},
		{
			ConditionType.healthCurveRandom,
			CurveArgConditionMethods.PawnHealthCurveRandomRoll
		}
	};

	public static readonly Dictionary<ConditionType, Func<Pawn, List<IntRange>, bool>> IntRangeListArgconditions = new Dictionary<ConditionType, Func<Pawn, List<IntRange>, bool>>
	{
		{
			ConditionType.isWithinDayOfQuadrumRange,
			IntRangeArgConditionMethods.DayOfQuadrumWithin
		},
		{
			ConditionType.isWithinDayOfSeasonRange,
			IntRangeArgConditionMethods.DayOfSeasonWithin
		},
		{
			ConditionType.isWithinDayOfTwelfthRange,
			IntRangeArgConditionMethods.DayOfTwelfthWithin
		},
		{
			ConditionType.isWithinDayOfYearRange,
			IntRangeArgConditionMethods.DayOfYearWithin
		},
		{
			ConditionType.isWithinHourOfDayRange,
			IntRangeArgConditionMethods.HourOfDayWithin
		},
		{
			ConditionType.isWithinTwelfthRange,
			IntRangeArgConditionMethods.TwelfthWithin
		}
	};
}
