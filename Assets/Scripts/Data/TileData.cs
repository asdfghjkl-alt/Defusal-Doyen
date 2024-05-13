public class TileData
{
    // Tells the program if the tile is flagged
    public bool flagged = false;

    // Tells the program whether the tile is revealed/opened
    public bool revealed = false;

    // Tells the program whether the tile has a bomb or not
    public bool hasBomb = false;

    // Variable determining how many bombs are adjacent to the tile
    public int bombsAdjacent = -1;

    // Tells program to ignore the tiles adjacent to the first tile revealed
    // in placing bombs
    public bool safeBcFClick = false;
}