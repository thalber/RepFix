using UnityEngine;

namespace WaspPile.RepFix
{
    public class AltMouseAI : MouseAI, FriendTracker.IHaveFriendTracker, IUseARelationshipTracker, IReactToSocialEvents
    {
        public AltMouseAI (AbstractCreature absc, World world) : base (absc, world)
        {
            this.AddModule(new FriendTracker(this));
            Debug.Log("Alternative mouse AI initiated: " + this.creature.ID.ToString());
			this.utilityComparer.AddComparedModule(this.friendTracker, null, 2f, 1.1f);
        }

        public void GiftRecieved(SocialEventRecognizer.OwnedItemOnGround gift)
        {
			this.friendTracker.friend = gift.owner;
			
        }

        CreatureTemplate.Relationship IUseARelationshipTracker.UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
        {
			if (dRelation.trackerRep.representedCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
            {
				this.friendTracker.friend = dRelation.trackerRep.representedCreature.realizedCreature;
				this.friendTracker.followClosestFriend = true;
				return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f);

			}
			if (dRelation.trackerRep.VisualContact)
			{
				dRelation.state.alive = dRelation.trackerRep.representedCreature.state.alive;
			}
			CreatureTemplate.Relationship result = base.StaticRelationship(dRelation.trackerRep.representedCreature);
			CreatureTemplate.Relationship.Type type = result.type;
			if (type == CreatureTemplate.Relationship.Type.Afraid)
			{
				if (!dRelation.state.alive)
				{
					result.intensity = 0f;
				}
				else if (dRelation.trackerRep.BestGuessForPosition().room == this.mouse.room.abstractRoom.index && !dRelation.trackerRep.representedCreature.creatureTemplate.canFly)
				{
					float num = Mathf.Lerp(0.1f, 1.6f, Mathf.InverseLerp(-100f, 200f, this.mouse.room.MiddleOfTile(dRelation.trackerRep.BestGuessForPosition().Tile).y - this.mouse.mainBodyChunk.pos.y));
					float value = float.MaxValue;
					if (this.dangle != null)
					{
						value = Mathf.Min(Vector2.Distance(this.mouse.mainBodyChunk.pos, this.mouse.room.MiddleOfTile(this.dangle.Value.attachedPos)), Vector2.Distance(this.mouse.mainBodyChunk.pos, this.mouse.room.MiddleOfTile(this.dangle.Value.bodyPos)));
					}
					num = Mathf.Lerp(num, 1f, Mathf.InverseLerp(50f, 500f, value));
					result.intensity *= num;
				}
			}
			if (dRelation.trackerRep.representedCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
            {

            }
			return result;
		}
		AIModule IUseARelationshipTracker.ModuleToTrackRelationship(CreatureTemplate.Relationship relationship)
        {
			if (relationship.type == CreatureTemplate.Relationship.Type.Afraid) return this.threatTracker;
			return null;
        }

		void IReactToSocialEvents.SocialEvent(SocialEventRecognizer.EventID ID, Creature subjectCrit, Creature objectCrit, PhysicalObject involvedItem)
        {
			if (objectCrit != this.mouse) return;
			Tracker.CreatureRepresentation rep = this.tracker.RepresentationForObject(subjectCrit, false);
			if (rep == null) return;
			
			switch (ID)
            {
				
				case SocialEventRecognizer.EventID.ItemOffering:
					if (objectCrit == this.mouse) this.friendTracker.ItemOffered(rep, involvedItem);
					break;
				default:
					break;
            }
        }

        public override void Update()
        {
            base.Update();
			if (this.friendTracker.friend != null) this.mouse.abstractCreature.abstractAI.SetDestination(this.friendTracker.friendDest);
        }

    }

}
