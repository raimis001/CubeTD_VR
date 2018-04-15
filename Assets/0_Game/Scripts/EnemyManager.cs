using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Wave
{
	[Range(0, 100)]
	public int count;
	[Range(0, 5)]
	public float spawnTime;

	public GameObject[] enemyPrefabs;

	internal bool waweStarted;

	private Transform wawePoint;
	private int currentWawe;
	private int currentEnemy;
	private EnemyManager manager;

	public void StartWave(Transform point, EnemyManager manager)
	{
		wawePoint = point;
		this.manager = manager;
		waweStarted = true;
		manager.StartCoroutine(WaveUpdate());
	}

	IEnumerator WaveUpdate()
	{
		yield return new WaitForSeconds(spawnTime);
		GameObject.Instantiate(enemyPrefabs[currentEnemy], wawePoint.position, Quaternion.identity);
		currentEnemy++;
		if (currentEnemy >= enemyPrefabs.Length) currentEnemy = 0;



		currentWawe++;
		if (currentWawe < count)
		{
			manager.StartCoroutine(WaveUpdate());
		} else
		{
			waweStarted = false;
			currentWawe = 0;
			currentEnemy = 0;
		}
	}

	public IEnumerator WaitForEnd()
	{
		while (waweStarted)
		{
			yield return null;
		}
	}
}

public class EnemyManager : MonoBehaviour
{
	public Transform[] wawePoints;
	public Wave[] waves;

	[Range(0, 100)]
	public float waweTime;

	private int currentWawe;
	// Use this for initialization
	void Start()
	{
		StartCoroutine(WaitForNextWawe());
	}

	// Update is called once per frame
	void Update()
	{

	}



	IEnumerator WaitForNextWawe()
	{
		yield return new WaitForSeconds(waweTime);
		waves[currentWawe].StartWave(wawePoints[0],this);
		yield return waves[currentWawe].WaitForEnd();

		++currentWawe;
		if (currentWawe >= waves.Length) currentWawe = 0;
		StartCoroutine(WaitForNextWawe());
	}
}
