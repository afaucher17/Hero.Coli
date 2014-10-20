﻿using UnityEngine;
using System.Collections;

public class RestartButton : ModalButton {
  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("RestartButton::OnPress()", Logger.Level.INFO);
      GameStateController.restart();
    }
    base.OnPress(isPressed);
  }
}
