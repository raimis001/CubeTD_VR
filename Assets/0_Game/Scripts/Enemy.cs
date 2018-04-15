using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

	public Transform hitTarget;

	public Gradient progressColors;

	[Range(0, 3)]
	public float shotTime = 1;
	[Range(0,1)]
	public float shotDamage = 0.1f;

	private NavMeshAgent agent;
	private float health = 10f;

	[SerializeField]
	private int testHits;

	// Use this for initialization
	void Start()
	{
		if (!hitTarget) hitTarget = transform;

		Renderer rend = GetComponent<Renderer>();
		if (rend)
		{
			rend.material.color = progressColors.Evaluate(1 - health);
		}
		agent = GetComponent<NavMeshAgent>();
		if (agent)
		{
			agent.SetDestination(Vector3.zero);
			agent.updateRotation = true;
		}
		StartCoroutine(Shot());
	}

	private IEnumerator Shot()
	{
		Chamber chamber = BuildManager.ClosestChamber(transform.position);
		if (chamber && Vector3.Distance(transform.position, chamber.cell.position) < 15)
		{
			DoShot(chamber);
		}

		yield return new WaitForSeconds(shotTime);
		StartCoroutine(Shot());
	}

	protected virtual void DoShot(Chamber chamber)
	{
		chamber.Damage(shotDamage);
	}

	public void Hit(float damage)
	{
		//Debug.Log("Hit enemy");
		testHits++;
		health -= damage;
		if (health > 0) return;

		Destroy(gameObject);
	}

	private void Update()
	{
		if (agent) return;
		transform.RotateAround(Vector3.zero, Vector3.up, 5f * Time.deltaTime);
	}



}
