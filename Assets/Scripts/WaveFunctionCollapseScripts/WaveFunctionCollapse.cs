using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WaveFunctionCollapse : MonoBehaviour {

    [Header("Maze Settings")]
    [SerializeField] private int dimensions;
    [SerializeField] private Tile[] tileObjects;

    [Header("Grid Settings")]
    [SerializeField] private List<Cell> gridComponents;
    [SerializeField] private Cell cellObj;

    [SerializeField] private Tile backupTile;

    private int iteration;

    private void Awake() {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    // Initialize the grid with cells that have the tile options
    private void InitializeGrid() {
        for(int y = 0; y < dimensions; y++) {
            for(int x = 0; x < dimensions; x++) {
                // Create a new cell at the specified position (x, y)
                Cell newCell = Instantiate(cellObj, new Vector3(x, 0, y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }
        // Start the wave function collapse algorithm
        StartCoroutine(CheckEntropy());
    }

    // Check the entropy of the grid to find the cell with the least amount of options
    private IEnumerator CheckEntropy() {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        tempGrid.RemoveAll(c => c.collapsed); // Remove all cells that have already been collapsed
        tempGrid.Sort((a, b) => a.tileOptions.Length - b.tileOptions.Length); // Sort the cells by the amount of tile options
        tempGrid.RemoveAll(a => a.tileOptions.Length != tempGrid[0].tileOptions.Length); // Remove all cells that have more tile options than the cell with the least amount of options

        yield return new WaitForSeconds(0.025f); // Wait for a short amount of time before collapsing the cell

        CollapseCell(tempGrid);
    }

    // Collapse a random cell from the list of cells with the least amount of options
    private void CollapseCell(List<Cell> tempGrid) {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        try {
            // Select a random tile from the list of tile options
            Tile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        } catch {
            // If there are no tile options left, use the backup tiles
            Tile selectedTile = backupTile;
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }

        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, foundTile.transform.rotation);

        UpdateGeneration();
    }

    // Updates the grid by recalculating the tile options for each non-collapsed cell based on its neighbors
    private void UpdateGeneration() {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for(int y = 0; y < dimensions; y++) {
            for(int x = 0; x < dimensions; x++) {
                var index = x + y * dimensions;

                if (gridComponents[index].collapsed) {
                    // if the cell has already been collapsed, keep the cell as is
                    newGenerationCell[index] = gridComponents[index];
                } else {
                    // Recalculate the tile options for the cell based on its neighbors
                    List<Tile> options = new List<Tile>();
                    foreach(Tile t in tileObjects) {
                        options.Add(t);
                    }

                    // Check the up neighbor
                    if(y > 0) {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach(Tile possibleOptions in up.tileOptions) {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].downNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    // Check the left neighbor
                    if(x < dimensions - 1) {
                        Cell left = gridComponents[x + 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach(Tile possibleOptions in left.tileOptions) {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].rightNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    // Check the down neighbor
                    if (y < dimensions - 1) {
                        Cell down = gridComponents[x + (y+1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in down.tileOptions) {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].upNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    // Check the right neighbor
                    if (x > 0) {
                        Cell right = gridComponents[x - 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in right.tileOptions) {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].leftNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }
                    
                    // Update the cell with the new set of valid tile options
                    Tile[] newTileList = new Tile[options.Count];

                    for(int i = 0; i < options.Count; i++) {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iteration++;

        // If the iteration is less than the total number of cells, continue collapsing cells
        if (iteration < dimensions * dimensions) {
            StartCoroutine(CheckEntropy());
        }
    }

    // Checks the validity of the available tile options by comparing them to the valid options from neighboring tiles
    private void CheckValidity(List<Tile> optionList, List<Tile> validOption) {
        for(int x = optionList.Count - 1; x >=0; x--) {
            var element = optionList[x];
            
            // If the option is not in the list of valid neighbor tiles, remove it from the options list
            if (!validOption.Contains(element)) {
                optionList.RemoveAt(x);
            }
        }
    }
}