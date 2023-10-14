using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static int SCREEN_WIDTH = 64;
    private static int SCREEN_HEIGHT = 48;

    public float speed = 0.25f;
    public float timer = 0;

    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    void Start()
    {
        PlaceCells();
    }

    void Update()
    {
        if (timer >= speed)
        {
            timer = 0;

            CountNeighbors();

            PopulationControl();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    void PlaceCells()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                Cell cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                grid[x, y] = cell;
                grid[x, y].SetAlive(RandomAliveCell());
            }
        }
    }
    
    void CountNeighbors()
    {
        for (int y = 0; y < SCREEN_HEIGHT; ++y)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                int numNeighbors = 0;

                if (y + 1 < SCREEN_HEIGHT)// North
                {
                    if (grid[x,y+1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                if(x + 1 < SCREEN_WIDTH)// East
                {
                    if (grid[x+1,y].isAlive)
                    {
                         numNeighbors++;
                    }
                }

                if (y - 1 >= 0)// South
                {
                    if (grid[x,y-1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                if (x - 1 >= 0)// West
                {
                    if (grid[x-1,y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                if (x + 1 < SCREEN_WIDTH && y + 1 < SCREEN_HEIGHT)// NorthEast
                {
                    if (grid[x+1,y+1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                if (x - 1 >= 0 && y + 1 < SCREEN_HEIGHT)// NorthWest
                {
                    if (grid[x-1,y+1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                if (x + 1 < SCREEN_WIDTH && y - 1 >= 0)// SouthEast
                {
                    if (grid[x+1,y-1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                if (x - 1 >= 0 && y - 1 >= 0)// SouthWest
                {
                    if (grid[x-1,y-1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                grid[x,y].numNeighbors = numNeighbors;
            }
        }
    }

    void PopulationControl()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                // Rules
                // Any live cell with 2 or 3 live neighbors survives
                // Any dead cell with 3 live neighbors becomes a live cell
                // All other live cells die in the next generation and all other dead cells stay dead.

                if (grid[x,y].isAlive)
                {
                    // Cell Alive
                    if (grid[x,y].numNeighbors != 2 && grid[x,y].numNeighbors != 3)
                    {
                        grid[x,y].SetAlive(false);
                    }
                }
                else
                {
                    // Cell Dead
                    if (grid[x,y].numNeighbors == 3)
                    {
                        grid[x,y].SetAlive(true);
                    }
                }
            }
        }
    }

    bool RandomAliveCell()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand > 75)
        {
            return true;
        }

        return false;
    }
}