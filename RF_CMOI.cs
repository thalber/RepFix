using OptionalUI;
using System.Collections.Generic;
using UnityEngine;

namespace WaspPile.RepFix
{
    internal class RF_CMOI : OptionInterface
    {
        public RF_CMOI() : base (RepFixMod.oneinstance)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            this.Tabs = new OpTab[1];
            this.Tabs[0] = new OpTab();
            List<UIelement> elms = new List<UIelement>();
            elms.Add(new OpCheckBox(new Vector2(40f, 500f), "RF_USE", true) { description = "Set whether you want to use rel alterations or not" });
            elms.Add(new OpCheckBox(new Vector2(80f, 500f), "RF_SPODE", true) { description = "Make wolf spiders afraid of the player"});
            elms.Add(new OpCheckBox(new Vector2(120f, 500f), "RF_TUCHDAGHOST", true) { description = "Set whether ghosts should be forced onto main collision layer"});
            elms.Add(new OpSlider(new Vector2(50f, 450f), "RF_AGGSCALE", new RWCustom.IntVector2(0, 200), 100) { description = "Set AGGSCALE, in percentage" }) ;
            elms.Add(new OpSlider(new Vector2(50f, 400f), "RF_SCALEFAC", new RWCustom.IntVector2(0, 200), 100) { description = "Set SCALEFAC, in percentage" });
            elms.Add(new OpLabelLong(new Vector2(50f, 100f), new Vector2(350f, 150f), "Resulting aggression priotity is determined by lerping between initial aggression priority and (initial priority * AGGSCALE)^2 by SCALEFAC. \n Initial aggression level is always between 0 and 1, and final result is clamped into [0;1] range.\n Example: if initial aggro is 1, AGGSCALE is 0.5 and SCALEFAC is 0.5, result will be ((1 + (1*0.5)^2))/2 = 0.625.\nApplies to lizards and vultures."));
            this.Tabs[0].AddItems(elms.ToArray());
        }

        public override void ConfigOnChange()
        {
            base.ConfigOnChange();
            RF_CFG.USE = bool.Parse(config["RF_USE"]);
            RF_CFG.AGGSCALE = ((float)int.Parse(config["RF_AGGSCALE"]) / 100);
            RF_CFG.SCALEFAC = (float)int.Parse(config["RF_SCALEFAC"]) / 100;
            RF_CFG.SCAREDSPIDERS = bool.Parse(config["RF_SPODE"]);
            if (RF_CFG.SCAREDSPIDERS) StaticWorld.EstablishRelationship(CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 1f));
            else StaticWorld.EstablishRelationship(CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.4f));
            
        }
    }

    public class RF_CFG
    {
        public static bool TUCHDAGHOST = true;
        public static bool USE = true;
        public static float AGGSCALE = 1f;
        public static float SCALEFAC = 0.5f;
        public static bool SCAREDSPIDERS = true;
    }
}
