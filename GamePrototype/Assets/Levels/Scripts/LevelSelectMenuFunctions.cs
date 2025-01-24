using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenuFunctions : MonoBehaviour
{

    public void WarpLevel(int index)
    {
        gameObject.SetActive(false);
        gamemanager.instance.stateUnpause();
        gamemanager.instance.LoadMap(index);


    }

    public void ButtonExit()
    {
        // close the panel
        gameObject.SetActive(false);
    }
}
