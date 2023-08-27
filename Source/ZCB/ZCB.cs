using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ZCB
{
    [StaticConstructorOnStartup]
    public static class ZCB
    {
        static ZCB()
        {
            new Harmony("zylle.ChildhoodBackstories").PatchAll();



            if (BackstorySettings.allowedBackstories.EnumerableNullOrEmpty())
            {
                BackstorySettings.allowedBackstories = new Dictionary<string, bool>();
            }
            List<ZCBackstoryDef> list = DefDatabase<ZCBackstoryDef>.AllDefs.ToList();

            foreach (ZCBackstoryDef story in list)
            {
                if (!BackstorySettings.allowedBackstories.ContainsKey(story.defName))
                {
                    BackstorySettings.allowedBackstories.Add(story.defName, true);
                }
            }
        }

        
    }
}
