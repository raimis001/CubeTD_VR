using System.Collections.Generic;
using UnityEngine;

public struct IntVector
{
	public int col;
	public int row;

	public IntVector(int col, int row)
	{
		this.col = col;
		this.row = row;
	}

}

public class HexaCell
{
	public int row;
	public int col;

	public Vector3 position;

	public Chamber chamber;

	public static string GetIndex(int col, int row)
	{
		return string.Format("{0}:{1}", col, row);
	}

	public string Index
	{
		get { return GetIndex(col, row); }
	}

	public IntVector Vector
	{
		get { return new IntVector(col, row); }
	}

	public HexaCell(int col, int row)
	{
		SetPosition(col, row);
	}

	public void SetPosition(int col, int row)
	{
		this.row = row;
		this.col = col;

		int odd = this.row & 1;

		position = new Vector3
		{
			z = this.col * BuildManager.hexaWidth + odd * BuildManager.hexaHoriz,
			x = this.row * BuildManager.hexaVert
		};
	}

	public override string ToString()
	{
		return "Cell:" + Index;
	}
}

public class BuildManager : MonoBehaviour
{
	private static BuildManager Instance;

	public GameObject wallPrefab;
	public GameObject hexaPrefab;

	private GameObject ghostObject;
	private bool canBuild;

	[SerializeField]
	private float hexaHeight = 10f;
	public static float hexaWidth;
	public static float hexaHoriz;
	public static float hexaVert;

	public static Dictionary<string, HexaCell> grid = new Dictionary<string, HexaCell>();



	public static readonly IntVector[,] n = {
		{
			new IntVector(+1, 0),
			new IntVector(0, -1),
			new IntVector(0, +1),
			new IntVector(-1, -1),
			new IntVector(-1, 0),
			new IntVector(-1, +1),
		},
		{
			new IntVector(+1, 0),
			new IntVector(+1, -1),
			new IntVector(0, -1),
			new IntVector(-1, 0),
			new IntVector(0, +1),
			new IntVector(+1, +1)
		}
};
	public static IntVector offset(IntVector hex, int direction) {

		int parity = hex.row & 1;

		var dir = n[parity,direction];

		return new IntVector(hex.col + dir.col, hex.row + dir.row);
	}

	private void Awake()
	{
		Instance = this;
	}
	// Use this for initialization
	void Start()
	{
		hexaWidth = Mathf.Sqrt(3) / 2f * hexaHeight;
		hexaHoriz = hexaWidth / 2f;
		hexaVert = hexaHeight * 3f / 4f;

		IntVector start = new IntVector(0, 0);
		CreateChamber(start.col, start.row);

		for (int i = -10; i < 10; i++) 
			for (int j = -10; j < 10; j++)
			{
				CreateChamber(i, j);
			}
	}

	// Update is called once per frame
	void Update()
	{
		MoveGhost();

		if (!ghostObject && Input.GetKeyDown(KeyCode.Alpha1))
		{
			ghostObject = Instantiate(wallPrefab);
		}

		if (ghostObject && Input.GetMouseButtonUp(1))
		{
			Destroy(ghostObject);
			return;
		}


		if (Input.GetMouseButtonDown(0))
		{
			canBuild = true;
		}
		if (raCamera.raCameraRTS.isPaning)
		{
			canBuild = false;
			return;
		}

		if (Input.GetMouseButtonUp(0) && canBuild)
		{
			ghostObject = null;
		}
	}


	void MoveGhost()
	{
		if (!ghostObject) return;

		if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.5f && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
		{
			Vector3 rot = ghostObject.transform.localEulerAngles;
			rot.y += Input.mouseScrollDelta.y * 10f;
			ghostObject.transform.localEulerAngles = rot;
		}

		Vector3 mousePos = Input.mousePosition;
		Vector3 wordPos;
		Ray ray = Camera.main.ScreenPointToRay(mousePos);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Terrain")))
		{
			wordPos = hit.point;
		}
		else
		{
			wordPos = Camera.main.ScreenToWorldPoint(mousePos);
		}
		wordPos.x = ((int)(wordPos.x * 10f)) / 10f;
		wordPos.z = ((int)(wordPos.z * 10f)) / 10f;
		ghostObject.transform.position = wordPos;
	}

	void Build(GameObject prefab)
	{
		Vector3 mousePos = Input.mousePosition;
		Vector3 wordPos;
		Ray ray = Camera.main.ScreenPointToRay(mousePos);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Terrain")))
		{
			wordPos = hit.point;
		}
		else
		{
			wordPos = Camera.main.ScreenToWorldPoint(mousePos);
		}
		Instantiate(prefab, wordPos, Quaternion.identity);
	}

	public static void CreateChamber(int col, int row)
	{
		if (grid.ContainsKey(HexaCell.GetIndex(col, row))) return;

		HexaCell cell = new HexaCell(col, row);

		Chamber.Create(cell, Instance.hexaPrefab);
		grid.Add(cell.Index, cell);
	}

	public static Chamber ClosestChamber(Vector3 position)
	{
		float dMax = Mathf.Infinity;
		Chamber result = null;
		foreach (HexaCell cell in grid.Values)
		{
			float d = Vector3.Distance(position, cell.position);
			if (Vector3.Distance(position, cell.position) >= dMax) continue;

			dMax = d;
			result = cell.chamber;
		}

		return result;
	}

	public static void DestroyChamber(HexaCell cell)
	{
		Destroy(cell.chamber.gameObject);
		grid.Remove(cell.Index);
	}
}
