using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandler : MonoBehaviour {
	/*
    Animator anim;
    private Transform leftHand, rightHand;
    public float offsetY;
    public Camera cam;
    void Start()
    {
        anim = GetComponent<Animator>();
        leftHand = InputMR.LeftPointer;
        rightHand = InputMR.RightPointer;
        cam = Camera.main;
        
    }
   

    private void OnAnimatorIK(int layerIndex)
    {
        HandToCameraPosition();

    }


    public void HandToCameraPosition()
    {
      
        if (InputMR.LeftControllerGO == null || InputMR.RightControllerGO == null)
        {
            return;
        }

        float lHweight = 1;// anim.GetFloat("LeftHand");
        float rHweight = 1;// anim.GetFloat("RightHand");
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, lHweight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rHweight);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, InputMR.LeftControllerGO.transform.position);
        anim.SetIKPosition(AvatarIKGoal.RightHand, InputMR.RightControllerGO.transform.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, lHweight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rHweight);

        anim.SetIKRotation(AvatarIKGoal.LeftHand, InputMR.LeftControllerGO.transform.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightHand, InputMR.RightControllerGO.transform.rotation);
    }*/
	public void OnTriggerEnter(Collider other)
	{
		Debug.Log(other);
	
	}
}
