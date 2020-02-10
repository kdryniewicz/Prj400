using System.Collections;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Wall, Floor, Enemy, Player
    }


    public int columns = 100;                                   // The number of columns on the board (how wide it will be).
    public int rows = 100;                                      // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);            // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);            // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);           // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);       // The range of lengths corridors between rooms can have.
    public IntRange enemiesPerRoom = new IntRange(0, 3);        // The range of enemies that each room can have.
    public IntRange maxEnemiesForDungeon = new IntRange(8, 12); // The range of enemies that the whole dungeon can have.
    public GameObject[] floorTiles;                             // An array of floor tile prefabs.
    public GameObject[] wallTiles;                              // An array of wall tile prefabs.
    public GameObject[] outerWallTiles;                         // An array of outer wall tile prefabs.
    public GameObject player;
    public GameObject[] enemies;
    private int enemiesCreatedCount;

    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.


    private void Start()
    {
        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();

        InstantiateTiles();
        InstantiateOuterWalls();

        SpreadEnemiesInRooms();
        InstantiateEnemies();

        CreatePlayer();
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
        rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }

            if(rooms[i].xPos == 0 && rooms[i].yPos == 0)
            {
                string.Format("Room: {0}, Corridor: {1}, Rows: {2}, Columns: {3}", i, rooms[i].enteringCorridor, rows, columns);
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
            rooms[i].TilePositions = new Vector2[currentRoom.roomWidth + currentRoom.roomHeight];

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
                rooms[i].TilePositions[j] = new Vector2(xpos, ypos);
                //Debug.Log(rooms[i].TilePositions[j]);
            }
        }
    }

    void SpreadEnemiesInRooms()
    {
        int totalEnemies = 0;

        //Set total enemies in the dungeon. There will be at least one enemy per each room or max of three enemies for each room.
        maxEnemiesForDungeon = new IntRange(rooms.Length, rooms.Length * 3);
        enemiesCreatedCount = maxEnemiesForDungeon.Random;

        //Keeping track of enemies left to spread throughout dungeon.
        totalEnemies = enemiesCreatedCount;

        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            //Room Object to keep track of current room.
            Room currentRoom = rooms[i];

            //make sure there are enemies to spread out.
            if(totalEnemies > 0)
            {
                //Make sure there aren't any enemies already in the room.
                if(currentRoom.enemiesInRoom == 0)
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

    Vector2 GetRandomPosInRoom(Room r)
    {
        Vector2 tempPos = Vector2.zero;
        if(r != null)
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

    void InstantiateEnemies()
    {
        foreach (Room currentRoom in rooms)
        {
            if(currentRoom.enemiesInRoom > 0)
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
                        if(enemyPos == currentRoom.EnemyPositions[j])
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

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;
    }
}