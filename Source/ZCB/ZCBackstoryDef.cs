using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ZCB
{
    public class ZCBackstoryDef : BackstoryDef
    {
        public List<ZCBReq> requiredRecords;
        public List<ZCBRecordRatio> recordRatios;
        public List<ZCBReq> requiredSkills;
        public List<string> requiredPassions;
        public IntRange colonySize = new IntRange(0, 9999);

        public bool IsAcceptable (Pawn pawn)
        {
            bool output = true;
            //Log.Message("EVALUATING " + this.defName);

            // disallow backstory if the backstory disables any skill the pawn has passion in
            List<SkillDef> allSkillDefs = DefDatabase<SkillDef>.AllDefsListForReading;
            foreach (SkillDef skill in allSkillDefs)
            {
                if(pawn.skills.GetSkill(skill).passion != 0 && (workDisables & skill.disablingWorkTags) != WorkTags.None)
                {
                    output = false;
                }
            }

            if(PawnsFinder.AllMaps_FreeColonistsSpawned.Count() < colonySize.min || PawnsFinder.AllMaps_FreeColonistsSpawned.Count() > colonySize.max)
            {
                return false;
            }

            List<RecordDef> allRecordDefs = DefDatabase<RecordDef>.AllDefsListForReading;
            if (!requiredRecords.NullOrEmpty())
            {
                foreach (ZCBReq reqR in requiredRecords)
                {
                    RecordDef reqDef = allRecordDefs.Where(r => r.defName == reqR.name).FirstOrDefault();
                    float reqValue = pawn.records.GetValue(reqDef);
                    if (reqValue < reqR.minValue)
                    {
                        output = false;
                    }
                    if (reqValue > reqR.maxValue)
                    {
                        output = false;
                    }
                }
            }
            if (!recordRatios.NullOrEmpty())
            {
                foreach (ZCBRecordRatio reqR in recordRatios)
                {
                    RecordDef bigRec = allRecordDefs.Where(r => r.defName == reqR.bigRecord).FirstOrDefault();
                    RecordDef smallRec = allRecordDefs.Where(r => r.defName == reqR.smallRecord).FirstOrDefault();
                    if (reqR.ratio == 0)
                    {
                        if (pawn.records.GetValue(bigRec) <= pawn.records.GetValue(smallRec))
                        {
                            output = false;
                        }
                    }
                    else if (pawn.records.GetValue(bigRec) / pawn.records.GetValue(smallRec) < reqR.ratio)
                    {
                        output = false;
                    }
                }
            }
            if (!requiredSkills.NullOrEmpty())
            {
                foreach (ZCBReq reqS in requiredSkills)
                {
                    SkillDef skillDef = allSkillDefs.Where(r => r.defName == reqS.name).FirstOrDefault();
                    float reqValue = pawn.skills.GetSkill(skillDef).Level;
                    if (reqValue < reqS.minValue)
                    {
                        output = false;
                    }
                    if (reqValue > reqS.maxValue)
                    {
                        output = false;
                    }
                }
            }
            if(!requiredPassions.NullOrEmpty())
            {
                foreach(string skill in requiredPassions)
                {
                    SkillDef skillDef = allSkillDefs.Where(r => r.defName == skill).FirstOrDefault();
                    if(pawn.skills.GetSkill(skillDef).passion == 0)
                    {
                        output = false;
                    }
                }
            }




            return output;
        }


    }


    public class ZCBReq
    {
        public string name;
        public float minValue = 0;
        public float maxValue = 999999999;
    }

    public class ZCBRecordRatio
    {
        public string bigRecord;
        public string smallRecord;
        public float ratio = 0f;
    }
}
