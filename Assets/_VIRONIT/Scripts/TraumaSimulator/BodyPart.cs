using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraumaMR {
	public class BodyPart : Interactable {

		[HideInInspector] public Takeable storedTakeable;
		public int freeHandActionClipIndex;
		public bool haveCloth;
		public List<Takeable> actedTakeables = new List<Takeable>();
		
		public override void Interact(HandGroup hand)
		{

			foreach (var part in hand.currentTakeable.targetParts)
			{
              
				if (part.bodyPart == this)
				{
					hand.currentTargetPart = part;
					hand.currentTakeable.DoAction(hand);
					return;
				}
			}
            
			if (storedTakeable!=null)
			{
				storedTakeable.Take(hand);
			}
		}
		public override void TurnIndicator(bool on)
		{
			base.TurnIndicator(on);
		}
	}
}
