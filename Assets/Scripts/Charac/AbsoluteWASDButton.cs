﻿using UnityEngine;
using System.Collections;

public class AbsoluteWASDButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Logger.Log("AbsoluteWASDButton::OnPress()", Logger.Level.INFO);
      ControlsMainMenuItemArray.get ().switchControlTypeToAbsoluteWASD();
    }
  }
}
