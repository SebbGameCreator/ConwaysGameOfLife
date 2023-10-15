using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    public GameObject cellPrefab; // Cell Object
    Cell[,] cells; // Array
    float cellSize = 0.25f; // Size of our cells
    int numberOfColums, numberOfRows; // x,y grid
    int spawnChancePercentage = 15; // Percentage chance

    void Start()
    {
        PlaceCells();
    }

    void Update()
    {
        PopulationControl();
    }

    private void PlaceCells()
    {
        // Lower framerate makes it easier to test and see whats happening.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 8;

        // Calculate our grid depending on size and cellSize
        numberOfColums = (int)Mathf.Floor((Camera.main.orthographicSize * Camera.main.aspect * 2) / cellSize);
        numberOfRows = (int)Mathf.Floor(Camera.main.orthographicSize * 2 / cellSize);

        // Initiate our matrix array
        cells = new Cell[numberOfColums, numberOfRows];

        // For each row
        for (int y = 0; y < numberOfRows; y++)
        {
            // For each column in each row
            for (int x = 0; x < numberOfColums; x++)
            {
                // Create our game cell objects, multiply by cellSize for correct world placement
                Vector2 newPos = new Vector2(x * cellSize - Camera.main.orthographicSize *
                    Camera.main.aspect,
                    y * cellSize - Camera.main.orthographicSize);

                var newCell = Instantiate(cellPrefab, newPos, Quaternion.identity);
                newCell.transform.localScale = Vector2.one * cellSize;
                cells[x, y] = newCell.GetComponent<Cell>();

                if (Random.Range(0, 100) < spawnChancePercentage) // Random check to see if it should be alive
                {
                    cells[x, y].alive = true;
                }

                cells[x, y].UpdateStatus();
            }
        }
    }

    int CountNeighbors(int x, int y)
    {
        int liveNeighbors = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue; // Skip the current cell
                }

                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX >= 0 && neighborX < numberOfColums && neighborY >= 0 && neighborY < numberOfRows)
                {
                    if (cells[neighborX, neighborY].alive)
                    {
                        liveNeighbors++;
                    }
                }
            }
        }
        return liveNeighbors;
    }

    void PopulationControl()
    {
        for (int y = 0; y < numberOfRows; y++)
        {
            for (int x = 0; x < numberOfColums; x++)
            {
                Cell currentCell = cells[x, y];
                int liveNeighbors = CountNeighbors(x, y);

                //Any live cell with fewer than two live neighbors dies as if caused by underpopulation.
                //Any live cell with two or three live neighbors lives on to the next generation.
                //Any live cell with more than three live neighbors dies, as if by overpopulation.
                //Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.

                if (cells[x, y].alive) // Cell Alive
                {
                    if (liveNeighbors < 2 || liveNeighbors > 3)
                    {
                        cells[x, y] = currentCell;
                        cells[x, y].nextGen = false;
                    }
                    else
                    {
                        cells[x, y] = currentCell;
                        cells[x, y].nextGen = true;
                    }
                }
                else // Cell Dead
                {
                    if (liveNeighbors == 3)
                    {
                        cells[x, y] = currentCell;
                        cells[x, y].nextGen = true;
                    }
                    else
                    {
                        cells[x, y] = currentCell;
                        cells[x, y].nextGen = false;
                    }
                }
            }
        }

        for (int y = 0; y < numberOfRows; y++) // Now, update the cells array after all calculations are complete
        {
            for (int x = 0; x < numberOfColums; x++)
            {
                cells[x, y].UpdateStatus();
                cells[x, y].alive = cells[x, y].nextGen;
            }
        }
    }
}