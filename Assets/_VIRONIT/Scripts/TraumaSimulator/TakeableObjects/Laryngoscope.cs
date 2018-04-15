using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraumaMR
{
	public class Laryngoscope : Takeable
	{
		/*
		private Vector3 actionPoint;
		Vector3 size = new Vector3(0.6f, 3, 0.7f);
		Bounds rect;
		Vector3 offset = new Vector3(-0.4f, 0, 0);
		bool complexActing;
		private HandGroup curHand;
		private float timer;

		public void Start()
		{
			// find relative center point
			actionPoint = targetParts[0].bodyPart.myTransform.position + (targetParts[0].bodyPart.myTransform.rotation * offset);
			rect = new Bounds(actionPoint, size);
		}

		public override void DoAction(HandGroup hand)
		{
			curHand = hand;
			actionPoint = targetParts[0].bodyPart.myTransform.position + (targetParts[0].bodyPart.myTransform.rotation * offset);

			if (!rect.Contains(player.cameraTransform.position))
			{
				return;
			}
			base.DoAction(hand);

			//complexActing = true;
		}

		public override IEnumerator TakeableActionTime(HandGroup hand, bool twoHands)
		{
			MoveTakeableToHand(player.rightHand);
			player.rightHand.handAnim.SetTrigger("Action");

			yield return new WaitForSeconds(hand.handAnim.GetAnimatorTransitionInfo(0).duration + actionClip.length);

			player.MoveHandModelToTarget(player.leftHand, hand.currentTargetPart.bodyPart.myTransform);
			player.leftHand.handAnim.SetInteger("StateIndex", animatorStateIndex);
			player.leftHand.handAnim.SetFloat("HoldIndex", 1);// support action  (index 0 for main hold animation, index 1 for support animation) 
			player.leftHand.handAnim.SetTrigger("Hold");

			complexActing = true;
			player.rightHand.handAnim.SetTrigger("ComplexIdle");
			timer = 10;

		}

		public void Update()
		{
			if (complexActing)
			{
				timer -= Time.deltaTime;
				if (player.rightHand.trigger.GetKeyPress())
				{
					player.rightHand.handAnim.SetTrigger("ComplexAction");
					anim.SetTrigger("Action");
				}
				if (player.rightHand.trigger.GetKeyUp())
				{
					player.rightHand.handAnim.SetTrigger("ComplexIdle");
					anim.SetTrigger("Idle");
				}
				if (timer <= 0)
				{
					complexActing = false;
					player.isActing = false;
					anim.SetTrigger("Idle");
					//player.rightHand.handAnim.SetTrigger("ComplexIdle");
					FinishAction(player.rightHand, twoHandsForAction);
				}
			}
		}*/

	}
}
