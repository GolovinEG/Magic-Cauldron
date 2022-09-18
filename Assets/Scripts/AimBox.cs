using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBox : MonoBehaviour
{
    public float xShift, yShift;

    protected int collumn = 0;
    protected int row = 0;
    protected GameManager game;

    protected void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    protected void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CancelSpell();
        }
    }

    protected void OnMouseDown()
    {
        if (GetComponent<MeshRenderer>().enabled)
        {
            CastSpell();
            CancelSpell();
        }
    }

    protected virtual void CastSpell()
    {
        collumn = Mathf.RoundToInt(transform.position.x + 3.5f - xShift);
        row = Mathf.RoundToInt(transform.position.y + 1.5f - yShift);
    }

    protected void CancelSpell()
    {
        Destroy(game.activeAimBox);
        game.activeAimBox = null;
    }
}
