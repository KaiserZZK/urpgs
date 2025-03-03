using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Global GameManager singleton accessible using GameManager.instance
    public static GameManager instance;

    // Input
    public Controls controls;

    // Current dungeon (null if no dungeon is active)
    public Dungeon dungeon;

    // Sequential progression
    public bool collectedNotebook = false;
    public bool checkedTerminal = false;
    
    void Awake()
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

    void Start()
    {
        controls = new Controls();
        controls.Enable();

        // StartGame();
    }

    public void StartGame()
    {
        dungeon = Instantiate(Resources.Load("Prefabs/Dungeon")).GetComponent<Dungeon>();
        dungeon.name = "Dungeon";
    }
}
