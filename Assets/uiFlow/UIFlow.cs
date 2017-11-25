using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class UIFlow : MonoBehaviour
{

    [Header("Interface Setup")]
    [SerializeField]
    GameObject StartMenu;
    [SerializeField]
    Canvas MenuCanvas;
    [SerializeField]
    GameObject[] PersistentComponents;

    public void OpenInterfaceIO() { }

    void Start()
    {
        LoadMenu();
    }

    void LoadMenu()
    {

        foreach (GameObject obj in PersistentComponents) { obj.SetActive(true); }
        foreach (Transform child in MenuCanvas.transform)
        {
            if (PersistentComponents.All(i => i.gameObject != child.gameObject))
            {
                child.gameObject.SetActive(false);
            }
        }
        StartMenu.SetActive(true);

    }
    void UnloadMenu()
    {

        foreach (Transform child in MenuCanvas.transform)
            child.gameObject.SetActive(false);

    }

    public void SwitchToMenu(GameObject GoToMenu)
    {
        foreach (Transform child in MenuCanvas.transform)
        {
            if (PersistentComponents.All(i => i.gameObject != child.gameObject))
            {
                child.gameObject.SetActive(false);
            }
        }
        GoToMenu.gameObject.SetActive(true);
    }
}
