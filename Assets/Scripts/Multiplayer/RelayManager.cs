using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    public TMP_InputField joinCodeInputField;
    public Button connectButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI lobbyCodeText;
    public TextMeshProUGUI playersCountText;
    public Button hostButton;
    public GameObject ConnectionWindow;
    public GameObject GameWindow;

    private void Start()
    {
        InitializeUnityServices();

        connectButton.onClick.AddListener(OnConnectButtonClicked);
        hostButton.onClick.AddListener(StartHost);

        ConnectionWindow.SetActive(true);
        GameWindow.SetActive(false);

        UpdatePlayersCount();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await SignInAsync();
            statusText.text = "������� Unity ������� ���������������� � �����������������!";
        }
        catch (System.Exception ex)
        {
            statusText.text = "������ ������������� Unity Services ��� ��������������: " + ex.Message;
            Debug.LogError("������ ������������� Unity Services ��� ��������������: " + ex.Message);
        }
    }

    private async Task SignInAsync()
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("�������� ���� � �������!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("������ ��������������: " + ex.Message);
            statusText.text = "������ ��������������: " + ex.Message;
        }
    }

    private async void OnConnectButtonClicked()
    {
        string joinCode = joinCodeInputField.text;

        if (string.IsNullOrEmpty(joinCode))
        {
            statusText.text = "��� �� �������, ��� ����� �� ����� ���� ������!";
            return;
        }

        statusText.text = "����������� � �����...";

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            ConnectionWindow.SetActive(false);
            GameWindow.SetActive(true);
            statusText.text = "�������� ����������� � �����!";

            lobbyCodeText.text = "��� �����:" + joinCode;
            lobbyCodeText.text += " | �� �����";

            UpdatePlayersCount();
        }
        catch (System.Exception ex)
        {
            statusText.text = "������ ����������� � �����: " + ex.Message;
        }
    }

    public async void StartHost()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log("Join Code: " + joinCode);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        NetworkManager.Singleton.StartHost();
        ConnectionWindow.SetActive(false);
        GameWindow.SetActive(true);

        lobbyCodeText.text = "��� �����:" + joinCode;
        lobbyCodeText.text += " | �� ����";

        UpdatePlayersCount();
    }

    private void UpdatePlayersCount()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        playersCountText.text = $"�������: {playerCount}/4"; 
    }


    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            UpdatePlayersCount();
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            UpdatePlayersCount();
        };
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= (clientId) =>
        {
            UpdatePlayersCount();
        };

        NetworkManager.Singleton.OnClientDisconnectCallback -= (clientId) =>
        {
            UpdatePlayersCount();
        };
    }
}
