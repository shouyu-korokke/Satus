
        // float horizontal = Input.GetAxis("Horizontal");
        // float vertical = Input.GetAxis("Vertical");
        // Vector3 dir = new Vector3(horizontal,0,vertical);
        // transform.rotation = Quaternion.LookRotation(dir);
        // transform.Translate(Vector3.forward * 2 * Time.deltaTime);      
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMove : MonoBehaviour
{
    private Animator _clickCharacter;
    // Start is called before the first frame update
    void Start()
    {
        _clickCharacter = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _clickCharacter.Play("runForward");
        }
    }
}

