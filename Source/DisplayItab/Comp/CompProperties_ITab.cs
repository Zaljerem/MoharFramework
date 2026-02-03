using System.Collections.Generic;
using UnityEngine;
using Verse;


namespace DisplayITab
{
	public class CompProperties_ITab : CompProperties
	{
        public List<ThingDef> Pages;

        public Vector2 imgSize = new Vector2(512,512);
        public DisplayWay displayWay = DisplayWay.UnscaledTexture;

        public CompProperties_ITab()
		{
			compClass = typeof(Comp_ITab);
		}
	}

    public enum DisplayWay
    {
        UnscaledTexture = 0,
        ProcessedTexture = 1
    }
}