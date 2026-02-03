using Verse;
using UnityEngine;


namespace OHPG
{
    public class OHPG_Settings : ModSettings
    {
        public bool SafeRemoval = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref SafeRemoval, "SafeRemoval");
        }

    }

    public class OHPG_Mod : Mod
    {
        OHPG_Settings settings;

        public OHPG_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<OHPG_Settings>();
        }

        public override string SettingsCategory()
        {
            return "ModName_OHPG".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("OHPLS_Label".Translate());
            listing.GapLine();
            listing.Label(
                "OHPLS_Label1".Translate() +
                "OHPLS_Label2".Translate() +
                "OHPLS_Label3".Translate() +
                "OHPLS_Label4".Translate() +
                "OHPLS_Label5".Translate()
            );

            listing.CheckboxLabeled("OHPLS_SMR".Translate(), ref settings.SafeRemoval);

            listing.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
