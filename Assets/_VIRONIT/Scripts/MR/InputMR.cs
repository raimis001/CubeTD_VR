using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlerKind
{
	left, right, any
}

public class InputMR : MonoBehaviour
{

	[HideInInspector] public static GameObject RightControllerGO { get; private set; }
	[HideInInspector] public static GameObject LeftControllerGO { get; private set; }
	[HideInInspector] public static bool HasRightController { get; private set; }
	[HideInInspector] public static bool HasLeftController { get; private set; }

	[HideInInspector] public static Transform Grap { get; private set; }
	[HideInInspector] public static Transform Thumstick { get; private set; }
	[HideInInspector] public static Transform Trigger { get; private set; }
	[HideInInspector] public static Transform Touchpad { get; private set; }

	[HideInInspector] public static Transform GrapTransformLeft { get; private set; }
	[HideInInspector] public static Transform ThumstickTransformLeft { get; private set; }
	[HideInInspector] public static Transform TriggerTransformLeft { get; private set; }
	[HideInInspector] public static Transform TouchpadTransformLeft { get; private set; }

	[HideInInspector] public static Transform RightPointer { get; private set; }
	[HideInInspector] public static Transform LeftPointer { get; private set; }
	private const string RightController = "RightController";
	private const string LeftController = "LeftController";
	private const string WayToPointer = "glTFController/GLTFScene/GLTFNode/GLTFNode/POINTING_POSE";
	private const string WayToThumstick = "glTFController/GLTFScene/GLTFNode/GLTFNode/THUMBSTICK_PRESS";
	private const string WayToSelect = "glTFController/GLTFScene/GLTFNode/GLTFNode/SELECT";
	private const string WayToTouchpad = "glTFController/GLTFScene/GLTFNode/GLTFNode/TOUCHPAD_PRESS";
	private const string WayToGrap = "glTFController/GLTFScene/GLTFNode/GLTFNode/GRASP";
	[HideInInspector] public Vector3 ControllerThumbstickPosition { get; private set; }
	[HideInInspector] public Vector3 ControllerTouchpadPosition { get; private set; }

	private bool leftControllerIsSetup;
	private bool rightControllerIsSetup;


	public static float leftThumbstickX;
	public static float leftThumbstickY;
	public static float rightThumbstickX;
	public static float rightThumbstickY;

	public static bool getLeft;
	public static bool getRight;

	#region Touched Input.GetKey()
	public const string leftTouchpadPath = "joystick button 16";
	public const string rightTouchpadPath = "joystick button 17";

	public const string leftTouchpadPathTouched = "joystick button 18";
	public const string rightTouchPadPathTouched = "joystick button 19";
	#endregion
	#region Pressed Input.Getkey()
	public const string leftTriggerPath = "joystick button 14";
	public const string rightTriggerPath = "joystick button 15";

	public const string leftGrabPath = "joystick button 4";
	public const string rightGrabPath = "joystick button 5";

	public const string leftMenuPath = "joystick button 6";
	public const string rightMenuPath = "joystick button 7";


	public const string leftThumbstickPath = "joystick button 8";
	public const string rightThumbstickPath = "joystick button 9";
	#endregion
	#region Axis Input.GetAxis()
	public const string LeftThumbstickXPath = "CONTROLLER_LEFT_STICK_HORIZONTAL";
	public const string LeftThumbstickYPath = "CONTROLLER_LEFT_STICK_VERTICAL";
	public const string RightThumbstickXPath = "CONTROLLER_RIGHT_STICK_HORIZONTAL";
	public const string RightThumbstickYPath = "CONTROLLER_RIGHT_STICK_VERTICAL";
	#endregion
	
	#region JOISTICS
	#region TRIGGER
	public static bool GetTrigger(ControlerKind kind)
	{
		return GetKey(kind, rightTriggerPath, leftTriggerPath);
	}

	public static bool GetTriggerDown(ControlerKind kind)
	{
		return GetKeyDown(kind, rightTriggerPath, leftTriggerPath);
	}

	public static bool GetTriggerUp(ControlerKind kind)
	{
		return GetKeyUp(kind, rightTriggerPath, leftTriggerPath);
	}
	#endregion
	
	#region GRAB
	public static bool GetGrab(ControlerKind kind)
	{
		return GetKey(kind, rightGrabPath, leftGrabPath);

	}
	public static bool GetGrabUp(ControlerKind kind)
	{
		return GetKeyUp(kind, rightGrabPath, leftGrabPath);
	}

	public static bool GetGrabDown(ControlerKind kind)
	{
		return GetKeyDown(kind, rightGrabPath, leftGrabPath);
	}
	#endregion

	#region MENU
	public static bool GetMenu(ControlerKind kind)
	{
		return GetKey(kind, rightMenuPath, leftMenuPath);

	}
	public static bool GetMenuUp(ControlerKind kind)
	{
		return GetKeyUp(kind, rightMenuPath, leftMenuPath);
	}

	public static bool GetMenuDown(ControlerKind kind)
	{
		return GetKeyDown(kind, rightMenuPath, leftMenuPath);
	}
	#endregion

	#region abstract
	private static bool GetKey(ControlerKind kind, string rightPath, string leftPath)
	{
		switch (kind)
		{
			case ControlerKind.right:
				return Input.GetKey(rightPath);
			case ControlerKind.left:
				return Input.GetKey(leftPath);
			case ControlerKind.any:
				return Input.GetKey(rightPath) || Input.GetKey(leftPath);
		}
		return false;
	}

	private static bool GetKeyUp(ControlerKind kind, string rightPath, string leftPath)
	{
		switch (kind)
		{
			case ControlerKind.right:
				return Input.GetKeyUp(rightPath);
			case ControlerKind.left:
				return Input.GetKeyUp(leftPath);
			case ControlerKind.any:
				return Input.GetKeyUp(rightPath) || Input.GetKeyUp(leftPath);
		}
		return false;
	}

	private static bool GetKeyDown(ControlerKind kind, string rightPath, string leftPath)
	{
		switch (kind)
		{
			case ControlerKind.right:
				return Input.GetKeyDown(rightPath);
			case ControlerKind.left:
				return Input.GetKeyDown(leftPath);
			case ControlerKind.any:
				return Input.GetKeyDown(rightPath) || Input.GetKeyDown(leftPath);
		}
		return false;
	}
	#endregion

	#endregion

	void Update()
	{
		leftThumbstickX = Input.GetAxis(LeftThumbstickXPath);
		leftThumbstickY = Input.GetAxis(LeftThumbstickYPath);
		rightThumbstickX = Input.GetAxis(RightThumbstickXPath);
		rightThumbstickY = Input.GetAxis(RightThumbstickYPath);

		if (Input.GetKey(InputMR.leftTouchpadPath))
		{
			Debug.LogFormat("Touchpad pressed y:{0:0.00} x:{0:0.00}", Input.GetAxis("CONTROLLER_LEFT_PAD_HORIZONTAL"), Input.GetAxis("CONTROLLER_LEFT_PAD_VERTICAL"));
		}
		var controllers = Input.GetJoystickNames();
		if (controllers.Length > 1)
		{
			foreach (var c in controllers)
			{
				if (c.Contains("Right") && !getRight)
				{
					getRight = true;
					StartCoroutine(FindingController(true));
				}
				if (c.Contains("Left") && !getLeft)
				{
					getLeft = true;
					StartCoroutine(FindingController(false));
				}
			}
		}
		if (getRight && RightControllerGO == null)
		{
			RightControllerGO = GameObject.Find(RightController);
		}
		if (getLeft && LeftControllerGO == null)
		{
			LeftControllerGO = GameObject.Find(LeftController);
		}


	}
	public IEnumerator FindingController(bool isRight)
	{
		if (isRight)
		{
			yield return new WaitUntil(() => RightControllerGO != null);
			HasRightController = true;
			rightControllerIsSetup = true;
			RightPointer = RightControllerGO.transform.Find(WayToPointer);
			ControllerSetup(true);

		}
		else
		{
			yield return new WaitUntil(() => LeftControllerGO != null);
			HasLeftController = true;
			leftControllerIsSetup = true;
			LeftPointer = LeftControllerGO.transform.Find(WayToPointer);
			ControllerSetup(false);

		}
	}
	public IEnumerator Wait(bool IsRight)
	{

		if (IsRight)
		{
			while (RightControllerGO == null)
			{
				RightControllerGO = GameObject.Find(RightController);
				yield return null;
			}
			if (RightControllerGO != null)
			{

				RightPointer = RightControllerGO.transform.Find(WayToPointer);
				if (RightPointer != null)
				{
					HasRightController = true;

					if (!rightControllerIsSetup)
					{
						rightControllerIsSetup = true;
						ControllerSetup(true);
					}
				}
			}
		}
		else
		{
			while (LeftControllerGO == null)
			{
				LeftControllerGO = GameObject.Find(LeftController);
				yield return null;
			}
			if (LeftControllerGO != null)
			{
				LeftPointer = LeftControllerGO.transform.Find(WayToPointer);
				if (LeftPointer != null)
				{
					HasLeftController = true;

					if (!leftControllerIsSetup)
					{
						leftControllerIsSetup = true;
						ControllerSetup(false);
					}
				}
			}
		}
	}
	public void ControllerSetup(bool IsRight)
	{

		if (IsRight)
		{

			RightPointer = RightControllerGO.transform.Find(WayToPointer);
			Thumstick = RightControllerGO.transform.Find(WayToThumstick);
			Trigger = RightControllerGO.transform.Find(WayToSelect);
			Touchpad = RightControllerGO.transform.Find(WayToTouchpad);
			Grap = RightControllerGO.transform.Find(WayToGrap);
			BoxCollider bc = RightControllerGO.AddComponent<BoxCollider>();
			bc.size = new Vector3(0.1f, 0.1f, 0.1f);
			bc.isTrigger = true;
			RightControllerGO.transform.tag = "RightController";
			Debug.Log("Right detected pointer", RightPointer.gameObject);
		}
		else
		{

			LeftPointer = LeftControllerGO.transform.Find(WayToPointer);
			ThumstickTransformLeft = LeftControllerGO.transform.Find(WayToThumstick);
			TriggerTransformLeft = LeftControllerGO.transform.Find(WayToSelect);
			TouchpadTransformLeft = LeftControllerGO.transform.Find(WayToTouchpad);
			GrapTransformLeft = LeftControllerGO.transform.Find(WayToGrap);
			BoxCollider bc = LeftControllerGO.AddComponent<BoxCollider>();
			bc.size = new Vector3(0.1f, 0.1f, 0.1f);
			bc.isTrigger = true;
			LeftControllerGO.transform.tag = "LeftController";
			Debug.Log("Left detected pointer", LeftPointer.gameObject);
		}
	}

}

