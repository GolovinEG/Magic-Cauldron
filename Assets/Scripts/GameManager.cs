using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] tiles;
    public float spawnHeight { get;} = 9.5f;
    public bool isSelectionActive { get; set; } = false;
    public int selectedTiles { get; set; } = 0;
    public GameObject[,] collumns { get; private set; } = new GameObject[8, 8];
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCollumn(1, 0));
        StartCoroutine(SpawnCollumn(2, 1));
        StartCoroutine(SpawnCollumn(2, 2));
        StartCoroutine(SpawnCollumn(3, 3));
        StartCoroutine(SpawnCollumn(3, 4));
        StartCoroutine(SpawnCollumn(2, 5));
        StartCoroutine(SpawnCollumn(2, 6));
        StartCoroutine(SpawnCollumn(1, 7));
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
        GameObject spawnedTile = tile;
        spawnedTile.GetComponent<Tile>().SetRow(row);
        collumns[collumn, row] = spawnedTile;
        Instantiate<GameObject>(spawnedTile, spawnPoint, tile.transform.rotation);
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
