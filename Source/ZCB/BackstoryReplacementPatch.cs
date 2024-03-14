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
            if ((pawn.story.Childhood == ZCBDefOf.ColonyChild59 || pawn.story.Childhood == ZCBDefOf.Child27 || pawn.story.Childhood == ZCBDefOf.ChildTribal || pawn.story.Childhood == ZCBDefOf.TribeChild19) && previousLifeStage != null && previousLifeStage.developmentalStage.Juvenile())
            {
                List<ZCBackstoryDef> backstories = DefDatabase<ZCBackstoryDef>.AllDefsListForReading;
                backstories = backstories.Where(b => b.IsAcceptable(pawn) && b.spawnCategories.Contains("ZCB") && BackstorySettings.allowedBackstories[b.defName]).ToList();
                ZCBackstoryDef backstory = ZCBDefOf.ZCB_ColonyChild;
                if (!backstories.NullOrEmpty())
                {
                    //backstory = backstories.RandomElement();
                    backstory = backstories.RandomElementByWeight(bk => bk.commonality);
                }

                pawn.story.Childhood = backstory;
                if (!backstory.skillGains.NullOrEmpty())
                {
                    foreach (SkillGain skillGain in backstory.skillGains)
                    {
                        pawn.skills.GetSkill(skillGain.skill).Level += skillGain.amount;
                    }
                }


                if(backstory.forcedTraits != null)
                {
                    List<BackstoryTrait> forcedTraits = backstory.forcedTraits;
                    for (int i = 0; i < forcedTraits.Count; i++)
                    {
                        BackstoryTrait te2 = forcedTraits[i];
                        if (te2.def == null)
                        {
                            Log.Error("Null forced trait def on " + pawn.story.Childhood);
                        }
                        else 
                        {
                            pawn.story.traits.GainTrait(new Trait(te2.def, te2.degree));
                        }
                    }
                }

                if (backstory.passionGains != null)
                {
                    foreach (KeyValuePair<SkillDef, int> passionGain in backstory.passionGains)
                    {
                        int passion = (int)pawn.skills.GetSkill(passionGain.Key).passion;
                        passion += passionGain.Value;
                        passion = Math.Min(Math.Max(passion, 0),2);

                        pawn.skills.GetSkill(passionGain.Key).passion = (Passion)passion;
                    }
                }

                if (pawn.story.Adulthood == ZCBDefOf.Colonist97 || pawn.story.Adulthood == ZCBDefOf.TribeMember57)
                {
                    pawn.story.Adulthood = null;
                }


            }

        }
    }
}
