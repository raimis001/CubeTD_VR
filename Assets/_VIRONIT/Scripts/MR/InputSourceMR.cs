using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.WSA.Input;
using HoloToolkit.Unity;


public enum InputState
{
    Wait, Press, Hold, Up
}
public class ControllerKey
{
    public InputState state;
	public InputState previousState;
	public bool changed;
	public bool GetKeyPress()
    {
        return state == InputState.Press;
    }
    public bool GetKeyHold()
    {
        return state == InputState.Hold;
    }
    public bool GetKeyUp()
    {
        return state == InputState.Up;
    }
    public bool GetKeyWait()
    {
        return state == InputState.Wait;
    }
}
public enum KeyCodeMR
{
   RightTrigger, LeftTrigger, LeftGrab, RightGrab, RightTouchpad, LeftTouchpad, LeftMenu, RightMenu
}

public class InputSourceMR : MonoBehaviour {
    
    private class ControllerState
    {
        public InteractionSourceHandedness Handedness;
        public Vector3 PointerPosition;
        public Quaternion PointerRotation;
        public Vector3 GripPosition;
        public Quaternion GripRotation;
        public bool Grasped;
        public bool MenuPressed;
        public bool SelectPressed;
        public float SelectPressedAmount;
        public bool ThumbstickPressed;
        public Vector2 ThumbstickPosition;
        public bool TouchpadPressed;
        public bool TouchpadTouched;
        public Vector2 TouchpadPosition;
    }
    

    [HideInInspector] public static ControllerKey RightGrab { get { return rightGrab; } private set { rightGrab = value; } }
    [HideInInspector] public static ControllerKey LeftGrab { get { return leftGrab; } private set { leftGrab = value; } }
    [HideInInspector] public static ControllerKey RightTrigger { get { return rightTrigger; } private set { rightTrigger = value; } }
    [HideInInspector] public static ControllerKey LeftTrigger { get { return leftTrigger; } private set { leftTrigger = value; } }
    [HideInInspector] public static ControllerKey RightTouchpad { get { return rightTouchpad; } private set { rightTouchpad = value; } }
    [HideInInspector] public static ControllerKey LeftTouchpad { get { return leftTouchpad; } private set { leftTouchpad = value; } }
	[HideInInspector] public static ControllerKey RightMenu { get { return rightMenu; } private set { rightMenu = value; } }
	[HideInInspector] public static ControllerKey LeftMenu { get { return leftMenu; } private set { leftMenu = value; } }

	private static ControllerKey rightGrab = new ControllerKey();
    private static ControllerKey leftGrab = new ControllerKey();
    private static ControllerKey rightTrigger = new ControllerKey();
    private static ControllerKey leftTrigger = new ControllerKey();
    private static ControllerKey rightTouchpad = new ControllerKey();
    private static ControllerKey leftTouchpad = new ControllerKey();
	private static ControllerKey rightMenu = new ControllerKey();
	private static ControllerKey leftMenu = new ControllerKey();

	private Dictionary<uint, ControllerState> controllers;
    [HideInInspector] public static GameObject RightControllerGO { get; private set; }
    [HideInInspector] public static GameObject LeftControllerGO { get; private set; }
    [HideInInspector] public  static bool HasRightController { get; private set; }
    [HideInInspector] public static bool HasLeftController { get; private set; }

    [HideInInspector]  public static Transform Grap { get; private set; }
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

    private List<ControllerKey> keyList = new List<ControllerKey>();
    private static Dictionary<KeyCodeMR, ControllerKey> keyDict = new Dictionary<KeyCodeMR, ControllerKey>();
    private bool leftControllerIsSetup;
    private bool rightControllerIsSetup;
	

    public static float leftThumbstickX;
    public static float leftThumbstickY;
    public static float rightThumbstickX;
    public static float rightThumbstickY;

	public static bool getLeft;
	public static bool getRight;
	#region Touched Input.GetKey()
	public const string leftTouchpadPathTouched = "joystick button 18";
	public const string rightTouchPadPathTouched = "joystick button 19";
	#endregion
	#region Pressed Input.Getkey()
	public const string leftTriggerPath = "joystick button 14";
	public const string rightTriggerPath = "joystick button 15";

	public const string leftGrabPath = "joystick button 4";
	public const string rightGrabPath= "joystick button 5";

	public const string leftMenuPath = "joystick button 6";
	public const string rightMenuPath = "joystick button 7";

	public const string leftTouchpadPath = "joystick button 16";
	public const string rightTouchPadPath = "joystick button 17";

	public const string leftThumbstickPath = "joystick button 8";
	public const string rightThumbstickPath = "joystick button 9";
	#endregion
	#region Axis Input.GetAxis()
	public const string LeftThumbstickXPath = "CONTROLLER_LEFT_STICK_HORIZONTAL";
	public const string LeftThumbstickYPath = "CONTROLLER_LEFT_STICK_VERTICAL";
	public const string RightThumbstickXPath = "CONTROLLER_RIGHT_STICK_HORIZONTAL";
	public const string RightThumbstickYPath = "CONTROLLER_RIGHT_STICK_VERTICAL";
	#endregion

	private void Awake()
    {
        controllers = new Dictionary<uint, ControllerState>();
		InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
		InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
		InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;

		keyList.Add(LeftTrigger);
        keyList.Add(RightTrigger);
        keyList.Add(LeftTouchpad);
        keyList.Add(RightTouchpad);
        keyList.Add(LeftGrab);
        keyList.Add(RightGrab);
		keyList.Add(LeftMenu);
		keyList.Add(RightMenu);

		keyDict[KeyCodeMR.RightTrigger] = RightTrigger;
        keyDict[KeyCodeMR.LeftTrigger] = LeftTrigger;
        keyDict[KeyCodeMR.RightGrab] = RightGrab;
        keyDict[KeyCodeMR.LeftGrab] = LeftGrab;
        keyDict[KeyCodeMR.LeftTouchpad] = LeftTouchpad;
        keyDict[KeyCodeMR.RightTouchpad] = RightTouchpad;
		keyDict[KeyCodeMR.LeftMenu] = LeftMenu;
		keyDict[KeyCodeMR.RightMenu] = RightMenu;
	}

    void Start () {
		if (DebugPanel.Instance != null)
		{
			DebugPanel.Instance.RegisterExternalLogCallback(GetControllerInfo);
			Debug.Log("getting");
		}

	}
	public IEnumerator WaitForControllers()
	{
		var controllers = Input.GetJoystickNames();
		yield return new WaitUntil(() => controllers.Length > 2);
	}
    void Update()
	{
		leftThumbstickX = Input.GetAxis(LeftThumbstickXPath);
		leftThumbstickY = Input.GetAxis(LeftThumbstickYPath);
		rightThumbstickX = Input.GetAxis(RightThumbstickXPath);
		rightThumbstickY = Input.GetAxis(RightThumbstickYPath);
		
       
		//var controllers = Input.GetJoystickNames();
		//if (controllers.Length>1)
		//{
		//	foreach (var c in controllers)
		//	{
		//		if (c.Contains("Right")&&!getRight)
		//		{
		//			getRight = true;
		//		 StartCoroutine(FindingController(true));
		//		}
		//		if (c.Contains("Left")&&!getLeft)
		//		{
		//			getLeft=true;
		//			StartCoroutine(FindingController(false));
		//		}
		//	}
		//}
		//if (getRight&&RightControllerGO==null)
		//{
		//	RightControllerGO = GameObject.Find(RightController);
		//}
		//if (getLeft && LeftControllerGO == null)
		//{
		//	LeftControllerGO = GameObject.Find(LeftController);
		//}

	}
	public IEnumerator FindingController(bool isRight)
	{
		if (isRight)
		{
			yield return new WaitUntil(() => RightControllerGO != null);
			HasRightController = true;
			rightControllerIsSetup = true;
			ControllerSetup(true);

		}
		else
		{
			yield return new WaitUntil(() => LeftControllerGO != null);
			HasLeftController = true;
			leftControllerIsSetup = true;
			ControllerSetup(false);

		}
	}
    void LateUpdate()
    {

		foreach (var key in keyList)
		{
			if (key.GetKeyPress()) key.state = InputState.Hold;
			if (key.GetKeyUp()) key.state = InputState.Wait;

		}
	}

	#region Public Methods
	public static bool GetKeyPress(KeyCodeMR keyCode)
    {
        if (keyDict.ContainsKey(keyCode))
        {
            return keyDict[keyCode].GetKeyPress();
        }
        return false;
    }
    public static bool GetKeyUp(KeyCodeMR keyCode)
    {
        if (keyDict.ContainsKey(keyCode))
        {
            return keyDict[keyCode].GetKeyUp();
        }
        return false;
    }
    public static bool GetKeyHold(KeyCodeMR keyCode)
    {
        if (keyDict.ContainsKey(keyCode))
        {
            return keyDict[keyCode].GetKeyHold();
        }
        return false;
    }
    #endregion
   
    #region Black Work
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
                if (RightPointer!=null)
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
            
        }
    }
    private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
		if (controllers.Count > 0)
		{
			controllers.Remove(obj.state.source.id);
		}

		Debug.LogFormat("{0} {1} Detected", obj.state.source.handedness, obj.state.source.kind);

		if (obj.state.source.kind == InteractionSourceKind.Controller && !controllers.ContainsKey(obj.state.source.id))
		{
			controllers.Add(obj.state.source.id, new ControllerState { Handedness = obj.state.source.handedness });
			
			if (obj.state.source.handedness == InteractionSourceHandedness.Right)
			{
				StartCoroutine(Wait(true));


			}
			if (obj.state.source.handedness == InteractionSourceHandedness.Left)
			{
				StartCoroutine(Wait(false));
			}
			//  StartCoroutine(Wait(true));     
		}
	}

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        Debug.LogFormat("{0} {1} Lost", obj.state.source.handedness, obj.state.source.kind);
        if (obj.state.source.handedness == InteractionSourceHandedness.Right)
        {
            HasRightController = false;
            RightControllerGO = null;
        }
        if (obj.state.source.handedness == InteractionSourceHandedness.Left)
        {
            HasLeftController = false;
             LeftControllerGO = null;
        }
       
        controllers.Remove(obj.state.source.id);
    }

    private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        ControllerState controllerState;

        if (controllers.TryGetValue(obj.state.source.id, out controllerState))
        {
            obj.state.sourcePose.TryGetPosition(out controllerState.PointerPosition, InteractionSourceNode.Pointer);
            obj.state.sourcePose.TryGetRotation(out controllerState.PointerRotation, InteractionSourceNode.Pointer);
            obj.state.sourcePose.TryGetPosition(out controllerState.GripPosition, InteractionSourceNode.Grip);
            obj.state.sourcePose.TryGetRotation(out controllerState.GripRotation, InteractionSourceNode.Grip);

            controllerState.Grasped = obj.state.grasped;
            controllerState.MenuPressed = obj.state.menuPressed;
            controllerState.SelectPressed = obj.state.selectPressed;
            controllerState.SelectPressedAmount = obj.state.selectPressedAmount;
            controllerState.ThumbstickPressed = obj.state.thumbstickPressed;
            controllerState.ThumbstickPosition = obj.state.thumbstickPosition;
            controllerState.TouchpadPressed = obj.state.touchpadPressed;
            controllerState.TouchpadTouched = obj.state.touchpadTouched;
            controllerState.TouchpadPosition = obj.state.touchpadPosition;
			
        }
    }


    private string GetControllerInfo()
    {
        string toReturn = string.Empty;

        foreach (ControllerState controllerState in controllers.Values)
        {
            // Text label display
            if (controllerState.Handedness.Equals(InteractionSourceHandedness.Left))
            {
                if (controllerState.SelectPressed && LeftTrigger.GetKeyWait())
                {
					LeftTrigger.state = InputState.Press;

                }
                else if (!controllerState.SelectPressed && LeftTrigger.GetKeyHold())
                {
					LeftTrigger.state = InputState.Up;
                }

                if (controllerState.TouchpadPressed && LeftTouchpad.GetKeyWait())
                {
					LeftTouchpad.state = InputState.Press;
                }
                else if (!controllerState.TouchpadPressed && LeftTouchpad.GetKeyHold())
                {
					LeftTouchpad.state = InputState.Up;
                }

                if (controllerState.Grasped && LeftGrab.GetKeyWait())
                {
					LeftGrab.state = InputState.Press;
                }
                else if (!controllerState.Grasped && LeftGrab.GetKeyHold())
                {	
					LeftGrab.state = InputState.Up;
                }
				if (controllerState.MenuPressed && LeftMenu.GetKeyWait())
				{
					LeftMenu.state = InputState.Press;
				}
				else if (!controllerState.MenuPressed && LeftMenu.GetKeyHold())
				{
					LeftMenu.state = InputState.Up;
				}
			}
            else if (controllerState.Handedness.Equals(InteractionSourceHandedness.Right))
            {
				
				if (controllerState.SelectPressed && RightTrigger.GetKeyWait())
                {
					RightTrigger.state = InputState.Press;
				}
                else if (!controllerState.SelectPressed && RightTrigger.GetKeyHold())
                {
					RightTrigger.state = InputState.Up;
                }

                if (controllerState.TouchpadPressed && RightTouchpad.GetKeyWait())
                {
					RightTouchpad.state = InputState.Press;
                }
                else if (!controllerState.TouchpadPressed && RightTouchpad.GetKeyHold())
                {
					RightTouchpad.state = InputState.Up;
                }

                if (controllerState.Grasped && RightGrab.GetKeyWait())
                {
					RightGrab.state= InputState.Press;
                }
                else if (!controllerState.Grasped && RightGrab.GetKeyHold())
                {
					RightGrab.state = InputState.Up;
                }
				if (controllerState.MenuPressed && RightMenu.GetKeyWait())
				{
					RightMenu.state = InputState.Press;
				}
				else if (!controllerState.MenuPressed && RightMenu.GetKeyHold())
				{
					RightMenu.state = InputState.Up;
				}

				ControllerThumbstickPosition = controllerState.ThumbstickPosition;
                ControllerTouchpadPosition = controllerState.TouchpadPosition;

            }
        }

        return toReturn.Substring(0, Math.Max(0, toReturn.Length - 2));
    }
    #endregion

}

