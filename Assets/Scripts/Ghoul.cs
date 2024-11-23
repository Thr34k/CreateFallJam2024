using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghoul : MonoBehaviour
{
    #region events
    public event Action<GameObject> GhouldDied;
    #endregion

    #region Necessary References
    [Header("Necessary References")]
    public Grid grid;
    public GameObject gridObject;
    public float distanceToNextPosition = 0f;
    #endregion

    #region Test variables
    [Header("Test variables")]
    public GameObject testNextPositionMarker;
    public GameObject finalTarget;
    public BoundsInt bounds;
    public Vector3 currentPosition;
    public Vector3Int chosenNextPosition = Vector3Int.zero;
    public List<Vector3Int> vector3Ints = new List<Vector3Int>();

    bool debug = false;
    GUIStyle gui = new GUIStyle();


    public float distanceToFinalTarget;
    #endregion

    #region AI variables
    [Header("AI variables")]
    public float xpReward = 5f;
    public float movementSpeed = .5f;
    public int startingHealth = 1;
    int remainingHealth;
    public int damage = 10;
    public float timeBetweenAttacks = 2.5f;
    public float remainingTimeToAttack;
    public bool isAttacking = false;

    public PlayerUnitCore lastAttacker;

    #endregion

    void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //currentPosition = grid.WorldToCell(transform.position);
        if (finalTarget != null)
        {
            distanceToFinalTarget = Vector3.Distance(transform.position, finalTarget.transform.position);
            distanceToNextPosition = Vector3.Distance(transform.position, testNextPositionMarker.transform.position);
            if (distanceToNextPosition >= 0f)
            {
                FindNextMovePosition();
            }
            if (distanceToFinalTarget > 0.6f)
            {
                transform.position = Vector2.MoveTowards(transform.position, testNextPositionMarker.transform.position, movementSpeed * Time.deltaTime);

                var targetDirection = finalTarget.transform.position - transform.position;
                transform.up = Vector2.MoveTowards(transform.up, targetDirection, 5 * Time.deltaTime);
            }
            else
            {
                if (!isAttacking && finalTarget.GetComponent<PlayerUnitCore>() != null) 
                { 
                    StartCoroutine("StartAttacking");
                }
            }
        }
        else 
        {
            FindNewTarget();
        }

    }

    void OnGUI()
    {
        if (debug)
        {
            gui.fontSize = 25;
            gui.normal.textColor = Color.cyan;
            GUI.BeginGroup(new Rect(10, 10, 600, 500));
            GUI.Label(new Rect(0, 200, 500, 30), $"Next position: {chosenNextPosition}", gui);
            GUI.Label(new Rect(0, 225, 500, 30), $"Current tile: {grid.WorldToCell(currentPosition)}", gui);
            GUI.Label(new Rect(0, 250, 500, 30), $"Final target position: {finalTarget.transform.position}", gui);
            GUI.Label(new Rect(0, 275, 500, 30), $"Final target distance: {distanceToFinalTarget}", gui);
            GUI.Label(new Rect(0, 300, 500, 30), $"Computed positions:", gui);
            int offSet = 0;
            foreach (var vec in vector3Ints) 
            {
                offSet += 20;
                GUI.Label(new Rect(0, 300+offSet, 500, 30), $"{vec}", gui);  
            }
            GUI.EndGroup();
        }
    }

    private void FindNewTarget()
    {
        var viableTargets = GameObject.FindGameObjectsWithTag("PlayerUnit").ToList();
        GameObject currentChosenTarget = null;
        if (!viableTargets.Any())
        {
            currentChosenTarget = GameObject.FindGameObjectWithTag("Nexus");
            finalTarget = currentChosenTarget;
        }
        else 
        { 
            float shortestTargetDistance = float.MaxValue;
            foreach (var target in viableTargets) 
            {
                float targetDistance = Vector3.Distance(transform.position, target.transform.position);
                if (targetDistance < shortestTargetDistance) 
                { 
                    shortestTargetDistance = targetDistance;
                    currentChosenTarget = target;
                }
            }
            
            finalTarget = currentChosenTarget;
        }
    }

    private void Init()
    {
        finalTarget = GameObject.FindGameObjectWithTag("PlayerUnit");
        gridObject = GameObject.Find("LevelRoot");
        testNextPositionMarker = transform.GetChild(0).gameObject;
        bounds = gridObject.GetComponent<Tilemap>().cellBounds;
        grid = gridObject.GetComponent<Grid>();
        remainingHealth = startingHealth;
        remainingTimeToAttack = timeBetweenAttacks;
    }

    public void TakeDamage(int damageToTake, PlayerUnitCore attacker) 
    {
        lastAttacker = attacker;
        remainingHealth -= damageToTake;
        if (remainingHealth <= 0) 
        {
            Die();
        }
    }

    IEnumerator StartAttacking() 
    {
        isAttacking = true;
        while (remainingTimeToAttack > 0) 
        { 
            remainingTimeToAttack -= Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        if (remainingTimeToAttack <= 0)
        {
            finalTarget.GetComponent<PlayerUnitCore>().TakeDamage(damage);
            remainingTimeToAttack = timeBetweenAttacks;
            isAttacking = false;
        }

    }

    public void Die()
    {
        if (lastAttacker != null)
        {
            lastAttacker.GainXp(xpReward);
        }
        GhouldDied?.Invoke(this.gameObject);
        GameManager.Instance.GhoulDeatchCounter();

        GhouldDied = null;
        Destroy(gameObject);
    }

    void Move() 
    { 
        
    }

    void FindNextMovePosition() 
    {
        currentPosition = transform.position;
        Vector3Int currentTile = grid.WorldToCell(currentPosition);
        List<Vector3Int> viablePositions = new List<Vector3Int>();

        viablePositions.Add(new Vector3Int(currentTile.x, currentTile.y+1, currentTile.z));
        viablePositions.Add(new Vector3Int(currentTile.x, currentTile.y-1, currentTile.z));
        viablePositions.Add(new Vector3Int(currentTile.x+1, currentTile.y, currentTile.z));
        viablePositions.Add(new Vector3Int(currentTile.x-1, currentTile.y, currentTile.z));

        vector3Ints = viablePositions;

        float shortestDistance = float.MaxValue;
        foreach (var position in viablePositions) 
        {
            float distance = Vector3.Distance(grid.CellToWorld(position), finalTarget.transform.position);
            if (distance < shortestDistance) 
            { 
                shortestDistance = distance;
                chosenNextPosition = position;
            }
        }

        testNextPositionMarker.transform.position = grid.CellToWorld(chosenNextPosition);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
