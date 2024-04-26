using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{
    public static float timer;
    public static bool reset = true;
    public static int noOfFlags = 0;
    public static TileData[,] tileArr;
    public static bool userFirstInput = false;
    public static int[] PowerUpNo = new int[3];
    public static bool won = false;
    public static bool endedGame = false;
}
