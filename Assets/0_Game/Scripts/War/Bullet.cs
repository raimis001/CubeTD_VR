using Forge3D;
using System;
using System.Collections;
using UnityEngine;

namespace game.tools
{
	public class Bullet : MonoBehaviour
	{
		public F3DFXType fxType;

		public LayerMask layerMask;

		public float velocity = 300f;
		public float raycastAdvance = 2f;
		public float lifeTime;

		internal RaycastHit hitPoint;
		internal bool isHit;
		internal Action<RaycastBullet> ImpactAction;


		public void OnSpawned()
		{
			isHit = false;
			hitPoint = new RaycastHit();
			if (lifeTime > 0) StartCoroutine(WaitDespawn());
		}

		public void OnDespawned()
		{
			StopAllCoroutines();
		}

		IEnumerator WaitDespawn()
		{
			yield return new WaitForSeconds(lifeTime);
			OnBulletDestroy();
		}


		private void Update()
		{
			Vector3 step = transform.forward * Time.deltaTime * velocity;
			isHit = Physics.Raycast(transform.position, transform.forward, out hitPoint, step.magnitude * raycastAdvance, layerMask);

			if (!isHit)
			{
				DoStep(step);
				return;
			}

			if (ImpactAction != null)
			{
				ImpactAction.Invoke(new RaycastBullet(hitPoint) { forward = transform.forward });
			}

			OnBulletDestroy();		
		}

		protected virtual void DoStep(Vector3 step)
		{
			transform.position += step;
		}

		protected void OnBulletDestroy()
		{
			PoolManager.Despawn(gameObject);
		}

	}

}
