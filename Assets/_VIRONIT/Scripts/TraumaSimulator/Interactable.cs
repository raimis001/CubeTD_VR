using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TraumaMR { 
	public class Interactable : MonoBehaviour {

		[HideInInspector] public PlayerController player;
		[HideInInspector] public Transform myTransform;
		[HideInInspector] public Animator anim;
		public GameObject indicatorPrefab;
		protected GameObject indicator;
		public virtual void Awake()
		{
			player = FindObjectOfType<PlayerController>();
			myTransform = transform;
			anim = GetComponent<Animator>();
			if (indicatorPrefab != null)
			{
				indicator = Instantiate(indicatorPrefab, myTransform.position, myTransform.rotation, myTransform);
				indicator.transform.localPosition = Vector3.zero;
				indicator.SetActive(false);
			}
		}
		public virtual void Interact(HandGroup hand)
		{
           
		}
		public virtual void TurnIndicator(bool on)
		{
			if (indicator != null)
			{
				indicator.SetActive(on);
			}
		}
	}
}
