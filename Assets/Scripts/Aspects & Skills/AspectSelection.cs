using UnityEngine;
using Unity.Netcode;

public class AspectSelection : MonoBehaviour
{
    public Aspect[] AvailableAspects; // ��������� �������

    public void SelectAspect(int aspectIndex)
    {
        var selectedAspect = AvailableAspects[aspectIndex];
        AspectManager.Instance.SetPlayerAspect(NetworkManager.Singleton.LocalClientId, selectedAspect);

        // ������� �� ��������� �����
        UnityEngine.SceneManagement.SceneManager.LoadScene("TEST");
    }
}
