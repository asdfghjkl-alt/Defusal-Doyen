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

    // To allow displaying the timer on the screen
    [SerializeField] private TMP_Text TimerText;

    // To allow displaying how much of each power up the user has.
    [SerializeField] private TMP_Text[] PowerUpNoText;

    // Text to see how many flags are on the board
    [SerializeField] private TMP_Text FlaggedInfoText;

    // Text telling user how many bombs are on the board
    [SerializeField] private TMP_Text NumberOfBombsText;

    // Text telling how many bomb testers the user is currently using
    [SerializeField] private TMP_Text BombTestersUsedText;

    // To allow the program to disable the buttons
    [SerializeField] private Button[] PowerUpButtons;

    // To control animation of changing scene
    [SerializeField] private Animator SceneTransition;
    
    // Animation of camera shaking
    [SerializeField] private Animator CameraShake;

    // Tells the board on how to place tiles based on their size
    [SerializeField] private float tileSize;

    // Width of board
    [SerializeField] private int width;

    // Height of board
    [SerializeField] private int height;

    // Number of bombs to place on board
    [SerializeField] private int noOfBombs;
    
    // To allow for the code to change the tile sprites
    // based on tile information and user input
    public TileScript[,] TileObjRef;

    // Variable determining if tiles are non-interactable
    [HideInInspector] public bool stopInteraction = false;

    // Keeps track if game has ended
    [HideInInspector] public bool endedGame = false;

    // Variable to track if the user is using the Anti Bomb
    [HideInInspector] public bool usingAntiBomb = false;

    // Variable to track if the user is using Plane Cho
    [HideInInspector] public bool usingPlaneCho = false;

    // Variable to track how many bomb testers the user is currently using
    [HideInInspector] public int bombTestersUsed = 0;

    // Variable determining if Plane Cho is being used in column or row
    bool isCol = true;

    public int tileRowHover;
    public int tileColHover;

    // Start is called before the first frame update
    void Start()
    {
        // Creating a function to create an initial game board

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
            StaticData.QuestionsAnswered = 0;

            for (int i = 0; i < 44; i++) {
                StaticData.AnsweredQs[i] = false;
            }

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

        // As default, data should reset when leaving scene
        StaticData.reset = true;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Z) && usingPlaneCho) {
            ChangeRowColLights(tileRowHover, tileColHover, false);
            isCol = !isCol;
        }
    }

    // This is a default function in unity that calls once every 0.02s
    void FixedUpdate() {
        if (!endedGame) {
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
                float xPos = col - (width - 1) / 2;

                // E.g. 3x3 grid, at row 0, y Position will be at 1, hence float(width - 1) / 2
                // As row increases by 1, y Position will decrease by 1, hence sutracting the row to y Position
                float yPos = (height - 1) / 2 - row;

                // Setting actual position in terms of tile's dimensions
                tile.localPosition = new Vector2(xPos * tileSize, yPos * tileSize);

                // Getting a reference to the component to allow for changing sprites in program
                TileObjRef[row, col] = tile.GetComponent<TileScript>();

                // Info on the row and column of the tile is passed to it
                // Mainly for when clicked, can pass the info into OpenTile, FlagTile, etc funcs 
                TileObjRef[row, col].tileRow = row;
                TileObjRef[row, col].tileCol = col;

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
                    // Revealed Sprite
                    selectedTileObj.ChangeSprite(1, false);
                } else if (selectedTile.flagged) {
                    // Flagged Sprite
                    selectedTileObj.ChangeSprite(2, false);
                } else {
                    // Unrevealed Sprite
                    selectedTileObj.ChangeSprite(3, false);
                }
            }
        }
    }

    void SetBombs() {
        // Note: Mines are set AFTER first click

        // Makes generation actually random
        Random.InitState((int) System.DateTime.Now.Ticks);

        int bombsPlaced = 0;
        // Keeps track of how many bombs are set

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
        // Loops through adjacent tiles
        for (int i = _row - 1; i <= _row + 1; i++) {
            for (int j = _col - 1; j <= _col + 1; j++) {
                // Checks if tile position is valid
                if (isValidPos(i, j)) {

                    // Gets data on the tile
                    TileData selectedTile = StaticData.TileArr[i, j];

                    if (!selectedTile.revealed) {
                        // If tile hasn't already been revealed, then open the tile
                        OpenTile(i, j);
                    }
                }
            }
        }
    }

    void OnFirstClick(int row, int col) {
        // After opening first tile, power up buttons are interactable
        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = true;
        }

        // Loops through adjacent tiles
        for (int i = row - 1; i <= row + 1; i++) {
            for (int j = col - 1; j <= col + 1; j++) {
                
                // Checks if the position is valid
                if (isValidPos(i, j)) {
                    // Sets those tiles to be safe (i.e. No bombs on them)
                    StaticData.TileArr[i, j].safeBcFClick = true;
                }
            }
        }

        // Sets the bomb positions
        SetBombs();

        // Function to set numbers on tiles
        SetNumbersOnTiles();
    }

    public void OpenTile(int tileRow, int tileCol) {
        // Gets data on the tile
        TileData selectedTile = StaticData.TileArr[tileRow, tileCol];

        // Allows for changing of sprites
        TileScript selectedTileObj = TileObjRef[tileRow, tileCol];
        
        // Tile is now revealed
        selectedTile.revealed = true;

        if (selectedTile.flagged) {
            // Number of flags should decrease by 1
            StaticData.noOfFlags -= 1;

            // Tile is no longer flagged
            selectedTile.flagged = false;
        }

        if (!StaticData.userFirstInput) {
            // If this is the user's first input
            StaticData.userFirstInput = true;

            // Function for user's first click
            OnFirstClick(tileRow, tileCol);
        }

        if (selectedTile.hasBomb) {
            // If the tile is a bomb, then change sprite to bomb
            selectedTileObj.ChangeSprite(0, true);

            // Play sound effect of a boom
            FindObjectOfType<AudioManager>().PlaySound("Boom");

            // Tells that program shold stop interaction of tiles
            stopInteraction = true;

            // Game has ended
            endedGame = true;

            // Sets power up buttons to be non-interactable
            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }
            
            // Function to reveal bomb positions
            StartCoroutine(RevealBombs());
        } else {
            // If the tile isn't a bomb, then change sprite to open tile
            selectedTileObj.ChangeSprite(1 ,true);

            // If the tile has no bombs adjacent it
            if (selectedTile.bombsAdjacent == 0) {
                // Open all adjacent tiles
                OpenAdjTiles(tileRow, tileCol);
            }
        }
    }

    public void FlagTile(int tileRow, int tileCol) {
        // Gets data on selected tile
        TileData selectedTile = StaticData.TileArr[tileRow, tileCol];

        // Allows the program to change sprites
        TileScript selectedTileObj = TileObjRef[tileRow, tileCol];

        // Changes flagged state to opposite value (Boolean)
        selectedTile.flagged = !selectedTile.flagged;

        // If it is flagged
        if (selectedTile.flagged) {
            // One more flag on board
            StaticData.noOfFlags += 1;

            // Changes sprite to flagged tile
            selectedTileObj.ChangeSprite(2, false);
        } else {
            // One less flag on board
            StaticData.noOfFlags -= 1;

            // Changes sprite to unrevealed tile
            selectedTileObj.ChangeSprite(3, false);
        }
        
        // Changes text to display how many bombs are flagged
        FlaggedInfoText.text = "Flagged: " + StaticData.noOfFlags.ToString();
    }

    public void CheckWinCondition() {
        // Starts from the first row in the check
        int tileRow = 0;

        // Keeps track if the player has won
        bool hasWon = true;

        // While no conditions have been detected that user hasn't won
        // And the tile row looping doesn't exceed the height of the board
        while (hasWon && tileRow < height) {

            // Loops through the columns
            for (int tileCol = 0; tileCol < width; tileCol++) {
                // Gets data of the tile at the position
                TileData selectedTile = StaticData.TileArr[tileRow, tileCol];

                // If the tile has a bomb, and isn't flagged then user hasn't won
                if (selectedTile.hasBomb) {
                    if (!selectedTile.flagged) {
                        hasWon = false;
                    }
                } else if (!selectedTile.revealed) {
                    // If safe tile hasn't been revealed, then user hasn't won
                    hasWon = false;
                }
            }

            // Iterate through next row  
            tileRow += 1;
        }

        // Changes text for how many flags are on the board
        // As this function is called by processes that change the number of flags
        // On the board
        FlaggedInfoText.text = "Flagged: " + StaticData.noOfFlags.ToString();

        // If the user has won
        if (hasWon) {
            // Stop interaction of tiles
            stopInteraction = true;

            // Tells program that game has ended
            endedGame = true;

            // Sets power up buttons to be non-interactable
            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }

            // Function to move to win screen
            StartCoroutine(MoveToWinScreen());
        }
    }

    // Powerup Functionality

    public void AntiBombClicked() {
        // Checks if user actually has that Anti Bomb
        if (StaticData.PowerUpNo[0] > 0) {
            // Tracks that User has one less Anti Bomb
            StaticData.PowerUpNo[0] -= 1;

            // Changes the text to display number of Anti Bombs
            PowerUpNoText[0].text = StaticData.PowerUpNo[0].ToString() + "x";

            // Keeps track if user is using Anti Bomb
            usingAntiBomb = true;

            // Power Up buttons non-interactable
            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }
        }
    }

    public void UseAntiBomb(int useRow, int useCol) {
        // User is no longer using anti bomb
        usingAntiBomb = false;

        // Power Up buttons now interactable
        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = true;
        }

        // Loops through adjacent tiles
        for (int i = useRow - 1; i <= useRow + 1; i++) {
            for (int j = useCol - 1; j <= useCol + 1; j++) {
                // If the position is valid (not out of board)
                if (isValidPos(i, j)) {
                    // Gets data on the tile
                    TileData selectedTile = StaticData.TileArr[i, j];

                    // Allows changing of tile sprite
                    TileScript selectedTileObj = TileObjRef[i, j];


                    // If the tile hasn't been revealed
                    if (!selectedTile.revealed) {
                        if (!selectedTile.hasBomb) {
                            // If the tile doesn't have a bomb, then open it
                            OpenTile(i, j);
                        } else if (!selectedTile.flagged) {
                            // If the tile is a bomb and hasn't been flagged

                            // One more flag is on board
                            StaticData.noOfFlags += 1;

                            // Tracks tile to be flagged
                            selectedTile.flagged = true;

                            // Changes sprite of tile to flagged
                            selectedTileObj.ChangeSprite(2, false);
                        }
                        
                    }
                }
            }
        }
        
        // Checks win condition after
        CheckWinCondition();
    }

    public void UseBombFlagger() {
        // Checks if user actually has a Bomb Flagger
        if (StaticData.PowerUpNo[1] > 0) {

            // Tracks that User has one less Bomb Flagger
            StaticData.PowerUpNo[1] -= 1;

            // Plays sound of flag
            FindObjectOfType<AudioManager>().PlaySound("Flag");

            // Displays how many Bomb Flaggers user has
            PowerUpNoText[1].text = StaticData.PowerUpNo[1].ToString() + "x";

            // Keeps track of how many bombs have been flagged with bomb flagger
            int bombsFlagged = 0;
            
            // Keeps track of the tile row and column looping through
            int tileRow = 0;
            int tileCol = 0;

            // While bomb flagger hasn't flagged 2 bombs or
            // if tileRow is out of bounds of board height
            while (bombsFlagged < 2 && tileRow < height) {

                // If the tile currently looped through has a bomb and isn't already flagged
                if (StaticData.TileArr[tileRow, tileCol].hasBomb && !StaticData.TileArr[tileRow, tileCol].flagged) {
                    // Then flag the tile
                    FlagTile(tileRow, tileCol);

                    // Tracks that One more bomb has been flagged by bomb flagger
                    bombsFlagged += 1;
                }

                // Adds 1 to iterate through column 
                tileCol += 1;

                // If the column is at the end of the board
                if (tileCol == width) {
                    // Reset value of tile column
                    tileCol = 0;

                    // Go through new row
                    tileRow += 1;
                }
            }

            // Checks win condition
            CheckWinCondition();
        }
    }

    public void BombTesterClicked() {
        // Checks if user has a bomb tester
        if (StaticData.PowerUpNo[2] > 0) {
            // Tracks that user has used a bomb tester
            StaticData.PowerUpNo[2] -= 1;
            
            // Displays how many bomb testers the user has
            PowerUpNoText[2].text = StaticData.PowerUpNo[2].ToString() + "x";

            // Tracks that 1 more bomb tester is being used
            bombTestersUsed += 1;

            // Changes text to show that one more bomb tester is being used
            BombTestersUsedText.text = "Bomb Testers being used: " + bombTestersUsed.ToString();

            // Sets all power up buttons but the bomb tester button
            for (int i = 0; i < 2; i++) {
                PowerUpButtons[i].interactable = false;
            }
            PowerUpButtons[3].interactable = false;
        }
    }

    public void UseBombTester(int useRow, int useCol) {
        // Tracks that user is using 1 less bomb tester
        bombTestersUsed -= 1;

        // Changes text to show that one more bomb tester is being used
        BombTestersUsedText.text = "Bomb Testers being used: " + bombTestersUsed.ToString();

        // If user is no longer using a bomb tester, set power up buttons to be active
        if (bombTestersUsed == 0) {
            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = true;
            }
        }

        // If the tile selected has a bomb, then flag it
        if (StaticData.TileArr[useRow, useCol].hasBomb) {
            FlagTile(useRow, useCol);
        } else {
            // If it doesn't have a bomb, then open it
            OpenTile(useRow, useCol);
        }

        // Checks if user has won
        CheckWinCondition();
    }

    public void PlaneChoClicked() {
        // Checks if user has Plane Cho
        if (StaticData.PowerUpNo[3] > 0) {
            // Tracks that user has used Plane Cho
            StaticData.PowerUpNo[3] -= 1;

            // Displays how many "Plane Cho" power up the user has
            PowerUpNoText[3].text = StaticData.PowerUpNo[3].ToString() + "x";

            // Tracks that player is using Plane Cho
            usingPlaneCho = true;

            // Sets power up buttons to be non-interactable
            for (int i = 0; i < 4; i++) {
                PowerUpButtons[i].interactable = false;
            }
        }
    }

    public void UsePlaneCho(int useRow, int useCol) {
        // Sets power up buttons to be interactable
        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = true;
        }

        // Tracks that player is no longer using "Plane Cho"
        usingPlaneCho = false;

        if (isCol) {
            // Iterates through rows of the column selected
            for (int row = 0; row < height; row++) {
                
                // Gets data on the tile at the position
                TileData selectedTile = StaticData.TileArr[row, useCol];

                // If tile hasn't been revealed
                if (!selectedTile.revealed) {

                    if (selectedTile.hasBomb) {
                        // If the tile has a bomb, and hasn't been flagged
                        // Then flag it
                        if (!selectedTile.flagged) {
                            FlagTile(row, useCol);
                        }
                    } else {
                        // If the tile doesn't have a bomb, then open the tile
                        OpenTile(row, useCol);
                    }
                }
            }
        } else {
            // Iterates through columns of the row selected
            for (int col = 0; col < width; col++) {
                
                // Gets data on the tile at the position
                TileData selectedTile = StaticData.TileArr[useRow, col];

                // If tile hasn't been revealed
                if (!selectedTile.revealed) {

                    if (selectedTile.hasBomb) {
                        // If the tile has a bomb, and hasn't been flagged
                        // Then flag it
                        if (!selectedTile.flagged) {
                            FlagTile(useRow, col);
                        }
                    } else {
                        // If the tile doesn't have a bomb, then open the tile
                        OpenTile(useRow, col);
                    }
                }
            }
        }

        // Checks if user has won
        CheckWinCondition();
    }

    public IEnumerator Questioning() {
        // Tells program not to reset data on reentry
        StaticData.reset = false;

        // Stops user from interacting with tiles
        stopInteraction = true;

        // Sets the power-up buttons to be non-interactable
        for (int i = 0; i < 4; i++) {
            PowerUpButtons[i].interactable = false;
        }

        // Sets scen transition animation
        SceneTransition.SetTrigger("Start");

        // Waits for animation to be over until moving to next scene
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Question");
    }

    public IEnumerator RevealBombs() {
        // Variable determining how many bombs should be revealed at once
        int bombRevealedPerTime = 1;

        // Ensures random generation
        Random.InitState((int) System.DateTime.Now.Ticks);

        // Iterates through the board
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                // Checks if the tile at the position isn't revealed
                if (!StaticData.TileArr[row, col].revealed) {
                    // Checks if the tile is a bomb and isn't flagged
                    if (StaticData.TileArr[row, col].hasBomb && !StaticData.TileArr[row, col].flagged) {
                        bombRevealedPerTime -= 1;
                        // Tracks that one more bomb has been revealed in the cycle

                        // Changes sprite to a bomb sprite
                        TileObjRef[row, col].ChangeSprite(0, true);

                        if (bombRevealedPerTime == 0) {
                            // All bombs that needed to be revealed in a cycle
                            // Have been revealed

                            // Waits for 0.5s before opening more bombs
                            yield return new WaitForSeconds(0.5f);
                            // Plays boom effect
                            FindObjectOfType<AudioManager>().PlaySound("Boom");

                            // Randomises how many bombs are revealed per time
                            bombRevealedPerTime = Random.Range(1, 8);
                        }
                    } else if (!StaticData.TileArr[row, col].hasBomb && StaticData.TileArr[row, col].flagged) {
                        // If the tile doesn't have a bomb and is flagged
                        // Change sprite to open tile
                        TileObjRef[row, col].ChangeSprite(3, false);
                    }
                }
            }
        }

        // Wait for 2 seconds before activating scene transition animation
        yield return new WaitForSeconds(2f);
        SceneTransition.SetTrigger("Start");

        // Waits for animation to be over before loading the End Game Lose Page

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("End Game Lose");
    }

    public IEnumerator MoveToWinScreen() {
        // Plays sound that user has won
        FindObjectOfType<AudioManager>().PlaySound("Correct");
        // Waits for sound to be over
        yield return new WaitForSeconds(0.5f);

        // Activates Scene Transition Animator and waits for animation to be over
        SceneTransition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        // Loads End Game Win Page
        SceneManager.LoadScene("End Game Win");
    }

    public void ChangeRowColLights(int useRow, int useCol, bool turnOn) {
        if (turnOn) {
            // If lights are to be turned on
            if (isCol) {
                for (int row = 0; row < height; row++) {
                    // For every non-revealed tile in the column
                    // Change the tile light to be green and other effects
                    if (!StaticData.TileArr[row, useCol].revealed) {
                        TileObjRef[row, useCol].ChangeTileLight(Color.green, 0.9f, 2, 3);
                    }
                }
            } else {
                for (int col = 0; col < height; col++) {
                    // For every non-revealed tile in the column
                    // Change the tile light to be green and other effects
                    if (!StaticData.TileArr[useRow, col].revealed) {
                        TileObjRef[useRow, col].ChangeTileLight(Color.green, 0.9f, 2, 3);
                    }
                }
            }
        } else {
            // If lights are to be turned off

            if (isCol) {
                for (int row = 0; row < height; row++) {
                    // For every non-revealed tile in the column
                    // Change the tile light to be green and other effects
                    if (!StaticData.TileArr[row, useCol].revealed) {
                        TileObjRef[row, useCol].ReturnTileNormal();
                    }
                }
            } else {
                for (int col = 0; col < height; col++) {
                    // For every non-revealed tile in the column
                    // Change the tile light to be green and other effects
                    if (!StaticData.TileArr[useRow, col].revealed) {
                        TileObjRef[useRow, col].ReturnTileNormal();
                    }
                }
            }
        }
    }
}
