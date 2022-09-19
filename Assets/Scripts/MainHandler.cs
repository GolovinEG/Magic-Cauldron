using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(0);
#endif
    }
}
