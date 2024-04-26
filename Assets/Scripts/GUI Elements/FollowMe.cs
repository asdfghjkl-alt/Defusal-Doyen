using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class FollowMe : MonoBehaviour
{
    private Vector2 mousePos;

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        transform.position = mousePos;
    }
}