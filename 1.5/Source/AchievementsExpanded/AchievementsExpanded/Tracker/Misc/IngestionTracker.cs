using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AchievementsExpanded
{
    public class IngestionTracker : Tracker2<Thing, Pawn>
    {
        
        public float count = 1;
        public ThingDef ingestorThingDef;
        public ThingDef foodDef;
        public bool checkIfCorpse = false;
        public bool onlyCountAnimals = false;
        public bool onlyPlayerFaction = true;

        [Unsaved]
        protected float triggeredCount;

        public override string Key
        {
            get { return "IngestionTracker"; }
            set { }
        }
        public override MethodInfo MethodHook => AccessTools.Method(typeof(Thing), nameof(Thing.Ingested)); 
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.IngestedThing));
        protected override string[] DebugText => new string[] {  $"Count: {count}" };

        public IngestionTracker()
        {
        }

        public IngestionTracker(IngestionTracker reference) : base(reference)
        {
            ingestorThingDef = reference.ingestorThingDef;
            foodDef = reference.foodDef;
            checkIfCorpse = reference.checkIfCorpse;
            onlyPlayerFaction = reference.onlyPlayerFaction;
            onlyCountAnimals = reference.onlyCountAnimals;
            count = reference.count;
            if (count <= 0)
                count = 1;
          
            triggeredCount = 0;
        }

       

        public override void ExposeData()
        {
            base.ExposeData();
         
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
            Scribe_Defs.Look(ref ingestorThingDef, "ingestorThingDef");
            Scribe_Defs.Look(ref foodDef, "foodDef");
            Scribe_Values.Look(ref checkIfCorpse, "checkIfCorpse", false);
            Scribe_Values.Look(ref onlyPlayerFaction, "onlyPlayerFaction", true);
            Scribe_Values.Look(ref onlyCountAnimals, "onlyCountAnimals", false);

        }

        public override (float percent, string text) PercentComplete =>  count > 1 ? (triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

        public override bool Trigger(Thing thingIngested, Pawn ingester)
        {

            if(onlyPlayerFaction && ingester.Faction != Faction.OfPlayerSilentFail)
            {
                return false;
            }

            if (onlyCountAnimals && ingester.RaceProps?.Humanlike==true)
            {
                return false;
            }

            bool ingestorRace = ingestorThingDef is null || ingester.def == ingestorThingDef;
            bool food = foodDef is null || thingIngested.def == foodDef;
            bool corpse = !checkIfCorpse || thingIngested as Corpse != null;

            return ingestorRace && food && corpse && (count <= 1 || ++triggeredCount >= count);

        }
    }
}
