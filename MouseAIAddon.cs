using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AssemblyCSharp;

namespace RepFix
{
    public class AltMouseAI : MouseAI, FriendTracker.IHaveFriendTracker, IUseARelationshipTracker
    {
        public AltMouseAI (AbstractCreature absc, World world) : base (absc, world)
        {
            this.AddModule(new FriendTracker(this));
            Debug.Log("Alternative mouse AI initiated: " + this.creature.ID.ToString());
        }

        public void GiftRecieved(SocialEventRecognizer.OwnedItemOnGround gift)
        {
            
        }

        CreatureTemplate.Relationship IUseARelationshipTracker.UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
        {
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

	}
}
