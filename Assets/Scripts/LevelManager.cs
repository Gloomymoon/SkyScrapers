using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    public static LevelManager Instance
    {
        get {
            if (instance == null){
                instance = FindObjectOfType<LevelManager>();
            }
            return instance;
        }
    }

}
