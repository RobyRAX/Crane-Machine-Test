using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    GameInit,
    Gameplay,
    Result,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action<GameState> OnGameStateChanged;

    public GameState currentState;

    [SerializeField] float delayToGameplay;

    private void Awake()
    {
        Instance = this;

        PrizeSpawner.OnPrizeSpawnDone += PrizeSpawnDoneHandler;
        PrizeManager.OnGotPrize += GotPrizeHandler;
    }

    private void Start()
    {
        UpdateGameState(GameState.Menu);
    }

    void PrizeSpawnDoneHandler()
    {
        StartCoroutine(ToGameplayCo());
    }

    void GotPrizeHandler(PrizeList prizeList)
    {
        UpdateGameState(GameState.Result);
    }

    IEnumerator ToGameplayCo()
    {
        yield return new WaitForSeconds(delayToGameplay);
        UpdateGameState(GameState.Gameplay);
    }


    //------------------ SIMPLE STATE MACHINE ------------------//
    void UpdateGameState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Menu:
                MenuHandler();
                break;
            case GameState.GameInit:
                InitHandler();
                break;
            case GameState.Gameplay:
                GameplayHandler();
                break;
            case GameState.Result:
                ResultHandler();
                break;
        }
        OnGameStateChanged(currentState);
    }

    void MenuHandler()
    {
        
    }

    void InitHandler()
    {

    }

    void GameplayHandler()
    {

    }

    void ResultHandler()
    {

    }

    //------------------ END ------------------//

    public void PlayButton()
    {
        UpdateGameState(GameState.GameInit);
    }

    public void PlayAgainButton()
    {
        UpdateGameState(GameState.Gameplay);
    }

}
