using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// NOTE: for [SerializeField] the variable is assigned in the Unity Editor

// Class for the prefab of the tile
public class TileScript : MonoBehaviour
{
    // Sprite for unrevealed tile
    [SerializeField] private Sprite unrevealedTile;

    // Sprite for flagged tile
    [SerializeField] private Sprite flaggedTile;

    // 9 sprites of revealed tiles based on how many bombs are next to them
    [SerializeField] private Sprite[] revealedTiles;

    // Sprite for tile revealed to be a bomb
    [SerializeField] private Sprite bombTile;

    // Sets component for light on tile when opened
    // Component for setting attributes of the light
    [SerializeField] private Light2D tileLight;

    // Sets component for light on tile when opened (Setting it to be active)
    [SerializeField] private GameObject tileLightObjRef;

    // Sets component for light on hovering on unopened tile (Setting it to be active)
    [SerializeField] private GameObject tileLightHoverRef;

    // Gets components of light to be set active on hovering unopened tile
    // Allows for setting attributes to the light (e.g. colour)
    [SerializeField] private Light2D tileLightHover;

    // Allows for control over particle system when tile is opened and not a bomb
    [SerializeField] private ParticleSystem ribbonParticles;

    // Allows for control over particle system for when tile is opened and a bomb
    [SerializeField] private ParticleSystem ribbonParticlesDeath;

    // Allows for getting game manager to conduct functions (e.g. Opening Tile)
    GameManager gameManagerRef;

    // Gets Animator to set a trigger for camera shaking
    Animator cameraShake;

    // 
    [HideInInspector] public int tileRow;

    [HideInInspector] public int tileCol;

    [HideInInspector] public SpriteRenderer spriteRenderer; // Component to allow changing sprites

    Color defaultColor = new Color(1f, 1f, 1f);
    Color highlightedColor = new Color(0.4f, 0.4f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        gameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }
    private void OnMouseOver()
    {
        TileData infoOnTile = StaticData.TileArr[tileRow, tileCol];

        if (!gameManagerRef.stopInteraction) {
            if (!infoOnTile.revealed) {
                
                if (!gameManagerRef.usingPlaneCho) {
                    spriteRenderer.material.SetColor("_Color", highlightedColor);
                }

                if (!infoOnTile.flagged) {
                    if (gameManagerRef.usingAntiBomb) {
                        ChangeTileLight(Color.green, 0, 1.3f, 10);
                    } else if (gameManagerRef.usingPlaneCho) {
                        gameManagerRef.ChangeColLights(tileCol, true);
                    } else if (gameManagerRef.bombTestersUsed > 0) {
                        ChangeTileLight(Color.green, 0.2f, 2, 3);
                    } else {
                        ChangeTileLight(Color.red, 0.3f, 2, 3);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0)) { // This means they left clicked a tile
                if (!infoOnTile.flagged && !infoOnTile.revealed) {
                    FindObjectOfType<AudioManager>().PlaySound("Opening Tile");

                    if (gameManagerRef.usingAntiBomb) {
                        gameManagerRef.UseAntiBomb(tileRow, tileCol);
                        cameraShake.SetTrigger("Opened_Blank");

                        gameManagerRef.CheckWinCondition();
                    } else if (gameManagerRef.bombTestersUsed > 0) {
                        gameManagerRef.UseBombTester(tileRow, tileCol);

                        if (!infoOnTile.flagged && infoOnTile.bombsAdjacent == 0) {
                            cameraShake.SetTrigger("Opened_Blank");
                        }
                    } else if (gameManagerRef.usingPlaneCho) {
                        gameManagerRef.UsePlaneCho(tileCol);
                        cameraShake.SetTrigger("Opened_Blank");
                    } else {
                        gameManagerRef.OpenTile(tileRow, tileCol); // Function to reveal the tile
                        
                        if (!infoOnTile.hasBomb) {
                            if (infoOnTile.bombsAdjacent == 0) {
                                cameraShake.SetTrigger("Opened_Blank");
                            }

                            gameManagerRef.CheckWinCondition();

                            if (!gameManagerRef.endedGame) {
                                Random.InitState((int)System.DateTime.Now.Ticks);

                                int RandomChance = Random.Range(0, 12);

                                if (RandomChance == 0) {
                                    StartCoroutine(gameManagerRef.Questioning());
                                }
                            }
                        }
                    }
                }
            } if (Input.GetMouseButtonDown(1) && !infoOnTile.revealed) { // This means they right clicked
                FindObjectOfType<AudioManager>().PlaySound("Flag");
                gameManagerRef.FlagTile(tileRow, tileCol);
                gameManagerRef.CheckWinCondition();
            }
        }
    }

    public void ChangeTileLight(Color lightColor, float lightFOIntensity, float lightFalloffSize, float lightIntensity) {
        tileLightHoverRef.SetActive(true);
        tileLightHover.color = lightColor;
        tileLightHover.falloffIntensity = lightFOIntensity;
        tileLightHover.shapeLightFalloffSize = lightFalloffSize;
        tileLightHover.intensity = lightIntensity;
    }

    public void OnMouseExit() {
        if (gameManagerRef.usingPlaneCho) {
            gameManagerRef.ChangeColLights(tileCol, false);
        } else {
            ReturnTileNormal();
        }
    }

    public void ReturnTileNormal() {
        tileLightHoverRef.SetActive(false);
        spriteRenderer.material.SetColor("_Color", defaultColor);
    }

    public void ChangeSprite(int state, bool ribbonEffects) {
        tileLightHoverRef.SetActive(false);
        spriteRenderer.material.SetColor("_Color", defaultColor);

        if (state == 0) {
            // If the tile has a bomb, then change to bomb sprite
            if (ribbonEffects) {
                ribbonParticlesDeath.Play();
            }
            spriteRenderer.sprite = bombTile;
            tileLight.color = Color.white;
            tileLight.intensity = 0.6f;
            tileLightObjRef.SetActive(true);
        } else if (state == 1) {
            if (ribbonEffects) {
                ribbonParticles.Play();
            }
            spriteRenderer.sprite = revealedTiles[StaticData.TileArr[tileRow, tileCol].bombsAdjacent];
            tileLight.color = Color.white;
            tileLight.intensity = 0.6f;
            tileLightObjRef.SetActive(true);
        } else if (state == 2) {
            spriteRenderer.sprite = flaggedTile;
            tileLight.color = new Color(0.1f, 0.5f, 1.0f);
            tileLight.intensity = 2f;
            tileLightObjRef.SetActive(true);
        } else {
            spriteRenderer.sprite = unrevealedTile;
            tileLightObjRef.SetActive(false);
        }
    }
}
