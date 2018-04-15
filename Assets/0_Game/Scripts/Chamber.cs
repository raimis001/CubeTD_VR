using TMPro;
using UnityEngine;

public class Chamber : MonoBehaviour
{
	public HexaCell cell;
	public TextMeshPro text;

	internal float lives = 1f;

	public static void Create(HexaCell cell, GameObject prefab, Transform parent)
	{
		GameObject obj = Instantiate(prefab);
		obj.transform.SetParent(parent);
		obj.transform.position = cell.position;

		Chamber chamber = obj.GetComponent<Chamber>();
		if (!chamber) return;

		chamber.cell = cell;
		cell.chamber = chamber;
	}

	// Use this for initialization
	void Start()
	{
		if (text) text.text = cell.Index;

	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Damage(float damage)
	{
		lives -= damage;
		if (lives > 0) return;

		BuildManager.DestroyChamber(cell);
	}

	private void OnMouseDown()
	{
		Debug.Log("Click on chamber:" + cell);

		for (int i = 0; i < 6; i++)
		{
			IntVector v = BuildManager.offset(cell.Vector, i);
			BuildManager.CreateChamber(v.col, v.row);
		}

	}
}
