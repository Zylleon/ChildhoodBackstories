using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ZCB
{
    [HarmonyPatch(typeof(RimWorld.LifeStageWorker_HumanlikeAdult), "Notify_LifeStageStarted")]
    static class BackstoryReplacementPatch
    {
        static void Postfix(Pawn pawn, LifeStageDef previousLifeStage)
        {
            if (pawn.story.Childhood == ZCBDefOf.ColonyChild59 && previousLifeStage.developmentalStage.Juvenile())
            {
                List<ZCBackstoryDef> backstories = DefDatabase<ZCBackstoryDef>.AllDefsListForReading;
                backstories = backstories.Where(b => b.IsAcceptable(pawn) && b.spawnCategories.Contains("ZCB")).ToList();
                ZCBackstoryDef backstory = ZCBDefOf.ZCB_ColonyChild;
                if (!backstories.NullOrEmpty())
                {
                    backstory = backstories.RandomElement();
                }

                pawn.story.Childhood = backstory;
                if (!backstory.skillGains.NullOrEmpty())
                {
                    foreach (KeyValuePair<SkillDef, int> skillGain in backstory.skillGains)
                    {
                        pawn.skills.GetSkill(skillGain.Key).Level += skillGain.Value;
                    }
                }
            }

        }
    }
}
