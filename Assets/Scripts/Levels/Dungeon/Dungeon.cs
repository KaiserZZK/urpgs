using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Dungeon : Level
{
    // Children/Components
    public DungeonManager dungeonManager;

    [Header("Input")]
    public Vector2 moveInput;

    [Header("Turn System")]
    public bool isPlayerTurn = true;
    public int currentTurnEntity = 0;

    [Header("Game State")]
    public int time = 0;
    public int alertMax = 100;
    public int alert = 0;

    // Entities
    public List<DungeonEntity> entities = new List<DungeonEntity>();

    void OnEnable()
    {
        GameManager.instance.controls.dungeon.esc.performed += onEsc;
    }

    void OnDisable()
    {
        GameManager.instance.controls.dungeon.esc.performed -= onEsc;
    }

    void Start()
    {
        Debug.Log("Dungeon level started");
        dungeonManager.SetUpGame();
    }

    void Update()
    {
        // Read moveInput only if it is the player's turn
        if (isPlayerTurn)
        {
            moveInput = GameManager.instance.controls.dungeon.move.ReadValue<Vector2>();
            if (moveInput != Vector2.zero)
            {
                if (moveInput.x == 0 || moveInput.y == 0) // We do not want to move if the player is trying to move diagonally
                {
                    Vector2Int moveInputV2Int = new Vector2Int((int)moveInput.x, (int)moveInput.y);
                    TryPlayerMove(moveInputV2Int);
                }
            }
        }
    }

    void TryPlayerMove(Vector2Int input)
    {
        if (isPlayerTurn)
        {
            Debug.Log("Player tried to move: " + input);
            Vector2Int playerPos = new Vector2Int((int)entities[0].transform.position.x, (int)entities[0].transform.position.y);
            Vector2Int targetPos = playerPos + input;
            if (dungeonManager.IsValidMove(targetPos))
            {
                DungeonActions.Move(this, entities[0], input);
                EndTurn();
                Debug.Log("Player moved to: " + entities[0].transform.position);
            }
            else
            {
                Debug.Log("Player tried to move to an invalid position");
            }
        }
    }

    void onEsc(InputAction.CallbackContext ctx)
    {
        Debug.Log("Esc pressed");
    }

    void StartTurn()
    {
        Debug.Log("Starting turn for entity: " + entities[currentTurnEntity].name);
        if (entities[currentTurnEntity].GetComponent<DungeonPlayer>())
        {
            isPlayerTurn = true;
            CenterCameraOnPlayer();
        }
        else if (entities[currentTurnEntity].isSentient)
        {
            DungeonActions.Wait(this, entities[currentTurnEntity]);
        }
    }

    public void EndTurn()
    {
        Debug.Log("Ending turn for entity: " + entities[currentTurnEntity].name);
        if (currentTurnEntity == entities.Count - 1)
        {
            currentTurnEntity = 0;
        }
        else
        {
            currentTurnEntity++;
        }
        time++;

        // Rerender
        dungeonManager.RefreshTiles();
        if (entities[currentTurnEntity] is DungeonPlayer)
        {
            DungeonPlayer player = (DungeonPlayer)entities[currentTurnEntity];
            isPlayerTurn = false;

            CenterCameraOnPlayer();
            dungeonManager.RevealMapAroundPoint(
                new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y),
                player.visionRange);
            dungeonManager.PlayerVisionAroundPoint(
                new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y),
                player.visionRange);

            // Generate alert level
            alert += 10;
        }
        StartCoroutine(TurnDelay());
    }
    public IEnumerator TurnDelay()
    {
        yield return new WaitForSeconds(0.1f);
        StartTurn();
    }

    public void CenterCameraOnPlayer()
    {
        Vector2 playerPos = entities[currentTurnEntity].transform.position;
        Camera.main.transform.position = new Vector3(playerPos.x, playerPos.y, Camera.main.transform.position.z);
    }
}
