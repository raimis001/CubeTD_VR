using UnityEngine;
using System.Collections;

public class PoolBehaviour : MonoBehaviour
{
	private bool _visible = true;
	public bool visible
	{
		get { return _visible; }
		set
		{
			_visible = value;

			Renderer r = GetComponent<Renderer>();
			if (r) r.enabled = _visible;

			Renderer[] renders = GetComponentsInChildren<Renderer>();
			foreach (Renderer rend in renders)
			{
				rend.enabled = _visible;
			}

		}
	}
}
