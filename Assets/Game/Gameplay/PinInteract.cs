using Funzilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInteract : MonoBehaviour
{
    [SerializeField] Pin pin;

    private void OnMouseDown()
    {
        if (Gameplay.Instance == null)
            return;
        pin.MouseDown();
    }
    private void OnMouseUp()
    {
        if (Gameplay.Instance == null)
            return;
        pin.MouseUp();
    }
    private void OnMouseExit()
    {
        if (Gameplay.Instance == null)
            return;
        pin.MouseExit();
    }
}
