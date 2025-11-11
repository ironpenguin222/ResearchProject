using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Buttons to be shown/hidden
    public GameObject buttons;

    public void ShowButton() // Show buttons
    {
        buttons.SetActive(true);
    }

    public void HideButton() // Hide buttons
    {
        buttons.SetActive(false);
    }
}
