using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject MenuPanel;
    public GameObject ReadyPanel;
    public GameObject ResultPanel;

    [Header("Result Report")]
    public GameObject panelReportPrefab;
    public Transform panelReportParent;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameStateChangeHandler;
        PrizeManager.OnGotPrize += CreateReport;
    }

    void GameStateChangeHandler(GameState state)
    {
        MenuPanel.SetActive(state == GameState.Menu);
        ReadyPanel.SetActive(state == GameState.GameInit);
        ResultPanel.SetActive(state == GameState.Result);

        if(state == GameState.Gameplay)
        {
            ClearReport();
        }
    }

    void CreateReport(PrizeList prizeList)
    {
        GameObject report = Instantiate(panelReportPrefab, panelReportParent);

        report.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prizeList.prizeName;
        report.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x {prizeList.count}";
    }

    void ClearReport()
    {
        foreach(Transform child in panelReportParent)
        {
            Destroy(child.gameObject);
        }
    }
}
