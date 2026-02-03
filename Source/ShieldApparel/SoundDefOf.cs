using RimWorld;
using Verse;

namespace ShieldApparel
{
    [DefOf]
    public static class SoundDefOf
    {
        public static SoundDef EnergyShield_Broken;
        public static SoundDef EnergyShield_AbsorbDamage;
        public static SoundDef EnergyShield_Reset;

        static SoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SoundDefOf));
        }
    }
}