using Com.Ryuuguu.HexGridCC;
using UnityEngine;

/// <summary>
/// Make a hex of superHexRadius
/// 
/// </summary>
public class UitHexGridExample : UitHexGrid {
    
    public int superHexRadius = 1;

    /// <summary>
    /// Makes all hexes in a given local space
    /// </summary>
    public void SetupHexes() {
        //make a hex or hexes 
        // FYI Construct returns a list of the hexes it makes as well as storing them in the local space
        cubeCoordinates.Construct(superHexRadius);
        MakeAllHexes(localSpaceId);
    }
    
}