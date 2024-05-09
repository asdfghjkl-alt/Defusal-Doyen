using System.Collections;
using UnityEngine;

// This class is for buttons to give a description on hover
public class DescOnHover : MonoBehaviour
{
    // Reference to Description Box as object and assigned in Unity Editor
    // Allows to set the Description Box as active and get Animator components
    [SerializeField] GameObject DescriptionBox;

    private void OnMouseOver()
    {
        // Sets object to be visible
        // Also on awake, an animation of fade in will occur
        // (Unity Animation Tools)
        DescriptionBox.SetActive(true);
    }

    private void OnMouseExit() {
        // StartCoroutine allows for pauses in certain elements of code
        // Without interrupting other processes
        StartCoroutine(HideDesc());
    }

    private IEnumerator HideDesc() {
        // Sets trigger in animator to conduct a fade out animation 
        DescriptionBox.GetComponent<Animator>().SetTrigger("Leave");

        // Waits for animation to be over
        yield return new WaitForSeconds(0.3f);

        // Sets object to be invisible
        DescriptionBox.SetActive(false);
    }
}
