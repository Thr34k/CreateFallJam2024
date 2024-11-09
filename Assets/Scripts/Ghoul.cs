using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghoul : MonoBehaviour
{
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

    bool debug = false;

    public float distanceToFinalTarget;
    #endregion

    #region AI variables
    [Header("AI variables")]
    public float movementSpeed = .5f;
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
        currentPosition = grid.WorldToCell(transform.position);
        distanceToFinalTarget = Vector3.Distance(transform.position, finalTarget.transform.position);
        distanceToNextPosition = Vector3.Distance(transform.position, testNextPositionMarker.transform.position);
        if (distanceToNextPosition >= 0f) 
        { 
            FindNextMovePosition();
        }

        if (distanceToFinalTarget > 0.1f) 
        { 
            transform.position = Vector2.MoveTowards(transform.position, testNextPositionMarker.transform.position, movementSpeed * Time.deltaTime);
        }
    }

    private void Init()
    {
        finalTarget = GameObject.FindGameObjectWithTag("PlayerUnit");
        gridObject = GameObject.Find("LevelRoot");
        testNextPositionMarker = GameObject.Find("TestNextPositionMarker");
        bounds = gridObject.GetComponent<Tilemap>().cellBounds;
        grid = gridObject.GetComponent<Grid>();
    }

    void Move() 
    { 
        
    }

    void FindNextMovePosition() 
    { 
        Vector3Int currentTile = grid.WorldToCell(currentPosition);
        List<Vector3Int> viablePositions = new List<Vector3Int>();

        viablePositions.Add(new Vector3Int(currentTile.x, currentTile.y+1, currentTile.z));
        viablePositions.Add(new Vector3Int(currentTile.x, currentTile.y-1, currentTile.z));
        viablePositions.Add(new Vector3Int(currentTile.x+1, currentTile.y, currentTile.z));
        viablePositions.Add(new Vector3Int(currentTile.x-1, currentTile.y, currentTile.z));

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


}
