using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraumaMR
{
	public class TakeablePackage : Interactable
	{
		public List<Takeable> storedTakeable= new List<Takeable>();
		public override void Interact(HandGroup hand)
		{
			Debug.Log("Interact");
			for (int i = 0; i < storedTakeable.Count; i++)
			{
				if (!storedTakeable[i].alreadyTaken&&hand.currentTakeable==hand.freeHand)
				{
					storedTakeable[i].gameObject.SetActive(true);
					storedTakeable[i].Take(hand);
					break;
				}
			}
            
			//Takeable takeable = Instantiate(takeablePrefab).GetComponent<Takeable>();
           
			Debug.Log("Interact");

		}
	}
}
