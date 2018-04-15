using System.Collections.Generic;
using UnityEngine;

namespace game.tools
{
	public class PoolClass
	{
		private const string broadcastSpawnName = "OnSpawned";
		private const string broadcastDespawnName = "OnDespawned";


		public readonly List<GameObject> spawn = new List<GameObject>();
		public readonly List<GameObject> pool = new List<GameObject>();
		public int hash;

		public PoolTemplate template;

		private Transform parent;
		private int visibleCount = 0;

		public PoolClass(string className)
		{
			GameObject o = new GameObject(className);
			o.transform.SetParent(PoolManager.trans);
			parent = o.transform;
		}

		public GameObject GetPool(GameObject prefab, Vector3 pos, Quaternion rotation)
		{
			GameObject obj;
			if (pool.Count < 1)
			{
				if (spawn.Count > template.count && template.dontCreate) return null;

				obj = GameObject.Instantiate(prefab, pos, rotation);
				if (parent) obj.transform.SetParent(parent);

				obj.AddComponent<PoolBehaviour>();

				spawn.Add(obj);
				pool.Add(obj);
			}


			obj = pool[0];
			obj.transform.position = pos;
			obj.transform.rotation = rotation;
			obj.SetActive(true);
			BroadcastSpawn(obj);
			pool.RemoveAt(0);


			PoolBehaviour pb = obj.GetComponent<PoolBehaviour>();
			pb.visible = visibleCount < template.count;
			if (pb.visible) ++visibleCount;

			return obj;
		}
		public bool Despawn(GameObject target)
		{
			if (spawn.Contains(target))
			{
				PoolBehaviour pb = target.GetComponent<PoolBehaviour>();
				if (pb && pb.visible) --visibleCount;

				target.SetActive(false);
				pool.Add(target);
				BroadcastDespawn(target);
				return true;
			}

			return false;
		}

		private void BroadcastSpawn(GameObject obj)
		{
			obj.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
		}
		private void BroadcastDespawn(GameObject obj)
		{
			obj.BroadcastMessage(broadcastDespawnName, SendMessageOptions.DontRequireReceiver);
		}
	}
}
