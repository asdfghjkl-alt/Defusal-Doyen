using UnityEngine;

// This class contains data that is able to be moved between scenes without resetting
public class StaticData : MonoBehaviour
{
    // Tracks how long the user takes to complete game
    public static float timer;

    // Tells program whether to reset the data in the class
    // E.g. If entering from Home Page, then reset
    // But if game goes into Question Page, then reset flag will be set to false
    public static bool reset = true;

    // Tracks how many flags the user has placed
    // Static to not have to recount flags when reentering game from Question Page
    public static int noOfFlags = 0;

    // Tracks data on all the tiles using a Class for attributes (Check TileData.cs)
    // Data on tiles shouldn't be reset when going to and from the Question Page
    public static TileData[,] TileArr;

    // Tells the program whether the user has made their first input
    // So that the first tile that they click will be safe.
    public static bool userFirstInput = false;

    // Tracks how many of each power up the user has
    public static int[] PowerUpNo = new int[4];

    // Tracks which questions the user has answered
    // True for answered, False for not
    public static bool[] AnsweredQs = new bool[44];

    public static int QuestionsAnswered = 0;

    // Tracks if background music is to be on
    public static bool soundOn = true;

    // Sets the difficulty mode of the game
    public static string difficulty = "E";
}
