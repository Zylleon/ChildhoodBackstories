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
        public List<ZCBReq> requiredSkills;
        public List<ZCBRecordRatio> recordRatios;

        public bool IsAcceptable (Pawn pawn)
        {
            bool output = true;
            //Log.Message("EVALUATING " + this.defName);
            
            if (!requiredRecords.NullOrEmpty())
            {
                List<RecordDef> allRecordDefs = DefDatabase<RecordDef>.AllDefsListForReading;
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
                List<RecordDef> allRecordDefs = DefDatabase<RecordDef>.AllDefsListForReading;
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
                List<SkillDef> allRecordDefs = DefDatabase<SkillDef>.AllDefsListForReading;
                foreach (ZCBReq reqS in requiredSkills)
                {
                    SkillDef skillDef = allRecordDefs.Where(r => r.defName == reqS.name).FirstOrDefault();
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
