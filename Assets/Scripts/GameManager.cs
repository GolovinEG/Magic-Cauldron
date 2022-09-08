using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject selectionBoxSample;
    public GameObject[] tiles;
    public float spawnHeight { get;} = 9.5f;
    public bool isMoving { get; set; } = false;
    public bool isSelectionActive { get; set; } = false;
    public List<GameObject> selectedTiles { get; set; }  = new List<GameObject>();
    public GameObject[,] collumns { get; private set; } = new GameObject[8, 8];
    // Start is called before the first frame update
    void Start()
    {
        int[] initialCollumns = {1, 2, 2, 3, 3, 2, 2, 1 };
        for (int i = 0; i < 8; i++)
            StartCoroutine(SpawnCollumn(initialCollumns[i], i));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnTile(int collumn)
    {
        Vector3 spawnPoint = new Vector3();
        int row = 0;
        for (int i = 0; i < 8; i++) if (collumns[collumn, i] == null)
            {
                spawnPoint = new Vector3(-3.5f + collumn, spawnHeight, 0);
                row = i;
                break;
            }
        GameObject tile = tiles[Random.Range(0, tiles.Length)];
        GameObject spawnedTile = Instantiate<GameObject>(tile, spawnPoint, tile.transform.rotation);
        spawnedTile.GetComponent<Tile>().SetPositionData(collumn, row);
        collumns[collumn, row] = spawnedTile;
        //Instantiate<GameObject>(spawnedTile, spawnPoint, tile.transform.rotation);
    }

    public void UndoSelection()
    {
        isSelectionActive = false;
        isMoving = false;
        Destroy(GetSelectionBox());
        selectedTiles = new List<GameObject>();
    }

    public GameObject GetSelectionBox()
    {
        GameObject selectionBox = GameObject.Find("SelectionBox(Clone)");
        return selectionBox;
    }

    IEnumerator SpawnCollumn(int amount, int collumn)
    {
        for (int i = 0; i < amount; i++)
        {
            yield return new WaitForSeconds(Time.deltaTime * 36);
            SpawnTile(collumn);
        }
    }
}
