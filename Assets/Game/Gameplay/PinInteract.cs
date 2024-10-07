using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInteract : MonoBehaviour
{
    [SerializeField] Pin pin;

    private void OnMouseDown()
    {
        pin.MouseDown();
    }
    private void OnMouseUp()
    {
        pin.MouseUp();
    }
    private void OnMouseExit()
    {
        pin.MouseExit();
    }
}
