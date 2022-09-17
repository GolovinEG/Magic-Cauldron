using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonTile : MonoBehaviour
{
    public int collumn { get; set; }
    public int row { get; set; }
    public Material pahntomMaterial;
    protected GameManager game;
    // Start is called before the first frame update
    protected void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Fall();
    }

    protected void OnMouseDown()
    {
        if (game.isInteractible)
        {
            if (game.isSelectionActive) game.UndoSelection();
            else
            {
                game.isSelectionActive = true;
                GameObject selectionBox = Instantiate<GameObject>(game.selectionBoxSample, transform.position, game.selectionBoxSample.transform.rotation);
                game.selectedTiles.Add(gameObject);
            }
        }
    }

    protected void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) && game.isSelectionActive && CheckSameRow() && CheckAdjacent() && !game.isMoving)
        {
            game.GetSelectionBox().transform.Translate(Vector3.right * CheckToTheRight() * 0.5f);
            game.selectedTiles.Add(gameObject);
            game.GetSelectionBox().transform.localScale = new Vector3(0.1f + game.selectedTiles.Count, 1.1f, 1.1f);
        }
    }

    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && (game.isInteractible) && (gameObject.tag != "Rare"))
        {
            CheckDetonation();
            if (game.detonatingTiles.Count >= 3)
            {
                int valueMultiplier = 0;
                List<GameObject> rareTiles = new List<GameObject>();
                foreach (GameObject tile in game.detonatingTiles)
                    tile.GetComponent<CommonTile>().CheckRareDetonation(rareTiles);
                foreach (GameObject rareTile in rareTiles)
                    game.detonatingTiles.Add(rareTile);
                foreach (GameObject tile in game.detonatingTiles)
                    if (tile.tag == "Rare")
                        tile.GetComponent<CommonTile>().Detonate();
                foreach (GameObject tile in game.detonatingTiles)
                    if (tile.tag != "Rare")
                    {
                        valueMultiplier++;
                        int tileRow = tile.GetComponent<CommonTile>().row;
                        int tileCollumn = tile.GetComponent<CommonTile>().collumn;
                        tile.GetComponent<CommonTile>().Detonate();
                        if (tileRow != 7)
                            if (game.collumns[tileCollumn, tileRow + 1] != null)
                                game.collumns[tileCollumn, tileRow + 1].GetComponent<CommonTile>().FallAfterEvent();
                    }
                foreach (GameObject tile in game.blastedTiles)
                {
                    int tileRow = tile.GetComponent<CommonTile>().row;
                    int tileCollumn = tile.GetComponent<CommonTile>().collumn;
                    if (tileRow != 7)
                        if (game.collumns[tileCollumn, tileRow + 1] != null)
                            game.collumns[tileCollumn, tileRow + 1].GetComponent<CommonTile>().FallAfterEvent();
                }
                foreach (GameObject tile in game.blastedTiles)
                    Destroy(tile);
                foreach (GameObject tile in game.fallingTiles)
                    tile.GetComponent<CommonTile>().Fall();
                game.HandleTurn();
                int detonationValue = valueMultiplier * game.detonatingTiles.Count;
                game.score += detonationValue;
                game.scoreText.text = "Score: " + game.score;
                switch (gameObject.tag)
                {
                    case "Air":
                        game.airMana += detonationValue;
                        if (game.airMana >= 100)
                        {
                            game.airMana = 100;
                            game.airButton.interactable = true;
                        }
                        game.airButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Air " + game.airMana + "/100";
                        break;
                    case "Earth":
                        game.earthMana += detonationValue;
                        if (game.earthMana >= 100)
                        {
                            game.earthMana = 100;
                            game.earthButton.interactable = true;
                        }
                        game.earthButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Earth " + game.earthMana + "/100";
                        break;
                    case "Fire":
                        game.fireMana += detonationValue;
                        if (game.fireMana >= 100)
                        {
                            game.fireMana = 100;
                            game.fireButton.interactable = true;
                        }
                        game.fireButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Fire " + game.fireMana + "/100";
                        break;
                    case "Water":
                        game.waterMana += detonationValue;
                        if (game.waterMana >= 100)
                        {
                            game.waterMana = 100;
                            game.waterButton.interactable = true;
                        }
                        game.waterButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Water " + game.waterMana + "/100";
                        break;
                }
                game.fallingTiles = new List<GameObject>();
                game.blastedTiles = new List<GameObject>();
            }
            game.detonatingTiles = new List<GameObject>();
        }
    }

    public virtual void Fall()
    {
        int fallTiles = Mathf.RoundToInt(transform.position.y + 1.5f - row);
        StartCoroutine(ProcessFall(fallTiles));
    }

    public void MoveCollumn(int rightShift)
    {
        collumn += rightShift;
        transform.Translate(Vector3.right * rightShift);
    }

    protected bool CheckSameRow()
    {
        bool isSameRow = Mathf.Approximately(gameObject.transform.position.y, game.GetSelectionBox().transform.position.y);
        return isSameRow;
    }

    protected int CheckToTheRight()
    {
        if (transform.position.x > game.GetSelectionBox().transform.position.x) return 1;
        else return -1;
    }

    protected bool CheckAdjacent()
    {
        bool isAdjacent = Mathf.Approximately(Mathf.Abs(transform.position.x - game.GetSelectionBox().transform.position.x) - 0.5f * (game.selectedTiles.Count + 1), 0);
        return isAdjacent;
    }

    protected void CheckDetonation()
    {
        game.detonatingTiles.Add(gameObject);
        List<GameObject> adjacentTiles = new List<GameObject>();
        if (row != 7)
            if ((game.collumns[collumn, row + 1] != null) && (!game.detonatingTiles.Contains(game.collumns[collumn, row + 1])))
                if (gameObject.tag == game.collumns[collumn, row + 1].tag)
                    adjacentTiles.Add(game.collumns[collumn, row + 1]);
        if (row != 0)
            if ((game.collumns[collumn, row - 1] != null) && (!game.detonatingTiles.Contains(game.collumns[collumn, row - 1])))
                if (gameObject.tag == game.collumns[collumn, row - 1].tag)
                    adjacentTiles.Add(game.collumns[collumn, row - 1]);
        if (collumn != 7)
            if ((game.collumns[collumn + 1, row] != null) && (!game.detonatingTiles.Contains(game.collumns[collumn + 1, row])))
                if (gameObject.tag == game.collumns[collumn + 1, row].tag)
                    adjacentTiles.Add(game.collumns[collumn + 1, row]);
        if (collumn != 0)
            if ((game.collumns[collumn - 1, row] != null) && (!game.detonatingTiles.Contains(game.collumns[collumn - 1, row])))
                if (gameObject.tag == game.collumns[collumn - 1, row].tag)
                    adjacentTiles.Add(game.collumns[collumn - 1, row]);
        foreach (GameObject tile in adjacentTiles)
            tile.GetComponent<CommonTile>().CheckDetonation();
    }

    protected void CheckRareDetonation(List<GameObject> rareTiles)
    {
        if (row != 7)
            if ((game.collumns[collumn, row + 1] != null) && (!rareTiles.Contains(game.collumns[collumn, row + 1])))
                if (game.collumns[collumn, row + 1].tag == "Rare")
                    rareTiles.Add(game.collumns[collumn, row + 1]);
        if (row != 0)
            if ((game.collumns[collumn, row - 1] != null) && (!rareTiles.Contains(game.collumns[collumn, row - 1])))
                if (game.collumns[collumn, row - 1].tag == "Rare")
                    rareTiles.Add(game.collumns[collumn, row - 1]);
        if (collumn != 7)
            if ((game.collumns[collumn + 1, row] != null) && (!rareTiles.Contains(game.collumns[collumn + 1, row])))
                if (game.collumns[collumn + 1, row].tag == "Rare")
                    rareTiles.Add(game.collumns[collumn + 1, row]);
        if (collumn != 0)
            if ((game.collumns[collumn - 1, row] != null) && (!rareTiles.Contains(game.collumns[collumn - 1, row])))
                if (game.collumns[collumn - 1, row].tag == "Rare")
                    rareTiles.Add(game.collumns[collumn - 1, row]);
    }

    IEnumerator ProcessFall(int fallTiles)
    {
        for (int i = 0; i < fallTiles * game.fallFrames; i++) {
            yield return new WaitForEndOfFrame();
            transform.Translate(Vector3.down / game.fallFrames);
        }
    }

    protected void FallAfterEvent()
    {
        if ((row != 0) && (!game.detonatingTiles.Contains(gameObject)) && (!game.blastedTiles.Contains(gameObject)))
        {
            if (!game.fallingTiles.Contains(gameObject))
                game.fallingTiles.Add(gameObject);
            if ((game.blastedTiles.Contains(game.collumns[collumn, row - 1])) || (game.detonatingTiles.Contains(game.collumns[collumn, row - 1])) || (game.collumns[collumn, row - 1] == null))
            {
                game.collumns[collumn, row] = null;
                game.collumns[collumn, row - 1] = gameObject;
                if ((row != 7) && (game.collumns[collumn, row + 1] != null))
                    game.collumns[collumn, row + 1].GetComponent<CommonTile>().FallAfterEvent();
                row--;
                FallAfterEvent();
            }
        }
    }

    public virtual void Detonate()
    {
        if (!game.detonatingTiles.Contains(gameObject))
        {
            game.score++;
            game.scoreText.text = "Score: " + game.score;
        }
        if(!game.blastedTiles.Contains(gameObject))
            Destroy(gameObject);
    }
}
