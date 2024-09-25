using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SceneManagerMenu : NetworkBehaviour
{
    public static SceneManagerMenu Instances;

    private void Awake()
    {
        if (Instances != null) Destroy(gameObject);
        else Instances = this;
    }
    public void ChangeScene(SceneName nameScene)
    {
        switch (nameScene)
        {
            case SceneName.GameSceneMultiPlayer:
                NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
                break;
            case SceneName.GameSceneSinglePlayer:
                SceneManager.LoadScene("Test", LoadSceneMode.Single);
                break;
            default:
                break;
        }
        
    }



}

public enum SceneName
{
    GameSceneMultiPlayer,
    GameSceneSinglePlayer
}
