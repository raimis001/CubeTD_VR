using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	public Camera cam;

	void Start()
	{
		if (cam) return;

		cam = Camera.main;
	}

	private void OnEnable()
	{
		UpdateCam();
	}


	void Update()
	{
		UpdateCam();
	}

	void UpdateCam()
	{
		if (!cam) return;
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
			cam.transform.rotation * Vector3.up);

	}
}
