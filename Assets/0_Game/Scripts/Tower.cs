using UnityEngine;
using System.Collections;
using Forge3D;

public class Tower : MonoBehaviour
{

	public Transform trail;
	public float radius = 5;

	public TurretFXController fxSettup;
	public F3DFXType fxType;

	public Transform[] muzles;

	private Enemy trackedEnemy;
	private int turretID;
	// Use this for initialization
	void Start()
	{
		if (trail) StartCoroutine(Trail());
		if (!fxSettup)
		{
			fxSettup = TurretFXController.instance;
		}

		turretID = fxSettup.AddTurret(fxType, muzles);
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
			fxSettup.Stop(turretID);
			return;
		}

		F3DTurret turret = GetComponent<F3DTurret>();
		if (turret) turret.SetNewTarget(trackedEnemy.hitTarget.position );
		StartCoroutine(RandomDelayFire());
	}

	IEnumerator RandomDelayFire()
	{
		yield return new WaitForSeconds(Random.value);
		fxSettup.Fire(turretID);
	}

	void Shot()
	{
		if (!trackedEnemy) return;
	}


	public Enemy FindClosestEnemy()
	{
		Enemy[] gos = FindObjectsOfType<Enemy>();
		
		Enemy closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (Enemy go in gos)
		{
			//if (Vector3.Distance(go.transform.position, position) > radius) continue;
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
