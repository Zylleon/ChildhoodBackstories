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
        }
    }
}
