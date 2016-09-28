using UnityEngine;
using System.Collections.Generic;

public class Inventory : DeviceContainer
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "DeviceInventory";
    private static Inventory _instance;
    public static Inventory get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("Inventory::get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<Inventory>();
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if((_instance != null) && (_instance != this))
        {            
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;  
    private void initializeIfNecessary()
    {
        if(!_initialized)
        {
            _deviceAdded = false;
            _initialized = true;
        }
    }

    protected override void Start()
    {
        base.Start();
        // Debug.Log("Inventory::Start()");        
    }

    public void initialize ()
    {
        loadDevices();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    /* array of file paths from which the devices available by default from start will be loaded,
     * provided that the required BioBricks are available - cf. AvailableBioBricksManager
     */
    //private string[] _deviceFiles = new string[]{};
    public string[] deviceFiles;
    public string deviceFilesPathPrefix;
    //private string[] _deviceFiles = new string[]{ "Parameters/Devices/available", Inventory._saveFilePathRead };

    //old device files
    //private string[] _deviceFiles = new string[]{ "Assets/Data/devices"};
    //private string[] _deviceFiles = new string[]{ "Assets/Data/raph/devices", Inventory._saveFilePathRead };
    //private string[] _deviceFiles = new string[]{ "Assets/Data/raph/repressilatorDevices", Inventory._saveFilePathRead };

    private const string _saveFilePathRead = "Parameters/Devices/exported.xml";
    private const string _saveFilePathWrite = "Assets/Resources/" + _saveFilePathRead;

    private string _genericDeviceNamePrefix = "device";

    private bool _deviceAdded;

    public bool getDeviceAdded() { return _deviceAdded; }
    public void setDeviceAdded(bool b) { _deviceAdded = b; }


    private static string[] proteinsIn = new string[]{
    "Collagen",
    "Actin",
    "Keratin",
    "Elastin"
  };
    private static string[] proteinsOut = new string[]{
    "Cadherin",
    "Ependymin",
    "Integrin",
    "Selectin"
  };

    public static bool isOpenable()
    {
        return 0 != _instance._devices.Count;
    }

    private static float getTestBeta() { return Random.Range(0.0f, 10.0f); }
    private static string getTestFormula()
    {
        string result = "";
        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            result += "!";
        }
        result += "[";
        result += (0.1f + Random.Range(0, 8) / 10);
        result += ",";
        result += Random.Range(0.1f, 3.0f);
        result += "]";
        result += proteinsIn[Random.Range(0, proteinsIn.Length - 1)];
        return result;
    }
    private static float getTestRBS() { return Random.Range(0.9f, 1.1f); }
    private static string getTestProtein() { return proteinsOut[Random.Range(0, proteinsOut.Length)]; }
    private static float getTestTerminator() { return Random.Range(0.9f, 1.1f); }

    private Device getTestDevice(string name)
    {
        // Debug.Log("Inventory::getTestDevice()");
        Device testDevice = Device.buildDevice(
        name,
        getTestBeta(),
        getTestFormula(),
        getTestRBS(),
        getTestProtein(),
        getTestTerminator()
        );
        // Debug.Log("Inventory::getTestDevice() testDevice=" + testDevice);
        return testDevice;
    }

    private List<Device> getTestDevices()
    {
        // Debug.Log("Inventory::getTestDevices()");
        List<Device> result = new List<Device>();
        result.Add(getTestDevice("MyDevice"));
        result.Add(getTestDevice("MonDispositif"));
        result.Add(getTestDevice("MeinGeraet"));
        // Debug.Log("Inventory::getTestDevices() result=" + Logger.ToString<Device>(result));
        return result;
    }

    private void addDevice(Device device)
    {
        // Debug.Log("Inventory::addDevice(" + device + "), count before=" + _devices.Count);
        Device copy = Device.buildDevice(device);
        if (device == null)
        {
            Debug.LogWarning("Inventory::addDevice device==null");
            return;
        }

        string displayerString = _displayer != null ? "name=" + _displayer.name : "null";
        // Debug.Log("Inventory::addDevice(" + device + ")"
        //   + ", copy=" + copy
        //   + ", count before=" + _devices.Count
        //   + ", _devices=" + _devices
        //   + ", _displayer=" + displayerString
        //   );
        _devices.Add(copy);
        _displayer.addInventoriedDevice(copy);
        // Debug.Log("Inventory::addDevice _devices.Add(copy); done");
    }

    public AddingResult canAddDevice(Device device)
    {
        // Debug.Log("Inventory::canAddDevice(" + device + ") with _devices=" + Logger.ToString<Device>(_devices));

        if (device == null)
        {
            Debug.LogWarning("Inventory::canAddDevice: device is null: AddingResult.FAILURE_DEFAULT");
            return AddingResult.FAILURE_DEFAULT;
        }
        else
        {
            //TODO test BioBricks equality (cf next line)
            if (_devices.Exists(d => d.Equals(device)))
            //if (_devices.Exists(d => d.getInternalName() == device.getInternalName()))
            {
                if (_devices.Exists(d => d.hasSameBricks(device)))
                {
                    // Debug.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_DEVICE");
                    return AddingResult.FAILURE_SAME_DEVICE;
                }
                else
                {
                    // Debug.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_NAME");
                    return AddingResult.FAILURE_SAME_NAME;
                }
            }
            else if (_devices.Exists(d => d.hasSameBricks(device)))
            {
                // Debug.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_BRICKS");
                return AddingResult.FAILURE_SAME_BRICKS;
            }
            else
            {
                // Debug.Log("Inventory::canAddDevice: AddingResult.SUCCESS");
                return AddingResult.SUCCESS;
            }
        }
    }

    public override AddingResult askAddDevice(Device device, bool reportToRedMetrics = false)
    {
        // Debug.Log("Inventory::askAddDevice");
        AddingResult addingResult = canAddDevice(device);
        if (addingResult == AddingResult.SUCCESS)
        {
            // Debug.Log("Inventory::askAddDevice: AddingResult.SUCCESS, will add device=" + device);
            addDevice(device);

            if (reportToRedMetrics)
            {
                RedMetricsManager.get().sendEvent(TrackingEvent.CRAFT, new CustomData(CustomDataTag.DEVICE, device.getInternalName()));
            }

            //uncomment to save user-created devices
            //TODO FIXME uncommenting this entails bugs on loading devices from _saveFilePath
            //DeviceSaver dSaver = new DeviceSaver();
            //dSaver.saveDevicesToFile(_devices, _saveFilePathWrite);
        }
        else
        {
            // Debug.Log("Inventory::askAddDevice: " + addingResult + ", didn't add device=" + device);
        }
        return addingResult;
    }

    public override void removeDevice(Device device)
    {
        _devices.Remove(device);
        safeGetDisplayer().removeInventoriedDevice(device);
        safeGetDisplayer().removeListedDevice(device);
    }

    public override void removeDevices(List<Device> toRemove)
    {
        // Debug.Log("Inventory::removeDevices");

        foreach (Device device in toRemove)
        {
            safeGetDisplayer().removeInventoriedDevice(device);
        }
        _devices.RemoveAll((Device obj) => toRemove.Contains(obj));
    }

    public override void editDevice(Device device)
    {
        //TODO
        Debug.LogError("Inventory::editeDevice NOT IMPLEMENTED");
    }

    //
    public string getAvailableDeviceDisplayedName()
    {
        // Debug.Log("Inventory::getAvailableDeviceDisplayedName()");
        bool taken;
        string currentName;
        int number = _devices.Count;
        do
        {
            currentName = _genericDeviceNamePrefix + number;
            taken = _devices.Exists(d => (d.displayedName == currentName));
            if (taken)
            {
                number++;
            }
            else
            {
                return currentName;
            }
        } while (taken);
        // Debug.Log("Inventory::getAvailableDeviceDisplayedName() returns " + currentName);
        return currentName;
    }

    // Warning: loads from inputFiles is an array of names of files inside 'biobrickFilesPathPrefix'
    void loadDevices()
    {
        LinkedList<BioBrick> availableBioBricks = AvailableBioBricksManager.get().getAvailableBioBricks();
        LinkedList<BioBrick> allBioBricks = AvailableBioBricksManager.get().getAllBioBricks();

        LevelInfo levelInfo = null;
        MemoryManager.get().tryGetCurrentLevelInfo(out levelInfo);

        List<Device> devices = new List<Device>();

        DeviceLoader dLoader = new DeviceLoader(availableBioBricks, allBioBricks);

        string[] filesToLoad;
        string currentMapDevicesFilePath = MemoryManager.get().configuration.getGameMapName();

        if (null == levelInfo || !levelInfo.areAllDevicesAvailable)
        {
            filesToLoad = new string[] { currentMapDevicesFilePath };
        }
        else
        {
            List<string> fileList = new List<string>(deviceFiles);
            fileList.Add(currentMapDevicesFilePath);
            filesToLoad = fileList.ToArray();
        }
        foreach (string file in filesToLoad)
        {
            string fullPathFile = deviceFilesPathPrefix + file;
            // Debug.Log("Inventory::loadDevices loads device file " + fullPathFile);
            devices.AddRange(dLoader.loadDevicesFromFile(fullPathFile));
        }
        UpdateData(devices, new List<Device>(), new List<Device>());
    }

    public void switchDeviceKnowledge()
    {
        if (0 == _devices.Count)
        {
            // Debug.Log("Inventory::switchDeviceKnowledge calls loadDevices");
            loadDevices();
        }
        else
        {
            // Debug.Log("Inventory::switchDeviceKnowledge calls forgetDevices");
            forgetDevices();
        }
    }

    private void forgetDevices()
    {
        // Debug.Log("Inventory::forgetDevices calls inventory.UpdateData(List(), " + Logger.ToString<Device>(_devices) + "), List())");
        foreach (Device device in _devices)
        {
            //ask Equipment instead
            _displayer.askRemoveEquipedDevice(device);
            //ask Crafting instead
            _displayer.removeListedDevice(device);
            //ok
            _displayer.removeInventoriedDevice(device);
        }
        UpdateData(new List<Device>(), _devices, new List<Device>());
    }
}
