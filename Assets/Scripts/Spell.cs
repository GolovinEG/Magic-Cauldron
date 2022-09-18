using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public GameObject aimBoxSample;
    private GameManager game;

    private void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void CastSpell()
    {
        GameObject aimBox = Instantiate<GameObject>(aimBoxSample);
        game.activeAimBox = aimBox;
    }
}
