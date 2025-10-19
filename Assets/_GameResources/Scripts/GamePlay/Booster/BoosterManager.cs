using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoosterManager : SingletonMono<BoosterManager>
{
    [SerializeField] private BoosterDataSO boosterData;
    [SerializeField] private List<BoosterBase> boosters;
    [SerializeField] private BoosterBase addTrayBooster;
    [SerializeField] private BoosterBase shuffleBooster;
    [SerializeField] private BoosterBase magnetBooster;
    [SerializeField] private BoosterBase handMoveBooster;
    public BoosterBase AddTrayBooster { get => addTrayBooster; set => addTrayBooster = value; }
    public BoosterBase ShuffleBooster { get => shuffleBooster; set => shuffleBooster = value; }
    public BoosterBase HandMoveBooster { get => handMoveBooster; set => handMoveBooster = value; }
    public BoosterBase MagnetBooster {get => magnetBooster; set => magnetBooster = value; }

    public BoosterDataSO BoosterData { get => boosterData; }
    public List<BoosterBase> Boosters { get => boosters; }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (var booster in Boosters)
        {
            booster.Init();
        }
    }

    public void UpdateVisualBooster()
    {
        foreach (var booster in Boosters)
        {
            booster.UpdateVisualBooster();
        }
    }

    public BoosterBase GetBoosterByType(BoosterType type)
    {
        return boosters.FirstOrDefault(booster => booster.BoosterType == type);
    }
}
