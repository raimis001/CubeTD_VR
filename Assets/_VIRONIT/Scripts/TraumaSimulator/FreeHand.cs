using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TraumaMR { 
	public class FreeHand : Takeable {

		public override void DoAction(HandGroup hand)
		{
			FreeHandAction(hand);
		}
		public void FreeHandAction(HandGroup hand)// only heart massage at the moment
		{
			if (hand.otherHand.currentTakeable != hand.otherHand.freeHand)
			{
				return;
			}
			//hand.currentTargetPart = hand.nearestTargetPart;
			hand.handAnim.SetFloat("ActionIndex", hand.currentTargetPart.bodyPart.freeHandActionClipIndex);
			hand.otherHand.handAnim.SetFloat("ActionIndex", hand.currentTargetPart.bodyPart.freeHandActionClipIndex);
			player.isActing = true;
			player.MoveHandModelToTarget(hand, hand.currentTargetPart.bodyPart.myTransform);
			player.MoveHandModelToTarget(hand.otherHand, hand.currentTargetPart.bodyPart.myTransform);
			StartCoroutine(FreeHandActionTime(hand, true));
		}
		public IEnumerator FreeHandActionTime(HandGroup hand, bool twoHands)
		{
			hand.handAnim.SetTrigger("Action");
			if (twoHands) hand.otherHand.handAnim.SetTrigger("Action");

			yield return new WaitForSeconds(3);
			hand.handAnim.SetTrigger("Idle");
			hand.otherHand.handAnim.SetTrigger("Idle");
			player.isActing = false;
			player.SetModelPosition(player.rightHand, InputMR.RightControllerGO.transform);
			player.SetModelPosition(player.leftHand, InputMR.LeftControllerGO.transform);
           
			//FinishAction();
		}

	}
}
