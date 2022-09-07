using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int fallFrames;
    public GameObject selectionBoxSample;

    private static int row;
    private GameManager game;
    // Start is called before the first frame update
    private void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        Fall();
    }
    private void OnMouseDown()
    {
        if (game.isSelectionActive)
        {
            //Undo selection
            game.isSelectionActive = false;
            Destroy(GetSelectionBox());
            game.selectedTiles = 0;

        } else
        {
            game.isSelectionActive = true;
            GameObject selectionBox = Instantiate<GameObject>(selectionBoxSample, transform.position, selectionBoxSample.transform.rotation);
            game.selectedTiles++;
        }
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) && game.isSelectionActive && CheckSameRow())
        {
            GetSelectionBox().transform.Translate(Vector3.right * CheckToTheRight() * 0.5f);
            game.selectedTiles++;
            GetSelectionBox().transform.localScale = new Vector3(0.1f + game.selectedTiles, 1.1f, 1.1f);
        }
    }

    public void Fall()
    {
        int fallTiles = 11 - row;
        StartCoroutine(ProcessFall(fallTiles));
    }

    public void SetRow(int number)
    {
        row = number;
    }

    private bool CheckSameRow()
    {
        bool isSameRow = Mathf.Approximately(gameObject.transform.position.y, GetSelectionBox().transform.position.y);
        return isSameRow;
    }

    private int CheckToTheRight()
    {
        if (transform.position.x > GetSelectionBox().transform.position.x) return 1;
        else return -1;
    }

    private GameObject GetSelectionBox()
    {
        GameObject selectionBox = GameObject.Find("SelectionBox(Clone)");
        return selectionBox;
    }

    IEnumerator ProcessFall(int fallTiles)
    {
        for (int i = 0; i < fallTiles * fallFrames; i++) {
            yield return new WaitForEndOfFrame();
            transform.Translate(Vector3.down / fallFrames);
        }
    }
}
