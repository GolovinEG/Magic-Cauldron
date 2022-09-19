using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameData : MonoBehaviour
{
    public TMPro.TMP_Text hiScoreText;
    public TMPro.TMP_InputField nameField;
    public static NameData Instance { get; private set; }

    private static string playerName;
    private static bool areSeperatorsRequired = false;

    [System.Serializable]
    public class Record
    {
        public int hiScore;
        public string playerName;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetName()
    {
        //int hiScore = 0;
        //string[] split;
        playerName = nameField.text;
        /*if (File.Exists(Application.persistentDataPath + "/Hiscore.json"))
        {
            split = File.ReadAllText(Application.persistentDataPath + "/Hiscore.json").Split('#');
            foreach (string json in split)
            {
                Record record = JsonUtility.FromJson<Record>(json);
                if (playerName == record.playerName)
                {
                    hiScore = record.hiScore;
                    break;
                }
            }
        }
        hiScoreText.text = "Name: " + playerName + " Hi-Score: " + hiScore;*/
    }

    public string GetName()
    {
        return playerName;
    }

    public bool CheckSeperatorRequirement()
    {
        return areSeperatorsRequired;
    }
}
