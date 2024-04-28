using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

// NOTE FOR WHOLE PROGRAM:
// Defining "Class VariableName = some class" is by default a pointer in C#
// E.g "TileData selectedTile = StaticData.tileArr[i, j];"
// Changing the selectedTile will also change StaticData.tileArr[i, j]

public class GameManager : MonoBehaviour
{
    // Object reference to spawn the tiles into the scene
    [SerializeField] private Transform tilePrefab;

    // Reference position for the tiles to spawn in
    [SerializeField] private Transform board;

    // Tells the board on how to place tiles based on their size
    [SerializeField] private float tileSize;

    // To allow displaying the timer on the screen
    [SerializeField] private TMP_Text timerText;

    // To allow displaying how much of each power up the user has.
    [SerializeField] private TMP_Text[] PowerUpNoText;

    // Text that allows the user to see how many bombs are correctly flagged (Defusing Report)
    [SerializeField] private GameObject DefusingRepInfoText;
    [SerializeField] private TMP_Text FlaggedInfoText;
    [SerializeField] private TMP_Text NumberOfBombsText;

    // To allow the program to disable the buttons while using "Antibomb"
    [SerializeField] private Button[] PowerUpButtons;
    [SerializeField] private Animator SceneTransition;
    [SerializeField] private Animator CameraShake;
    
    // To allow for the code to change the tile sprites
    // based on tile information and user input
    public TileScript[,] tileObjRef;

    // Specifies how many tiles in 1 row
    private int width;

    // Specifies how many tiles in 1 column
    private int height;

    // Specifies how many bombs are to be placed on the board
    private int noOfBombs;
    public bool stopInteraction;
    bool endedGame = false;

    // Variable to track if the user is using the "Antibomb"
    [HideInInspector] public bool usingAntiBomb = false;

    // Start is called before the first frame update
    void Start()
    {
        // Creating a function to create an initial game board, input width: 15, height: 15, number of bombs: 35
        
        width = 15;
        height = 15;
        noOfBombs = 30;

        NumberOfBombsText.text = "Total Bombs: " + noOfBombs;

        Random.InitState((int) System.DateTime.Now.Ticks);
        
        // Allows the program to control the sprites of tiles
        tileObjRef = new TileScript[width, height];

        // If the game is to be reset (Not going back from question page)
        if (StaticData.reset) {
            // All data is reset to initial status
            StaticData.tileArr = new TileData[width, height];
            StaticData.userFirstInput = false;
            StaticData.timer = 0;
            StaticData.won = false;
            StaticData.noOfFlags = 0;
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
        for (int i = 0; i < 3; i++) {
            if (StaticData.reset) {
                // If game is to be reset, powerups user has should be 0
                StaticData.PowerUpNo[i] = 0;
            }
            // Displaying how many powerups the user has on screen
            PowerUpNoText[i].text = StaticData.PowerUpNo[i].ToString() + "x";
        }

        StaticData.reset = true;
        stopInteraction = false;
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
            timerText.text = minutes.ToString() + "MIN " + seconds.ToString() + "S";
        }
    }

    void createGameBoard() {
        float xPos;
        float yPos;

        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                // Create a new tile for each row and column
                Transform tile = Instantiate(tilePrefab);
                
                // Each tile needs to be under another game object to allow them
                // To be used collectively (Setting up the 2D array of bricks)
                tile.parent = board;
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
                tileObjRef[row, col] = tile.GetComponent<TileScript>();

                // Info on the row and column of the tile is passed to it
                // Mainly for when clicked, can pass the info into OpenTile, FlagTile, etc funcs 
                tileObjRef[row, col].tileRow = row;
                tileObjRef[row, col].tileCol = col;

                tileObjRef[row, col].spriteRenderer = tileObjRef[row, col].GetComponent<SpriteRenderer>();

                if (StaticData.reset) {
                    // If reset, then the information on tiles should also be reset.
                    StaticData.tileArr[row, col] = new TileData();
                }
            }
        }
    }

    void DisplayBoard() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                // Getting reference to the data on the tile
                TileData selectedTile = StaticData.tileArr[row, col];

                // Getting reference to the script of the object
                // To allow changing sprite
                TileScript selectedTileObj = tileObjRef[row, col];

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

        int bombsPlaced = 0;
        // Keeps track of how many mines are set

        while (bombsPlaced < noOfBombs) {
            // Generating random position for a new bomb position
            int bombRow = Random.Range(0, height);
            int bombCol = Random.Range(0, width);

            // Getting reference to the data on the tile
            // Note: THIS IS A POINTER, NOT A COPY
            TileData selectedTile = StaticData.tileArr[bombRow, bombCol];

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
                TileData selectedTile = StaticData.tileArr[row, col];

                // Checks only if the tile isn't a bomb
                if (!selectedTile.hasBomb) {
                    // Loops through adjacent tiles
                    for (int i = row - 1; i <= row + 1; i++) {
                        for (int j = col - 1; j <= col + 1; j++) {
                            // Checks if the position is valid first
                            // And then checks if the tile has a bomb
                            if (isValidPos(i, j) && StaticData.tileArr[i, j].hasBomb) {
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
                    TileData selectedTile = StaticData.tileArr[i, j];

                    if (!selectedTile.revealed) {
                        OpenTile(i, j);
                    }
                }
            }
        }
    }

    void OnFirstClick(int row, int col) {
        for (int i = row - 1; i <= row + 1; i++) {
            for (int j = col - 1; j <= col + 1; j++) {
                if (isValidPos(i, j)) {
                    StaticData.tileArr[i, j].safeBcFClick = true;
                }
            }
        }

        SetMines();
        SetNumbersOnTiles();
    }

    public void OpenTile(int tileRow, int tileCol) {
        DefusingRepInfoText.SetActive(false);
        
        TileData selectedTile = StaticData.tileArr[tileRow, tileCol];
        TileScript selectedTileObj = tileObjRef[tileRow, tileCol];

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

            for (int i = 0; i < 3; i++) {
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
        TileData selectedTile = StaticData.tileArr[tileRow, tileCol];
        TileScript selectedTileObj = tileObjRef[tileRow, tileCol];

        DefusingRepInfoText.SetActive(false);

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

        StaticData.won = true;

        while (StaticData.won && tileRow < height) {
            for (int tileCol = 0; tileCol < width; tileCol++) {
                TileData selectedTile = StaticData.tileArr[tileRow, tileCol];

                if (selectedTile.hasBomb) {
                    if (!selectedTile.flagged) {
                        StaticData.won = false;
                    }
                } else if (!selectedTile.revealed) {
                    StaticData.won = false;
                }
                
            }
            tileRow += 1;
        }

        FlaggedInfoText.text = "Flagged: " + StaticData.noOfFlags.ToString();

        if (StaticData.won) {
            stopInteraction = true;
            endedGame = true;

            for (int i = 0; i < 3; i++) {
                PowerUpButtons[i].interactable = false;
            }

            StartCoroutine(MoveToWinScreen());
        }
    }

    // Powerup Functionality

    public void UseAntiBomb(int useRow, int useCol) {
        usingAntiBomb = false;

        for (int i = 0; i < 3; i++) {
            PowerUpButtons[i].interactable = true;
        }

        for (int i = useRow - 1; i <= useRow + 1; i++) {
            for (int j = useCol - 1; j <= useCol + 1; j++) {
                if (isValidPos(i, j)) {
                    TileData selectedTile = StaticData.tileArr[i, j];
                    TileScript selectedTileObj = tileObjRef[i, j];

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
    }

    public void AntiBombClicked() {
        if (StaticData.PowerUpNo[0] > 0) {
            StaticData.PowerUpNo[0] -= 1;

            PowerUpNoText[0].text = StaticData.PowerUpNo[0].ToString() + "x";

            usingAntiBomb = true;

            for (int i = 0; i < 3; i++) {
                PowerUpButtons[i].interactable = false;
            }
        }
    }

    public void UseBombFlagger() {
        if (StaticData.PowerUpNo[1] > 0) {
            DefusingRepInfoText.SetActive(false);
            
            StaticData.PowerUpNo[1] -= 1;
            FindObjectOfType<AudioManager>().PlaySound("Flag");

            PowerUpNoText[1].text = StaticData.PowerUpNo[1].ToString() + "x";

            int bombsFlagged = 0;

            int tileRow = 0;
            int tileCol = 0;

            while (bombsFlagged < 2 && tileRow < height) {
                if (StaticData.tileArr[tileRow, tileCol].hasBomb && !StaticData.tileArr[tileRow, tileCol].flagged) {
                    FlagTile(tileRow, tileCol);

                    bombsFlagged += 1;
                }

                tileCol += 1;

                if (tileCol == width) {
                    tileCol = 0;
                    tileRow += 1;
                }
            }
        }
    }

    public void UseDefusingReport() {
        if (StaticData.PowerUpNo[2] > 0) {
            int noOfBombsCorrectlyFlagged = 0;

            StaticData.PowerUpNo[2] -= 1;

            PowerUpNoText[2].text = StaticData.PowerUpNo[2].ToString() + "x";

            for (int row = 0; row < height; row++) {
                for (int col = 0; col < width; col++) {
                    if (StaticData.tileArr[row, col].hasBomb && StaticData.tileArr[row, col].flagged) {
                        noOfBombsCorrectlyFlagged += 1;
                    }
                }
            }

            DefusingRepInfoText.SetActive(true);

            TMP_Text tmpComponent = DefusingRepInfoText.GetComponent<TMP_Text>();

            tmpComponent.text = "Bombs correctly flagged: " + noOfBombsCorrectlyFlagged.ToString();
        }
    }

    public IEnumerator Questioning() {
        StaticData.reset = false;
        stopInteraction = true;

        for (int i = 0; i < 3; i++) {
            PowerUpButtons[i].interactable = true;
        }

        SceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Question");
    }

    public IEnumerator RevealBombs() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                if (!StaticData.tileArr[row, col].revealed) {
                    if (StaticData.tileArr[row, col].hasBomb && !StaticData.tileArr[row, col].flagged) {
                        yield return new WaitForSeconds(0.5f);
                        tileObjRef[row, col].ChangeSprite(0, true);
                        FindObjectOfType<AudioManager>().PlaySound("Boom");
                    } else if (!StaticData.tileArr[row, col].hasBomb && StaticData.tileArr[row, col].flagged) {
                        yield return new WaitForSeconds(0.5f);
                        tileObjRef[row, col].ChangeSprite(3, false);
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
}
