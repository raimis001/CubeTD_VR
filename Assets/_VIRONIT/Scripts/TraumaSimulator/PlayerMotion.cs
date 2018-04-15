using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.XR.WSA.Input;


namespace TraumaMR
{
	public class PlayerMotion : MonoBehaviour
	{
		public enum RotType
		{
			steps,
			continuously
		}

		
		private bool teleportLeft;
		private bool teleportRight;
		private bool teleportStart;

		private Transform camParentTransform;

		private bool isPlayerRotate;
		private bool isPlayerTeleporting;
		private Quaternion playerTargetRotation;
		private Vector3 playerTargetPosition;
		private float rotateTime;
		private float teleportTime;
		private Transform camTransform;

		private Ray cameraRay;

		private bool playerCanRotate;



		private Vector3 teleportPoint;
		[Header("Teleport properties")]
		public LayerMask roomLayer;
		public GameObject teleportMarker;
		public float wallOffset = 0.3f; //0.3f as default

		[Header("Hands rays")]
		public LineRenderer RightLineRenderer;
		public LineRenderer LeftLineRenderer;
		[Tooltip("Ray length")]
		public float rayLength = 10;

		[Header("Movement properties")]

		public float movementSpeed = 2;//2f as default

		[Header("Rotation properties")]
		[Tooltip("For continiusly need only continuouslySpeed ")]
		public RotType rotationType;

		public float rotateStepAngle = 30;//30f as default
		[Tooltip("Time between steps")]
		public float rotationStepDelay = 0.15f;//0.15f as default
		[Tooltip("Time for rotate to desctination angle")]
		public float timeForStep;//0.1f sec as default
		[Tooltip("Rotation speed in continiusly mode")]
		public float continuouslySpeed = 50;

		private BoxCollider parentCol;
		private Rigidbody myRb;

		private bool lineRightActive;
		private bool lineLeftActive;

		public Action<RaycastHit, ControlerKind> RaycastAction;
		public Action<ControlerKind> RaycastEmpty;

		private void Awake()
		{
			MotionControllerVisualizer.Instance.OnControllerModelLoaded += AttachElementToController;
			MotionControllerVisualizer.Instance.OnControllerModelUnloaded += DetachElementFromController;

		}

		private void AttachElementToController(MotionControllerInfo newController)
		{
			//Debug.LogFormat("Controler {0} attached", newController.Handedness);

		}

		private void DetachElementFromController(MotionControllerInfo oldController)
		{
			//Debug.LogFormat("Controler {0} dettached", oldController.Handedness);
		}

		void Start()
		{
			camParentTransform = Camera.main.transform.parent;
			camTransform = Camera.main.transform;
			parentCol = camParentTransform.GetComponent<BoxCollider>();
			myRb = camParentTransform.GetComponent<Rigidbody>();
		}

		private void LateUpdate()
		{
			if (InputMR.HasRightController) SetLineFromController(RightLineRenderer, InputMR.RightPointer, ControlerKind.right);
			if (InputMR.HasLeftController) SetLineFromController(LeftLineRenderer, InputMR.LeftPointer, ControlerKind.left);

			camTransform.position = Vector3.zero;
		}

		void FixedUpdate()
		{

			RotateCondition();
			Rotating();


			if (myRb != null)
			{
				myRb.velocity = Vector3.zero;
				myRb.angularVelocity = Vector3.zero;
				parentCol.center = new Vector3(camTransform.localPosition.x, parentCol.center.y, camTransform.localPosition.z);
			}
			MoveInAllDirection();

		}


		public void Rotating()
		{
			if (rotationType == RotType.steps)
			{
				if (isPlayerRotate)
				{
					rotateTime += Time.deltaTime;
					if (rotateTime < timeForStep)
					{
						camParentTransform.RotateAround(camTransform.position, Vector3.up, (rotateStepAngle / timeForStep) * Time.deltaTime);
					}
					else if (rotateTime >= rotationStepDelay)
					{
						rotateTime = 0;
						isPlayerRotate = false;
					}
				}
			}
			else
			{
				if (isPlayerRotate)
				{
					camParentTransform.RotateAround(camTransform.position, Vector3.up, (continuouslySpeed) * Time.deltaTime);
				}
			}

		}

		public void MoveInAllDirection()
		{
			Vector3 tumpstickPos = new Vector3(InputMR.leftThumbstickX, Mathf.Abs(InputMR.rightThumbstickY) > 0.3f ? InputMR.rightThumbstickY : 0 , InputMR.leftThumbstickY);
			Vector3 stepInDirection = Quaternion.AngleAxis(camTransform.eulerAngles.y, Vector3.up) * tumpstickPos;


			//Vector3 targetPosition = new Vector3(stepInDirection.x,0 , stepInDirection.z);

			if (tumpstickPos.x != 0 || tumpstickPos.z != 0 || tumpstickPos.y != 0)
			{
				myRb.velocity = stepInDirection * movementSpeed;
			}
			
			
		}

		public void RotateCondition()
		{
			float angleDir = 0;
			bool rightThumbstickMoveLeft = InputMR.rightThumbstickX < -0.8 && Mathf.Abs(InputMR.rightThumbstickY) < 0.3;
			bool rightThumbstickMoveRight = InputMR.rightThumbstickX > 0.8 && Mathf.Abs(InputMR.rightThumbstickY) < 0.3;
			if (rightThumbstickMoveLeft)
			{
				angleDir = -1;
			}
			else if (rightThumbstickMoveRight)
			{
				angleDir = 1;
			}
			if (rotationType == RotType.continuously)
			{
				if (angleDir != 0 && !teleportStart)
				{

					isPlayerRotate = true;
					continuouslySpeed = Mathf.Abs(continuouslySpeed) * angleDir;
				}
				else
				{
					isPlayerRotate = false;
				}
				return;
			}
			if (angleDir != 0 && !isPlayerRotate && !teleportStart)
			{
				isPlayerRotate = true;
				rotateStepAngle = Mathf.Abs(rotateStepAngle) * angleDir;
			}

		}


		public void SetLineFromController(LineRenderer lineRenderer, Transform StartPoint, ControlerKind kind)
		{
			if (!lineRenderer || !StartPoint)
			{
				return;
			}

			if (kind == ControlerKind.right && !lineRightActive)
			{
				lineRenderer.transform.SetParent(StartPoint);
				lineRightActive = true;
			}
			if (kind == ControlerKind.left && !lineLeftActive)
			{
				lineRenderer.transform.SetParent(StartPoint);
				lineLeftActive = true;
			}

			lineRenderer.gameObject.SetActive(true);
			lineRenderer.transform.localPosition = Vector3.zero;
			lineRenderer.transform.GetChild(0).localPosition = Vector3.zero;

			Ray ray = new Ray(StartPoint.position, StartPoint.forward);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, rayLength))
			{
				lineRenderer.transform.GetChild(1).position = (StartPoint.position + hit.point) * 0.5f;
				lineRenderer.transform.GetChild(2).position = hit.point;


				if (Physics.Raycast(ray, out hit, rayLength, LayerMask.GetMask("Body")))
				{
					if (RaycastAction != null) RaycastAction.Invoke(hit, kind);
					return;
				}

			}
			else
			{

				Vector3 end = StartPoint.position + ray.direction * rayLength;

				lineRenderer.transform.GetChild(1).position = (StartPoint.position + end) * 0.5f;
				lineRenderer.transform.GetChild(2).position = end;
			}

			if (RaycastEmpty != null) RaycastEmpty.Invoke(kind);
		}
	}

}

