using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject PlayerInfoScreenUI;

    public ItemInfoUI ItemInfoUI;

    public PanelPopIn PopInUI;

    public bool isOpen;

    private void Start()
    {
        PlayerInfoScreenUI.SetActive(false);

        isOpen = false;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && !isOpen)
        {
            PlayerInfoScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

            isOpen = true;
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame && isOpen)
        {
            PlayerInfoScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SelectionManager.Instance.EnableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;

            isOpen = false;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            HidePopInUI();
        }
    }

    public static void ShowItemInfoUI(string name, string description, string function, Vector2 position, Vector2 pivot)
    {
        Instance.ItemInfoUI.SetText(name, description, function);
        Instance.ItemInfoUI.SetPosition(position, pivot);
        Instance.ItemInfoUI.gameObject.SetActive(true);
    }

    public static void HideItemInfoUI()
    {
        Instance.ItemInfoUI.gameObject.SetActive(false);
    }

    public static void ShowPopInUI(string text)
    {
        Instance.PopInUI.SetText(text);
        Instance.PopInUI.gameObject.SetActive(true);
    }

    public static void HidePopInUI()
    {
        Instance.PopInUI.gameObject.SetActive(false);
    }


}