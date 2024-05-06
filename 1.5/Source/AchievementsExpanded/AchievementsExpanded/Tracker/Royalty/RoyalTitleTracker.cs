using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace AchievementsExpanded
{
    public class RoyalTitleTracker : TrackerBase
    {
        public RoyalTitleDef title;
        public int count;
        public bool countTemporary = false;



        public override string Key
        {
            get { return "RoyalTitleTracker"; }
            set { }
        }
        public override MethodInfo MethodHook => AccessTools.Method(typeof(Pawn_RoyaltyTracker), nameof(Pawn_RoyaltyTracker.SetTitle)); 
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.TitleSet));
        protected override string[] DebugText => new string[] { $"title: {title?.defName ?? "[NullDef]"}", $"Count: {count}"};

        public RoyalTitleTracker()
        {
        }

        public RoyalTitleTracker(RoyalTitleTracker reference) : base(reference)
        {
            title = reference.title;
            count = reference.count;
            countTemporary = reference.countTemporary;


        }

        public override bool UnlockOnStartup
        {
            get
            {            
               return Trigger();              
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref title, "title");
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref countTemporary, "countTemporary", false);

        }

        public override bool Trigger()
        {

            base.Trigger();

            List<Pawn> factionPawns;
            
            if (!countTemporary)
            {
                factionPawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoLodgers;
            }
            else
            {
                factionPawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction;
            }
            if (factionPawns is null)
                return false;

            if (factionPawns.Where(f => f.royalty?.MainTitle()?.seniority>=title?.seniority).Count() >= count)
            {
                return true;
            }
            return false;


        }
    }
}
