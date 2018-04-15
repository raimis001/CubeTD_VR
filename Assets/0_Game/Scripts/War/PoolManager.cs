using Simulator.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace game.tools
{

	[Serializable]
	public class PoolTemplate
	{
		public GameObject prefab;
		public int count;
		public bool dontCreate;
	}

	public class PoolManager : MonoBehaviour
	{

		private static PoolManager Instance;
		public static Transform trans
		{
			get { return Instance ? Instance.transform : null; }
		}

		private static readonly Dictionary<int, PoolClass> pools = new Dictionary<int, PoolClass>();

		public List<PoolTemplate> templates;

		private void Awake()
		{
			Instance = this;
		}

		public static GameObject Spawn(GameObject prefab, Transform target)
		{
			return Spawn(prefab, target.position, target.rotation);
		}

		public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rotattion)
		{
			//Log.r(prefab.GetHashCode().ToString());

			int hash = prefab.GetHashCode();
			PoolClass pool;
			if (!pools.TryGetValue(hash, out pool))
			{
				PoolTemplate template = FindTemplate(prefab);
				pool = new PoolClass(prefab.name) { hash = hash, template = template};
				pools.Add(hash, pool);
			}

			return pool.GetPool(prefab, pos, rotattion);

		}

		public static void Despawn(GameObject target)
		{
			//pools.ContainsValue()
			foreach (PoolClass pool in pools.Values)
			{
				if (pool.Despawn(target)) break;
			}
		}

		private static PoolTemplate FindTemplate(GameObject template)
		{
			if (!Instance) return null;

			foreach (PoolTemplate temp in Instance.templates)
			{
				if (temp.prefab != template) continue;
				return temp;
			}
			return null;
		}
	}
}
