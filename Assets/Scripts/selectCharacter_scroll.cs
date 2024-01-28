using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectCharacter_scroll : MonoBehaviour
{
    // Start is called before the first frame update
     //public Quaternion[] CharacterRotationPos;
     
    // [System.Serializable]
    // public struct Character
    // {
    //     public string name;
    //     // public GameObject characterPrefab;
    //     //public GameObject 2Dsprite;
    //     public Transform characterPos;
    //     public int attack_range;
    //     public int attack_damage;
    //     public int max_health;
    //     public int movement_range;
    // }
    
    public float[] rotationAngle = {120f,-120f,0f};
    public int currentCharacterIndex = 0;
    public Character[] character;
    // public Transform[] character;
    public new Transform camera;
    public Vector3 vector;

    /// ///////////////
 
    public Text name_txt;
    public Text attack_range_txt;
    public Text attack_damage_txt;
    public Text max_health_txt;
    public Text movement_range_txt;

    /// ////////////////////

    public AudioSource audioSource;
    [SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;

    void Start()
    {
         audioSource = GetComponent<AudioSource>();
    }

    public void setCurrentCharacter(int index)
    {
        currentCharacterIndex += index;
        if(currentCharacterIndex >= character.Length)
        {
            currentCharacterIndex = 0;
        }
        if(currentCharacterIndex < 0)
        {
            currentCharacterIndex = character.Length - 1;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Character[] characters = new Character[3];

        characters = getCharacterforStats();
        //showing...............................
        // name_txt.text = "Name :   "+ character[currentCharacterIndex].name.ToString();
        // attack_range_txt.text = "Attack Range :  " + character[currentCharacterIndex].attack_range.ToString() + "m";
        // attack_damage_txt.text = "Attack Damage :   " + character[currentCharacterIndex].attack_damage.ToString();
        // max_health_txt.text = "Max Health :   " + character[currentCharacterIndex].max_health.ToString() + "hp";
        // movement_range_txt.text = "Movement Range :   " + character[currentCharacterIndex].movement_range.ToString() + "m";

        name_txt.text = "Name :   "+ characters[currentCharacterIndex].name.ToString();
        attack_range_txt.text = "Attack Range :  " + characters[currentCharacterIndex].attack_range.ToString() + "m";
        attack_damage_txt.text = "Attack Damage :   " + characters[currentCharacterIndex].attack_damage.ToString();
        max_health_txt.text = "Max Health :   " + characters[currentCharacterIndex].max_health.ToString() + "hp";
        movement_range_txt.text = "Movement Range :   " + characters[currentCharacterIndex].movement_range.ToString() + "m";


        // character roll.................................
        //Transform characterPos = character[currentCharacterIndex].GetComponent<Transform>();
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,  
            Quaternion.Euler(0f, rotationAngle[currentCharacterIndex], 0f),
            0.05f);

    }

    private Character[] getCharacterforStats()
    {
        Character[] result = new Character[3];
        result[0] = progressionScript.instance.c1;
        result[1] = progressionScript.instance.c2;
        result[2] = progressionScript.instance.c3;

        return result;
    }
}
