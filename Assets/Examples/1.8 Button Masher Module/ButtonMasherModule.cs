﻿using System;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// A simple module that requires the player to push the exactly button 50 times, but only
/// when the timer has a "4" in any position.
/// </summary>
public class ButtonMasherModule : MonoBehaviour
{
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public KMAudio KMAudio;
    public KMSelectable Button;
    public TextMesh Counter;

    protected int currentCount;

	protected void Start()
    {
        Button.OnInteract += HandlePress;
	}

    protected bool HandlePress()
    {
        BombModule.HandlePass();
        return false;
    }
}
