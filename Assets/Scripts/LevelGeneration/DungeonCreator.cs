using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public class Cell
    {
        public bool visited;  // Flag indicating whether the cell has been visited
        public bool[] status = new bool[4];  // Array to store the status of each side (wall or door) up down right left
        public bool[] copystatus = new bool[4]; //up down right left
        public bool doneComparison = false;
        public int debugIndex;
        public Vector2Int position;  // Position of the cell in the grid
        public int gScore;  // Cost of getting from the start cell to this cell
        public int hScore;  // Heuristic (estimated) cost of getting from this cell to the goal cell
        public int fScore;  // Sum of gScore and hScore
        public Cell Parent;
        public List<int> neighborIndex;
        public Cell UpCell;
        public Cell DownCell;
        public Cell RightCell;
        public Cell LeftCell;

        // Default constructor
        public Cell()
        {
            visited = false;
            for (int i = 0; i < status.Length; i++)
            {
                status[i] = false;
            }
            gScore = int.MaxValue;
            hScore = 0;
            fScore = int.MaxValue;
        }
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;
        public bool obligatory;

        public int CalculateSpawnProbability(int x, int y)
        {
            // Calculate the spawn probability of the room based on its position
            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }
            return 0;
        }
    }

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 roomOffset;
    [SerializeField] private int startCellIndex = 0;
    [SerializeField] private Rule[] roomRules;
    [SerializeField] private GameObject finalRoomPrefab;


    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject prebossPrefab;

    private Cell[] grid;
    //Path von ASternAlgorithmus
    List<Cell> astarPath = new List<Cell>();

    bool bUpdateDone = false;

    public NavMeshSurface[] Surfaces;
    void Start()
    {
        GenerateMaze();
      
    }

    private void Update()
    {
        if(!bUpdateDone)
        {
            bUpdateDone = true;
            for (int i = 0; i < Surfaces.Length; i++)
            {
                Surfaces[i].BuildNavMesh();
            }

        }
    }
    void GenerateDungeon()
    {
        //begrenzungen fuer fallenraeume
        int maxTrapGrinder = 1;
        int currentTrapGrinder = 0;
        int maxSpinningTop = 1;
        int currentSpinningTop = 0;
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Cell currentCell = grid[i + j * gridSize.x];
                if (currentCell.visited)
                {
                    int totalRooms = 0;
                    int obligatoryRoomIndex = -1;
                    List<int> availableRooms = new List<int>();

                    // Iterate over the room rules to determine the available rooms
                    for (int k = 0; k < roomRules.Length; k++)
                    {
                        int spawnProbability = roomRules[k].CalculateSpawnProbability(i, j);

                        if (spawnProbability == 2)
                        {
                            obligatoryRoomIndex = k;
                            break;
                        }
                        else if (spawnProbability == 1)
                        {
                            availableRooms.Add(k);
                            totalRooms++;
                        }
                    }

                    int randomRoomIndex = -1;
                    if (obligatoryRoomIndex != -1)
                    {
                        randomRoomIndex = obligatoryRoomIndex;
                    }
                    else if (totalRooms > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, totalRooms);
                        randomRoomIndex = availableRooms[randomIndex];
                    }

                    //maximal 1 trapraum pro "sorte"
                    if (roomRules[randomRoomIndex].room.tag == "TrapGrinder")
                        currentTrapGrinder++;
                    else if (roomRules[randomRoomIndex].room.tag == "TrapSpinning")
                        currentSpinningTop++;

                    if ((currentSpinningTop > maxSpinningTop || currentTrapGrinder > maxTrapGrinder) && (roomRules[randomRoomIndex].room.tag == "TrapSpinning" || roomRules[randomRoomIndex].room.tag == "TrapGrinder"))
                    {
                        //solange der neue zufaellige raum ein trapraum ist obwohl schon maximalanzahl von jeweiligen trapraum drin is -> neuen random raum aussuchen
                        do
                        {
                            int randomIndex = UnityEngine.Random.Range(0, totalRooms);
                            randomRoomIndex = availableRooms[randomIndex];
                        } while ((roomRules[randomRoomIndex].room.tag == "TrapSpinning" || roomRules[randomRoomIndex].room.tag == "TrapGrinder"));

                    }

                    // Instantiate a room based on the randomly selected room index
                    var newRoom = Instantiate(roomRules[randomRoomIndex].room, new Vector3(i * roomOffset.x, 0, -j * roomOffset.y), Quaternion.identity, transform).GetComponent<RoomBehavior>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.cell = currentCell; //fuer astern
                    newRoom.name += " " + i + "-" + j;
                }
            }
        }

        // Replace the last room with the final room
        List<GameObject> roomObjects = new List<GameObject>();
        foreach (Transform child in transform)
        {
            roomObjects.Add(child.gameObject);
        }
        int replaceIndex = roomObjects.Count - 1;
        GameObject replacedRoom = roomObjects[replaceIndex];
        Debug.LogWarning("replaceindex" + replaceIndex + "name raum: " + replacedRoom.name);

        GameObject finalRoomObject = Instantiate(finalRoomPrefab, replacedRoom.transform.position, Quaternion.identity, this.transform);

        RoomBehavior replacedRoomBehavior = replacedRoom.GetComponent<RoomBehavior>();
        RoomBehavior finalRoomBehavior = finalRoomObject.GetComponent<RoomBehavior>();
        finalRoomBehavior.resetEnemySpawn();
        finalRoomBehavior.UpdateRoom(replacedRoomBehavior.status);

        Destroy(replacedRoom);
        //DestroyImmediate(replacedRoom);


        //for (int i = 0; i < Surfaces.Length; i++)
        //{
        //    Surfaces[i].BuildNavMesh();
        //}
        float t = 6.0f;
        //debugpath fur AStern

        for (int i = 0; i < roomObjects.Count ; i++)
        {
            if (IsCellInAStarPath(roomObjects[i].GetComponent<RoomBehavior>().cell))
                roomObjects[i].GetComponent<RoomBehavior>().activateDebugAStar();
        }


        //ausrichtung der path
        for (int i = 0; i < astarPath.Count; i++)
        {
           
            Cell aktuelleZelle = astarPath.ElementAt(i);
            int aktuellerIndex = -1;
            for (int j = 0; j < roomObjects.Count - 1; j++)
            {
                if (aktuelleZelle == roomObjects[j].GetComponent<RoomBehavior>().cell)
                {
                    aktuellerIndex = j;
                }
            }
            //im letzen raum deaktivieren //////////////
            //if (i == astarPath.Count -2)
            //{
            //    roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().deactivateDebugAStar(); //noch verschieben zu in der nähe von boss für den drachen
            //}
            if (aktuellerIndex != -1)
            {
                //Debug.LogError("INDEX im Path: " + i + " Raumname " + roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().name);

                Cell zelleDanach = astarPath.ElementAt(i + 1);
                int indexDanach = -1;
                //for (int j = 0; j < roomObjects.Count - 1; j++)
                for (int j = 0; j < roomObjects.Count ; j++)
                {
                    if (zelleDanach == roomObjects[j].GetComponent<RoomBehavior>().cell)
                    {
                        indexDanach = j;
                    }
                }
                if (indexDanach != -1)
                {
                    var currentPos = roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().DebugAstar.transform.position;
                    var nextPos = roomObjects[indexDanach].GetComponent<RoomBehavior>().DebugAstar.transform.position;
                    //Debug.LogError("für den Raum gilt: " + roomObjects[aktuellerIndex].name + "Position an diesem Raum: " + currentPos);
                    //Debug.LogError("für den Raum Danachgilt: " + roomObjects[indexDanach].name + "Position an diesem Raum: " + nextPos);

                    if (IsCellInAStarPath(roomObjects[indexDanach].GetComponent<RoomBehavior>().cell))
                    {
                        var test = currentPos - nextPos;
                        //Debug.LogError("für den Raum gilt: " + roomObjects[aktuellerIndex].name + "   currentPos - nextPos = " + test);

                        if (test.x == 0) //danach up aktuell down --> -t weil in richtung down (-z)
                        {
                            if (test.z < 0)
                            {
                                roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().DebugAstar.transform.Translate(new Vector3(0.0f, 0.0f, t)); // nach unten
                            }
                            else
                            {
                                roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().DebugAstar.transform.Translate(new Vector3(0.0f, 0.0f, -t)); // nach unten
                            }
                            //roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().ComputeNearLightSources();


                        }
                        else if (test.z == 0) //down
                        {
                            if (test.x < 0)
                            {
                                roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().DebugAstar.transform.Translate(new Vector3(t, 0.0f, 0.0f)); // nach unten

                            }
                            else
                            {
                                roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().DebugAstar.transform.Translate(new Vector3(-t, 0.0f, 0.0f)); // nach unten
                            }
                            //roomObjects[aktuellerIndex].GetComponent<RoomBehavior>().ComputeNearLightSources();

                        }
                    }
                }
            }


        }

        // old lever stuff
        //-----------------------------------------------
        //int RoomIndexWithKey = 0;
        //do
        //{
        ////nur einen key
        //RoomIndexWithKey = UnityEngine.Random.Range(1, roomObjects.Count - 2);
        //
        //    for (int i = 0; i < roomObjects.Count - 1; i++)
        //    {
        //        if (i == RoomIndexWithKey)
        //            continue;
        //        if (roomObjects[i].GetComponent<RoomBehavior>().bCanHaveKey == true)
        //            roomObjects[i].GetComponent<RoomBehavior>().setKeyActive(false);
        //
        //    }
        //    if (roomObjects[RoomIndexWithKey].GetComponent<RoomBehavior>().bCanHaveKey == true)
        //        roomObjects[RoomIndexWithKey].GetComponent<RoomBehavior>().setKeyActive(true);
        //    Debug.Log("Key at index: " + RoomIndexWithKey);
        //}
        //while (roomObjects[RoomIndexWithKey].GetComponent<RoomBehavior>().bCanHaveKey == false);
        //-----------------------------------------------

        int RoomIndexWithClosedDoor = 2;
        int a = 0;
        do
        {
            RoomIndexWithClosedDoor = UnityEngine.Random.Range(1, roomObjects.Count - 2);// nicht beim boss raum und auch nicht vor index 2 -> sonst direkt im tutotiral raum 
            a++;
            if (a > 100)
            {
                RoomIndexWithClosedDoor = -1;
                //Debug.LogError("ES WURDE ABGEBROCHEN!");
                break;
            }

        }
        while (!(IsCellInAStarPath(roomObjects[RoomIndexWithClosedDoor].GetComponent<RoomBehavior>().cell) && indexInAStarPath(roomObjects[RoomIndexWithClosedDoor].GetComponent<RoomBehavior>().cell) != 1));
        //&& indexInAStarPath(roomObjects[RoomIndexWithClosedDoor].GetComponent<RoomBehavior>().cell) != astarPath.Count -1 && indexInAStarPath(roomObjects[RoomIndexWithClosedDoor].GetComponent<RoomBehavior>().cell) != astarPath.Count - 2));
        // ! true -> false -> fertig ---                                                                         == 1 -> ssoll schlecht sein --> true -> 
        
        //alles aus
        for (int i = 0; i < roomObjects.Count - 1; i++)
        {
            roomObjects[i].GetComponent<RoomBehavior>().setBrokenWalls(false);
            roomObjects[i].GetComponent<RoomBehavior>().setKugelActive(false);
        }
        if (RoomIndexWithClosedDoor != -1)
        {
            roomObjects[RoomIndexWithClosedDoor].GetComponent<RoomBehavior>().setBrokenWalls(true);
            roomObjects[RoomIndexWithClosedDoor].GetComponent<RoomBehavior>().setKugelActive(true);

        }



        //im vorherigen path raum kugel aktivieren
        if (RoomIndexWithClosedDoor != -1)
        {
            int DoorIndexAstar = indexInAStarPath(roomObjects[RoomIndexWithClosedDoor].GetComponent<RoomBehavior>().cell);
            Cell vorherigeZelle = astarPath.ElementAt(DoorIndexAstar - 1);
            int richtigerIndex = -1;
            for (int i = 0; i < roomObjects.Count - 1; i++)
            {
                if (vorherigeZelle == roomObjects[i].GetComponent<RoomBehavior>().cell)
                {
                    richtigerIndex = i;
                }
            }
            roomObjects[richtigerIndex].GetComponent<RoomBehavior>().setKugelActive(true);
        }


        //monsterspawns 

        for (int i = 1; i < roomObjects.Count - 1; i++) // bossraum sonderbehandlung
        {
            RoomBehavior currentRoom = roomObjects[i].GetComponent<RoomBehavior>();
            if (currentRoom.bCanEnemys == true)
            {
                GameObject[] currentRoomSpawns = roomObjects[i].GetComponent<RoomBehavior>().getactiveSpawnableChildrenPositions();
                for (int j = 0; j < roomObjects[i].GetComponent<RoomBehavior>().getactiveSpawnableChildrenPositions().Length; j++)
                {
                    //chance fuer zombie oder knight
                    int rndNumber = UnityEngine.Random.Range(0, 2);
                    if (rndNumber == 0)
                    {
                        var knight = Instantiate(knightPrefab, currentRoomSpawns[j].transform.position, currentRoomSpawns[j].transform.rotation, currentRoomSpawns[j].transform);
                        knight.SetActive(false);

                    }
                    else
                    {
                        var zombie = Instantiate(zombiePrefab, currentRoomSpawns[j].transform.position, currentRoomSpawns[j].transform.rotation, currentRoomSpawns[j].transform);
                        zombie.SetActive(false);
                    }
                }
                currentRoom.MonsterHaveBeenSpawned();
            }
        }
        if (finalRoomBehavior.bCanEnemys == true)
        {
            GameObject[] currentRoomSpawns = finalRoomBehavior.GetComponent<RoomBehavior>().getactiveSpawnableChildrenPositions();
            for (int j = 0; j < finalRoomBehavior.GetComponent<RoomBehavior>().getactiveSpawnableChildrenPositions().Length; j++)
            {
                var boss = Instantiate(bossPrefab, currentRoomSpawns[j].transform.position, currentRoomSpawns[j].transform.rotation, currentRoomSpawns[j].transform);
                //boss.transform.localPosition = new Vector3 (0,0,0);
                boss.SetActive(false);
            }
        }
        //all spawns done:
        //debug light
        for (int i = 0; i < roomObjects.Count - 1; i++)
        {
            if (IsCellInAStarPath(roomObjects[i].GetComponent<RoomBehavior>().cell))
            {
                roomObjects[i].GetComponent<RoomBehavior>().ComputeNearLightSources();
                roomObjects[i].GetComponentInChildren<RoomScanner>().UpdateWayPointFlags();
            }
        }
    }

    void GenerateMaze()
    {
        grid = new Cell[gridSize.x * gridSize.y];

        // Initialize the grid with empty cells
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                grid[i + j * gridSize.x] = new Cell();
            }
        }

        Vector2Int currentCellIndex = new Vector2Int(startCellIndex % gridSize.x, startCellIndex / gridSize.x);

        List<int> path = new List<int>();

        int iteration = 0;
        int maxIterations = 1000;

        while (iteration < maxIterations)
        {
            iteration++;

            Cell currentCell = grid[currentCellIndex.x + currentCellIndex.y * gridSize.x];
            currentCell.visited = true;

            // If the current cell is the last cell, exit the loop
            if (currentCellIndex.x == gridSize.x - 1 && currentCellIndex.y == gridSize.y - 1)
            {
                //finalCell = currentCell;
                break;
            }

            List<int> neighbors = GetNeighborIndices(currentCellIndex);
            //nachbarn einmalig festlegen
            if (currentCell.neighborIndex == null)
                currentCell.neighborIndex = neighbors;

            if (neighbors.Count == 0)
            {
                if (path.Count == 0) //last cell of this path erreicht
                {
                    //finalCell = currentCell;

                    break;
                }
                else
                {
                    // Backtrack to the previous cell
                    currentCellIndex.x = path[path.Count - 1] % gridSize.x;
                    currentCellIndex.y = path[path.Count - 1] / gridSize.x;
                    path.RemoveAt(path.Count - 1);
                    //finalCell = currentCell;

                }
            }
            else
            {
                path.Add(currentCellIndex.x + currentCellIndex.y * gridSize.x);

                int randomIndex = UnityEngine.Random.Range(0, neighbors.Count);
                int newCellIndex = neighbors[randomIndex];

                Cell newCell = grid[newCellIndex];
                int newCellX = newCellIndex % gridSize.x;
                int newCellY = newCellIndex / gridSize.x;

                // Update the status of the current cell and the new cell based on their indices
                if (newCellIndex > currentCellIndex.x + currentCellIndex.y * gridSize.x)
                {
                    //down or right
                    if (newCellIndex - 1 == currentCellIndex.x + currentCellIndex.y * gridSize.x)
                    {
                        //rechts
                        currentCell.status[2] = true; //rechts
                        currentCell.RightCell = newCell;
                        newCell.status[3] = true; //vom nachbarn das gegenteilige oeffnen -> left
                        newCell.LeftCell = currentCell;
                    }
                    else
                    {
                        //downwards
                        currentCell.status[1] = true; //down
                        currentCell.DownCell = newCell;
                        newCell.status[0] = true; //upwards
                        newCell.UpCell = currentCell;
                    }
                }
                else
                {
                    //up or left
                    if (newCellIndex + 1 == currentCellIndex.x + currentCellIndex.y * gridSize.x)
                    {
                        currentCell.status[3] = true; //left
                        currentCell.LeftCell = newCell;
                        newCell.status[2] = true; //right
                        newCell.RightCell = currentCell;
                    }
                    else
                    {
                        currentCell.status[0] = true; //up
                        currentCell.UpCell = newCell;
                        newCell.status[1] = true; //down
                        newCell.DownCell = currentCell;
                    }
                }

                currentCellIndex.x = newCellX;
                currentCellIndex.y = newCellY;

            }

        }
        AStarAlgorithm();
        GenerateDungeon();
    }

    List<int> GetNeighborIndices(Vector2Int cellIndex)
    {
        List<int> neighborIndices = new List<int>();

        // Check the neighboring cells and add their indices to the list if they are valid
        if (cellIndex.y > 0 && grid[cellIndex.x + (cellIndex.y - 1) * gridSize.x].visited) //updneighbor
        {
            neighborIndices.Add(cellIndex.x + (cellIndex.y - 1) * gridSize.x);
        }

        if (cellIndex.y < gridSize.y - 1 && !grid[cellIndex.x + (cellIndex.y + 1) * gridSize.x].visited) //right 
        {
            neighborIndices.Add(cellIndex.x + (cellIndex.y + 1) * gridSize.x);
        }

        if (cellIndex.x < gridSize.x - 1 && !grid[cellIndex.x + 1 + cellIndex.y * gridSize.x].visited) //down
        {
            neighborIndices.Add(cellIndex.x + 1 + cellIndex.y * gridSize.x);
        }

        if (cellIndex.x > 0 && !grid[cellIndex.x - 1 + cellIndex.y * gridSize.x].visited) //up //reversed???
        {
            neighborIndices.Add(cellIndex.x - 1 + cellIndex.y * gridSize.x);
        }

        return neighborIndices;
    }


    // so ist es normal -> schlecht wenn bossraum bei 11 ist aber raum bei 14 noch gibt -> max index reversen
    // 3	7	11	15
    // 2	6	10	14
    // 1	5	9	13
    // 0	4	8	12
    //
    //temporary "umformen" zu dem und danach wieder back
    // 12	13	14	15
    // 8	9	10	11
    // 4	5	6	7
    // 0	1	2	3
    //
    //
    //
    int GetReversedIndex(int index, int numRows, int numCols)
    {
        int row = index / numCols;
        int col = index % numCols;
        return col * numRows + row;
    }
    void AStarAlgorithm()
    {
        int numRows = gridSize.x; // Anzahl der Zeilen in deiner aktuellen Darstellung
        int numCols = gridSize.y; // Anzahl der Spalten in deiner aktuellen Darstellung

        // Initialize the start and goal cells
        Cell startCell = grid[0];
        //Cell goalCell = grid[gridSize.x * gridSize.y - 1]; /klappt nur wenn letzter raum bei 4 4 ist

        //zeigt an welche zelle wo ist
        int[] debugMaze = new int[gridSize.x * gridSize.y];
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i].visited)
                debugMaze[i] = i;
            else
                debugMaze[i] = -1;

            grid[i].debugIndex = i;

            //Debug.LogWarning("debugMazeNoReverse: " + debugMaze[i]);
        }

        int highestVisitedIndex = 0;
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i].visited)
            {
                if (highestVisitedIndex < GetReversedIndex(i, numRows, numCols))
                {
                    highestVisitedIndex = GetReversedIndex(i, numRows, numCols);
                    //Debug.LogWarning("debugMaze HighestVisted Reversed: " + highestVisitedIndex);

                }
            }
        }
        //zurück reversen:
        int tmp = GetReversedIndex(highestVisitedIndex, numRows, numCols);
        //Debug.LogWarning("debugMaze HighestVisted ReversedBack: " + tmp);

        Cell goalCell = grid[tmp];

        // Set the gScore of the start cell to 0
        startCell.gScore = 0;

        // Create a list of cells to store the open set
        List<Cell> openSet = new List<Cell>();
        openSet.Add(startCell);

        while (openSet.Count > 0)
        {
            // Find the cell with the lowest fScore in the open set
            Cell currentCell = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fScore < currentCell.fScore)
                {
                    currentCell = openSet[i];
                }
            }

            // If the current cell is the goal cell, exit the loop
            if (currentCell == goalCell)
            {
                astarPath = ReconstructPath(currentCell);
                break;
            }

            // Remove the current cell from the open set
            openSet.Remove(currentCell);

            // Get the neighbors of the current cell
            List<Cell> neighbors = GetNeighbors(currentCell);

            foreach (Cell neighbor in neighbors)
            {
                // Calculate the tentative gScore for the neighbor
                int tentativeGScore = currentCell.gScore + 1; // Assuming all neighbors have a distance of 1

                if (tentativeGScore < neighbor.gScore)
                {
                    // Update the neighbor's gScore, hScore, and fScore
                    neighbor.gScore = tentativeGScore;
                    neighbor.hScore = CalculateHeuristic(neighbor, goalCell);
                    neighbor.fScore = neighbor.gScore + neighbor.hScore;
                    // Set the neighbor's parent to the current cell
                    neighbor.Parent = currentCell;
                    if (!openSet.Contains(neighbor))
                    {
                        // Add the neighbor to the open set
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    int CalculateHeuristic(Cell cell, Cell goalCell)
    {
        // Calculate the Manhattan distance between two cells
        int dx = Math.Abs(cell.position.x - goalCell.position.x);
        int dy = Math.Abs(cell.position.y - goalCell.position.y);
        return dx + dy;
    }

    List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        if (cell.neighborIndex != null)
        {
            //Debug.LogWarning("index of currentcell: " + cell.debugIndex + " Anzahl nachbarn: " + cell.neighborIndex.Count);
            //0 up z achse
            //1 down 
            //2 right
            //3 left
            for (int i = 0; i < cell.neighborIndex.Count; i++)
            {
                //if (
                //   (cell.status[0] == true && grid[cell.neighborIndex[i]].status[1] == true) ||
                //   (cell.status[1] == true && grid[cell.neighborIndex[i]].status[0] == true) ||
                //   (cell.status[2] == true && grid[cell.neighborIndex[i]].status[3] == true) ||
                //   (cell.status[3] == true && grid[cell.neighborIndex[i]].status[2] == true))
                //if (
                //   ((cell.status[0] == true && grid[cell.neighborIndex[i]].status[1] == true) && cell.UpCell == grid[cell.neighborIndex[i]]) ||
                //   ((cell.status[1] == true && grid[cell.neighborIndex[i]].status[0] == true) && cell.DownCell == grid[cell.neighborIndex[i]]) ||
                //   ((cell.status[2] == true && grid[cell.neighborIndex[i]].status[3] == true) && cell.RightCell == grid[cell.neighborIndex[i]]) ||
                //   ((cell.status[3] == true && grid[cell.neighborIndex[i]].status[2] == true) && cell.LeftCell == grid[cell.neighborIndex[i]]))
                if (cell.UpCell == grid[cell.neighborIndex[i]] && grid[cell.neighborIndex[i]].DownCell == cell)
                {
                    //Debug.LogWarning("index von currentCell: " + cell.debugIndex + " Nachbar UP true");
                    neighbors.Add(grid[cell.neighborIndex[i]]);
                }
                if (cell.DownCell == grid[cell.neighborIndex[i]] && grid[cell.neighborIndex[i]].UpCell == cell)
                {
                    //Debug.LogWarning("index von currentCell: " + cell.debugIndex + " Nachbar DOWN true");
                    neighbors.Add(grid[cell.neighborIndex[i]]);
                }
                if (cell.RightCell == grid[cell.neighborIndex[i]] && grid[cell.neighborIndex[i]].LeftCell == cell)
                {
                    //Debug.LogWarning("index von currentCell: " + cell.debugIndex + " Nachbar RIGHT true");
                    neighbors.Add(grid[cell.neighborIndex[i]]);
                }
                if (cell.LeftCell == grid[cell.neighborIndex[i]] && grid[cell.neighborIndex[i]].RightCell == cell)
                {
                    //Debug.LogWarning("index von currentCell: " + cell.debugIndex + " Nachbar LEFT true");
                    neighbors.Add(grid[cell.neighborIndex[i]]);
                }
            }
        }

        return neighbors;
    }

    bool IsCellInAStarPath(Cell cell)
    {
        return astarPath.Contains(cell);
    }

    int indexInAStarPath(Cell cell)
    {
        int v = astarPath.FindIndex(a => a == cell);
        //Debug.LogError(" XD?" + v);
        return v;
    }
    int indexPreviousAStarPath(Cell cell)
    {
        int v = astarPath.FindIndex(a => a == cell);
        //Debug.LogError(" XD?" + v);
        return v;
    }
    List<Cell> ReconstructPath(Cell currentCell)
    {
        List<Cell> path = new List<Cell>();
        path.Add(currentCell);

        while (currentCell.Parent != null)
        {
            currentCell = currentCell.Parent;
            path.Insert(0, currentCell);
        }

        return path;
    }

}

