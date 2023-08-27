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
        public float commonality = 1f;
        public TechLevel minTechLevel = TechLevel.Animal;
        public TechLevel maxTechLevel = TechLevel.Ultra;
        public List<ZCBReq> requiredRecords;
        public List<ZCBRecordRatio> recordRatios;
        public List<ZCBReq> requiredSkills;
        public List<string> requiredPassions;
        public List<BackstoryTrait> requiredTraits;
        public IntRange colonySize = new IntRange(0, 9999);
        public FamilyStatusFlags father = FamilyStatusFlags.Any;
        public FamilyStatusFlags mother = FamilyStatusFlags.Any;
        

        public bool IsAcceptable (Pawn pawn)
        {
            bool output = true;
            //Log.Message("EVALUATING " + this.defName);
            
            if(Faction.OfPlayer.def.techLevel < minTechLevel)
            {
                output = false;
            }
            if (Faction.OfPlayer.def.techLevel > maxTechLevel)
            {
                output = false;
            }

            // disallow backstory if the backstory disables any skill the pawn has passion in
            List<SkillDef> allSkillDefs = DefDatabase<SkillDef>.AllDefsListForReading;
            foreach (SkillDef skill in allSkillDefs)
            {
                if(pawn.skills.GetSkill(skill).passion != 0 && (workDisables & skill.disablingWorkTags) != WorkTags.None)
                {
                    output = false;
                }
            }

            // family
            if (father != FamilyStatusFlags.Any)
            {
                Pawn fatherPawn = pawn.GetFather();
                FamilyStatusFlags fatherStatus = FamilyStatusFlags.Absent;
                if (fatherPawn != null)
                {
                    if (fatherPawn.Dead)
                    {
                        fatherStatus = FamilyStatusFlags.Dead;
                    }
                    else
                    {
                        if (fatherPawn.Faction == pawn.Faction)
                        {
                            fatherStatus = FamilyStatusFlags.Present;
                        }
                        else
                        {
                            fatherStatus = FamilyStatusFlags.Absent;
                        }
                    }
                }
                else
                {
                    fatherStatus = FamilyStatusFlags.Absent;
                }
                if ((father & fatherStatus) == 0)
                {
                    output = false;
                }
            }

            if (mother != FamilyStatusFlags.Any)
            {
                Pawn motherPawn = pawn.GetMother();
                FamilyStatusFlags motherStatus = FamilyStatusFlags.Absent;
                if (motherPawn != null)
                {
                    if (motherPawn.Dead)
                    {
                        motherStatus = FamilyStatusFlags.Dead;
                    }
                    else
                    {
                        if (motherPawn.Faction == pawn.Faction)
                        {
                            motherStatus = FamilyStatusFlags.Present;
                        }
                        else
                        {
                            motherStatus = FamilyStatusFlags.Absent;
                        }
                    }
                }
                else
                {
                    motherStatus = FamilyStatusFlags.Absent;
                }
                if ((mother & motherStatus) == 0)
                {
                    output = false;
                }
            }

            // community
            if (PawnsFinder.AllMaps_FreeColonistsSpawned.Count() < colonySize.min || PawnsFinder.AllMaps_FreeColonistsSpawned.Count() > colonySize.max)
            {
                output = false;
            }

            //traits
            foreach (BackstoryTrait trait in requiredTraits)
            {
                if (!pawn.story.traits.HasTrait(trait.def))
                {
                    output = false;
                }
            }
            foreach (BackstoryTrait trait in disallowedTraits)
            {
                if (pawn.story.traits.HasTrait(trait.def))
                {
                    output = false;
                }
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
                    RecordDef bigRec = allRecordDefs.Where(r => r.defName == reqR.numerator).FirstOrDefault();
                    RecordDef smallRec = allRecordDefs.Where(r => r.defName == reqR.denominator).FirstOrDefault();
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
        public string numerator;
        public string denominator;
        public float ratio = 0f;
    }

    [Flags]
    public enum FamilyStatusFlags
    {
        Any = 0x1,
        Present = 0x2,
        Absent = 0x4,
        Dead = 0x8
    }




}
