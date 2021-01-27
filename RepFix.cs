using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace WaspPile.RepFix
{
    public class RepFixMod : Partiality.Modloader.PartialityMod
    {
        public RepFixMod()
        {
            this.ModID = "RepFix";
            this.author = "thlbr";
            this.Version = "v0+1";
            oneinstance = this;
        }

        public static RepFixMod oneinstance;

        public delegate CreatureTemplate.Relationship orig_UpdateDynamicRelationship(IUseARelationshipTracker self, RelationshipTracker.DynamicRelationship dRelation);
        public static CreatureTemplate.Relationship VariousAIs_UpdateDynRel_Hook(orig_UpdateDynamicRelationship orig, IUseARelationshipTracker self, RelationshipTracker.DynamicRelationship dRelation)
        {
            CreatureTemplate.Relationship resrel = orig(self, dRelation);
            if (dRelation.trackerRep.representedCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat && resrel.type == CreatureTemplate.Relationship.Type.Eats)
            {
                resrel.intensity = Mathf.Clamp(Mathf.Lerp(Mathf.Pow(resrel.intensity * RF_CFG.AGGSCALE, 2), resrel.intensity, RF_CFG.SCALEFAC), 0f, 1f);
                if (resrel.intensity == 0f) resrel.type = CreatureTemplate.Relationship.Type.Ignores;
            }
            return resrel;
        }

        public static void ShelterDoor_KillHostiles_Hook(On.ShelterDoor.orig_KillAllHostiles orig, ShelterDoor instance)
        {
            foreach (AbstractCreature absc in instance.room.abstractRoom.creatures)
            {
                bool killflag = true;
                if (CritsToKill.Contains(absc.creatureTemplate.type))
                {
                    
                    if (absc.realizedCreature != null)
                    {
                        absc.state.Die();
                        absc.realizedCreature.Die();
                        Debug.Log($"Killed creature {absc.creatureTemplate.type}, ID {absc.ID}. in shelter {instance.room.abstractRoom.name}.");
                    }
                }
                else if (CritsToPacify.Contains(absc.creatureTemplate.type))
                {
                    if (absc.realizedCreature != null)
                    {
                        absc.realizedCreature.Stun(600);
                        Debug.Log($"Pacified creature {absc.creatureTemplate.type}, ID {absc.ID}, in shelter {instance.room.abstractRoom.name}.");
                    }
                }
                if (killflag) instance.killHostiles = false;
            }
        }
        public static void Absc_InitAI_Hook(On.AbstractCreature.orig_InitiateAI orig, AbstractCreature instance)
        {
            orig(instance);
            if (instance.creatureTemplate.type == CreatureTemplate.Type.LanternMouse) instance.abstractAI.RealAI = new AltMouseAI(instance, instance.world);
            
        }
        public static void Voidspawn_MakeBody_Hook(On.VoidSpawn.orig_ctor orig, VoidSpawn instance, AbstractPhysicalObject apo, float voidmelt, bool daylight)
        {
            orig(instance, apo, voidmelt, daylight);
            foreach (BodyChunk chunk in instance.bodyChunks)
            {
                chunk.collideWithObjects = true;
            }
            instance.collisionLayer = 1;
        }

        public static void Room_Loaded_Hook(On.Room.orig_Loaded orig, Room instance)
        {
            orig(instance);
            Debug.Log(string.Empty);
            
        }

        public static OptionalUI.OptionInterface LoadOI()
        {
            return new RF_CMOI();
        }

        public static List<CreatureTemplate.Type> CritsToKill = new List<CreatureTemplate.Type>()
        { 
            CreatureTemplate.Type.Centipede,
            CreatureTemplate.Type.Centiwing,
            CreatureTemplate.Type.BrotherLongLegs,
            CreatureTemplate.Type.DaddyLongLegs, 
            CreatureTemplate.Type.MirosBird, 
            CreatureTemplate.Type.PoleMimic, 
            CreatureTemplate.Type.TentaclePlant, 
            CreatureTemplate.Type.Vulture, 
            CreatureTemplate.Type.KingVulture, 
            CreatureTemplate.Type.Spider,
            CreatureTemplate.Type.SpitterSpider,
            CreatureTemplate.Type.DropBug
        };
        public static List<CreatureTemplate.Type> CritsToPacify = new List<CreatureTemplate.Type>()
        {
            CreatureTemplate.Type.BigSpider,
        };

        public static List<IDetour> HOOKS = new List<IDetour>();

        public override void OnEnable()
        {
            base.OnEnable();
            if (RF_CFG.SCAREDSPIDERS) StaticWorld.EstablishRelationship(CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.5f));
            StaticWorld.EstablishRelationship(CreatureTemplate.Type.LanternMouse, CreatureTemplate.Type.Slugcat, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            HOOKS.Add(new Hook(typeof(LizardAI).GetMethod("IUseARelationshipTracker.UpdateDynamicRelationship", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public), typeof(RepFixMod).GetMethod(nameof(RepFixMod.VariousAIs_UpdateDynRel_Hook))));
            HOOKS.Add(new Hook(typeof(VultureAI).GetMethod("IUseARelationshipTracker.UpdateDynamicRelationship", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public), typeof(RepFixMod).GetMethod(nameof(RepFixMod.VariousAIs_UpdateDynRel_Hook))));
            On.ShelterDoor.KillAllHostiles += new On.ShelterDoor.hook_KillAllHostiles(ShelterDoor_KillHostiles_Hook);
            On.VoidSpawn.ctor += new On.VoidSpawn.hook_ctor(Voidspawn_MakeBody_Hook);
            On.AbstractCreature.InitiateAI += new On.AbstractCreature.hook_InitiateAI(Absc_InitAI_Hook);
            //On.Room.Loaded += new On.Room.hook_Loaded(Room_Loaded_Hook);

            this.gs = new GameObject().AddComponent<GarbageScript>();
        }

        private GarbageScript gs;

    }
}