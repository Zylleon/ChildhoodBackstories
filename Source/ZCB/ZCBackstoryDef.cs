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

}
