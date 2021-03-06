﻿using UnityEngine;
using System.Collections;

public class LearnMoreOptionsMainMenuItemArray : MainMenuItemArray {
    
    private bool isPlatformSet = false;
    public GameObject learnMoreOptionsPanel;
    public MainMenuItem newTabButton;
    public MainMenuItem sameTabButton;
	
	public static bool isHelp = false;
	private string urlKey = "LEARNMORE.LINK.MOOCFULL";

	public void goToMOOCSameTab() {
		goToMOOC (false);
	}
	public void goToMOOCNewTab() {
		goToMOOC (true);
	}

	private void goToMOOC (bool newTab) {
        RedMetricsManager.get ().sendEvent (TrackingEvent.GOTOMOOC);
        URLOpener.open(urlKey, newTab);
	}
    
    private static string learnMoreKeyPrefix = "MENU.LEARNMORE.";
    private static string sameTabKey = learnMoreKeyPrefix+"GOTOSAMETAB";
    private static string newTabKey = learnMoreKeyPrefix+"GOTONEWTAB";
    private static string browserKey = learnMoreKeyPrefix+"GOTOBROWSER";

    public void setPlatform() {
        if(!isPlatformSet) {
            switch (Application.platform) {
                case RuntimePlatform.WindowsPlayer: 
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    Logger.Log ("LearnMoreOptionsMainMenuItemArray::setPlatform Editor/Standalone prepares choices");
                    //hide "same tab" option
                    MainMenuManager.setVisibility(_items, sameTabKey, false, "setPlatform");
                    //rename "new tab" into "open in browser"
                    MainMenuManager.replaceTextBy(newTabKey, browserKey, _items, "setPlatform");
                    break;
                default:
                    Logger.Log ("LearnMoreOptionsMainMenuItemArray::setPlatform default nothing to do");
                    MainMenuManager.setVisibility(_items, sameTabKey, true, "setPlatform");
                    //rename "open in browser tab" into "new tab"
                    MainMenuManager.replaceTextBy(browserKey, newTabKey, _items, "setPlatform");
                    break;
            }
            isPlatformSet = true;
        }
    }
    
    void OnEnable ()
    {
        learnMoreOptionsPanel.SetActive(true);
    }
    
    void OnDisable ()
    {
        learnMoreOptionsPanel.SetActive(false);
    }
}
