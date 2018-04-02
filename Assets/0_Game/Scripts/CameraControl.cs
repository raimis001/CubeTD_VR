using Simulator.Scripts;
using UnityEngine;

namespace Assets.Scripts
{
	public class CameraControl : MonoBehaviour
	{
		public static bool Panning;

		[Header("Zoom")]
		[Range(0,100)]
		public float minZoom = 50;
		[Range(0, 100)]
		public float maxZoom = 90;
		[Range(0, 5)]
		public float zoomSpeed = 1;

		[Header("Pan")]
		[Range(0,5)]
		public float panSpeed = 1;
		private Vector3 mouseOrigin;

		public Camera cam;

		private float zoomDelta;
		private int maskTerrain;
		void Start()
		{
			maskTerrain = LayerMask.GetMask("Terrain");

			if (!cam)
			{
				cam = GetComponent<Camera>();
			}
			if (!cam)
			{
				cam = Camera.main;
			}
		}
		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				// Get mouse origin
				mouseOrigin = Input.mousePosition;
			}

			Zoom();
			if (Input.GetMouseButton(0))
			{
				Pan();
			}
			if (Input.GetMouseButtonUp(0))
			{
				Panning = false;
			}
		}

		void Zoom()
		{
			float zoom = Input.mouseScrollDelta.y;
			if (Mathf.Abs(zoom) < Mathf.Epsilon) return;

			zoomDelta += zoom;
			zoomDelta = Mathf.Clamp(zoomDelta, -5f, 5f);

			RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward,Mathf.Infinity,maskTerrain);
			if (hits.Length < 1)
			{
				Debug.Log("No terrain");
				return;
			}
			float d = 0;
		
			foreach (RaycastHit h in hits)
			{
				if (!h.collider.gameObject.GetComponent<Terrain>()) continue;
				d = h.distance;
			}
			if (zoom > 0 && d < minZoom || zoom < 0 && d > maxZoom) return;

			Vector3 move = zoom * zoomSpeed * transform.forward;
			transform.Translate(move, Space.World);
		}

		void Pan()
		{
			Debug.Log(Vector3.Distance(Input.mousePosition, mouseOrigin));
			if (Vector3.Distance(Input.mousePosition,mouseOrigin) < 10)
			{
				return;
			}
			Panning = true;

			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			Vector3 move = new Vector3(pos.x , -pos.y ) * panSpeed * 10f;
			transform.Translate(move, Space.Self);

			mouseOrigin = Input.mousePosition;
		}

	}
}
