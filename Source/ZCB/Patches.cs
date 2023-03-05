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
    static class PredAdjustment
    {
        static void Postfix(Pawn pawn, LifeStageDef previousLifeStage)
        {
            if (pawn.story.Childhood == ZCBDefOf.ColonyChild59)
            {
                Log.Message("Shuffling Backstory");

                List<ZCBackstoryDef> backstories = DefDatabase<ZCBackstoryDef>.AllDefsListForReading;
                backstories = backstories.Where(b => b.IsAcceptable(pawn)).ToList();
                Log.Message("Available backstories: " + backstories.Count());
                ZCBackstoryDef backstory = backstories.RandomElement();
                pawn.story.Childhood = backstory;

                Log.Message(backstory.defName);

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
