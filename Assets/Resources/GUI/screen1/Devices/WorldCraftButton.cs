using UnityEngine;
using System.Collections;


//TODO move to Scripts folder instead of Resources folder
//TODO rename class & filename to match & respect standards (cf ToEquipButton & ToWorldButton & ToCraftButton)
public class EquipCraftButton : MonoBehaviour {

  private void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("EquipCraftButton::OnPress()", Logger.Level.INFO);
      GUITransitioner.get().SwitchScreen(GUITransitioner.GameScreen.screen1, GUITransitioner.GameScreen.screen3);
    }    
  }  

}
