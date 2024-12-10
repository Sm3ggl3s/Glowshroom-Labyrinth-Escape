using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cinemachine;

public class WaveFunctionCollapse : MonoBehaviour {

    [Header("Maze Settings")]
    [SerializeField] private int dimensions;
    [SerializeField] private Tile[] tileObjects;
    private Vector3 endLocation;
    private float endReachThreshold = 10f; // The distance threshold for the player to reach the end cell

    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private GameObject startPrefab;


    [Header("Grid Settings")]
    [SerializeField] private List<Cell> gridComponents;
    [SerializeField] private Cell cellObj;

    [SerializeField] private Tile backupTile;

    [Header("Border Cell Settings")]
    [SerializeField] private Tile[] topTiles;
    [SerializeField] private Tile[] bottomTiles;
    [SerializeField] private Tile[] leftTiles;
    [SerializeField] private Tile[] rightTiles;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineFreeLook freeLookCamera; // Reference to the FreeLook Camera
    [SerializeField] private string lookAtChildName = "LookAt";

    private GameObject player; // Reference to the instantiated player object
    private int iteration;

    private void Awake() {
        gridComponents = new List<Cell>();
        GUIManager.Instance.ShowLoadingScreen(); // Show the loading screen
        InitializeGrid();
    }

    private void Update() {
        CheckIfPlayerReachedEnd();
    }

    

    // Initialize the grid with cells that have the tile options
    private void InitializeGrid() {
        for(int y = 0; y < dimensions; y++) {
            for(int x = 0; x < dimensions; x++) {
                // Create a new cell at the specified position (x, y)
                Cell newCell = Instantiate(cellObj, new Vector3(x * 20f, 0, y * 20f), Quaternion.identity);

                // If the cell is on the border, set the tile options to the border tiles
                if (y == 0) {
                    // Top row
                    newCell.CreateCell(false, topTiles);
                } else if (y == dimensions - 1) {
                    // Bottom row
                    newCell.CreateCell(false, bottomTiles);
                } else if (x == 0) {
                    // Left column
                    newCell.CreateCell(false, leftTiles);
                } else if (x == dimensions - 1) {
                    // Right column 
                    newCell.CreateCell(false, rightTiles);
                } else {
                    newCell.CreateCell(false, tileObjects);
                }

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

        // Update progress bar based on the iteration and total number of cells
        float progress = (float)iteration / (dimensions * dimensions);
        GUIManager.Instance.UpdateLoadingProgress(progress);
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
        } else {
            // If all cells have been collapsed, spawn the player at a random start cell
            SpawnPlayerAtRandomStartCell();

            // Set a random cell as the end cell
            SetEndCell();

            // Hide the loading screen after a short delay
            StartCoroutine(WaitAndHideLoadingScreen());
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

    // Check if the player has reached the end
    private void CheckIfPlayerReachedEnd() {
        if (player != null && endLocation != Vector3.zero) {
            float distance = Vector3.Distance(player.transform.position, endLocation);
            Debug.Log($"Distance from player to end: {distance} (Threshold: {endReachThreshold})");

            if (distance < endReachThreshold) {
                OnPlayerReachedEnd();
            }
        }
    }

    // Called when the player reaches the end
    private void OnPlayerReachedEnd() {
        Debug.Log("Player has reached the end!");
        // Handle the end game logic, like transitioning to a new level or showing a message.
    }

    // Place the player at a random spawn cell
    private void SpawnPlayerAtRandomStartCell() {
        // Get all valid spawn cells
        List<Cell> validStartCells = gridComponents.Where(cell => cell.collapsed).ToList();

        // Debug error if no valid cells are available
        if (validStartCells.Count == 0) {
            Debug.LogError("No valid cells available to spawn the player.");
            return;
        }

        // Choose a random cell
        int randomIndex = UnityEngine.Random.Range(0, validStartCells.Count);
        Cell startCell = validStartCells[randomIndex];

        // Calculate the spawn position slightly above the cell
        Vector3 spawnLocation = startCell.transform.position + new Vector3(0, 2f, 0);

        // Spawn the player prefab at the cell's position
        player = Instantiate(playerPrefab, spawnLocation, Quaternion.identity);

        Vector3 entranceSpawnLocation = startCell.transform.position + new Vector3(0, 7f, 0);

        // Instantiate the start prefab at the player's position
        Instantiate(startPrefab, entranceSpawnLocation, Quaternion.identity);

        Debug.Log($"Player spawned at: {spawnLocation}");

        // Set the Cinemachine FreeLook camera's LookAt target to the player's transform
        if (freeLookCamera != null) {
            freeLookCamera.Follow = player.transform;

            // Find the LookAt child object in the player prefab
            Transform lookAtTarget = player.transform.Find(lookAtChildName);
            if (lookAtTarget != null) {
                freeLookCamera.LookAt = lookAtTarget;
            } else {
                Debug.LogError($"LookAt target '{lookAtChildName}' not found as a child of the player prefab.");
            }
        } else {
            Debug.LogError("FreeLookCamera is not assigned in the inspector.");
        }

        // Show the health bar UI'
        GUIManager.Instance.ShowHealthBarAfterLoading();
    }

    // Set a random cell to be the end cell
    private void SetEndCell() {
        List<Cell> validEndCells = gridComponents.Where(cell => cell.collapsed).ToList();
        if (validEndCells.Count == 0) {
            Debug.LogError("No valid cells available to set as the end cell.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, validEndCells.Count);
        Cell endCell = validEndCells[randomIndex];
        endLocation = endCell.transform.position + new Vector3(0, 4f, 0); // Set the end location slightly above the end cell

        // Instantiate the exit prefab at the end cell's position
        Instantiate(exitPrefab, endLocation, Quaternion.identity);

        Debug.Log($"End cell set at: {endLocation}");
    }

    // Return the end location for the player to reach
    public Vector3 GetEndLocation() {
        return endLocation;
    }

    // Coroutine to wait for a short amount of time before hiding the loading screen
    private IEnumerator WaitAndHideLoadingScreen() {
        yield return new WaitForSeconds(2f);
        GUIManager.Instance.HideLoadingScreen(); // Hide the loading screen
    }
}