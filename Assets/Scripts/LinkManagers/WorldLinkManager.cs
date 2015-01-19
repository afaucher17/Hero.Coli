using UnityEngine;
using System.Collections;


/*DESCRIPTION
 * This class create the links between the World's Scene, classes and GameObject and the others
 * */

public class WorldLinkManager : MonoBehaviour {

    public MineManager mineManager;

	// Use this for initialization
	void Start () {

    GameObject perso = GameObject.Find ("Perso");
    mineManager.hero = perso.GetComponent<Hero>();

		//tuto End
		EndGameCollider endGameCollider = GameObject.Find("TutoEnd").GetComponent<EndGameCollider>();
    endGameCollider.hero = perso;
		endGameCollider.gameStateController = GameStateController.get ();
    endGameCollider.endInfoPanel = GameStateController.get().endWindow;
    endGameCollider.endInfoPanelRestartButton = GameStateController.get ().endRestartButton;
            
            //tuto End
    InfoWindowCollisionTrigger tutoRFP = GameObject.Find("TutoRFP").GetComponent<InfoWindowCollisionTrigger>();
    tutoRFP.heroCollider = perso.GetComponent<CapsuleCollider>();

		Logger.Log ("EndGameCollider.infoPanel"+endGameCollider.endInfoPanel,Logger.Level.INFO);
	
	}

}
