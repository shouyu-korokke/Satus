using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class stageInitScript : MonoBehaviour
{    

    public Character[] characters = new Character[3];
    

    public void SetFriendlyStats()
    {
        // Get references to all game objects with the "PlayerUnit" tag
        GameObject[] playerUnitObjects = GameObject.FindGameObjectsWithTag("PlayerUnit");



        // Set the stats for each player unit
        for (int i = 0; i < playerUnitObjects.Length; i++)
        {
            UnitScript playerUnit = playerUnitObjects[i].GetComponent<UnitScript>();
            playerUnit.unitName = characters[i].name;
            playerUnit.attackDamage = characters[i].attack_damage;
            playerUnit.maxHealthPoints = characters[i].max_health;
            playerUnit.moveSpeed = characters[i].movement_range;
            playerUnit.attackRange = characters[i].attack_range;
            playerUnit.currentHealthPoints = playerUnit.maxHealthPoints;
            playerUnit.Awake();
        }

    }
}
