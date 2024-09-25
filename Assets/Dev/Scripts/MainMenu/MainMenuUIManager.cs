using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] GameObject[] uiMenuPanels;
    [SerializeField] GameObject bg;
    int currentPanelActivated;
    int lastPanelActivated;
   
    public void MainMenuChangePanels(int indexPanel)
    {
        if (indexPanel >= uiMenuPanels.Length) return;
        if (indexPanel == 4) bg.SetActive(false);
        else bg.SetActive(true);
        uiMenuPanels[currentPanelActivated].SetActive(false);
        lastPanelActivated = currentPanelActivated;
        currentPanelActivated = indexPanel;
        uiMenuPanels[currentPanelActivated].SetActive(true);
    }

    public void BackButton()
    {
        MainMenuChangePanels(lastPanelActivated);
    }

    public void MultiPlayerBtn()
    {
        MainMenuManager.Instances.InitUnityServiceAuth();
    }

    public void SinglePlayerBtn()
    {
        //MainMenuManager.Instances.StartSinglePlayer();
    }
}
