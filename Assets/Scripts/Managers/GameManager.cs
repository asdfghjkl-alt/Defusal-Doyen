using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

// NOTE FOR WHOLE PROGRAM:
// Defining "Class VariableName = some class" is by default a pointer in C#
// E.g "TileData selectedTile = StaticData.TileArr[i, j];"
// Changing the selectedTile will also change StaticData.TileArr[i, j]

public class GameManager : MonoBehaviour
{
    // Object reference to spawn the tiles into the scene
    [SerializeField] private Transform TilePrefab;

    // Reference position for the tiles to spawn in
    [SerializeField] private Transform Board;

    // Tells the board on how to place tiles based on their size
    [SerializeField] private float tileSize;

    // To allow displaying the timer on the screen
    [SerializeField] private TMP_Text TimerText;

    // To allow displaying how much of each power up the user has.
    [SerializeField] private TMP_Text[] PowerUpNoText;

    // Text that allows the user to see how many bombs are correctly flagged (Defusing Report)
    [SerializeField] private TMP_Text FlaggedInfoText;
    [SerializeField] private TMP_Text NumberOfBombsText;
    [SerializeField] private TMP_Text BombTestersUsedText;

    // To allow the program to disable the buttons while using "Antibomb"
    [SerializeField] private Button[] PowerUpButtons;
    [SerializeField] private Animator SceneTransition;
    [SerializeField] private Animator CameraShake;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int noOfBombs;
    
    // To allow for the code to change the tile sprites
    // based on tile information and user input
    public TileScript[,] TileObjRef;

    [HideInInspector] public bool stopInteraction = false;
    [HideInInspector] public bool endedGame = false;

    // Variable to track if the user is using the "Antibomb"
    [HideInInspector] public bool usingAntiBomb = false;
    [HideInInspector] public bool usingPlaneCho = false;
    [HideInInspector] public int bombTestersUsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Creating a function to create an initial game board, input width: 15, height: 15, number of bombs: 30

        NumberOfBombsText.text = "Total Bombs: " + noOfBombs;
        
        // Allows the program to control the sprites of tiles
        TileObjRef = new TileScript[width, height];

        // If the game is to be reset (Not going back from question page)
        if (StaticData.reset) {
            // All data is reset to initial status
            StaticData.TileArr = new TileData[width, height];
            StaticData.userFirstInput = false;
            StaticData.timer = 0;
            StaticData.noOfFlags = 0;

            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }
        }

        // Creating the board
        createGameBoard();

        // If it is going back from the question page
        // Then the tile sprites should be updated to the previous board
        if (!StaticData.reset) {
            FlaggedInfoText.text = "Flagged: " + StaticData.noOfFlags.ToString();
            DisplayBoard();
        }

        // Code to display how many of each powerup the user has
        for (int i = 0; i < 4; i++) {
            if (StaticData.reset) {
                // If game is to be reset, powerups user has should be 1
                StaticData.PowerUpNo[i] = 1;
            }
            // Displaying how many powerups the user has on screen
            PowerUpNoText[i].text = StaticData.PowerUpNo[i].ToString() + "x";
        }

        StaticData.reset = true;
    }

    // This is a default function in unity that calls once every 0.02s
    void FixedUpdate() {
        if (!endedGame){
            StaticData.timer += Time.fixedDeltaTime;
            // Updating the timer

            // Rounding the timer to allow calculations of minutes and seconds
            int roundedTimer = (int)StaticData.timer;

            // Calculation for minutes and seconds
            int minutes = roundedTimer / 60;
            int seconds = roundedTimer % 60;
            
            // Displaying the minutes and seconds
            TimerText.text = minutes.ToString() + "MIN " + seconds.ToString() + "S";
        }
    }

    void createGameBoard() {
        float xPos;
        float yPos;

        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                // Create a new tile for each row and column
                Transform tile = Instantiate(TilePrefab);
                
                // Each tile needs to be under another game object to allow them
                // To be used collectively (Setting up the 2D array of bricks)
                tile.parent = Board;
                // Centre is at (0, 0)
                // E.g. 3x3 grid, at column 0, x Position will be at -1, aka -float(width - 1) / 2
                // As column increases by 1, x Position will also have to add 1, hence add the column to x Position
                xPos = col - (width - 1) / 2;

                // E.g. 3x3 grid, at row 0, y Position will be at 1, hence float(width - 1) / 2
                // As row increases by 1, y Position will decrease by 1, hence sutracting the row to y Position
                yPos = (height - 1) / 2 - row;

                // Setting actual position in terms of tile's dimensions
                tile.localPosition = new Vector2(xPos * tileSize, yPos * tileSize);

                // Getting a reference to the component to allow for changing sprites in program
                TileObjRef[row, col] = tile.GetComponent<TileScript>();

                // Info on the row and column of the tile is passed to it
                // Mainly for when clicked, can pass the info into OpenTile, FlagTile, etc funcs 
                TileObjRef[row, col].tileRow = row;
                TileObjRef[row, col].tileCol = col;

                TileObjRef[row, col].spriteRenderer = TileObjRef[row, col].GetComponent<SpriteRenderer>();

                if (StaticData.reset) {
                    // If reset, then the information on tiles should also be reset.
                    StaticData.TileArr[row, col] = new TileData();
                }
            }
        }
    }

    void DisplayBoard() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                // Getting reference to the data on the tile
                TileData selectedTile = StaticData.TileArr[row, col];

                // Getting reference to the script of the object
                // To allow changing sprite
                TileScript selectedTileObj = TileObjRef[row, col];

                // Choosing what sprite to change the tile to based on 
                // Its information

                // Check the TileScript ChangeSprite() function for more info
                if (selectedTile.revealed) {
                    selectedTileObj.ChangeSprite(1, false);
                } else if (selectedTile.flagged) {
                    selectedTileObj.ChangeSprite(2, false);
                } else {
                    selectedTileObj.ChangeSprite(3, false);
                }
            }
        }
    }

    void SetMines() {
        // Note: Mines are set AFTER first click

        Random.InitState((int) System.DateTime.Now.Ticks);

        int bombsPlaced = 0;
        // Keeps track of how many mines are set

        while (bombsPlaced < noOfBombs) {
            // Generating random position for a new bomb position
            int bombRow = Random.Range(0, height);
            int bombCol = Random.Range(0, width);

            // Getting reference to the data on the tile
            // Note: THIS IS A POINTER, NOT A COPY
            TileData selectedTile = StaticData.TileArr[bombRow, bombCol];

            // Bomb positions cannot be adjacent to the tile the user first clicked
            // Nor at a position that already has a bomb
            if (!selectedTile.hasBomb && !selectedTile.safeBcFClick) {
                // Sets the tile to have a bomb
                selectedTile.hasBomb = true;

                // One more bomb has been placed
                bombsPlaced += 1;
            }
        }
    }

    void SetNumbersOnTiles() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                // Keeping track of how many bombs are near/adjacent the tile
                int bombsNearTile = 0;
                
                // Getting reference to the data on the tile
                // Note: THIS IS A POINTER, NOT A COPY
                TileData selectedTile = StaticData.TileArr[row, col];

                // Checks only if the tile isn't a bomb
                if (!selectedTile.hasBomb) {
                    // Loops through adjacent tiles
                    for (int i = row - 1; i <= row + 1; i++) {
                        for (int j = col - 1; j <= col + 1; j++) {
                            // Checks if the position is valid first
                            // And then checks if the tile has a bomb
                            if (isValidPos(i, j) && StaticData.TileArr[i, j].hasBomb) {
                                // One more adjacent tile has a bomb
                                bombsNearTile += 1;
                            }
                        }
                    }
                    
                    // Keeps information in the array (through pointer)
                    selectedTile.bombsAdjacent = bombsNearTile;
                }
            }
        }
    }

    bool isValidPos(int _row, int _col) {
        // Checking if the row and column are in range of board
        if (_row < 0 || _row > height - 1 || _col < 0 || _col > width - 1) {
            return false;
        } else {
            return true;
        }
    }

    public void OpenAdjTiles(int _row, int _col) {
        for (int i = _row - 1; i <= _row + 1; i++) {
            for (int j = _col - 1; j <= _col + 1; j++) {
                if (isValidPos(i, j)) {
                    TileData selectedTile = StaticData.TileArr[i, j];

                    if (!selectedTile.revealed) {
                        OpenTile(i, j);
                    }
                }
            }
        }
    }

    void OnFirstClick(int row, int col) {
        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = true;
        }

        for (int i = row - 1; i <= row + 1; i++) {
            for (int j = col - 1; j <= col + 1; j++) {
                if (isValidPos(i, j)) {
                    StaticData.TileArr[i, j].safeBcFClick = true;
                }
            }
        }

        SetMines();
        SetNumbersOnTiles();
    }

    public void OpenTile(int tileRow, int tileCol) {
        TileData selectedTile = StaticData.TileArr[tileRow, tileCol];
        TileScript selectedTileObj = TileObjRef[tileRow, tileCol];

        selectedTile.revealed = true;

        if (selectedTile.flagged) {
            StaticData.noOfFlags -= 1;
            selectedTile.flagged = false;
        }

        if (!StaticData.userFirstInput) {
            StaticData.userFirstInput = true;

            OnFirstClick(tileRow, tileCol);
        }

        if (selectedTile.hasBomb) {
            selectedTileObj.ChangeSprite(0, true);

            FindObjectOfType<AudioManager>().PlaySound("Boom");

            stopInteraction = true;
            endedGame = true;

            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }

            StartCoroutine(RevealBombs());
        } else {
            selectedTileObj.ChangeSprite(1 ,true);
            if (selectedTile.bombsAdjacent == 0) {
                OpenAdjTiles(tileRow, tileCol);
            }
        }
    }

    public void FlagTile(int tileRow, int tileCol) {
        TileData selectedTile = StaticData.TileArr[tileRow, tileCol];
        TileScript selectedTileObj = TileObjRef[tileRow, tileCol];

        selectedTile.flagged = !selectedTile.flagged;

        if (selectedTile.flagged) {
            StaticData.noOfFlags += 1;
            selectedTileObj.ChangeSprite(2, false);
        } else {
            StaticData.noOfFlags -= 1;
            selectedTileObj.ChangeSprite(3, false);
        }

        FlaggedInfoText.text = "Flagged: " + StaticData.noOfFlags.ToString();
    }

    public void CheckWinCondition() {
        int tileRow = 0;

        bool hasWon = true;

        while (hasWon && tileRow < height) {
            for (int tileCol = 0; tileCol < width; tileCol++) {
                TileData selectedTile = StaticData.TileArr[tileRow, tileCol];

                if (selectedTile.hasBomb) {
                    if (!selectedTile.flagged) {
                        hasWon = false;
                    }
                } else if (!selectedTile.revealed) {
                    hasWon = false;
                }
                
            }
            tileRow += 1;
        }

        FlaggedInfoText.text = "Flagged: " + StaticData.noOfFlags.ToString();

        if (hasWon) {
            stopInteraction = true;
            endedGame = true;

            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }

            StartCoroutine(MoveToWinScreen());
        }
    }

    // Powerup Functionality

    public void AntiBombClicked() {
        if (StaticData.PowerUpNo[0] > 0) {
            StaticData.PowerUpNo[0] -= 1;

            PowerUpNoText[0].text = StaticData.PowerUpNo[0].ToString() + "x";

            usingAntiBomb = true;

            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }
        }
    }

    public void UseAntiBomb(int useRow, int useCol) {
        usingAntiBomb = false;

        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = true;
        }

        for (int i = useRow - 1; i <= useRow + 1; i++) {
            for (int j = useCol - 1; j <= useCol + 1; j++) {
                if (isValidPos(i, j)) {
                    TileData selectedTile = StaticData.TileArr[i, j];
                    TileScript selectedTileObj = TileObjRef[i, j];

                    if (!selectedTile.revealed) {
                        if (!selectedTile.hasBomb) {
                            OpenTile(i, j);
                        } else {
                            if (!selectedTile.flagged) {
                                StaticData.noOfFlags += 1;
                                selectedTile.flagged = true;
                                selectedTileObj.ChangeSprite(2, false);
                            }
                        }
                    }
                }
            }
        }

        CheckWinCondition();
    }

    public void UseBombFlagger() {
        if (StaticData.PowerUpNo[1] > 0) {
            StaticData.PowerUpNo[1] -= 1;
            FindObjectOfType<AudioManager>().PlaySound("Flag");

            PowerUpNoText[1].text = StaticData.PowerUpNo[1].ToString() + "x";

            int bombsFlagged = 0;

            int tileRow = 0;
            int tileCol = 0;

            while (bombsFlagged < 2 && tileRow < height) {
                if (StaticData.TileArr[tileRow, tileCol].hasBomb && !StaticData.TileArr[tileRow, tileCol].flagged) {
                    FlagTile(tileRow, tileCol);

                    bombsFlagged += 1;
                }

                tileCol += 1;

                if (tileCol == width) {
                    tileCol = 0;
                    tileRow += 1;
                }
            }

            CheckWinCondition();
        }
    }

    public void BombTesterClicked() {
        if (StaticData.PowerUpNo[2] > 0) {
            StaticData.PowerUpNo[2] -= 1;

            PowerUpNoText[2].text = StaticData.PowerUpNo[2].ToString() + "x";

            bombTestersUsed += 1;

            BombTestersUsedText.text = "Bomb Testers being used: " + bombTestersUsed.ToString();

            for (int i = 0; i < 2; i++) {
                PowerUpButtons[i].interactable = false;
            }
            PowerUpButtons[3].interactable = false;
        }
    }

    public void UseBombTester(int useRow, int useCol) {
        bombTestersUsed -= 1;

        BombTestersUsedText.text = "Bomb Testers being used: " + bombTestersUsed.ToString();

        if (bombTestersUsed == 0) {
            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = true;
            }
        }

        if (StaticData.TileArr[useRow, useCol].hasBomb) {
            FlagTile(useRow, useCol);
        } else {
            OpenTile(useRow, useCol);
        }

        CheckWinCondition();
    }

    public void PlaneChoClicked() {
        if (StaticData.PowerUpNo[3] > 0) {
            StaticData.PowerUpNo[3] -= 1;

            PowerUpNoText[3].text = StaticData.PowerUpNo[3].ToString() + "x";

            usingPlaneCho = true;

            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }
        }
    }

    public void UsePlaneCho(int useCol) {
        
        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = true;
        }

        usingPlaneCho = false;

        for (int row = 0; row < height; row++) {
            TileData selectedTile = StaticData.TileArr[row, useCol];

            if (!selectedTile.revealed) {
                if (selectedTile.hasBomb ) {
                    if (!selectedTile.flagged) {
                        FlagTile(row, useCol);
                    }
                } else {
                    OpenTile(row, useCol);
                }
            }
        }

        CheckWinCondition();
    }

    public IEnumerator Questioning() {
        StaticData.reset = false;
        stopInteraction = true;

        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = true;
        }

        SceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Question");
    }

    public IEnumerator RevealBombs() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                if (!StaticData.TileArr[row, col].revealed) {
                    if (StaticData.TileArr[row, col].hasBomb && !StaticData.TileArr[row, col].flagged) {
                        yield return new WaitForSeconds(0.5f);
                        TileObjRef[row, col].ChangeSprite(0, true);
                        FindObjectOfType<AudioManager>().PlaySound("Boom");
                    } else if (!StaticData.TileArr[row, col].hasBomb && StaticData.TileArr[row, col].flagged) {
                        yield return new WaitForSeconds(0.5f);
                        TileObjRef[row, col].ChangeSprite(3, false);
                        FindObjectOfType<AudioManager>().PlaySound("Flag");
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        SceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("End Game Lose");
    }

    public IEnumerator MoveToWinScreen() {
        FindObjectOfType<AudioManager>().PlaySound("Correct");
        yield return new WaitForSeconds(0.5f);

        SceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("End Game Win");
    }

    public void ChangeColLights(int useCol, bool turnOn) {
        if (turnOn) {
            for (int row = 0; row < height; row++) {
                if (!StaticData.TileArr[row, useCol].revealed) {
                    TileObjRef[row, useCol].ChangeTileLight(Color.green, 0.9f, 2, 3);
                }
            }
        } else {
            for (int row = 0; row < height; row++) {
                if (!StaticData.TileArr[row, useCol].revealed) {
                    TileObjRef[row, useCol].ReturnTileNormal();
                }
            }
        }
    }
}
