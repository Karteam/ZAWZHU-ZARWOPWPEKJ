using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class AspectManager : NetworkBehaviour
{
    public static AspectManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // ������ �������� ��� ������� ������
    public List<PlayerAspectData> PlayerAspects = new List<PlayerAspectData>();

    [System.Serializable]
    public class PlayerAspectData
    {
        public ulong PlayerId; // ID ������
        public Aspect PlayerAspect; // ��������� ������
    }

    // ����� ��� ���������� ������� ������
    public void SetPlayerAspect(ulong playerId, Aspect aspect)
    {
        var existing = PlayerAspects.Find(p => p.PlayerId == playerId);
        if (existing != null)
        {
            existing.PlayerAspect = aspect;
        }
        else
        {
            PlayerAspects.Add(new PlayerAspectData { PlayerId = playerId, PlayerAspect = aspect });
        }
    }

    // ��������� ������� �� ID ������
    public Aspect GetPlayerAspect(ulong playerId)
    {
        var playerData = PlayerAspects.Find(p => p.PlayerId == playerId);
        return playerData?.PlayerAspect;
    }
}
