using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameJump : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Jump_0()
    {
        SceneManager.LoadScene(0);
    }

    public void Jump_1()
    {
        SceneManager.LoadScene(3);
    }

    public void Jump_2()
    {
        SceneManager.LoadScene(4);
    }
}
