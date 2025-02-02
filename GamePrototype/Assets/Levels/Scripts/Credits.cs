using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.ProBuilder.MeshOperations;

public class Credits : MonoBehaviour
{
    
    [SerializeField] float scrollSpeed;
    [SerializeField] Font m_Font;
    [SerializeField] int headerSize;
    [SerializeField] int nameSize;
    [SerializeField] Color m_Color;
    [SerializeField] int textDiv;

    List<string> headers = new List<string>();
    List<List<string>> names = new List<List<string>>();
    List<GameObject> creditsText = new List<GameObject>();

    [SerializeField] GameObject CreditsScreen;
    [SerializeField] GameObject MainMenuScreen;
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] Button creditsButton;

    bool buttonPressed;
    string path = "Assets/Levels/Scripts/Credits.txt";
    

    public void Awake()
    {

        if (!File.Exists(path))
            path = Path.Combine(Application.streamingAssetsPath, "Credits.txt");

        string tempPath = Application.dataPath;
        //Debug.Log("dataPath : " + tempPath);

        StreamReader reader = new StreamReader(path);

        string line = "";
        bool newStart = false;
        while ((line = reader.ReadLine()) != null)
        {
            string firstChar = line.Substring(0, 1);
            bool isHeader = firstChar.Equals("!");
            if (isHeader)
            {
                newStart = true;
                headers.Add(line.Substring(1));
            }
            else
            {
                if (newStart)
                {
                    names.Add(new List<string>());
                    newStart = false;
                }
                names[names.Count - 1].Add(line);
            }
        }
        reader.Close();
    }

    public void rePopulate()
    {
        Vector3 lastPosition = new Vector3(Screen.width * 0.5f, 0, 0);
        for (int i = 0; i < headers.Count; i++)
        {
            GameObject newObj = newText(headers[i], true);
            Vector3 nextPosition = new Vector3(Screen.width * 0.5f, lastPosition.y - (Screen.height / textDiv), 0);
            newObj.transform.position = nextPosition;
            lastPosition = nextPosition;
            creditsText.Add(newObj);
            for (int j = 0; j < names[i].Count; j++)
            {
                nextPosition = new Vector3(Screen.width * 0.5f, lastPosition.y - (Screen.height / textDiv), 0);

                if (names[i][j] != null)
                {
                    GameObject otherObj = newText(names[i][j], false);
                    otherObj.transform.position = nextPosition;
                    creditsText.Add(otherObj);
                    lastPosition = nextPosition;
                }
            }
        }
    }

    public void Start()
    {
        rePopulate();
    }

    private void Update()
    {
        //if button click, rollcredits
        if (buttonPressed)
            rollCredits();

        if (Input.GetButtonDown("Cancel"))
        {
            for (int i = 0; i < creditsText.Count; i++)
            {
                if (creditsText[i] != null)
                {
                    Destroy(creditsText[i]);
                    creditsText[i] = null;
                }
            }
            SettingsMenu.SetActive(false);
            MainMenuScreen.SetActive(true);
            CreditsScreen.SetActive(false);
        }
    }

    public void CreditsButton()
    {
        buttonPressed = true;
    }

    public void rollCredits()
    {
        for (int i = 0; i < creditsText.Count; i++)
        {
            if (creditsText[i] != null)
            {
                creditsText[i].transform.position = new Vector3(creditsText[i].transform.position.x, creditsText[i].transform.position.y + scrollSpeed, 0);
                if (creditsText[i].transform.position.y > Screen.height * 1.2)
                {
                    Destroy(creditsText[i]);
                    creditsText[i] = null;
                }
            }
        }

        if (!creditsText[creditsText.Count - 1])
        {

            buttonPressed = false;
            //StartCoroutine(CreditsFinish());
            CreditsScreen.SetActive(false);
            MainMenuScreen.SetActive(true);
            rePopulate();

        }
    }

    IEnumerator CreditsFinish()
    {
        yield return new WaitForSeconds(1.0f);

        
    }

    public GameObject newText(string labelText, bool isHeader)
    {
        GameObject textObj = new GameObject(labelText);
        textObj.transform.SetParent(this.transform);
        Text myText;
        myText = textObj.AddComponent<Text>();
        myText.text = labelText;
        myText.font = m_Font;
        myText.horizontalOverflow = HorizontalWrapMode.Overflow;
        myText.alignment = TextAnchor.MiddleCenter;
        myText.color = m_Color;

        if(isHeader)
        {
            myText.fontStyle = FontStyle.Bold;
            myText.fontSize = headerSize;            
        }
        else
        {
            myText.fontSize = nameSize;
        }
        textObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        textObj.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.25f, 0);

        return textObj;
    }



}
