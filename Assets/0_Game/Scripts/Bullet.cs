using UnityEngine;

public class Bullet : MonoBehaviour
{

	[Range(0, 10)]
	public float speed = 1;

	internal Enemy enemy;



	private void Update()
	{
		if (!enemy)
		{
			Destroy(gameObject);
			return;
		}
		transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, Time.deltaTime * 10f * speed);
	}

}
