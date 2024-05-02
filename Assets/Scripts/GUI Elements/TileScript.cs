using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TileScript : MonoBehaviour
{
    [SerializeField] private Sprite unrevealedTile;
    [SerializeField] private Sprite flaggedTile;
    [SerializeField] private List<Sprite> revealedTiles;
    [SerializeField] private Sprite bombTile;
    [SerializeField] private Light2D tileLight;
    [SerializeField] private GameObject tileLightObjRef;
    [SerializeField] private GameObject tileLightHoverRef;
    [SerializeField] private Light2D tileLightHover;
    [SerializeField] private ParticleSystem ribbonParticles;
    [SerializeField] private ParticleSystem ribbonParticlesDeath;

    GameManager gameManagerRef;
    Animator cameraShake;

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
        TileData infoOnTile = StaticData.tileArr[tileRow, tileCol];

        if (!gameManagerRef.stopInteraction) {
            if (!infoOnTile.revealed) {
                spriteRenderer.material.SetColor("_Color", highlightedColor);
                tileLightHoverRef.SetActive(true);

                if (gameManagerRef.usingAntiBomb) {
                    tileLightHover.falloffIntensity = 0;
                    tileLightHover.shapeLightFalloffSize = 1.3f;
                    tileLightHover.intensity = 10f;
                } else {
                    tileLightHover.shapeLightFalloffSize = 2;
                    tileLightHover.falloffIntensity = 1;
                    tileLightHover.intensity = 3f;
                }
            }

            if (Input.GetMouseButtonDown(0)) { // This means they left clicked a tile
                if (!infoOnTile.flagged && !infoOnTile.revealed) {
                    FindObjectOfType<AudioManager>().PlaySound("Opening Tile");

                    if (gameManagerRef.usingAntiBomb) {
                        gameManagerRef.UseAntiBomb(tileRow, tileCol);
                        cameraShake.SetTrigger("Opened_Blank");

                        gameManagerRef.CheckWinCondition();
                    } else {
                        gameManagerRef.OpenTile(tileRow, tileCol); // Function to reveal the tile
                        
                        if (!infoOnTile.hasBomb) {
                            if (infoOnTile.bombsAdjacent == 0) {
                                cameraShake.SetTrigger("Opened_Blank");
                            }

                            gameManagerRef.CheckWinCondition();

                            if (!StaticData.won) {
                                Random.InitState((int)System.DateTime.Now.Ticks);

                                int RandomChance = Random.Range(0, 12);
                                Debug.Log(RandomChance);

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

    private void OnMouseExit() {
        tileLightHoverRef.SetActive(false);
        spriteRenderer.material.SetColor("_Color", defaultColor);
    }

    public void ChangeSprite(int state, bool ribbonEffects) {
        if (state == 0) {
            // If the tile has a bomb, then change to bomb sprite
            if (ribbonEffects) {
                ribbonParticlesDeath.Play();
            }
            spriteRenderer.sprite = bombTile;
            tileLight.color = Color.white;
            tileLight.intensity = 0.5f;
            tileLightObjRef.SetActive(true);
        } else if (state == 1) {
            if (ribbonEffects) {
                ribbonParticles.Play();
            }
            spriteRenderer.sprite = revealedTiles[StaticData.tileArr[tileRow, tileCol].bombsAdjacent];
            tileLight.color = Color.blue;
            tileLight.intensity = 1f;
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
