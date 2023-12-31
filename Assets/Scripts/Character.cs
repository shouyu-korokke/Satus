using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string name = "Nameless";
    public GameObject characterPrefab;
    //public GameObject 2Dsprite;
    public int attack_range = 2;
    public int attack_damage = 4;
    public int max_health = 15;
    public int movement_range = 6;


    //hidden
    public int luck;
    public bool extra_action;


    //WIP
    //public int armor;
    //public 

    
}
