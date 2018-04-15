using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraumaMR
{
	public class Napkin : Takeable
	{
		public Transform myPackage;
		public override void Drop(HandGroup hand, bool dropPhysic = true)
		{
			base.Drop(hand, dropPhysic);
			myTransform.position = myPackage.transform.position;
			gameObject.SetActive(false);
		}

		
	}
}
