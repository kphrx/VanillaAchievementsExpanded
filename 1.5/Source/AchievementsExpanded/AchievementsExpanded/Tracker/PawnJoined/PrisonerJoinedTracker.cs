using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using HarmonyLib;
using Verse.AI.Group;

namespace AchievementsExpanded
{
	public class PrisonerJoinedTracker : TrackerBase
	{
		public int numberOfPrisoners;

	
        public override string Key
        {
            get { return "PrisonerJoinedTracker"; }
            set { }
        }

        public override MethodInfo MethodHook => AccessTools.Method(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.SetGuestStatus));
		public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.PrisonerJoined));
        protected override string[] DebugText => new string[] { $"Number Of Prisoners:  {numberOfPrisoners}" };


        public PrisonerJoinedTracker()
		{
		}

		public PrisonerJoinedTracker(PrisonerJoinedTracker reference) : base(reference)
		{
            numberOfPrisoners = reference.numberOfPrisoners;
            if (numberOfPrisoners <= 0)
                numberOfPrisoners = 1;

        }

		public override bool UnlockOnStartup => Trigger();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref numberOfPrisoners, "numberOfPrisoners", 0);
		}

        public override bool Trigger()
        {
            base.Trigger();
           
            ThingDef raceDef = ThingDefOf.Human;
            List<Pawn> factionPawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_PrisonersOfColony;
            
            if (factionPawns is null)
                return false;

            if (factionPawns.Count() >= numberOfPrisoners)
            {
                return true;
            }
            return false;
        }
    }
}
