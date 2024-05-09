using UnityEngine;

// This will be for the circular light that follows the mouse
public class FollowMe : MonoBehaviour
{
    private Vector2 mousePos;

    // Update is called once per frame
    void Update()
    {
        // Gets info on the mouse position on the application
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);


        // Sets the light's position to be on the mouse
        transform.position = mousePos;
    }
}
