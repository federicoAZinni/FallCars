using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using TMPro;


public class MainMenuManager : NetworkBehaviour
{
    public static MainMenuManager Instances;

    [SerializeField] bool TestMode;
    const string KEY_REALEY_JOIN_CODE = "RelayJoinCode";

    public TextMeshProUGUI inputField_CodeRelay;
    public TextMeshProUGUI buttonTextCodeRelay;

    public int maxPlayerTest;


    private void Awake()
    {
        if (Instances != null) Destroy(gameObject);
        else Instances = this;
    }


    #region MultiPlayer
    public async void InitUnityServiceAuth()
    {
        var options = new InitializationOptions();

        string name = "";
        for (int i = 0; i < 10; i++)
        {
            string code = "qwertyuiopasdfghjklzxcvbnm";
            name += code.ToCharArray()[Random.Range(0, code.Length)];
        }
        options.SetProfile(name);
        await UnityServices.InitializeAsync(options);

        AuthenticationService.Instance.SignedIn += () => { Debug.Log(AuthenticationService.Instance.PlayerId); };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

       NetworkManager.Singleton.OnConnectionEvent += DebugClientConnection;
    }



    public async void CreateRelayMatch()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4); //webgl "southamerica-east1"
            string joinCodeRelay = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls"); // "wss"
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(relayServerData);
            // unityTransport.UseWebSockets = true;  "southamerica-east1"

            NetworkManager.Singleton.StartHost();

            buttonTextCodeRelay.text = joinCodeRelay;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

  

    public async void JoinRelayMatch() //dtls
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(ClearUnvailableCharacterOfTextMeshPro(inputField_CodeRelay.text));
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }


    public void DebugClientConnection(NetworkManager networkManager, ConnectionEventData connectionEventData)
    {
        if (!IsHost) return;
        
        if (NetworkManager.Singleton.ConnectedClientsList.Count == maxPlayerTest) //Aca mando el cambio de escena hay que cmabiarlo
            SceneManagerMenu.Instances.ChangeScene(SceneName.GameSceneMultiPlayer);
    }

    #endregion
    #region SinglePlayer
    public void StartSinglePlayer()
    {
        SceneManagerMenu.Instances.ChangeScene(SceneName.GameSceneSinglePlayer);
    }
    #endregion


    public void CopyToClipboard()
    {
        TextEditor te = new TextEditor();
        te.text = buttonTextCodeRelay.text;
        te.SelectAll();
        te.Copy();
    }


    public static string ClearUnvailableCharacterOfTextMeshPro(string a)
    {
        string result = "";

        foreach (char c in a)
            if (c >= 32 && c <= 126)
                result += c;

        return result;
    }

    private async void OnDisable()
    {
        if(AuthenticationService.Instance !=null)
        {
            if (TestMode) await AuthenticationService.Instance.DeleteAccountAsync();
        }
        
    }

}
