using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace ZCB
{
    public class BackstorySettings : ModSettings
    {
        public static Dictionary<string, bool> allowedBackstories = new Dictionary<string, bool>();

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref allowedBackstories, "allowedBackstories", LookMode.Value, LookMode.Value);
        }
    }

    public class BackstoryMod : Mod
    {
        BackstorySettings settings;
        private static Vector2 scrollPosition = Vector2.zero;
        private static float viewHeight;

        public BackstoryMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<BackstorySettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            List<ZCBackstoryDef> list = DefDatabase<ZCBackstoryDef>.AllDefs.OrderBy(b => b.title).ToList();
            list.Remove(ZCBDefOf.ZCB_ColonyChild);
            list.Remove(ZCBDefOf.ZCB_TribalChild);

            Listing_Standard outerListing = new Listing_Standard();
            outerListing.Begin(inRect);

            if (outerListing.ButtonText("ZCB_AllowAllBackstories".Translate()))
            {
                AllowAllBackstories(list);
            }
            outerListing.Gap();
            outerListing.Label("ZCB_Warning".Translate());
            outerListing.GapLine();

            Rect windowRect = outerListing.GetRect(inRect.height - outerListing.CurHeight).ContractedBy(4f);

            Rect viewRect = new Rect(0f, 0f, 200f, 50f + 24 * list.Count());

            Widgets.BeginScrollView(windowRect, ref scrollPosition, viewRect, true);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            foreach (ZCBackstoryDef story in list)
            {
                bool allowed = BackstorySettings.allowedBackstories[story.defName];
                //listing.CheckboxLabeled(story.title, ref allowed);
                listing.CheckboxLabeled(story.title, ref allowed, BackstoryTooltip(story));

                BackstorySettings.allowedBackstories[story.defName] = allowed;
            }

            listing.End();

            Widgets.EndScrollView();

            outerListing.End();
            //base.DoSettingsWindowContents(inRect);
        }

        public void AllowAllBackstories( List<ZCBackstoryDef> list)
        {
            Dictionary<string, bool> newStorySettings = new Dictionary<string, bool>();

            foreach (ZCBackstoryDef story in list)
            {
                newStorySettings.Add(story.defName, true);
            }

            BackstorySettings.allowedBackstories = newStorySettings;
        }


        public string BackstoryTooltip(ZCBackstoryDef story)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(story.baseDesc);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                SkillDef skillDef = allDefsListForReading[i];
                if (story.skillGains.ContainsKey(skillDef))
                {
                    stringBuilder.AppendLine(skillDef.skillLabel.CapitalizeFirst() + ":   " + story.skillGains[skillDef].ToString("+##;-##"));
                }
            }
            if (story.DisabledWorkTypes.Any() || story.DisabledWorkGivers.Any())
            {
                stringBuilder.AppendLine();
            }
            foreach (WorkTypeDef disabledWorkType in story.DisabledWorkTypes)
            {
                stringBuilder.AppendLine(disabledWorkType.gerundLabel.CapitalizeFirst() + " " + "DisabledLower".Translate());
            }
            foreach (WorkGiverDef disabledWorkGiver in story.DisabledWorkGivers)
            {
                stringBuilder.AppendLine(disabledWorkGiver.workType.gerundLabel.CapitalizeFirst() + ": " + disabledWorkGiver.LabelCap + " " + "DisabledLower".Translate());
            }
            

            string str = stringBuilder.ToString().TrimEndNewlines();
            return str;
        }


        public override string SettingsCategory()
        {
            return "ZCB_ModName".Translate();
        }

    }
}
