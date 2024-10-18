using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    // Setting up the possible neighbors for each tile
    [Header("Tile Neighbors Lists")]
    public Tile[] upNeighbors;
    public Tile[] rightNeighbors;
    public Tile[] downNeighbors;
    public Tile[] leftNeighbors;
}
