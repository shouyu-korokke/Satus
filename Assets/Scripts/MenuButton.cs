using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] AnimatorFunctions animatorFunctions;
    [SerializeField] int thisIndex;
    [SerializeField] string sceneToLoad;
    public GameObject Options;
    // public AudioSource whichSound;

    // Update is called once per frame


    void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (Input.GetAxis("Submit") == 1)
            {
                animator.SetBool("pressed", true);
                if (thisIndex == 1)
                {

                    StartCoroutine(PlaySoundAndLoadScene()); // 使用协程来播放音效并加载场景
                    //SceneManager.LoadScene("CharacterList"); 

                }
                if (thisIndex == 2)
                {
                    Application.Quit();
                }
            }
            else if (animator.GetBool("pressed"))
            {
                animator.SetBool("pressed", false);
                animatorFunctions.disableOnce = true;
            }
        }
        else
        {
            animator.SetBool("selected", false);
        }

        // void OnPointerClick(PointerEventData eventData)
        // {
        //     if (menuButtonController.index == thisIndex)
        //     {
        //         animator.SetBool("pressed", true);
        //         StartCoroutine(PlaySoundAndLoadScene());
        //     }
        // }
    }

     IEnumerator PlaySoundAndLoadScene()
    {
       
        // animatorFunctions.disableOnce = false;
        yield return new WaitForSeconds(0.6f); // 等待音效播放完毕
        SceneManager.LoadScene("CharacterList"); // 加载新场景
    }

}
