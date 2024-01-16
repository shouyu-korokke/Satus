using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jump()
    {
        StartCoroutine(PlaySoundAndLoadScene());
        // SceneManager.LoadScene(1);
        
    }

    IEnumerator PlaySoundAndLoadScene()
    {
       
        // animatorFunctions.disableOnce = false;
        yield return new WaitForSeconds(0.6f); // 等待音效播放完毕
        SceneManager.LoadScene(1); // 加载新场景
    }
}
