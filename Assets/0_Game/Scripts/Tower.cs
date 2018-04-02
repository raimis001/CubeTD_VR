using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{

	public Transform rotor;
	public Transform otter;
	public Transform trail;
	public float radius = 5;

	[Range(0, 3)]
	public float shotTime = 1;
	[Range(0, 3)]
	public float rotateSpeed = 1;

	public GameObject bulletPrefab;

	private Enemy trackedEnemy;
	private bool isShot;
	// Use this for initialization
	void Start()
	{
		StartCoroutine(Trail());
	}

	// Update is called once per frame
	void Update()
	{
		if (!trackedEnemy)
		{
			trackedEnemy = FindClosestEnemy();
		}
		if (!trackedEnemy)
		{
			return;
		}

		RotateTower();


		if (!isShot) Shot();
	}

	void RotateTower()
	{
		if (!trackedEnemy)
		{
			return;
		}

		Vector3 targetDir = trackedEnemy.transform.position - rotor.position;

		// The step size is equal to speed times frame time.
		float step = rotateSpeed * Time.deltaTime;

		Vector3 newDir = Vector3.RotateTowards(rotor.forward, targetDir, step, 0.0f);
		Debug.DrawRay(rotor.position, newDir, Color.red);

		// Move our position a step closer to the target.
		rotor.rotation = Quaternion.LookRotation(newDir);
	}

	void Shot()
	{
		if (!trackedEnemy) return;

		Vector3 targetDir = trackedEnemy.transform.position - rotor.position;
		if (Vector3.Angle(targetDir, rotor.forward) > 20) return;

		Bullet bullet = Instantiate(bulletPrefab, otter.position, otter.rotation).GetComponent<Bullet>();
		bullet.enemy = trackedEnemy;
		StartCoroutine(WaitForShot());
	}

	IEnumerator WaitForShot()
	{
		isShot = true;
		yield return new WaitForSeconds(shotTime);
		isShot = false;
	}

	public Enemy FindClosestEnemy()
	{
		Enemy[] gos = FindObjectsOfType<Enemy>();
		Enemy closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (Enemy go in gos)
		{
			if (Vector3.Distance(go.transform.position, position) > radius) continue;
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance)
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	IEnumerator Trail()
	{
		float angle = 0;
		while (true)
		{
			
			float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
			float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

			trail.localPosition = new Vector3(x, trail.localPosition.y, y);

			yield return new WaitForSeconds(0.05f);

			angle += 5;
		}
	}
}
