﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoryManager : MonoBehaviour {
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "MemoryManager";
    private static MemoryManager _instance;
    public static MemoryManager get() {
        if (_instance == null)
        {
            Logger.Log("MemoryManager::get was badly initialized", Logger.Level.ERROR);
            _instance = GameObject.Find(gameObjectName).GetComponent<MemoryManager>();
            _instance.initializeIfNecessary();
        }
        
        return _instance;
    }
    void Awake()
    {
        Logger.Log("MemoryManager::Awake", Logger.Level.DEBUG);
        _instance = this;
        initializeIfNecessary();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////
    
    public string[] inputFiles;
    private Dictionary<string, string> _savedData = new Dictionary<string, string>();
    private Dictionary<string, LevelInfo> _loadedLevelInfo = new Dictionary<string, LevelInfo>();

    private void initializeIfNecessary(bool onlyIfEmpty = true)
    {
        Debug.LogError("MemoryManager::Awake loadLevelData before");
        if(!onlyIfEmpty || 0 == _loadedLevelInfo.Count)
        {
            loadLevelData(inputFiles, _loadedLevelInfo);
        }
        Debug.LogError("MemoryManager::Awake loadLevelData after");
    }

    public bool addData(string key, string value)
    {
        if(!_savedData.ContainsKey(key))
        {
            _savedData.Add(key, value);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool tryGetData(string key, out string value)
    {
        Debug.LogError("MemoryManager::tryGetData("+key+", out value)");
        return _savedData.TryGetValue(key, out value);
    }

    private void loadLevelData(string[] inputFiles, Dictionary<string, LevelInfo> dico)
    {      
        Debug.LogError("MemoryManager::loadLevelData starts");
        FileLoader loader = new FileLoader();
    
        foreach (string file in inputFiles)
        {
            Debug.LogError("MemoryManager::loadLevelData processing file="+file);
            LinkedList<LevelInfo> lis = loader.loadObjectsFromFile<LevelInfo>(file,LevelInfoXMLTags.INFO);
            if(null != lis)
            {
                Debug.LogError("MemoryManager::loadLevelData null != lis");
                foreach( LevelInfo li in lis)
                {
                    Debug.LogError("MemoryManager::loadLevelData adding li="+li);
                    dico.Add(li.code, li);
                }
            }

        }
        
        Logger.Log("ModalManager::loadDataIntoDico loaded ", Logger.Level.DEBUG);
        Debug.LogError("MemoryManager::loadLevelData ends");
    }
    
    private static LevelInfo retrieveFromDico(string code)
    {
        LevelInfo info;
        //TODO set case-insensitive
        if(!_instance._loadedLevelInfo.TryGetValue(code, out info))
        {
            Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
            info = null;
        }
        return info;
    }

    public bool tryGetCurrentLevelInfo(out LevelInfo levelInfo)
    {
        levelInfo = null;
        string currentLevelCode;
        if(tryGetData(GameStateController._currentLevelKey, out currentLevelCode))
        {
            return _loadedLevelInfo.TryGetValue(currentLevelCode, out levelInfo);
        }
        else
        {
            Logger.Log("MemoryManager::tryGetCurrentLevelInfo failed to provide data; GameStateController._currentLevelKey="+GameStateController._currentLevelKey, Logger.Level.WARN);
            return false;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
