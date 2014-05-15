using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipedDisplayedDevice : DisplayedDevice {
  private LinkedList<GenericDisplayedBioBrick> _currentDisplayedBricks = new LinkedList<GenericDisplayedBioBrick>();

	
  private static GameObject           equipedDevice = null;
  private static GameObject           tinyBioBrickIcon;
  private static GameObject           tinyBioBrickIcon2;
  private float                       _tinyIconVerticalShift = 0.0f;
  private static float                _width = 0.0f;

  private static string               _equipedDeviceButtonPrefabPosString = "EquipedDeviceButtonPrefabPos";
  private static string               _tinyBioBrickPosString              = "TinyBioBrickIconPrefabPos";
  private static string               _tinyBioBrickPosString2             = _tinyBioBrickPosString + "2";

  void OnEnable() {
    Logger.Log("EquipedDisplayedDevice::OnEnable "+_device, Logger.Level.TRACE);

    initIfNecessary();

    foreach(GenericDisplayedBioBrick brick in _currentDisplayedBricks)
    {
      brick.gameObject.SetActive(true);
    }
  }

  void OnDisable() {
    Logger.Log("EquipedDisplayedDevice::OnDisable "+_device, Logger.Level.TRACE);
    foreach(GenericDisplayedBioBrick brick in _currentDisplayedBricks)
    {
      brick.gameObject.SetActive(false);
    }
  }

  protected override void OnPress(bool isPressed) {
    if(isPressed) {
	    Logger.Log("EquipedDisplayedDevice::OnPress() "+getDebugInfos(), Logger.Level.INFO);
      if(_device == null)
      {
        Logger.Log("EquipedDisplayedDevice::OnPress _device == null", Logger.Level.WARN);
        return;
      }
	    if (_devicesDisplayer.IsEquipScreen()) {
        TooltipManager.displayTooltip();
	      _devicesDisplayer.askRemoveEquipedDevice(_device);
	    }
	  }
  }



  void initIfNecessary() {
    if(equipedDevice == null) {
      //equipedDevice = GameObject.Find(_equipedDeviceButtonPrefabPosString);
			equipedDevice = DevicesDisplayer.get().equipedDevice;
    //  tinyBioBrickIcon = GameObject.Find (_tinyBioBrickPosString);
			tinyBioBrickIcon = GameObject.Find("InterfaceLinkManager").GetComponent<InterfaceLinkManager>().tinyBioBrickIconPrefabPos;
			tinyBioBrickIcon2 = GameObject.Find("InterfaceLinkManager").GetComponent<InterfaceLinkManager>().tinyBioBrickIconPrefabPos2;

    }
    if(_tinyIconVerticalShift == 0.0f)
    {
      _tinyIconVerticalShift = (tinyBioBrickIcon.transform.localPosition - equipedDevice.transform.localPosition).y;
      _width = tinyBioBrickIcon2.transform.localPosition.x - tinyBioBrickIcon.transform.localPosition.x;
    }
  }

  void displayBioBricks() {
    Logger.Log("EquipedDisplayedDevice::displayBioBricks", Logger.Level.DEBUG);
    initIfNecessary();
    if(_device != null)
    {
      //add biobricks
      int index = 0;
      foreach (ExpressionModule module in _device.getExpressionModules())
      {
        foreach(BioBrick brick in module.getBioBricks())
        {
          GenericDisplayedBioBrick dbbrick = TinyBioBrickIcon.Create(transform, getNewPosition(index), null, brick);
          _currentDisplayedBricks.AddLast(dbbrick);
          index++;
        }
      }
    } else {
      Logger.Log("EquipedDisplayedDevice::displayBioBricks _device == null", Logger.Level.WARN);
    }
  }

  //needs tinyBioBrickIcon to be initialized, e.g. using initIfNecessary()
  private Vector3 getNewPosition(int index ) {
    Vector3 shiftPos = new Vector3(index*_width, _tinyIconVerticalShift, -1.0f);
    if(tinyBioBrickIcon == null) {
      Logger.Log("EquipedDisplayedDevice::getNewPosition tinyBioBrickIcon == null", Logger.Level.WARN);
      return new Vector3(index*_width, -95.0f, -0.1f) + shiftPos ;
    } else {
      //return tinyBioBrickIcon.transform.localPosition + shiftPos;
			return shiftPos;
    }
  }

  // Use this for initialization
  void Start () {
    Logger.Log("EquipedDisplayedDevice::Start", Logger.Level.TRACE);

    initIfNecessary();

    displayBioBricks();
  }
}