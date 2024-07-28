using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using System.Collections;

public class PlayFabManager : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public GameObject ErrorLoginWindow;
    public GameObject BanWindow;
    public GameObject Buttons;
    public GameObject DevelopersSegmentObject; // ������, ������� ����������, ���� ����� � �������� Developers

    private string playFabId;

    private void Start()
    {
        LoginWithAnonymous();
    }

    private void LoginWithAnonymous()
    {
        string customId = SystemInfo.deviceUniqueIdentifier;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        playFabId = result.PlayFabId; // ��������� PlayFabID
        NameText.text = playFabId;
        Debug.Log("�������� ����: " + playFabId);

        // �������� ���������� � ������������ � ��������� ��������
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnBanned);
        GetPlayerSegments(); // �������� �������� ����� ��������� �����
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        if (result.AccountInfo.TitleInfo.isBanned == true)
        {
            Debug.Log("����� �������.");
        }
        else
        {
            Debug.Log("����� ������� ����������� � �� �������.");
        }
    }

    private void OnBanned(PlayFabError fabError)
    {
        Debug.Log("����� �������.");
        StartCoroutine(BanWindowWaitForSecondCoroutine());
        Buttons.SetActive(false);
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.LogError("������ �����: " + error.ErrorMessage);

        if (error.Error == PlayFabErrorCode.AccountBanned)
        {
            OnBanned(error);
        }
        else
        {
            StartCoroutine(ErrorWindowWaitForSecondCoroutine());
            Buttons.SetActive(false);
        }
    }

    private IEnumerator ErrorWindowWaitForSecondCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        ErrorLoginWindow.SetActive(true);
    }

    private IEnumerator BanWindowWaitForSecondCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        BanWindow.SetActive(true);
    }

    private void GetPlayerSegments()
    {
        // ������ ���������� � ������������
        var request = new GetPlayerSegmentsRequest
        {
            PlayFabId = playFabId // ���������� ����������� PlayFabID
        };

        PlayFabClientAPI.GetPlayerSegments(request, OnGetPlayerSegmentsSuccess, OnGetPlayerSegmentsFailure);
    }

    private void OnGetPlayerSegmentsSuccess(GetPlayerSegmentsResult result)
    {
        Debug.Log("�������� ��� ������: " + playFabId);

        // �������� � ����� ���������
        if (result.Segments != null && result.Segments.Count > 0)
        {
            foreach (var segment in result.Segments)
            {
                Debug.Log("��� ��������: " + segment.Name);
                if (segment.Name == "Developers")
                {
                    DevelopersSegmentObject.SetActive(true); // �������� ������, ���� ����� � �������� Developers
                }
            }
        }
        else
        {
            Debug.Log("���� ����� �� ����������� �� � ������ ��������.");
        }
    }

    private void OnGetPlayerSegmentsFailure(PlayFabError error)
    {
        Debug.LogError("������ ��������� ���������: " + error.GenerateErrorReport());
    }
}
