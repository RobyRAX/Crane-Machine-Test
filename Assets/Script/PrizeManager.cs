using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrizeList
{
    public string prizeName;
    public int count;
}

public class PrizeManager : MonoBehaviour
{
    public static event Action<PrizeList> OnGotPrize;

    public static PrizeManager Instance;
    public List<PrizeList> prizeLists;

    private void Awake()
    {
        Instance = this;

        PrizeController.OnPrizeGranted += StorePrize;
        ClawController.OnStateReset += CheckPrizes;
        GameManager.OnGameStateChanged += GameStateChangeHandler;
    }

    private void Start()
    {
        foreach(GameObject prize in PrizeSpawner.Instance.prizePrefabs)
        {
            PrizeList tempPrizeList = new PrizeList();
            tempPrizeList.prizeName = prize.name;
            tempPrizeList.count = 0;
            prizeLists.Add(tempPrizeList);
        }
    }

    void GameStateChangeHandler(GameState state)
    {
        if(state == GameState.Gameplay)
        {
            ResetPrize();
        }
    }

    private void ResetPrize()
    {
        foreach (PrizeList prizeList in prizeLists)
        {
            prizeList.count = 0;
        }
    }

    void StorePrize(GameObject prize)
    {
        foreach(PrizeList prizeList in prizeLists)
        {
            if(prize.name == prizeList.prizeName)
            {
                prizeList.count++;
            }
        }
    }

    void CheckPrizes()
    {
        foreach (PrizeList prizeList in prizeLists)
        {
            if (prizeList.count > 0)
            {
                OnGotPrize(prizeList);
            }                
        }
    }
}
