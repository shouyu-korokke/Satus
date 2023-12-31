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

    public int generalAdaptation;
    public int recentAdaptation;


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

}
