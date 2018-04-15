using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraumaMR
{
	[System.Serializable]
	public class HandGroup
	{
		public Transform handPoint;
		public Transform takePoint;
		public Transform handModel;
		public RaycastHit hit;
		public Ray ray;
		public Animator handAnim;
		public Transform lineRenderer;
		internal string grab;
		internal string trigger;
		internal string touchpad;
		internal string menu;
		
		public HandGroup otherHand;
		public Interactable nearestInteractable;
		public Takeable currentTakeable;
		[HideInInspector] public TargetPart nearestTargetPart;
		[HideInInspector] public TargetPart currentTargetPart;
		public Takeable freeHand;
	}

	public class PlayerController : MonoBehaviour
	{
		public HandGroup leftHand;
		public HandGroup rightHand;
		public Transform cameraTransform;
		public LayerMask interactableLayer;

		[HideInInspector] public bool isTeleporting;
		[HideInInspector] public Vector3 teleportPoint;
		[HideInInspector] public bool canMotion;
		[HideInInspector] public bool isActing;
		[HideInInspector]public List<Interactable> interactableObjects = new List<Interactable>();
		private List<HandGroup> hands = new List<HandGroup>();
		Collider[] colls;
		//List<Collider> collList = new List<Collider>();
		public bool getHands = false;
		//public TextMeshPro debugText;
		private int debugCount;

		internal bool handsIsReady;


		void Start()
		{
			hands.Add(leftHand);
			hands.Add(rightHand);

			rightHand.grab = InputMR.rightGrabPath;
			leftHand.grab = InputMR.leftGrabPath;
			rightHand.trigger = InputMR.rightTriggerPath;
			leftHand.trigger = InputMR.leftTriggerPath;
			rightHand.touchpad = InputMR.rightTouchpadPath;
			leftHand.touchpad = InputMR.leftTouchpadPath;
			rightHand.menu = InputMR.rightMenuPath;
			leftHand.menu = InputMR.leftMenuPath;
			
			leftHand.otherHand = rightHand;
			rightHand.otherHand = leftHand;
			if (leftHand.handModel) leftHand.freeHand = leftHand.handModel.GetComponent<Takeable>();
			if (rightHand.handModel) rightHand.freeHand = rightHand.handModel.GetComponent<Takeable>();
			leftHand.currentTakeable = leftHand.freeHand;
			rightHand.currentTakeable = rightHand.freeHand;

			canMotion = true;
			cameraTransform = Camera.main.transform;
			interactableObjects.AddRange(FindObjectsOfType<Interactable>());

		}


		public void SetModelPosition(HandGroup hand, Transform target)
		{
			if (!hand.handModel) return;

			hand.handModel.parent = target;
			hand.handModel.position = target.position;
			hand.handModel.localPosition = new Vector3(0, 0, -0.2f);
			hand.handModel.localRotation = (hand.handModel == rightHand.handModel) ? Quaternion.Euler(-90, -90, 0) : Quaternion.Euler(-90, 90, 0);
		}

		public void HideControllers()
		{
			return;
			List<MeshRenderer> meshes = new List<MeshRenderer>();
			meshes.AddRange(InputMR.LeftControllerGO.GetComponentsInChildren<MeshRenderer>());
			meshes.AddRange(InputMR.RightControllerGO.GetComponentsInChildren<MeshRenderer>());
			foreach (var m in meshes)
			{
				m.enabled = false;
			}
			
		}
		public  void GettingHands()
		{
			if (InputMR.RightControllerGO!=null && InputMR.LeftControllerGO!=null) {
				
				HideControllers();
				SetModelPosition(leftHand, InputMR.LeftControllerGO.transform);
				SetModelPosition(rightHand, InputMR.RightControllerGO.transform);
				getHands = true;
			}
		}
		void Update()
		{
		}

		public void MoveHandModelToTarget(HandGroup hand, Transform target)
		{
			hand.handModel.parent = target;
			hand.handModel.position = target.position;
			hand.handModel.localPosition = Vector3.zero;
			hand.handModel.rotation = target.rotation;
			hand.handModel.localRotation = Quaternion.identity;
		}

		
		
	}
}
