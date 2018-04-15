using System.Collections;
using UnityEngine;

namespace game.tools {
	public class BulletDespawner : MonoBehaviour
	{

		[Range(0, 5)]
		public float despawnTime = 0.1f;

		public void OnSpawned()
		{
			StartCoroutine(Wait());
		}

		public void OnDespawned()
		{
		}

		IEnumerator Wait()
		{
			yield return new WaitForSeconds(despawnTime);
			PoolManager.Despawn(gameObject);
		}

	}
}
