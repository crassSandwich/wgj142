﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickAwayMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Minimizer Minimizer;
    public Button OpenButton;

    bool mouseOver;

	void Start ()
    {
        OpenButton.onClick.AddListener(Minimizer.UnMinimize);
    }

    void Update ()
    {
        OpenButton.interactable = Minimizer.Minimized;

        if (Input.GetMouseButtonDown(0) && !mouseOver)
        {
            Minimizer.Minimize();
        }
    }

	public void OnPointerEnter (PointerEventData eventData)
	{
        mouseOver = true;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
        mouseOver = false;
	}
}