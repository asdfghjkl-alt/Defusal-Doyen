public class TileData
{
    public bool flagged = false; // Initially tile isn't flagged
    public bool revealed = false; // Initially tile is unrevealed
    public bool hasBomb = false; // Whether the tile has a bomb or not
    public int bombsAdjacent = -1; // Variable determining how many bombs are adjacent to the tile
    public bool safeBcFClick = false; // Tiles adjacent to clicked first tile are safe (no mines)
}