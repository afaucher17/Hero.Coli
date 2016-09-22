﻿using UnityEngine;
using System.Collections.Generic;

public class MineManager : MonoBehaviour
{


    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private List<ResettableMine> _minesToReset = new List<ResettableMine>();
    private List<GameObject> _particleSystems = new List<GameObject>();
    public static string gameObjectName = "MineManager";
    private static MineManager _instance;
    public static MineManager get()
    {
        if (_instance == null)
        {
            Logger.Log("MineManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<MineManager>();
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("MineManager::Awake", Logger.Level.DEBUG);
        _instance = this;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    //public TextAsset sceneFilePath3;

    [SerializeField]
    private GameObject _mine;
    [SerializeField]
    private GameObject _particleSystem;
    private Vector3 positionShift = new Vector3(0, 10f, 0);

    public void detonate(ResettableMine mine)
    {
        // Debug.Log(mine.gameObject.name + " detonates");
        _minesToReset.Add(mine);
        GameObject instance = Instantiate(_particleSystem, mine.transform.position + positionShift, mine.transform.rotation, mine.transform.parent) as GameObject;
        _particleSystems.Add(instance);
    }

    public void resetSelectedMine(ResettableMine mine)
    {
        GameObject target = mine.gameObject;

        iTween.Stop(target, true);
        
        GameObject go = (GameObject)Instantiate(_mine, mine.transform.position, mine.transform.rotation, mine.transform.parent);
        ResettableMine newMine = go.GetComponent<ResettableMine>();
        newMine.mineName = mine.mineName;
        
        Destroy(target);
    }

    public void resetAllMines()
    {
        // Debug.Log("resetAllMines");
        foreach (ResettableMine mine in _minesToReset)
        {
            // Debug.Log("reset " + mine.gameObject.name);
            resetSelectedMine(mine);
        }

        foreach (GameObject particleSystem in _particleSystems)
        {
            // Debug.Log("Destroy " + particleSystem.gameObject.name);
            Destroy(particleSystem.gameObject);
        }

        _minesToReset.Clear();
        _particleSystems.Clear();
    }
}
