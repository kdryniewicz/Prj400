using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMapGen : MonoBehaviour
{
    public int width;                                                   //Width of the CA generated map.
    public int height;                                                  //Height of the CA generated map.

    public int seedMin = 0,seedMax = 100;                           //Values to use in map generation.
    [Range(0,100)]
    public int randomFilPercent;                                        //A range between 0 and 100 deciding how much of the room will be filled with "wall"
    int[,] map;                                                         //Multidimensional array for cellular automata grid.

    public string seed;                                                 //A Variable used in generation of CA map. This parameter will allow for specific generation 
    public bool useRandomSeed;                                         //Determines whether the seed will be used.

    public int iterationRate = 5;                                       //In order to smooth out the map and make it more "cavern" like,
                                                                        //this value determines the amount of times this will be done.

    public GameObject[] WallObjects;                                    //Game Objects to be used to place the walls.
    public GameObject[] Floors;                                         //Gane Ibhects to be used to place the floors of the cavern.
    public GameObject boardHolder;

    // Start is called before the first frame update
    void Start()
    {
        GenerateCAMap();
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector2 pos = Vector2.one;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 0 && ((x == width / 2) && (y == height / 2)))
                    {
                        
                        pos = new Vector2(x, y);
                    }
                }
            }
            GameObject.FindWithTag("Player").gameObject.transform.position = pos;
        }
    }

    void GenerateCAMap()
    {
        ClearBoard();
        //The map is going to be generated to the size of width by height
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < iterationRate; i++)
        {
            MapSmoothing();
        }
        InstantiateMap();

    }



    void RandomFillMap()
    {
        //It's going to use pseudo 'random' generating seed, which will allow to get same generation if same seed code is applied.
        if (useRandomSeed)
        {
            //Time code applied in order to receive a unique 'seed' value
            seed = DateTime.Now.Ticks.ToString();
        }
        //We generate a unique 'code' from hashed value of string to be able to recreate the CA with the randomized seed.
        System.Random rng = new System.Random(seed.GetHashCode());
        Debug.Log(string.Format("The generated seed hash is: {0} and a rng number: {1}", seed.GetHashCode(), rng.Next(seedMin, seedMax)));

        //In order to generate the CA map, we go through x.. y.. values by width and height
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //check if the generated values are either 0 or width minus one, if so, make them a 
                //wall, this is to ensure that the edges are always walls 
                //since we don't want to have empty gaps which would allow player to get out of the generated map.
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    //For each "cell"/"tile", if the RNG value is 1, we want to have it as a wall, otherwise, leave it empty.
                    map[x, y] = (rng.Next(seedMin, seedMax) < randomFilPercent) ? 1 : 0;
                }
            }
        }
    }

    void ClearBoard()
    {
        //Simply empties out all the "wall" and "floor" game objects to prepare for next generation.
        if (boardHolder.transform.childCount > 0)
        {
            foreach (Transform child in boardHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    //private void OnDrawGizmos()
    //{ //this method fills in a "test map" to see if generation works as should by filling out "walls" as black and empty cells as white.
    //    if (map != null)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int y = 0; y < height; y++)
    //            {
    //                Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
    //                Vector3 pos = new Vector3((-width /2 + x + 0.5f), 0, (-height / 2 + y + 0.5f));
    //                Gizmos.DrawCube(pos, Vector3.one);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// The following method goes through map's X/Y values that were generated previously and chooses a position
    /// in the middle of the generated "cell" as a position for either a wall object or a floor object and
    /// instantiates either a random wall or floor on it.
    /// </summary>
    private void InstantiateMap()
    {
        //We make sure that a map was generated
        if (map != null)
        {
            //Go through the X... and y...
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //The position is the middle of each cell.
                    Vector3 pos = new Vector3((-width / 2 + x + 0.5f), (-height / 2 + y + 0.5f), 0.0f);
                    //if the value that was generated for the cell is one, it's going to be a wall
                    if(map[x,y] == 1)
                    {
                        InstantiateFromArray(WallObjects, pos);
                    }
                    else
                    {//otherwise, create a floor at the position.
                        InstantiateFromArray(Floors, pos);
                    }
                }
            }
        }
    }


    private void MapSmoothing()
    {
        //We go through the X/Y Values
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //We check each cell's individual neighbours, how many are "walls" (of value 1)
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                //If there are more than 4 walls, we change the current cell to also be a wall
                if (neighbourWallTiles > 4)
                {
                    map[x, y] = 1;
                } //Otherwise, if there is less or four, we turn it to be a floor. (of value 0)
                else if (neighbourWallTiles <= 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    private int GetSurroundingWallCount(int gridX, int gridY)
    { //A method to check how many surrounding wall "cells" are at the position
        int wallCount = 0;

        //Need to iterate through each neighbour X/Y Positions
        for(int neightbourX = gridX - 1; neightbourX <= gridX + 1; neightbourX++)
        {
            for (int neightbourY = gridY - 1; neightbourY <= gridY + 1; neightbourY++)
            {
                //We need to make sure we're inside the edges/inside of the map.
                if(neightbourX >= 0 && neightbourX < width && neightbourY >= 0 && neightbourY < height)
                {
                    //If the position in map at "neighbour's"  X/Y position is '1', i.e. meaning it's a wall cell, the wall count will increase.
                    if (neightbourX != gridX || neightbourY != gridY)
                    {
                        wallCount += map[neightbourX, neightbourY];
                    }
                }
                else
                {
                    //Encourage increasing the walls around the edges.
                    wallCount++;
                }
            }
        }
        return wallCount;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //For testing purposes,
        if(Debug.isDebugBuild || Application.isEditor)
        {
            if (Input.GetKeyUp(KeyCode.G))
            {
                GenerateCAMap();
            }
        }
        
    }


    void InstantiateFromArray(GameObject[] prefabs, Vector3 pos)
    {
        // Create a random index for the array.
        int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], pos, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;
    }
}
