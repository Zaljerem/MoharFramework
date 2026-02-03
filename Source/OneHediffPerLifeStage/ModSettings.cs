using Verse;
using UnityEngine;


namespace OHPLS
{
    public class OHPLS_Settings : ModSettings
    {
        public bool SafeRemoval = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref SafeRemoval, "SafeRemoval");
        }

    }

    public class OHPLS_Mod : Mod
    {
        OHPLS_Settings settings;

        public OHPLS_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<OHPLS_Settings>();
        }

        public override string SettingsCategory()
        {
            return "ModName_OHPLS".Translate();
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
