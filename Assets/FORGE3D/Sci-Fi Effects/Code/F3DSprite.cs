using UnityEngine;
using System;
using game.tools;

namespace Forge3D
{
	public class F3DSprite : MonoBehaviour
	{

		public Action<RaycastBullet> ImpactAction;
		public float fxOffset; // Fx offset from bullet's touch point      
		public LayerMask layerMask;

		public F3DFXType fxType; // Weapon type

		protected RaycastHit hitPoint; // Raycast structure 


		public void SetOffset(float offset)
		{
			fxOffset = offset;
		}

		protected void ApplyForce(float force)
		{
			if (hitPoint.rigidbody != null)
				hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.VelocityChange);
		}

		// OnDespawned called by pool manager 
		protected void OnProjectileDestroy()
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(transform);
			PoolManager.Despawn(gameObject);
		}
	}
}
