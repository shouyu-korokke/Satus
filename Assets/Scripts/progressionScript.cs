using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class progressionScript : MonoBehaviour
{
    public static progressionScript instance;
    // Start is called before the first frame update
    
    public int numCombat;
    public Character c1;
    public Character c2;
    public Character c3;

    //cap at 100
    public int generalAdaptation = 0;
    public int lastAdaptation = 0;

    //
    public int lastcombatLoss = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void promote()
    {
        //TODO
    }

    public void modifyEnemy(UnitScript enemy)
    {
        enemy.maxHealthPoints = enemy.maxHealthPoints + generalAdaptation/20 + lastAdaptation/10 + numCombat;
        enemy.attackDamage = enemy.attackDamage + generalAdaptation/40 + lastAdaptation/20 + numCombat/2; 
        //TODO
    }

    public void adapt()
    {

        //TODO
    }

}
