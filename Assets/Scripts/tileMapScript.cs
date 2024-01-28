﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class tileMapScript : MonoBehaviour    
{
    //Reference holders for the other two scripts that are currently running
    //alongside this script
    [Header("Manager Scripts")]
    public battleManagerScript BMS;
    public gameManagerScript GMS;
    public stageInitScript SIS;

    //List of tiles that are used to generate the map
    //Try chaging tilesTypes to enum later   
    [Header("Tiles")]
    public Tile[] tileTypes;
    public int[,] tiles;

    //This is used when the game starts and there are pre-existing units
    //It uses this variable to check if there are any units and then maps them to the proper tiles
    [Header("Units on the board")]
    public GameObject unitsOnBoard;

    //This 2d array is the list of tile gameObjects on the board
    public GameObject[,] tilesOnMap;

    //This 2d array is the list of quadUI gameObjects on the board
    public GameObject[,] quadOnMap;
    public GameObject[,] quadOnMapForUnitMovementDisplay;
    public GameObject[,] quadOnMapCursor;
    
    //public is only to set them in the inspector, if you change these to private then you will
    //need to re-enable them in the inspector
    //Game object that is used to overlay onto the tiles to show possible movement
    public GameObject mapUI;
    //Game object that is used to highlight the mouse location
    public GameObject mapCursorUI;
    //Game object that is used to highlight the path the unit is taking
    public GameObject mapUnitMovementUI;

    //Nodes along the path of shortest path from the pathfinding
    public List<Node> currentPath = null;

    //Node graph for pathfinding purposes
    public Node[,] graph;

    //containers (parent gameObjects) for the UI tiles
    [Header("Containers")]
    public GameObject tileContainer;
    public GameObject UIQuadPotentialMovesContainer;
    public GameObject UIQuadCursorContainer;
    public GameObject UIUnitMovementPathContainer;
    

    //Set in the inspector, might change this otherwise.
    //This is the map size (please put positive numbers it probably wont work well with negative numbers)
    [Header("Board Size")]
    public int mapSizeX;
    public int mapSizeY;

    //In the update() function mouse down raycast sets this unit
    [Header("Selected Unit Info")]
    public GameObject selectedUnit;
    //These two are set in the highlightUnitRange() function
    //They are used for other things as well, mainly to check for movement, or finalize function
    public HashSet<Node> selectedUnitTotalRange;
    public HashSet<Node> selectedUnitMoveRange;

    public bool unitSelected = false;

    public int unitSelectedPreviousX;
    public int unitSelectedPreviousY;

    public GameObject previousOccupiedTile;


    //public AudioSource selectedSound;
    //public AudioSource unselectedSound;
    //public area to set the material for the quad material for UI purposes
    [Header("Materials")]
    public Material greenUIMat;
    public Material redUIMat;
    public Material blueUIMat;


    [Header("EnemyAction")]
    private bool ExecutingEnemyAI = false;


    private void Start()
    {
        //Get the battlemanager running
        //BMS = GetComponent<battleManagerScript>();
        //GMS = GetComponent<gameManagerScript>();
        //Generate the map info that will be used
        generateMapInfo();
        //Generate pathfinding graph
        generatePathFindingGraph();
        //With the generated info this function will read the info and produce the map
        generateMapVisuals();
        //Check if there are any pre-existing units on the board
        setIfTileIsOccupied();
        //Set the friendly unit stats
        SIS.SetFriendlyStats();
        //Set the enemy unit stats
        SIS.SetEnemyStats();


    }

    private void Update()
    {
        if(GMS.currentTeam == 1 && !ExecutingEnemyAI){
            StartCoroutine(EnemyAction());
        }
        else if (GMS.currentTeam == 0)
        {
            //If input is left mouse down then select the unit
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedUnit == null)
                {
                    //mouseClickToSelectUnit();
                    mouseClickToSelectUnit();

                }
                //After a unit has been selected then if we get a mouse click, we need to check if the unit has entered the selection state (1) 'Selected'
                //Move the unit
                else if (selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(1) && selectedUnit.GetComponent<UnitScript>().movementQueue.Count == 0)
                {


                    if (selectTileToMoveTo())
                    {
                        //selectedSound.Play();
                        Debug.Log("movement path has been located");
                        unitSelectedPreviousX = selectedUnit.GetComponent<UnitScript>().x;
                        unitSelectedPreviousY = selectedUnit.GetComponent<UnitScript>().y;
                        previousOccupiedTile = selectedUnit.GetComponent<UnitScript>().tileBeingOccupied;
                        selectedUnit.GetComponent<UnitScript>().setWalkingAnimation();
                        moveUnit();

                        StartCoroutine(moveUnitAndFinalize());
                        //The moveUnit function calls a function on the unitScriptm when the movement is completed the finalization is called from that script.


                    }

                }
                //Finalize the movement
                else if (selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(2))
                {
                    finalizeOption();
                }

            }
            //Unselect unit with the right click
            if (Input.GetMouseButtonDown(1))
            {
                if (selectedUnit != null)
                {
                    if (selectedUnit.GetComponent<UnitScript>().movementQueue.Count == 0 && selectedUnit.GetComponent<UnitScript>().combatQueue.Count == 0)
                    {
                        if (selectedUnit.GetComponent<UnitScript>().unitMoveState != selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(3))
                        {
                            //unselectedSound.Play();
                            //selectedUnit.GetComponent<UnitScript>().setIdleAnimation();
                            deselectUnit();
                        }
                    }
                    else if (selectedUnit.GetComponent<UnitScript>().movementQueue.Count == 1)
                    {
                        selectedUnit.GetComponent<UnitScript>().visualMovementSpeed = 0.5f;
                    }
                }
            }


        }

    }

    public void generateMapInfo()
    {
        tiles = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }
        if (SceneManager.GetActiveScene().name == "Combat 4")
        {
            //combat 1
            
            for(int i = 0;i<mapSizeY;i++)
            {
                tiles[0,i] = 2;
            }

            tiles[1,6] = 2;
            tiles[1,7] = 2;
            tiles[2,6] =2;
            tiles[2,7] = 2;
            tiles[3,6] = 2;
            tiles[3,7] = 2;
            tiles[4,6] = 2;
            tiles[4,7] = 2;


            tiles[1,13]=2;
            tiles[2,13]=2;
            tiles[3,13]=2;
            tiles[4,13]=2;
            tiles[5,13]=2;
            tiles[6,13]=2;
            tiles[7,13] = 2;

            tiles[1,14]=2;
            tiles[2,14]=2;
            tiles[3,14]=2;
            tiles[4,14]=2;
            tiles[5,14]=2;
            tiles[6,14]=2;
            tiles[7,14] = 2;

            tiles[1,15]=2;
            tiles[2,15]=2;
            tiles[3,15]=2;
            tiles[4,15]=2;
            tiles[5,15]=2;
            tiles[6,15]=2;
            tiles[7,15] = 2; 


            tiles[1,23] = 2;
            tiles[1,24] = 2;    

            tiles[1,24] = 2;
            tiles[1,25] = 2;

            tiles[4,18] = 2;
            tiles[4,19] = 2;
            tiles[4,20] = 2;
            tiles[4,21] = 2;
            tiles[4,22] = 2;
            tiles[4,23] = 2;
            tiles[4,24] = 2;
            tiles[5,18] = 2;
            tiles[5,19] = 2;
            tiles[5,20] = 2;
            tiles[5,21] = 2;
            tiles[5,22] = 2;
            tiles[5,23] = 2;
            tiles[5,24] = 2;

            tiles[1,25] = 2;
            tiles[1,26] = 2;
            tiles[1,27] = 2;
            tiles[1,28] = 2;
            tiles[2,25] = 2;
            tiles[2,26] = 2;
            tiles[2,27] = 2;
            tiles[2,28] = 2;

            tiles[4,19] = 2;
            tiles[4,20] = 2;
            tiles[4,21] = 2;
            tiles[4,22] = 2;
            tiles[4,23] = 2;
            tiles[4,24] = 2;
            tiles[5,19] = 2;
            tiles[5,20] = 2;
            tiles[5,21] = 2;
            tiles[5,22] = 2;
            tiles[5,23] = 2;
            tiles[5,24] = 2;

            
            tiles[4,31] = 2;
            tiles[4,32] = 2;
            tiles[4,33] = 2;
            tiles[4,34] = 2;
            // tiles[4,35] = 2;
            tiles[5,31] = 2;
            tiles[5,32] = 2;
            tiles[5,33] = 2;
            tiles[5,34] = 2;
            // tiles[5,35] = 2;


            tiles[4,35] = 2;
            tiles[4,36] = 2;
            tiles[4,37] = 2;
            tiles[4,38] = 2;
            tiles[4,39] = 2;

            tiles[4,40] = 2;
            tiles[4,41] = 2;
            tiles[4,42] = 2;
            tiles[4,43] =2;
            tiles[4,44] = 2;
            
            tiles[5,40] = 2;
            tiles[5,41] = 2;
            tiles[5,42] = 2;
            tiles[5,43] = 2;
            tiles[5,44] = 2;
            


            tiles[6,43] = 2;
            tiles[6,44] = 2;
            tiles[7,43] = 2;
            tiles[7,44] = 2;

            tiles[8,41] = 2;
            tiles[8,43] = 2;
            tiles[8,44] = 2;

            tiles[9,43] = 2;
            tiles[9,44] = 2;
            tiles[10,43] = 2;
            tiles[10,44] = 2;
            tiles[11,43] = 2;
            tiles[11,44] = 2;
            tiles[12,43] = 2;
            tiles[12,44] = 2;


            tiles[14,38] = 2;
            tiles[14,39] = 2;
            tiles[14,40] = 2;
            tiles[14,41] = 2;
            tiles[14,42] = 2;
            tiles[14,43] = 2;
            tiles[14,44] = 2;


            tiles[16,34] = 2;
            tiles[16,35] = 2;
            tiles[16,36] = 2;
            tiles[16,37] = 2;
            tiles[16,38] = 2;
            tiles[16,39] = 2;
            tiles[16,40] = 2;
            tiles[16,41] = 2;
            tiles[16,42] = 2;
            tiles[17,34] = 2;
            tiles[17,35] = 2;
            tiles[17,36] = 2;
            tiles[17,37] = 2;
            tiles[17,38] = 2;
            tiles[17,39] = 2;
            tiles[17,40] = 2;
            tiles[17,41] = 2;
            tiles[17,42] = 2;

            tiles[18,34] = 2;
            tiles[18,35] = 2;
            tiles[18,36] = 2;
            tiles[18,37] = 2;
            tiles[18,38] = 2;

            tiles[19,34] = 2;
            tiles[19,35] = 2;
            tiles[19,36] = 2;
            tiles[19,37] = 2;
            tiles[19,38] = 2;

            tiles[21,34] = 2;
            tiles[21,35] = 2;
            tiles[21,36] = 2;
            tiles[22,34] = 2;
            tiles[22,35] = 2;
            tiles[22,36] = 2;
            tiles[23,34] = 2;
            tiles[23,35] = 2;
            tiles[23,36] = 2;
            tiles[24,34] = 2;
            tiles[24,35] = 2;
            tiles[24,36] = 2;



            tiles[9,5] = 2;

            tiles[8,7] = 2;
            tiles[8,8] = 2;
            tiles[8,9] = 2;
            tiles[8,10] = 2;
            tiles[9,7] = 2;
            tiles[9,8] = 2;
            tiles[9,9] = 2;
            tiles[9,10] = 2;

            // tiles[1, 3] = 2;
            // tiles[1, 4] = 2;
            // tiles[1, 5] = 2;
            // tiles[1, 6] = 2;
            // tiles[2, 3] = 2;
            // tiles[2, 4] = 2;
            // tiles[2, 5] = 2;
            // tiles[2, 6] = 2;
            // tiles[3, 3] = 2;
            // tiles[3, 4] = 2;
            // tiles[3, 5] = 2;
            // tiles[3, 6] = 2;
            // tiles[7, 2] = 2;

            // tiles[5, 8] = 2;

            // tiles[8, 1] = 1;
            // tiles[9, 1] = 1;
        }
        else if (SceneManager.GetActiveScene().name == "Combat 2")
        {
            //combat 2
            tiles[2, 3] = 2;
            tiles[3, 3] = 2;

            tiles[8, 5] = 2;
            tiles[8, 4] = 2;
            tiles[9, 5] = 2;
            tiles[9, 4] = 2;
            tiles[10, 5] = 2;
            tiles[10, 4] = 2;

            tiles[5, 3] = 2;

            tiles[11, 1] = 2;
            tiles[11, 2] = 1;
            tiles[12, 2] = 1;

            tiles[4, 7] = 1;
            tiles[5, 7] = 1;
        }
        else if (SceneManager.GetActiveScene().name == "Combat 3")
        {

            tiles[2, 4] = 2;
            tiles[2, 5] = 2;
            tiles[2, 6] = 2;

            tiles[5, 3] = 2;
            tiles[5, 4] = 2;
            tiles[5, 5] = 2;
            tiles[5, 6] = 2;
            tiles[6, 3] = 2;
            tiles[6, 4] = 2;
            tiles[6, 5] = 2;
            tiles[6, 6] = 2;
            tiles[7, 3] = 2;
            tiles[7, 4] = 2;
            tiles[7, 5] = 2;
            tiles[7, 6] = 2;
            tiles[8, 3] = 2;
            tiles[8, 4] = 2;
            tiles[8, 5] = 2;
            tiles[8, 6] = 2;

            tiles[12, 3] = 2;
            tiles[12, 4] = 2;
            tiles[12, 5] = 2;
            tiles[13, 3] = 2;
            tiles[13, 4] = 2;
            tiles[13, 5] = 2;

            tiles[13, 8] = 1;
            tiles[12, 8] = 1;
        }

    }
    //Creates the graph for the pathfinding, it sets up the neighbours
    //This is also from Quill18Create's tutorial
    public void generatePathFindingGraph()
    {
        graph = new Node[mapSizeX, mapSizeY];

        //initialize graph 
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }
        //calculate neighbours
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {              
                //X is not 0, then we can add left (x - 1)
                if (x > 0)
                {                   
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                }
                //X is not mapSizeX - 1, then we can add right (x + 1)
                if (x < mapSizeX-1)
                {                   
                    graph[x, y].neighbours.Add(graph[x + 1, y]);
                }
                //Y is not 0, then we can add downwards (y - 1 ) 
                if (y > 0)
                {
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                }
                //Y is not mapSizeY -1, then we can add upwards (y + 1)
                if (y < mapSizeY - 1)
                {
                    graph[x, y].neighbours.Add(graph[x, y + 1]);
                }
               
               
            }
        }
    }


    //In: 
    //Out: void
    //Desc: This instantiates all the information for the map, the UI Quads and the map tiles
    public void generateMapVisuals()
    {
        //generate list of actual tileGameObjects
        tilesOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMapForUnitMovementDisplay = new GameObject[mapSizeX, mapSizeY];
        quadOnMapCursor = new GameObject[mapSizeX, mapSizeY];
        int index;
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                index = tiles[x, y];
                GameObject newTile = Instantiate(tileTypes[index].tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity);
                newTile.GetComponent<ClickableTileScript>().tileX = x;
                newTile.GetComponent<ClickableTileScript>().tileY = y;
                newTile.GetComponent<ClickableTileScript>().map = this;
                newTile.transform.SetParent(tileContainer.transform);
                tilesOnMap[x, y] = newTile;

              
                GameObject gridUI = Instantiate(mapUI, new Vector3(x, 0.501f, y),Quaternion.Euler(90f,0,0));
                gridUI.transform.SetParent(UIQuadPotentialMovesContainer.transform);
                quadOnMap[x, y] = gridUI;

                GameObject gridUIForPathfindingDisplay = Instantiate(mapUnitMovementUI, new Vector3(x, 0.502f, y), Quaternion.Euler(90f, 0, 0));
                gridUIForPathfindingDisplay.transform.SetParent(UIUnitMovementPathContainer.transform);
                quadOnMapForUnitMovementDisplay[x, y] = gridUIForPathfindingDisplay;

                GameObject gridUICursor = Instantiate(mapCursorUI, new Vector3(x, 0.503f, y), Quaternion.Euler(90f, 0, 0));
                gridUICursor.transform.SetParent(UIQuadCursorContainer.transform);              
                quadOnMapCursor[x, y] = gridUICursor;

            }
        }
    }

    //Moves the unit
    public void moveUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.GetComponent<UnitScript>().MoveNextTile();
        }
    }

    //In: the x and y of a tile
    //Out: vector 3 of the tile in world space, theyre .75f off of zero
    //Desc: returns a vector 3 of the tile in world space, theyre .75f off of zero
    public Vector3 tileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, 0.75f, y);
    }



    //In: 
    //Out: void
    //Desc: sets the tile as occupied, if a unit is on the tile
    public void setIfTileIsOccupied()
    {
        foreach (Transform team in unitsOnBoard.transform)
        {
            //Debug.Log("Set if Tile is Occupied is Called");
            foreach (Transform unitOnTeam in team) { 
                int unitX = unitOnTeam.GetComponent<UnitScript>().x;
                int unitY = unitOnTeam.GetComponent<UnitScript>().y;
                unitOnTeam.GetComponent<UnitScript>().tileBeingOccupied = tilesOnMap[unitX, unitY];
                tilesOnMap[unitX, unitY].GetComponent<ClickableTileScript>().unitOnTile = unitOnTeam.gameObject;
            }
            
        }
    }
    //In: x and y position of the tile to move to
    //Out: void
    //Desc: generates the path for the selected unit
    //Think this one is also partially from Quill18Create's tutorial
    public void generatePathTo(int x, int y)
    {

        if (selectedUnit.GetComponent<UnitScript>().x == x && selectedUnit.GetComponent<UnitScript>().y == y){
            Debug.Log("clicked the same tile that the unit is standing on");
            currentPath = new List<Node>();
            selectedUnit.GetComponent<UnitScript>().path = currentPath;
            
            return;
        }
        if (unitCanEnterTile(x, y) == false)
        {
            //cant move into something so we can probably just return
            //cant set this endpoint as our goal

            return;
        }

        selectedUnit.GetComponent<UnitScript>().path = null;
        currentPath = null;
        //from wiki dijkstra's
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        Node source = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];
        Node target = graph[x, y];
        dist[source] = 0;
        prev[source] = null;
        //Unchecked nodes
        List<Node> unvisited = new List<Node>();

        //Initialize
        foreach (Node n in graph)
        {

            //Initialize to infite distance as we don't know the answer
            //Also some places are infinity
            if (n != source)
            {
                dist[n] = Mathf.Infinity;
                prev[n] = null;
            }
            unvisited.Add(n);
        }
        //if there is a node in the unvisited list lets check it
        while (unvisited.Count > 0)
        {
            //u will be the unvisited node with the shortest distance
            Node u = null;
            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }


            if (u == target)
            {
                break;
            }

            unvisited.Remove(u);

            foreach (Node n in u.neighbours)
            {

                //float alt = dist[u] + u.DistanceTo(n);
                float alt = dist[u] + costToEnterTile(n.x, n.y);
                if (alt < dist[n])
                {
                    dist[n] = alt;
                    prev[n] = u;
                }
            }
        }
        //if were here we found shortest path, or no path exists
        if (prev[target] == null)
        {
            //No route;
            return;
        }
        currentPath = new List<Node>();
        Node curr = target;
        //Step through the current path and add it to the chain
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        //Now currPath is from target to our source, we need to reverse it from source to target.
        currentPath.Reverse();

        selectedUnit.GetComponent<UnitScript>().path = currentPath;
       



    }

    //In: tile's x and y position
    //Out: cost that is requiredd to enter the tile
    //Desc: checks the cost of the tile for a unit to enter
    public float costToEnterTile(int x, int y)
    {

        if (unitCanEnterTile(x, y) == false)
        {
            return Mathf.Infinity;

        }

        //Gotta do the math here
        Tile t = tileTypes[tiles[x, y]];
        float dist = t.movementCost;

        return dist;
    }

    //In:  tile's x and y position
    //Out: true or false if the unit can enter the tile that was entered
    //Desc: if the tile is not occupied by another team's unit, then you can walk through and if the tile is walkable 
    public bool unitCanEnterTile(int x, int y)
    {
        if (tilesOnMap[x, y].GetComponent<ClickableTileScript>().unitOnTile != null)
        {
            if (tilesOnMap[x, y].GetComponent<ClickableTileScript>().unitOnTile.GetComponent<UnitScript>().teamNum != GMS.currentTeam)
            {
                return false;
            }
        }
        return tileTypes[tiles[x, y]].isWalkable;
    }


    //In: tile's x and y position
    //Out: cost that is requiredd to enter the tile (ignore the occupant)
    //Desc: checks the cost of the tile for a unit to enter
    public float costToEnterTileIgnoreOccupant(int x, int y)
    {

        if (tileTypes[tiles[x, y]].isWalkable == false)
        {
            return Mathf.Infinity;

        }

        //Gotta do the math here
        Tile t = tileTypes[tiles[x, y]];
        float dist = t.movementCost;

        return dist;
    }

    //In:  
    //Out: void
    //Desc: uses a raycast to see where the mouse is pointing, this is used to select units
    public void mouseClickToSelectUnit()
    {
        GameObject tempSelectedUnit;
        
        RaycastHit hit;       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        
       
        if (Physics.Raycast(ray, out hit))
        {


            //Debug.Log(hit.transform.tag);
            if (unitSelected == false)
            {
               
                if (hit.transform.gameObject.CompareTag("Tile"))
                {
                    if (hit.transform.GetComponent<ClickableTileScript>().unitOnTile != null)
                    {


                        tempSelectedUnit = hit.transform.GetComponent<ClickableTileScript>().unitOnTile;
                        if (tempSelectedUnit.GetComponent<UnitScript>().unitMoveState == tempSelectedUnit.GetComponent<UnitScript>().getMovementStateEnum(0)
                            && tempSelectedUnit.GetComponent<UnitScript>().teamNum == GMS.currentTeam
                            )
                        {
                            disableHighlightUnitRange();
                            selectedUnit = tempSelectedUnit;
                            selectedUnit.GetComponent<UnitScript>().map = this;
                            selectedUnit.GetComponent<UnitScript>().setMovementState(1);
                            selectedUnit.GetComponent<UnitScript>().setSelectedAnimation();//----------

                            unitSelected = true;
                            
                            highlightUnitRange();
                        }
                    }
                }

                else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit"))
                {
                    
                    tempSelectedUnit = hit.transform.parent.gameObject;
                    if (tempSelectedUnit.GetComponent<UnitScript>().unitMoveState == tempSelectedUnit.GetComponent<UnitScript>().getMovementStateEnum(0)
                          && tempSelectedUnit.GetComponent<UnitScript>().teamNum == GMS.currentTeam
                        )
                    {

                        disableHighlightUnitRange();
                        selectedUnit = tempSelectedUnit;
                        selectedUnit.GetComponent<UnitScript>().setMovementState(1);
                        selectedUnit.GetComponent<UnitScript>().setSelectedAnimation(); //--------------
                        selectedUnit.GetComponent<UnitScript>().map = this;
                        unitSelected = true;
                       
                        highlightUnitRange();
                    }
                }
            }

         }
    }



    //In:  
    //Out: void
    //Desc: finalizes the movement, sets the tile the unit moved to as occupied, etc
    public void finalizeMovementPosition()
    {
        tilesOnMap[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y].GetComponent<ClickableTileScript>().unitOnTile = selectedUnit;
        //After a unit has been moved we will set the unitMoveState to (2) the 'Moved' state


        selectedUnit.GetComponent<UnitScript>().setMovementState(2);
        selectedUnit.GetComponent<UnitScript>().setWalkingAnimation();//----------------
       
        highlightUnitAttackOptionsFromPosition();
        highlightTileUnitIsOccupying();
    }



    //In:  
    //Out: void
    //Desc: selects a unit based on the cursor from the other script
    public void mouseClickToSelectUnitV2()
    {
        
        if (unitSelected == false && GMS.tileBeingDisplayed!=null)
        {

            if (GMS.tileBeingDisplayed.GetComponent<ClickableTileScript>().unitOnTile != null)
            {
                GameObject tempSelectedUnit = GMS.tileBeingDisplayed.GetComponent<ClickableTileScript>().unitOnTile;
                if (tempSelectedUnit.GetComponent<UnitScript>().unitMoveState == tempSelectedUnit.GetComponent<UnitScript>().getMovementStateEnum(0)
                               && tempSelectedUnit.GetComponent<UnitScript>().teamNum == GMS.currentTeam
                               )
                {
                    disableHighlightUnitRange();
                    //selectedSound.Play();
                    selectedUnit = tempSelectedUnit;
                    selectedUnit.GetComponent<UnitScript>().map = this;
                    selectedUnit.GetComponent<UnitScript>().setMovementState(1);
                    selectedUnit.GetComponent<UnitScript>().setSelectedAnimation();
                    unitSelected = true;
                    highlightUnitRange();
                   
                }
            }
        }
        
}
    //In:  
    //Out: void
    //Desc: finalizes the player's option, wait or attack
    public void finalizeOption()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HashSet<Node> attackableTiles = getUnitAttackOptionsFromPosition();

        if (Physics.Raycast(ray, out hit))
        {

            //This portion is to ensure that the tile has been clicked
            //If the tile has been clicked then we need to check if there is a unit on it
            if (hit.transform.gameObject.CompareTag("Tile"))
            {
                if (hit.transform.GetComponent<ClickableTileScript>().unitOnTile != null)
                {
                    GameObject unitOnTile = hit.transform.GetComponent<ClickableTileScript>().unitOnTile;
                    int unitX = unitOnTile.GetComponent<UnitScript>().x;
                    int unitY = unitOnTile.GetComponent<UnitScript>().y;

                    if (unitOnTile == selectedUnit)
                    {
                        disableHighlightUnitRange();
                        Debug.Log("ITS THE SAME UNIT JUST WAIT");
                        selectedUnit.GetComponent<UnitScript>().wait();
                        selectedUnit.GetComponent<UnitScript>().setWaitIdleAnimation();
                        selectedUnit.GetComponent<UnitScript>().setMovementState(3);
                        deselectUnit();


                    }
                    else if (unitOnTile.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum && attackableTiles.Contains(graph[unitX, unitY]))
                    {
                        if (unitOnTile.GetComponent<UnitScript>().currentHealthPoints > 0)
                        {
                            Debug.Log("We clicked an enemy that should be attacked");
                            Debug.Log(selectedUnit.GetComponent<UnitScript>().currentHealthPoints);
                            StartCoroutine(BMS.attack(selectedUnit, unitOnTile));


                            StartCoroutine(deselectAfterMovements(selectedUnit, unitOnTile));
                        }
                    }
                }
            }
            else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit"))
            {
                GameObject unitClicked = hit.transform.parent.gameObject;
                int unitX = unitClicked.GetComponent<UnitScript>().x;
                int unitY = unitClicked.GetComponent<UnitScript>().y;

                if (unitClicked == selectedUnit)
                {
                    disableHighlightUnitRange();
                    Debug.Log("ITS THE SAME UNIT JUST WAIT");
                    selectedUnit.GetComponent<UnitScript>().wait();
                    selectedUnit.GetComponent<UnitScript>().setWaitIdleAnimation();
                    selectedUnit.GetComponent<UnitScript>().setMovementState(3);
                    deselectUnit();

                }
                else if (unitClicked.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum && attackableTiles.Contains(graph[unitX, unitY]))
                {
                    if (unitClicked.GetComponent<UnitScript>().currentHealthPoints > 0)
                    {

                        Debug.Log("We clicked an enemy that should be attacked");
                        Debug.Log("Add Code to Attack enemy");
                        //selectedUnit.GetComponent<UnitScript>().setAttackAnimation();
                        StartCoroutine(BMS.attack(selectedUnit, unitClicked));

                        // selectedUnit.GetComponent<UnitScript>().wait();
                        //Check if soemone has won
                        //GMS.checkIfUnitsRemain();
                        StartCoroutine(deselectAfterMovements(selectedUnit, unitClicked));
                    }
                }

            }
        }

    }

    //In:  
    //Out: void
    //Desc: de-selects the unit
    public void deselectUnit()
    {
        
        if (selectedUnit != null)
        {
            if (selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(1))
            {
            disableHighlightUnitRange();
            disableUnitUIRoute();
            selectedUnit.GetComponent<UnitScript>().setMovementState(3);//------------0
            selectedUnit.GetComponent<UnitScript>().setWaitIdleAnimation(); //---------------

       
            selectedUnit = null;
            unitSelected = false;
            }
            else if (selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(3) )
            {
                disableHighlightUnitRange();
                disableUnitUIRoute();
                unitSelected = false;
                selectedUnit = null;
            }
            else
            {
                disableHighlightUnitRange();
                disableUnitUIRoute();
                tilesOnMap[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y].GetComponent<ClickableTileScript>().unitOnTile = null;
                tilesOnMap[unitSelectedPreviousX, unitSelectedPreviousY].GetComponent<ClickableTileScript>().unitOnTile = selectedUnit;

                selectedUnit.GetComponent<UnitScript>().x = unitSelectedPreviousX;
                selectedUnit.GetComponent<UnitScript>().y = unitSelectedPreviousY;
                selectedUnit.GetComponent<UnitScript>().tileBeingOccupied = previousOccupiedTile;
                selectedUnit.transform.position = tileCoordToWorldCoord(unitSelectedPreviousX, unitSelectedPreviousY);
                selectedUnit.GetComponent<UnitScript>().setMovementState(3);//-----------------0
                selectedUnit.GetComponent<UnitScript>().setWaitIdleAnimation();
                selectedUnit = null;
                unitSelected = false;
            }
        }
    }


    //In:  
    //Out: void
    //Desc: highlights the units range options
    public void highlightUnitRange()
    {
       
       
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        HashSet<Node> finalEnemyUnitsInMovementRange = new HashSet<Node>();
      
        int attRange = selectedUnit.GetComponent<UnitScript>().attackRange;
        int moveSpeed = selectedUnit.GetComponent<UnitScript>().moveSpeed;


        Node unitInitialNode = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];
        finalMovementHighlight = getUnitMovementOptions();
        totalAttackableTiles = getUnitTotalAttackableTiles(finalMovementHighlight, attRange, unitInitialNode);
        //Debug.Log("There are this many available tiles for the unit: "+finalMovementHighlight.Count);

        foreach (Node n in totalAttackableTiles)
        {

            if (tilesOnMap[n.x, n.y].GetComponent<ClickableTileScript>().unitOnTile != null)
            {
                GameObject unitOnCurrentlySelectedTile = tilesOnMap[n.x, n.y].GetComponent<ClickableTileScript>().unitOnTile;
                if (unitOnCurrentlySelectedTile.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum)
                {
                    finalEnemyUnitsInMovementRange.Add(n);
                }
            }
        }

        
        highlightEnemiesInRange(totalAttackableTiles);
        //highlightEnemiesInRange(finalEnemyUnitsInMovementRange);
        highlightMovementRange(finalMovementHighlight);
        //Debug.Log(finalMovementHighlight.Count);
        selectedUnitMoveRange = finalMovementHighlight;

        //This final bit sets the selected Units tiles, which can be accessible in other functions
        //Probably bad practice, but I'll need to get things to work for now (2019-09-30)
        selectedUnitTotalRange = getUnitTotalRange(finalMovementHighlight, totalAttackableTiles);
        //Debug.Log(unionTiles.Count);
        
        //Debug.Log("exiting the while loop");
        //This will for each loop will highlight the movement range of the units
       

    }


    //In:  
    //Out: void
    //Desc: disables the quads that are being used to highlight position
    public void disableUnitUIRoute()
    {
        foreach(GameObject quad in quadOnMapForUnitMovementDisplay)
        {
            if (quad.GetComponent<Renderer>().enabled == true)
            {
                
                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    //In:  
    //Out: HashSet<Node> of the tiles that can be reached by unit
    //Desc: returns the hashSet of nodes that the unit can reach from its position
    public HashSet<Node> getUnitMovementOptions()
    {
        float[,] cost = new float[mapSizeX, mapSizeY];
        HashSet<Node> UIHighlight = new HashSet<Node>();
        HashSet<Node> tempUIHighlight = new HashSet<Node>();
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();      
        int moveSpeed = selectedUnit.GetComponent<UnitScript>().moveSpeed;
        Node unitInitialNode = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];

        ///Set-up the initial costs for the neighbouring nodes
        finalMovementHighlight.Add(unitInitialNode);
        foreach (Node n in unitInitialNode.neighbours)
        {
            cost[n.x, n.y] = costToEnterTile(n.x, n.y);
            //Debug.Log(cost[n.x, n.y]);
            if (moveSpeed - cost[n.x, n.y] >= 0)
            {
                UIHighlight.Add(n);
            }
        }

        finalMovementHighlight.UnionWith(UIHighlight);

        while (UIHighlight.Count != 0)
        {
            foreach (Node n in UIHighlight)
            {
                foreach (Node neighbour in n.neighbours)
                {
                    if (!finalMovementHighlight.Contains(neighbour))
                    {
                        cost[neighbour.x, neighbour.y] = costToEnterTile(neighbour.x, neighbour.y) + cost[n.x, n.y];
                        //Debug.Log(cost[neighbour.x, neighbour.y]);
                        if (moveSpeed - cost[neighbour.x, neighbour.y] >= 0)
                        {
                            //Debug.Log(cost[neighbour.x, neighbour.y]);
                            tempUIHighlight.Add(neighbour);
                        }
                    }
                }

            }

            UIHighlight = tempUIHighlight;
            finalMovementHighlight.UnionWith(UIHighlight);
            tempUIHighlight = new HashSet<Node>();
           
        }
        Debug.Log("The total amount of movable spaces for this unit is: " + finalMovementHighlight.Count);
        Debug.Log("We have used the function to calculate it this time");
        return finalMovementHighlight;
    }

    //In:  finalMovement highlight and totalAttackabletiles
    //Out: a hashSet of nodes that are the combination of the two inputs
    //Desc: returns the unioned hashSet
    public HashSet<Node> getUnitTotalRange(HashSet<Node> finalMovementHighlight, HashSet<Node> totalAttackableTiles)
    {
        HashSet<Node> unionTiles = new HashSet<Node>();
        unionTiles.UnionWith(finalMovementHighlight);
        //unionTiles.UnionWith(finalEnemyUnitsInMovementRange);
        unionTiles.UnionWith(totalAttackableTiles);
        return unionTiles;
    }
    //In:  finalMovement highlight, the attack range of the unit, and the initial node that the unit was standing on
    //Out: hashSet Node of the total attackable tiles for the unit
    //Desc: returns a set of nodes that represent the unit's total attackable tiles
    public HashSet<Node> getUnitTotalAttackableTiles(HashSet<Node> finalMovementHighlight, int attRange, Node unitInitialNode)
    {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        foreach (Node n in finalMovementHighlight)
        {
            neighbourHash = new HashSet<Node>();
            neighbourHash.Add(n);
            for (int i = 0; i < attRange; i++)
            {
                foreach (Node t in neighbourHash)
                {
                    foreach (Node tn in t.neighbours)
                    {
                        tempNeighbourHash.Add(tn);
                    }
                }

                neighbourHash = tempNeighbourHash;
                tempNeighbourHash = new HashSet<Node>();
                if (i < attRange - 1)
                {
                    seenNodes.UnionWith(neighbourHash);
                }

            }
            neighbourHash.ExceptWith(seenNodes);
            seenNodes = new HashSet<Node>();
            totalAttackableTiles.UnionWith(neighbourHash);
        }
        totalAttackableTiles.Remove(unitInitialNode);
        
        //Debug.Log("The unit node has this many attack options" + totalAttackableTiles.Count);
        return (totalAttackableTiles);
    }


    //In:  
    //Out: hashSet of nodes get all the attackable tiles from the current position
    //Desc: returns a set of nodes that are all the attackable tiles from the units current position
    public HashSet<Node> getUnitAttackOptionsFromPosition()
    {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        Node initialNode = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];
        int attRange = selectedUnit.GetComponent<UnitScript>().attackRange;


        neighbourHash = new HashSet<Node>();
        neighbourHash.Add(initialNode);
        for (int i = 0; i < attRange; i++)
        {
            foreach (Node t in neighbourHash)
            {
                foreach (Node tn in t.neighbours)
                {
                    tempNeighbourHash.Add(tn);
                }
            }
            neighbourHash.UnionWith(tempNeighbourHash);
            tempNeighbourHash.Clear();
        }
        neighbourHash.ExceptWith(seenNodes);
        neighbourHash.Remove(initialNode);
        return neighbourHash;
    }

    //In:  
    //Out: hashSet node that the unit is currently occupying
    //Desc: returns a set of nodes of the tile that the unit is occupying
    public HashSet<Node> getTileUnitIsOccupying()
    {
       
        int x = selectedUnit.GetComponent<UnitScript>().x;
        int y = selectedUnit.GetComponent<UnitScript>().y;
        HashSet<Node> singleTile = new HashSet<Node>();
        singleTile.Add(graph[x, y]);
        return singleTile;
        
    }

    //In:  
    //Out: void
    //Desc: highlights the selected unit's options
    public void highlightTileUnitIsOccupying()
    {
        if (selectedUnit != null)
        {
            highlightMovementRange(getTileUnitIsOccupying());
        }
    }

    //In:  
    //Out: void
    //Desc: highlights the selected unit's attackOptions from its position
    public void highlightUnitAttackOptionsFromPosition()
    {
        if (selectedUnit != null)
        {
            highlightEnemiesInRange(getUnitAttackOptionsFromPosition());
        }
    }

    //In:  Hash set of the available nodes that the unit can range
    //Out: void - it changes the quadUI property in the gameworld to visualize the selected unit's movement
    //Desc: This function highlights the selected unit's movement range
    public void highlightMovementRange(HashSet<Node> movementToHighlight)
    {
        foreach (Node n in movementToHighlight)
        {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = blueUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;
        }
    }



    //In:  Hash set of the enemies in range of the selected Unit
    //Out: void - it changes the quadUI property in the gameworld to visualize an enemy
    //Desc: This function highlights the enemies in range once they have been added to a hashSet
    public void highlightEnemiesInRange(HashSet<Node> enemiesToHighlight)
    {
        foreach (Node n in enemiesToHighlight)
        {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = redUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;
        }
    }


    //In:  
    //Out: void 
    //Desc: disables the highlight
    public void disableHighlightUnitRange()
    {
        foreach(GameObject quad in quadOnMap)
        {
            if(quad.GetComponent<Renderer>().enabled == true)
            {
                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    //In:  
    //Out: void 
    //Desc: moves the unit then finalizes the movement
    public IEnumerator moveUnitAndFinalize()
    {
        disableHighlightUnitRange();
        disableUnitUIRoute();
        while (selectedUnit.GetComponent<UnitScript>().movementQueue.Count != 0)
        {
            yield return new WaitForEndOfFrame();
        }
        finalizeMovementPosition();
        selectedUnit.GetComponent<UnitScript>().setSelectedAnimation();
    }


    //In:  both units engaged in a battle
    //Out:  
    //Desc: deselects the selected unit after the action has been taken
    public IEnumerator deselectAfterMovements(GameObject unit, GameObject enemy)
    {
        //selectedSound.Play();
        selectedUnit.GetComponent<UnitScript>().setMovementState(3);
        selectedUnit.GetComponent<UnitScript>().setWaitIdleAnimation();//---------------
        disableHighlightUnitRange();
        disableUnitUIRoute();
        //If i dont have this wait for seconds the while loops get passed as the coroutine has not started from the other script
        //Adding a delay here to ensure that it all works smoothly. (probably not the best idea)
        yield return new WaitForSeconds(.25f);
        while (unit.GetComponent<UnitScript>().combatQueue.Count > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        while (enemy.GetComponent<UnitScript>().combatQueue.Count > 0)
        {
            yield return new WaitForEndOfFrame();
          
        }
        Debug.Log("All animations done playing");
       
        deselectUnit();


    }

    //In:  
    //Out: true if there is a tile that was clicked that the unit can move to, false otherwise 
    //Desc: checks if the tile that was clicked is move-able for the selected unit
    public bool selectTileToMoveTo()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
           
            if (hit.transform.gameObject.CompareTag("Tile")){
               
                int clickedTileX = hit.transform.GetComponent<ClickableTileScript>().tileX;
                int clickedTileY = hit.transform.GetComponent<ClickableTileScript>().tileY;
                Node nodeToCheck = graph[clickedTileX, clickedTileY];
                //var unitScript = selectedUnit.GetComponent<UnitScript>();

                if (selectedUnitMoveRange.Contains(nodeToCheck)) {
                    if ((hit.transform.gameObject.GetComponent<ClickableTileScript>().unitOnTile == null || hit.transform.gameObject.GetComponent<ClickableTileScript>().unitOnTile == selectedUnit) && (selectedUnitMoveRange.Contains(nodeToCheck)))
                    {
                        Debug.Log("We have finally selected the tile to move to");
                        generatePathTo(clickedTileX, clickedTileY);
                        return true;
                    }
                }
            }
            else if (hit.transform.gameObject.CompareTag("Unit"))
            {
              
                if (hit.transform.parent.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum)
                {
                    Debug.Log("Clicked an Enemy");
                }
                else if(hit.transform.parent.gameObject == selectedUnit)
                {
                   
                    generatePathTo(selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y);
                    return true;
                }
            }

        }
        return false;
    }

    //In: enemy unit
    //Out: HashSet of attackable tiles for the enemy unit
    //Desc:
    public HashSet<Node> getAttackableTilesForEnemy(GameObject unit){
        
        float[,] cost = new float[mapSizeX, mapSizeY];
        int attackrange = unit.GetComponent<UnitScript>().attackRange;
        int moverange = unit.GetComponent<UnitScript>().moveSpeed;
        Node unitInitialNode = graph[unit.GetComponent<UnitScript>().x, unit.GetComponent<UnitScript>().y];
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        HashSet<Node> initialNeighbours = new HashSet<Node>();
        HashSet<Node> tempReachableNeighbours = new HashSet<Node>();

        foreach (Node n in unitInitialNode.neighbours)
        {
            initialNeighbours.Add(n);
        }

        totalAttackableTiles.UnionWith(initialNeighbours);

        while (initialNeighbours.Count != 0)
        {
            foreach (Node n in initialNeighbours)
            {
                foreach (Node neighbour in n.neighbours)
                {
                    if (!totalAttackableTiles.Contains(neighbour))
                    {
                        
                        cost[neighbour.x, neighbour.y] = costToEnterTileIgnoreOccupant(neighbour.x, neighbour.y) + cost[n.x, n.y];

                        if (moverange + attackrange - cost[neighbour.x, neighbour.y] >= 0)
                        {
                            tempReachableNeighbours.Add(neighbour);
                        }
                    }
                }

            }

            initialNeighbours = tempReachableNeighbours;
            totalAttackableTiles.UnionWith(initialNeighbours);
            tempReachableNeighbours = new HashSet<Node>();
           
        }


        return totalAttackableTiles;
    }

    //In: player unit and enemy unit
    //Out: player unit's empty neighbour that is in the attackable tiles of the enemy unit
    //Desc:
    public Node getAnyEmptyNeighbour(GameObject playerUnit, GameObject enemyUnit){

        //TODO: ranged version
        HashSet<Node> attackableTiles = getAttackableTilesForEnemy(enemyUnit);
        foreach (Node n in graph[playerUnit.GetComponent<UnitScript>().x, playerUnit.GetComponent<UnitScript>().y].neighbours){
            int tiletype = tiles[n.x, n.y];
            if (tileTypes[tiletype].isWalkable == true && 
                attackableTiles.Contains(n) && 
                tilesOnMap[n.x, n.y].GetComponent<ClickableTileScript>().unitOnTile == null){
                    return n;
            }
            
        }
        return null;
    }
    public void enemyAction()
    {
        if (ExecutingEnemyAI)
        {
            return;
        }
        Debug.Log("Enemy Action");
        ExecutingEnemyAI = true;
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("EnemyUnit"))
        {

            GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
            if (unit.GetComponent<UnitScript>().currentHealthPoints > 0)
            {
                //select unit
                selectedUnit = unit;
                selectedUnit.GetComponent<UnitScript>().map = this;
                selectedUnit.GetComponent<UnitScript>().setMovementState(1);
                unitSelected = true;

                //move unit
                previousOccupiedTile = selectedUnit.GetComponent<UnitScript>().tileBeingOccupied;
                selectedUnit.GetComponent<UnitScript>().setWalkingAnimation();
                HashSet<Node> reachableTiles = getAttackableTilesForEnemy(unit);
                foreach (GameObject playerUnit in playerUnits)
                {
                    Debug.Log(playerUnit.GetComponent<UnitScript>().x + ", " + playerUnit.GetComponent<UnitScript>().y);
                    if (reachableTiles.Contains(graph[playerUnit.GetComponent<UnitScript>().x, playerUnit.GetComponent<UnitScript>().y]))
                    {
                        Debug.Log("valid target");
                        if (getAnyEmptyNeighbour(playerUnit, unit) != null)
                        {
                            Debug.Log("valid tile");
                            Node dest = getAnyEmptyNeighbour(playerUnit, unit);
                            generatePathTo(dest.x, dest.y);
                            Debug.Log(dest.x + ", " + dest.y);
                            Debug.Log("path count" + unit.GetComponent<UnitScript>().path.Count);
                            moveUnit();
                            StartCoroutine(moveUnitAndFinalize());

                            StartCoroutine(BMS.attack(unit, playerUnit));
                            StartCoroutine(deselectAfterMovements(unit, playerUnit));

                            break;
                        }
                    }
                    //TODO: walk to the general direction of player units if no player units in range
                }
                deselectUnit();
            }
        }
        GMS.endTurn();
        ExecutingEnemyAI = false;
    }
    public IEnumerator EnemyAction()
    {
        if (ExecutingEnemyAI)
        {
            yield break; // Exiting the coroutine if enemy AI is already executing
        }

        Debug.Log("Enemy Action");
        ExecutingEnemyAI = true;

        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("EnemyUnit"))
        {
            GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
            if (unit.GetComponent<UnitScript>().currentHealthPoints > 0)
            {
                //select unit
                selectedUnit = unit;
                selectedUnit.GetComponent<UnitScript>().map = this;
                selectedUnit.GetComponent<UnitScript>().setMovementState(1);
                unitSelected = true;

                //move unit
                previousOccupiedTile = selectedUnit.GetComponent<UnitScript>().tileBeingOccupied;
                selectedUnit.GetComponent<UnitScript>().setWalkingAnimation();
                HashSet<Node> reachableTiles = getAttackableTilesForEnemy(unit);
                foreach (GameObject playerUnit in playerUnits)
                {
                    if (playerUnit.GetComponent<UnitScript>().currentHealthPoints > 0)
                    {

                        if (reachableTiles.Contains(graph[playerUnit.GetComponent<UnitScript>().x, playerUnit.GetComponent<UnitScript>().y]))
                        {
                            if (getAnyEmptyNeighbour(playerUnit, unit) != null)
                            {
                                Node dest = getAnyEmptyNeighbour(playerUnit, unit);
                                generatePathTo(dest.x, dest.y);
                                moveUnit();
                                yield return StartCoroutine(moveUnitAndFinalize());

                                yield return StartCoroutine(BMS.attack(unit, playerUnit));
                                yield return StartCoroutine(deselectAfterMovements(unit, playerUnit));

                                break;
                            }
                        }
                    }
                    //TODO: walk to the general direction of player units if no player units in range
                }
                deselectUnit();
            }
        }

        GMS.endTurn();
        ExecutingEnemyAI = false;
    }
}
