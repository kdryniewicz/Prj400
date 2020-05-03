using ProceduralGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Wall, Floor, Enemy, Player
    }

    public int columns = 50;                                   // The number of columns on the board (how wide it will be).
    public int rows = 50;                                      // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(3, 5);              // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);            // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);           // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);       // The range of lengths corridors between rooms can have.
    public IntRange enemiesPerRoom = new IntRange(0, 3);        // The range of enemies that each room can have.
    public IntRange maxEnemiesForDungeon = new IntRange(8, 12); // The range of enemies that the whole dungeon can have.
    public GameObject[] floorTiles;                             // An array of floor tile prefabs.
    public GameObject[] wallTiles;                              // An array of wall tile prefabs.
    public GameObject[] outerWallTiles;                         // An array of outer wall tile prefabs.
    public GameObject player;                                   // Character used by the player to move around and play the game
    public GameObject[] enemies;                                // Enemy Objects that are used as challenges in game
    private int enemiesCreatedCount;                            // Amount of Enemies that were created in total.
    public List<GameObject> enemiesCreated = new List<GameObject>();

    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.
    public GameObject exitObject;
    public List<GameObject> objectivesToSpawn;                 //List of objects that are objectives to spawn
    private int objectivesAmount;

    public IntRange ItemsRange = new IntRange(1, 2);
    public GameObject[] itemsToSpawn;
    public int itemsAmount;

    public int ObjectLimitPerRoom = 2;                  //In order to limit clutter per room, we want max. of 2 objects of any type per room.

    private void Start()
    {
    }

    //Resets and reinitializes all the game objects / Variables.
    void CleanUpOnAisleFour()
    {
        numRooms = new IntRange(numRooms.m_Min, numRooms.m_Max);
        roomWidth = new IntRange(roomWidth.m_Min, roomWidth.m_Max);
        roomHeight = new IntRange(roomHeight.m_Min, roomHeight.m_Max);
        corridorLength = new IntRange(corridorLength.m_Min, corridorLength.m_Max);
        enemiesPerRoom = new IntRange(enemiesPerRoom.m_Min, enemiesPerRoom.m_Max);
        maxEnemiesForDungeon = new IntRange(maxEnemiesForDungeon.m_Min, maxEnemiesForDungeon.m_Max);
        ItemsRange = new IntRange(ItemsRange.m_Min, ItemsRange.m_Max);
        objectivesToSpawn = new List<GameObject>();
        

        enemiesCreatedCount = 0;
        objectivesAmount = 0;
        itemsAmount = 0;

        tiles = new TileType[][] { };
        rooms = new Room[] { };
        corridors = new Corridor[] { };
    }

    void genValForEnemies()
    {
        maxEnemiesForDungeon = new IntRange(Random.Range(rooms.Length / 2, rooms.Length), Random.Range(rooms.Length + GameManager.instance.level, rooms.Length + (GameManager.instance.level * 2)));
    }

    public void SetupFloor()
    {
        Debug.LogWarning("Setup Floor was called");
        //Cleans up the board to prepare for next generation.
        CleanUpOnAisleFour();

        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");

        //Sets up all the arrays of tiles in a 'Grid Style System'
        SetupTilesArray();

        //Next we need to create out Room Objects and corridor objects connected to those Rooms.
        CreateRoomsAndCorridors();

        //Sets out the tile values in the grid created above.
        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();

        //Instantiates tile Game Objects based on the tile values generated above.
        InstantiateTiles();
        InstantiateOuterWalls();

        //Based on parameters of Room objects, the TilePosition Array of Vector2 is assigned to keep track of where each tile is.
        AssignTilePositions();

        genValForEnemies();
        //Picks out random positions inside each room in order to be able to spawn enemies inside of each room.
        SpreadEnemiesInRooms();

        //Instantiates the enemies in their pre-assigned positions previously.
        InstantiateEnemies();



        //Instantiates an "Exit" object inside of the last room at the last position available inside the TilePositions array in last room.
        InstantiateExit();
        //Debug.Log(string.Format("Middle of room {0} pos is: {1}", rooms[0].Name, rooms[0].))

       
        //Cleans up rooms and other game objects in order to be able to keep track of them in a much more organised manner.
        SortRooms();

        //Procedurally generate, distribute and spawn items (Potions) depending on the level player is on and how many enemies they killed
        CreateAndSpawnItems();

        //Creates a PlayerObject from a prefab
        if (Player.playerInstance == null || GameObject.FindWithTag("Player") == null)
        {
            CreatePlayer();
        }
        else
        {
            Debug.Log(string.Format("The start pos of Player at idx {0} is {1}", rooms[0].TilePositions.Length / 2, rooms[0].TilePositions[rooms[0].TilePositions.Length / 2]));
            Vector2 playerPos = rooms[0].TilePositions[rooms[0].TilePositions.Length / 2];
            GameObject.FindWithTag("Player").transform.position = playerPos;
            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().playerLock = false;
            Debug.Log(string.Format("player should have been spawned at {0} was spawned at {1}", playerPos, player.transform.position));
        }


    }

    void SortRooms()
    {

        //First we need to look through all the Room objects that were prepared.
        for (int i = 0; i < rooms.Length; i++)
        {
            GameObject newParent = new GameObject();
            //Create a new parent object to store tiles associated with that room.
            if (boardHolder.transform.Find(rooms[i].Name + ("Clone")) == null)
            {
               newParent = Instantiate(new GameObject("Room " + i), boardHolder.transform);
            }
            else
            {
                newParent = boardHolder.transform.Find(rooms[i].Name + ("Clone")).gameObject;
            }
            //Debug.Log("Created a " + newParent.name);
            //newParent.transform.parent = boardHolder.transform;
            //Now we need to look through all available tile positions in that room to sort them into that Room Game object we created above.
            for (int j = 0; j < rooms[i].TilePositions.Length; j++)
            {
                //We're sorting through all the tile Game Objects in the board holder game object in order to see if we have any walls or floors.
                foreach (Transform child in boardHolder.transform)
                {
                    //If we find a floor or a wall object we'll change the parent from board holder to the current room game object.
                    if (child.tag == "Floor" || child.tag == "Wall" || child.tag == "Enemy" || child.tag == "Exit" || child.tag == "Collectable")
                    {

                        //We also need to check if the tile position matches any of the ones in TilePosition array.
                        if (rooms[i].TilePositions[j] == new Vector2(child.position.x, child.position.y))
                        {

                            //Debug.Log(string.Format("{0} object's position is: {1}", child.name, childTempPos));

                            //We have a match so we change the parent of that object to put it into the new room game object.
                            child.parent = newParent.transform;
                            //child.parent = newParent.transform;
                        }
                    }



                }
            }
        }
        GameObject othersParent = new GameObject();
        if (boardHolder.transform.Find("Other Objects" + ("Clone")) == null)
        {
            othersParent = Instantiate(new GameObject("Other Objects"), boardHolder.transform);
        }
        else
        {
            othersParent = boardHolder.transform.Find("Other Objects" + ("Clone")).gameObject;
        }


        for (int i = 0; i < rooms.Length; i++)
        {

            for (int j = 0; j < rooms[i].TilePositions.Length; j++)
            {
                foreach (Transform child in boardHolder.transform)
                {
                    if (rooms[i].TilePositions[j] != new Vector2(child.position.x, child.position.y))
                    {
                        if (child.tag == "Floor" || child.tag == "Wall" || child.tag == "OuterWall")
                        {
                            child.parent = othersParent.transform;
                        }
                    }
                }
            }
        }
    }


    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[rows];
        }
    }


    public void CreatePlayer()
    {
        Vector3 playerPos = new Vector3(rooms[0].xPos, rooms[0].yPos, 0);
        Instantiate(player, playerPos, Quaternion.identity);
    }

    void CreateRoomsAndCorridors()
    {
        // Create the rooms array with a random size.
        rooms = new Room[numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(0, roomWidth, roomHeight, columns, rows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(i, roomWidth, roomHeight, columns, rows, corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }
            //if (i == rooms.Length * .5f)
            //{
            //Check the positions and if 0,0 then output to console. i, previous corridor, rows, cols
            //}

        }

    }


    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];
            float xpos, ypos = 0f;
            //Vector2 tilepos = Vector2.zero;
            //rooms[i].TilePositions = new Vector2[currentRoom.roomWidth + currentRoom.roomHeight];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;
                xpos = xCoord;
                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;
                    ypos = yCoord;
                    // The coordinates in the jagged array are based on the room's position and it's width and height.
                    tiles[xCoord][yCoord] = TileType.Floor;
                }
                //rooms[i].TilePositions[j] = new Vector2(xpos, ypos);
                //Debug.Log(rooms[i].TilePositions[j]);
            }
            // Debug.Log(string.Format("Tile Pos X: {0}, Y: {1}", rooms[i].TilePositions[i].x, rooms[i].TilePositions[i].y));
            //Debug.Log(string.Format("Tile Pos X: {0}, Y: {1}", tilepos.x, tilepos.y));
        }
    }

    void SpreadEnemiesInRooms()
    {
        int totalEnemies = 0;

        //Set total enemies in the dungeon. There will be at least one enemy per each room or max of three enemies for each room.
        maxEnemiesForDungeon = new IntRange(rooms.Length, rooms.Length * GameManager.instance.level);
        enemiesCreatedCount = maxEnemiesForDungeon.Random;

        //Keeping track of enemies left to spread throughout dungeon.
        totalEnemies = enemiesCreatedCount;

        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            //Room Object to keep track of current room.
            Room currentRoom = rooms[i];

            //make sure there are enemies to spread out.
            if (totalEnemies > 0)
            {
                //Make sure there aren't any enemies already in the room.
                if (currentRoom.enemiesInRoom == 0)
                {
                    //Set the enemies in room to a random between a range.
                    currentRoom.enemiesInRoom = enemiesPerRoom.Random;
                    totalEnemies -= currentRoom.enemiesInRoom;
                }
            }
            else
            {
                break;
            }
        }
    }

    void AssignTilePositions()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            Room current = rooms[i];
            rooms[i].TilePositions = new Vector2[current.roomWidth * current.roomHeight];
            // Debug.Log("Room " + i + " Position: " + current.xPos + "," + current.yPos);
            int idx = 0;

            for (int k = 0; k < current.roomWidth; k++)
            {
                for (int j = 0; j < current.roomHeight; j++)
                {
                    rooms[i].TilePositions[idx] = new Vector2(current.xPos + k, current.yPos + j);
                    idx++;

                }
            }

        }
        //Debug Code:
        //for (int i = 0; i < rooms[0].TilePositions.Length; i++)
        //{
        //    Debug.Log(string.Format("Amount of tiles: {0}, Tile Index: {1}, X: {2}, Y: {3}", rooms[0].TilePositions.Length, i, rooms[0].TilePositions[i].x, rooms[0].TilePositions[i].y));
        //}
    }



    void CreateAndSpawnItems()
    {
        SortRooms();
        if (GameManager.instance.level > 1)
        {
            ItemsRange.m_Max = (ItemsRange.m_Max * (int)(GameManager.instance.enemiesKilled / GameManager.instance.level));
        }
        else
        {
            ItemsRange.m_Max = 5;
        }
        itemsAmount = ItemsRange.Random;
        List<GameObject> spawnable = new List<GameObject>();
        for (int i = 0; i < itemsAmount; i++)
        {
            // Create a random index for the array.
            int randomIndex = Random.Range(0, itemsToSpawn.Length - 1);
            spawnable.Add(itemsToSpawn[randomIndex]);
        }

        InstantiateObjects(spawnable);
        
    }

    void SetTilesValuesForCorridors()
    {
        // Go through every corridor...
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

            // and go through it's length.
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                // Depending on the direction, add or subtract from the appropriate
                // coordinate based on how far through the length the loop is.
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }

                // Set the tile at these coordinates to Floor.
                tiles[xCoord][yCoord] = TileType.Floor;
            }
        }
    }


    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // ... and instantiate a floor tile for it.
                InstantiateFromArray(floorTiles, i, j);

                // If the tile type is Wall...
                if (tiles[i][j] == TileType.Wall)
                {
                    // ... instantiate a wall over the top.
                    InstantiateFromArray(wallTiles, i, j);
                }
            }
        }
    }


    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        float leftEdgeX = -1f;
        float rightEdgeX = columns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = rows + 0f;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }


    void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY)
    {
        // Start the loop at the starting value for Y.
        float currentY = startingY;

        // While the value for Y is less than the end value...
        while (currentY <= endingY)
        {
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            InstantiateFromArray(outerWallTiles, xCoord, currentY);

            currentY++;
        }
    }


    void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(outerWallTiles, currentX, yCoord);

            currentX++;
        }
    }

    bool CheckIfPosEmpty(Room r, Vector3 PosToCheck)
    {
        //SortRooms();
        GameObject roomObj = GameObject.Find(r.Name + "(Clone)");
        if (roomObj != null)
        {
            foreach (Transform child in roomObj.transform)
            {
                if (child.position == PosToCheck)
                {
                    if (child.gameObject.CompareTag("Player") || child.gameObject.CompareTag("Enemy") || child.gameObject.CompareTag("Exit") || child.gameObject.CompareTag("Collectable"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

        }
        else
        {
            Debug.Log("No room found! R: " +  r.Name);
        }
        return false;

    }

    public Vector2 GetRandomPosInRoom(Room r)
    {
        Vector2 tempPos = Vector2.zero;
        if (r != null)
        {
            //IntRange randomRange = new IntRange(r.xPos, r.roomWidth);

            //Each room has a range of vector2 values for each tile. Pick one at random, make sure previous enemy doesn't use same value and choose one at random to spawn enemy into.
            IntRange tileRange = new IntRange(0, r.TilePositions.Length - 1);
            int index = tileRange.Random;
            tempPos = new Vector2(r.TilePositions[index].x, r.TilePositions[index].y);

        }
        else
        {
            Debug.Log("Room object is empty!");
        }

        return tempPos;
    }
    void InstantiateObjectives(int maxLenghtPerRoom)
    {
        if (objectivesAmount < objectivesToSpawn.Count)
        {
            foreach (Room currentRoom in rooms)
            {
                if (currentRoom.ObjectiveObjsPositions.Length < maxLenghtPerRoom)
                {
                    for (int i = 0; i < currentRoom.ObjectiveObjsPositions.Length; i++)
                    {
                        Vector2 objPos = GetRandomPosInRoom(currentRoom);

                        //This was breaking with J < i check
                        //Check if the generated position doesn't match a previously generated objective's position.
                        for (int j = 0; j < currentRoom.ObjectiveObjsPositions.Length; j++)
                        {
                            //if there is a match, try generating one again.
                            if (objPos == currentRoom.ObjectiveObjsPositions[j])
                            {
                                objPos = GetRandomPosInRoom(currentRoom);
                                j = -1;
                            }
                        }

                        InstantiateFromArray(objectivesToSpawn.ToArray(), objPos.x, objPos.y);
                        currentRoom.EnemyPositions[i] = objPos;
                    }
                }
            }
        }
    }
    void InstantiateEnemies()
    {
        foreach (Room currentRoom in rooms)
        {
            if (currentRoom.enemiesInRoom > 0)
            {
                currentRoom.EnemyPositions = new Vector2[currentRoom.enemiesInRoom];

                for (int i = 0; i < currentRoom.enemiesInRoom; i++)
                {
                    Vector2 enemyPos = GetRandomPosInRoom(currentRoom);

                    //This was breaking with J < i check
                    //Check if the generated position doesn't match a previously generated enemy's position.
                    for (int j = 0; j < currentRoom.EnemyPositions.Length; j++)
                    {
                        //if there is a match, try generating one again.
                        if (enemyPos == currentRoom.EnemyPositions[j])
                        {
                            enemyPos = GetRandomPosInRoom(currentRoom);
                            j = -1;
                        }
                    }

                    InstantiateFromArray(enemies, enemyPos.x, enemyPos.y);
                    currentRoom.EnemyPositions[i] = enemyPos;
                }
            }
        }
    }

    int GetAmountObjsInRoom(Room r, CollectibleType typeToFind)
    {
        int amount = 0;
        //First search through boardholder's objects to locate the current room
        GameObject room = new GameObject();
        foreach (Transform child in boardHolder.transform)
        {
            if(child.name == r.Name + "(Clone)")
            {
                room = child.gameObject;
                break;
            }
        }

        foreach (Transform item in room.transform)
        {
            if(item.GetComponent<Collectible>() != null)
            {
                if(item.GetComponent<Collectible>().collType == typeToFind)
                {
                    amount++;
                }
            }
        }
        return amount;
    }

    public void InstantiateObjects(List<GameObject> objectsToInst)
    {
        int[] indexes = new int[] { };
        for (int i = 0; i < objectsToInst.Count; i++)
        {
            indexes = new int[objectsToInst.Count];
            IntRange roomRange = new IntRange(0, rooms.Length - 1);
            indexes[i] = roomRange.Random;
        }
        for (int i = 0; i < objectsToInst.Count; i++)
        {
            //We check to see if there's less than
            if (GetAmountObjsInRoom(rooms[indexes[i]], objectsToInst[i].GetComponent<Collectible>().collType) <= ObjectLimitPerRoom)
            {
                Vector2 pos = GetRandomPosInRoom(rooms[indexes[i]]);
                if (CheckIfPosEmpty(rooms[indexes[i]], pos))
                {
                    pos = GetRandomPosInRoom(rooms[indexes[i]]);
                    //Debug.Log(string.Format("Room: {0} and pos: (1},{2}", currentRoom.Name, pos.x,pos.y));
                }
                GameObject item = Instantiate(objectsToInst[i], pos, Quaternion.Euler(0, 0, 0));
                item.transform.SetParent(boardHolder.transform.Find(rooms[indexes[i]].Name + ("Clone")));
            }
        }
    }   
   


    public void InstantiateExit()
    {
        
        Vector2 lastPos = rooms[rooms.Length - 1].TilePositions[rooms[rooms.Length - 1].TilePositions.Length - 1];
        //for (int i = 0; i < rooms[rooms.Length - 1].TilePositions.Length; i++)
        //{
        //    Debug.Log(string.Format("Tile Pos at {0} is {1}",i, rooms[rooms.Length - 1]));
        //}
        Debug.Log("Stairs position is: " + lastPos);
        GameManager.instance.Stairs = Instantiate(exitObject, lastPos, Quaternion.identity, boardHolder.transform);
        GameManager.instance.Stairs.SetActive(false);
        GameManager.instance.stairsExist = true;
    }


    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        if(prefabs[randomIndex].tag.ToUpper() == "ENEMY")
        {
            enemiesCreated.Add(tileInstance);
        }
        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;
    }
}