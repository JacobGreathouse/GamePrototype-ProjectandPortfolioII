using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Credits : MonoBehaviour
{
    [SerializeField] string path = "Assets/Levels/Scripts/Credits.txt";
    [SerializeField] float scrollSpeed;

    [SerializeField] Font m_Font;
    [SerializeField] int headerSize;
    [SerializeField] int nameSize;
    [SerializeField] Color m_Color;
    [SerializeField] int textDiv;

    List<string> headers = new List<string>();
    List<List<string>> names = new List<List<string>>();
    List<GameObject> creditsText = new List<GameObject>();


    public void Awake()
    {
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

    public void Start()
    {
        Vector3 lastPosition = new Vector3(Screen.width * 0.5f, 0, 0);
        for (int i = 0; i < headers.Count; i++)
        {
            GameObject newObj = newText(headers[i], true);
            Vector3 nextPosition = new Vector3(Screen.width * 0.5f, lastPosition.y - (Screen.height / textDiv), 0);
            newObj.transform.position = nextPosition;
            lastPosition = nextPosition;
            creditsText.Add(newObj);
            for (int j = 0; j < names.Count; j++)
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

    private void Update()
    {
        for (int i = 0; i < creditsText.Count; i++)
        {
            if (creditsText[i] != null)
            {
                creditsText[i].transform.position = new Vector3(creditsText[i].transform.position.x, creditsText[i].transform.position.y + scrollSpeed, 0);
                if (creditsText[i].transform.position.y > Screen.height * 2)
                {
                    Destroy(creditsText[i]);
                    creditsText[i] = null;
                }
            }
        }

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
