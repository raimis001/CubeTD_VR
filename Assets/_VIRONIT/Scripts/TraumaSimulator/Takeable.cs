using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TraumaMR
{
	public enum ActionType {
		put,
		keep,
		tool,
		disposable,
		action
	}
	[System.Serializable]
	public class TargetPart{

		public BodyPart bodyPart;
		public Takeable requaredActedTakeable;
	}
	public class Takeable : Interactable
	{
		public ActionType actionType;
		public List<TargetPart> targetParts = new List<TargetPart>();
		public int animatorStateIndex; // zero for default
		public int holdClipIndex;
		public int actionClipIndex;
		public AnimationClip actionClip;// for get duration
		public bool twoHandsForAction;
		public bool onceAction;
		public bool oneInBodyPart;
		public bool requiredCloth;
		[HideInInspector] public BodyPart storeBodyPart;
		internal bool alreadyTaken;

		internal Vector3 startPosition;
		internal Quaternion startRotation;
		internal Transform startParent;
		public virtual void Start()
		{
			startPosition = myTransform.position;
			startRotation = myTransform.rotation;
			startParent = transform.parent;
		}
		public override void Interact(HandGroup hand)
		{
			if (hand.currentTakeable!=hand.freeHand)
			{
				return;
			}
			if (alreadyTaken)
			{
				hand.otherHand.currentTakeable.UnHold(hand.otherHand);
			}
			Take(hand);
		}
		public virtual void Take(HandGroup hand)
		{
			hand.nearestInteractable = null;
			hand.currentTakeable = this;
			alreadyTaken = true;
			if (storeBodyPart != null)
			{
				storeBodyPart.storedTakeable = null;
			}

			Start();
			MoveTakeableToHand(hand);
			TurnAllIndicators(hand, true);
			hand.handAnim.SetInteger("StateIndex", animatorStateIndex); // state machine index
			hand.handAnim.SetFloat("HoldIndex", holdClipIndex);
			hand.handAnim.SetTrigger("Hold");

		}
		public void TurnAllIndicators(HandGroup hand, bool on)
		{
			if (this==hand.freeHand)
			{
				return;
			}
			foreach (var t in targetParts)
			{
				t.bodyPart.TurnIndicator(on);
			}
		}
		public void MoveTakeableToHand(HandGroup hand)// set object child of hand
		{
			myTransform.parent = hand.takePoint;
			myTransform.position = hand.takePoint.position;
			myTransform.localPosition = Vector3.zero;
			myTransform.rotation = hand.takePoint.rotation;
			myTransform.localRotation = Quaternion.identity;
		}
		public virtual void Drop(HandGroup hand, bool dropPhysic = true)// throw the object,  clear hand
		{
			hand.handAnim.SetTrigger("Drop");
			alreadyTaken = false;
			#region optionally

			if (dropPhysic)
			{
				var dropCopy = Instantiate(gameObject, myTransform.position, myTransform.rotation);
				dropCopy.transform.localRotation = myTransform.localRotation;
				dropCopy.GetComponent<Takeable>().enabled = false;
				dropCopy.layer = LayerMask.NameToLayer("Default");
				dropCopy.AddComponent<Rigidbody>();
				Destroy(dropCopy, 1);
			}

			#endregion
			myTransform.parent = startParent;
			myTransform.position= startPosition;
			myTransform.rotation = startRotation;
			
			hand.currentTakeable = hand.freeHand;
			TurnAllIndicators(hand, false);

		}
		public virtual void UnHold(HandGroup hand)// let go of the object from when we taking object from another hand
		{
			hand.handAnim.SetTrigger("Idle");
			alreadyTaken = false;
			hand.currentTakeable = hand.freeHand;
			myTransform.parent = startParent;
		}
		public virtual void DoAction(HandGroup hand)// do default action with animation on target bodypart
		{
			if (!CheckCondition(hand))
			{
				return;
			}
			TurnAllIndicators(hand, false);
			player.isActing = true;
			player.MoveHandModelToTarget(hand, hand.currentTargetPart.bodyPart.myTransform);
			hand.handAnim.SetFloat("ActionIndex", actionClipIndex);
			if (!hand.currentTargetPart.bodyPart.actedTakeables.Contains(this))
				hand.currentTargetPart.bodyPart.actedTakeables.Add(this);

			StartCoroutine(TakeableActionTime(hand, twoHandsForAction));
		}
		public virtual bool CheckCondition(HandGroup hand)// check all condition for any object
		{
			bool check = true;
			TargetPart targetPart = hand.currentTargetPart;
			if (targetPart.requaredActedTakeable != null && !targetPart.bodyPart.actedTakeables.Contains(targetPart.requaredActedTakeable))
			{
				check = false;// order actions (this action required other action before)
			}
			if (onceAction && targetPart.bodyPart.actedTakeables.Contains(this))
			{
				check = false;// only one time using on body part
			}
			if (oneInBodyPart && targetPart.bodyPart.storedTakeable != null)
			{
				check = false;// place on body part already busy
			}
			if (twoHandsForAction && hand.otherHand.currentTakeable != hand.otherHand.freeHand)
			{
				check = false;// for this action need free second hand
			}
			if (requiredCloth && !targetPart.bodyPart.haveCloth)
			{
				check = false;// only for scissors
			}
			return check;
		}
       

		public virtual IEnumerator TakeableActionTime(HandGroup hand, bool twoHands) // wait while animation finish
		{
			hand.handAnim.SetTrigger("Action");
			if (twoHands) hand.otherHand.handAnim.SetTrigger("Action");

			yield return new WaitForSeconds(hand.handAnim.GetAnimatorTransitionInfo(0).duration + actionClip.length);

			FinishAction(hand, twoHands);
		}
		public virtual void FinishAction(HandGroup hand,bool twoHands)
		{
           
			switch (actionType)
			{
				case ActionType.put:
					PlaceOnBodyPart(hand, twoHands);
					break;
				case ActionType.keep:
					KeepInHand(hand, twoHands);
					break;
				case ActionType.disposable:
					HideAfterUse(hand, twoHands);
					break;
				default:
					break;
			}

			if (requiredCloth)
			{
				hand.currentTargetPart.bodyPart.haveCloth = false;
			}
			player.SetModelPosition(player.rightHand, InputMR.RightControllerGO.transform);
			player.SetModelPosition(player.leftHand, InputMR.LeftControllerGO.transform);
			player.isActing = false;
			TurnAllIndicators(hand, true);
		}
		public void PlaceOnBodyPart(HandGroup hand, bool twoHands)// after action place object on bodyPart
		{
			hand.currentTargetPart.bodyPart.storedTakeable = hand.currentTakeable;
			storeBodyPart = hand.currentTargetPart.bodyPart;
			myTransform.parent = hand.currentTargetPart.bodyPart.myTransform;
			hand.handAnim.SetTrigger("Idle");
			if (twoHands) hand.otherHand.handAnim.SetTrigger("Idle");
			alreadyTaken = false;
			hand.currentTakeable = hand.freeHand;
		}
		public void HideAfterUse(HandGroup hand, bool twoHands)// for disposable object. hide object after use
		{
			if (hand.currentTakeable.anim != null)
			{
				hand.currentTakeable.anim.SetTrigger("Idle");
			}
			hand.handAnim.SetTrigger("Idle");
			if (twoHands) hand.otherHand.handAnim.SetTrigger("Idle");
			alreadyTaken = false;
			hand.currentTakeable = hand.freeHand;
			myTransform.parent = null;
			gameObject.SetActive(false);
		}
		public void KeepInHand(HandGroup hand, bool twoHands)// after action keep object in hand
		{
			if (anim != null)
			{
				anim.SetTrigger("Idle");
			}
			hand.handAnim.SetTrigger("Hold");
			if (twoHands) hand.otherHand.handAnim.SetTrigger("Idle");
		}

		public virtual void DemonstrateAction(HandGroup hand)  // for objects with animator like a scissors
		{
			//hand.handAnim.SetInteger("StateIndex", animatorStateIndex);
			//hand.handAnim.SetTrigger("DemonstrateAction");
			Debug.Log("check demonstrate");
		}

		public virtual IEnumerator DemostrateActionTime(HandGroup hand, bool twoHands)// wait for demonstrate animation is finish 
		{
			hand.handAnim.SetTrigger("Action");
			if (twoHands) hand.otherHand.handAnim.SetTrigger("Action");
			yield return new WaitForSeconds(hand.handAnim.GetAnimatorTransitionInfo(0).duration + actionClip.length);
			if(!player.isActing)KeepInHand(hand, twoHands);
		}
	}

}
