using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

	public Gradient progressColors;

	[Range(0, 3)]
	public float shotTime = 1;
	[Range(0,1)]
	public float shotDamage = 0.1f;

	private NavMeshAgent agent;
	private float health = 1f;

	// Use this for initialization
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		Renderer rend = GetComponent<Renderer>();
		rend.material.color = progressColors.Evaluate(1-health);
		agent.SetDestination(Vector3.zero);
		agent.updateRotation = true;
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

	private void OnTriggerEnter(Collider collision)
	{
		Bullet bullet = collision.gameObject.GetComponent<Bullet>();
		if (!bullet) return;

		Destroy(bullet.gameObject);

		health -= 0.1f;
		Renderer rend = GetComponent<Renderer>();
		rend.material.color = progressColors.Evaluate(1-health);
		if (health <= 0)
		{
			Destroy(gameObject);
		}
	}
}
