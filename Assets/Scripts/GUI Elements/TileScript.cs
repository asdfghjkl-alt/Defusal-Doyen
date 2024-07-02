using UnityEngine;
using UnityEngine.Rendering.Universal;

// NOTE: for [SerializeField] the variable is assigned in the Unity Editor
// EXTRA NOTE: active in Unity is whether the user can see that game object or not

// Class for the prefab of the tile
public class TileScript : MonoBehaviour
{
    // Sprite for unrevealed tile
    [SerializeField] private Sprite UnrevealedTileSpr;

    // Sprite for flagged tile
    [SerializeField] private Sprite FlaggedTileSpr;

    // 9 sprites of revealed tiles based on how many bombs are next to them
    [SerializeField] private Sprite[] RevealedTilesSpr;

    // Sprite for tile revealed to be a bomb
    [SerializeField] private Sprite BombTileSpr;

    // Sets component for light on tile when opened
    // Component for setting attributes of the light
    [SerializeField] private Light2D TileOpenLight;

    // Sets component for light on tile when opened (Setting it to be active/visible)
    [SerializeField] private GameObject TileOpenLightObjRef;

    // Sets component for light on hovering on unopened tile (Setting it to be active/visible)
    [SerializeField] private GameObject TileLightHoverRef;
    // Allows for setting attributes to the light (e.g. colour)
    [SerializeField] private Light2D TileLightHover;

    // Allows for control over particle system when tile is opened and not a bomb
    [SerializeField] private ParticleSystem RibbonParticles;

    // Allows for control over particle system for when tile is opened and a bomb
    [SerializeField] private ParticleSystem RibbonParticlesDeath;

    // Component to allow changing sprites
    [SerializeField] private SpriteRenderer SpriteRendRef;

    // Allows for getting game manager to conduct functions (e.g. Opening Tile)
    GameManager GameManagerRef;

    // Gets Animator to set a trigger for camera shaking
    Animator CameraShake;

    // Row of tile on board
    [HideInInspector] public int tileRow;

    // Column of tile on board
    [HideInInspector] public int tileCol;


    // Sets default color of a tile
    Color defaultColor = new Color(1f, 1f, 1f);

    // Sets color of tile that is hovered over
    Color highlightedColor = new Color(0.4f, 0.4f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        // Getting components for the tile object to access later
        GameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        CameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }
    private void OnMouseOver()
    {
        // Getting info on the tile
        TileData infoOnTile = StaticData.TileArr[tileRow, tileCol];

        // If interactions haven't been disabled
        if (!GameManagerRef.stopInteraction) {

            // If tile hasn't been revealed
            if (!infoOnTile.revealed) {
                
                // If using plane cho, for an improved effect
                // Tile shouldn't change color
                if (!GameManagerRef.usingPlaneCho) {
                    // Setting color to highlighted colour
                    SpriteRendRef.material.SetColor("_Color", highlightedColor);
                }

                // If the tile isn't flagged
                if (!infoOnTile.flagged) {
                    // Function ChangeTileLight (defined below) changes tile light on hovering
                    // Parameters: Colour, how much the light effect falls off,
                    // How much should the light go outside the tile borders, 
                    // And intensity of the light

                    if (GameManagerRef.usingAntiBomb) {
                        // Changes tiles to green, has light surrounding 8
                        // Adjacent tiles
                        ChangeTileLight(Color.green, 0, 1.3f, 10);
                    } else if (GameManagerRef.usingPlaneCho) {
                        // Retains this information to tell game which
                        // Tile is being hovered over when using Plane Cho
                        // To accurately display lights on tiles when being switched
                        // From row to column
                        GameManagerRef.tileColHover = tileCol;
                        GameManagerRef.tileRowHover = tileRow;

                        // Sends info of the column of the tile to the game manager
                        // To make the whole column have green lights
                        GameManagerRef.ChangeRowColLights(tileRow, tileCol, true);
                    } else if (GameManagerRef.bombTestersUsed > 0) {
                        // Changes the tile light to be green when using bomb tester
                        ChangeTileLight(Color.green, 0.2f, 2, 3);
                    } else {
                        // Red when no power up is being used
                        ChangeTileLight(Color.red, 0.3f, 2, 3);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0)) { // This means they left clicked a tile
                // If the tile isn't flagged or revealed
                if (!infoOnTile.flagged && !infoOnTile.revealed) {
                    // Plays sound effect
                    FindObjectOfType<AudioManager>().PlaySound("Opening Tile");

                    // Sees if player is using antibomb
                    if (GameManagerRef.usingAntiBomb) {
                        // Function in using Anti Bomb
                        GameManagerRef.UseAntiBomb(tileRow, tileCol);

                        // Makes camera shake
                        CameraShake.SetTrigger("Opened_Blank");
                    } else if (GameManagerRef.bombTestersUsed > 0) {
                        // Function using a Bomb Tester
                        GameManagerRef.UseBombTester(tileRow, tileCol);

                        // If adjacent tiles don't have bomb, shake camera
                        if (!infoOnTile.flagged && infoOnTile.bombsAdjacent == 0) {
                            CameraShake.SetTrigger("Opened_Blank");
                        }
                    } else if (GameManagerRef.usingPlaneCho) {
                        // Function for using Plane Cho
                        GameManagerRef.UsePlaneCho(tileRow, tileCol);

                        // Shakes Camera
                        CameraShake.SetTrigger("Opened_Blank");
                    } else {
                        GameManagerRef.OpenTile(tileRow, tileCol); // Function to reveal the tile
                        
                        if (!infoOnTile.hasBomb) {
                            // If no bombs adjacent it, then shake camera
                            if (infoOnTile.bombsAdjacent == 0) {
                                CameraShake.SetTrigger("Opened_Blank");
                            }

                            // Checks win condition
                            GameManagerRef.CheckWinCondition();

                            // Precaution to not let user go to question page
                            // If game ended
                            if (!GameManagerRef.endedGame && StaticData.QuestionsAnswered < 20) {
                                // Generates actual randomness
                                Random.InitState((int)System.DateTime.Now.Ticks);

                                int randomChance;

                                // Chance of 1/12 to get a question for Easy
                                if (StaticData.difficulty == "E") {
                                    randomChance = Random.Range(0, 12);
                                    Debug.Log(randomChance);
                                } else {
                                    // Chance of 1/14 to get a question for Hard
                                    randomChance = Random.Range(0, 14);
                                }

                                if (randomChance == 0) {
                                    // Starts function to transition to Question Page
                                    StaticData.QuestionsAnswered += 1;
                                    StartCoroutine(GameManagerRef.Questioning());
                                }
                            }
                        }
                    }
                }
            } if (Input.GetMouseButtonDown(1) && !infoOnTile.revealed) { // This means they right clicked
                // Play Sound of Flag
                FindObjectOfType<AudioManager>().PlaySound("Flag");

                // Function to flag a tile, and checks if player won
                GameManagerRef.FlagTile(tileRow, tileCol);
                GameManagerRef.CheckWinCondition();
            }
        }
    }

    public void ChangeTileLight(Color lightColor, float lightFOIntensity, float lightFalloffSize, float lightIntensity) {
        // Sets light to be active
        TileLightHoverRef.SetActive(true);

        // Sets Color
        TileLightHover.color = lightColor;

        // Sets how much the light effect fades away
        TileLightHover.falloffIntensity = lightFOIntensity;

        // Sets how much the light should go outside the borders
        TileLightHover.shapeLightFalloffSize = lightFalloffSize;

        // Intensity of light
        TileLightHover.intensity = lightIntensity;
    }

    public void OnMouseExit() {
        if (GameManagerRef.usingPlaneCho) {
            // Makes tiles in that previous column hovered over have no lights
            GameManagerRef.ChangeRowColLights(tileRow, tileCol, false);
        } else {
            // Sets hover light to be inactive, and back to default colour
            ReturnTileNormal();
        }
    }

    public void ReturnTileNormal() {
        // Setting hover light to be inactive
        TileLightHoverRef.SetActive(false);

        // Back to default color
        SpriteRendRef.material.SetColor("_Color", defaultColor);
    }

    public void ChangeSprite(int state, bool ribbonEffects) {
        // Sets hover light to be inactive
        TileLightHoverRef.SetActive(false);

        // Sets color to be default
        SpriteRendRef.material.SetColor("_Color", defaultColor);

        if (state == 0) { // Tile is a bomb
            if (ribbonEffects) {
                // Plays ribbon/explosion effects
                RibbonParticlesDeath.Play();
            }
            // Change to bomb sprite
            SpriteRendRef.sprite = BombTileSpr;

            // Change light color to white
            TileOpenLight.color = Color.white;

            // Changes intensity of light
            TileOpenLight.intensity = 0.6f;

            // Sets the light to be active/visible
            TileOpenLightObjRef.SetActive(true);
        } else if (state == 1) {
            if (ribbonEffects) {
                // Plays ribbon effects on opening
                RibbonParticles.Play();
            }

            // Changes Sprite based on how many bombs are adjacent the tile
            SpriteRendRef.sprite = RevealedTilesSpr[StaticData.TileArr[tileRow, tileCol].bombsAdjacent];

            // Changes light color to be white
            TileOpenLight.color = Color.white;

            // Changes intensity
            TileOpenLight.intensity = 0.6f;

            // Sets the light to be visible/active
            TileOpenLightObjRef.SetActive(true);
        } else if (state == 2) {
            // Changes sprite to flagged tile sprite
            SpriteRendRef.sprite = FlaggedTileSpr;

            // Changes light color
            TileOpenLight.color = new Color(0.1f, 0.5f, 1.0f);

            // Changes light intensity
            TileOpenLight.intensity = 2f;

            // Sets light to be active/visible
            TileOpenLightObjRef.SetActive(true);
        } else {
            // Sets Sprite to be unrevealed sprite
            SpriteRendRef.sprite = UnrevealedTileSpr;

            // Sets light to be inactive
            TileOpenLightObjRef.SetActive(false);
        }
    }
}
