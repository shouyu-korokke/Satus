using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameSceneManager : MonoBehaviour
{
    
    public Canvas mainCanvas;
    public Canvas helpCanvas;
    public Canvas helpCanvas2;
    
    public void loadMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void loadLevelOne()
    {
        SceneManager.LoadScene(1);
    }

     public void loadCharacterList()
    {
        SceneManager.LoadScene(2);
    }


    public void loadHelpCanvas()
    {
        mainCanvas.enabled = false;
        helpCanvas.enabled = true;
    }

    public void loadHelpCanvasBack()
    {
        helpCanvas2.enabled = false;
        helpCanvas.enabled = true;
    }

    public void loadHelpCanvas2()
    {
        helpCanvas.enabled = false;
        helpCanvas2.enabled = true;
    }

    public void loadMainCanvas()
    {
        mainCanvas.enabled = true;
        helpCanvas.enabled = false;
        helpCanvas2.enabled = false;
    }
}
