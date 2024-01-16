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
        // int Scene = 0;
        // StartCoroutine(PlaySoundAndLoadScene(Scene));
        SceneManager.LoadScene(0); 
    }

    public void Jump_1()
    {
        // int Scene = 3;
        // StartCoroutine(PlaySoundAndLoadScene(Scene));
        SceneManager.LoadScene(3); 
    }

    public void Jump_2()
    {
        // int Scene = 4;
        // StartCoroutine(PlaySoundAndLoadScene(Scene));
        SceneManager.LoadScene(4); 
    }

    public void loadCombat1()
    {
        // int Scene = 3;
        // StartCoroutine(PlaySoundAndLoadScene(Scene));
        SceneManager.LoadScene(3); 
    }

    public void loadCombat2()
    {
        // int Scene = 4;
        // StartCoroutine(PlaySoundAndLoadScene(Scene));
        SceneManager.LoadScene(4); 
    }
    public void loadCombat3()
    {
        // int Scene = 5; 
        // StartCoroutine(PlaySoundAndLoadScene(Scene));
        SceneManager.LoadScene(5); 
    }

    // IEnumerator PlaySoundAndLoadScene(int Scene)
    // {
       
    //     yield return new WaitForSeconds(0.5f); 
    //     SceneManager.LoadScene(Scene); 
    // }
}
