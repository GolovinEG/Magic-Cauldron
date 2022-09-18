using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int rareChance;
    public GameObject selectionBoxSample;
    public GameObject[] commonTiles, rareTiles;
    public TMPro.TMP_Text scoreText, gameOverText, hiScoreText;
    public Button airButton, earthButton, fireButton, waterButton;
    public int fallFrames { get; set; } = 12;
    public float spawnHeight { get;} = 9.5f;
    public bool isMoving { get; set; } = false;
    public bool isSelectionActive { get; set; } = false;
    public bool isInteractible { get; set; } = false;
    public bool isTurnFree { get; set; } = false;
    public GameObject activeAimBox { get; set; }
    private int spawnCount = 2;
    public int score { get; set; } = 0;
    public int hiScore { get; set; } = 0;
    public int airMana { get; set; } = 0;
    public int earthMana { get; set; } = 0;
    public int fireMana { get; set; } = 0;
    public int waterMana { get; set; } = 0;
    public List<GameObject> selectedTiles { get; set; }  = new List<GameObject>();
    public List<GameObject> detonatingTiles { get; set; } = new List<GameObject>();
    public List<GameObject> fallingTiles { get; set; } = new List<GameObject>();
    public List<GameObject> blastedTiles { get; set; } = new List<GameObject>();
    public GameObject[,] collumns { get; private set; } = new GameObject[8, 8];

    [System.Serializable]
    private class Record
    {
        public int hiScore;
    }
    // Start is called before the first frame update
    void Start()
    {
        int[] initialCollumns = {1, 2, 2, 3, 3, 2, 2, 1 };
        for (int i = 0; i < 8; i++)
            StartCoroutine(SpawnCollumn(initialCollumns[i], i));
        isInteractible = true;
        if (File.Exists(Application.persistentDataPath + "/Hiscore.json"))
        {
            hiScore = JsonUtility.FromJson<Record>(File.ReadAllText(Application.persistentDataPath + "/Hiscore.json")).hiScore;
            hiScoreText.text = "Hi-Score: " + hiScore; 
        }
    }

    private void SpawnTile(int collumn)
    {
        Vector3 spawnPoint = new Vector3();
        int row = 0;
        for (int i = 0; i < 8; i++) if (collumns[collumn, i] == null)
            {
                spawnPoint = new Vector3(-3.5f + collumn, spawnHeight, 0);
                row = i;
                break;
            }
        GameObject tile = commonTiles[0];
        if (Random.Range(0, 100) < rareChance)
            tile = rareTiles[Random.Range(0, rareTiles.Length)];
        else
            tile = commonTiles[Random.Range(0, commonTiles.Length)];
        GameObject spawnedTile = Instantiate<GameObject>(tile, spawnPoint, tile.transform.rotation);
        spawnedTile.GetComponent<CommonTile>().collumn = collumn;
        spawnedTile.GetComponent<CommonTile>().row = row;
        collumns[collumn, row] = spawnedTile;
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

    public void HandleTurn()
    {
        bool isGameOver = false;
        isInteractible = false;
        if (!isTurnFree)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                int randomCollumn = Random.Range(0, 8);
                if (collumns[randomCollumn, 7] != null)
                {
                    isGameOver = true;
                    gameOverText.enabled = true;
                    if (score > hiScore)
                    {
                        hiScore = score;
                        hiScoreText.text = "Hi-Score: " + hiScore;
                        Record record = new Record();
                        record.hiScore = hiScore;
                        File.WriteAllText(Application.persistentDataPath + "/Hiscore.json", JsonUtility.ToJson(record));
                    }
                }
                else StartCoroutine(DelaySpawn(randomCollumn));
            }
        }
        else
            isTurnFree = false;
        if (!isGameOver)
        {
            isInteractible = true;
            score++;
            scoreText.text = "Score: " + score;
        }
    }

    IEnumerator DelaySpawn(int collumn)
    {
        yield return new WaitForEndOfFrame();
        SpawnTile(collumn);
    }

    public void CleanUp()
    {
        detonatingTiles = new List<GameObject>();
        blastedTiles = new List<GameObject>();
        fallingTiles = new List<GameObject>();
    }
}
