using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorSt : NetworkBehaviour
{
    public AudioSource audioSource;

    [HideInInspector]
    public NetworkVariable<bool> IsLocked = new NetworkVariable<bool>(true); // Начальное состояние - заблокировано

    [Range(0, 1)]
    public float ChanceOfLock = 0.5f;

    public override void OnNetworkSpawn()
    {
        // Выполняется только для сервера
        if (IsOwner)
        {
            IsLocked.Value = Random.value > ChanceOfLock;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") && IsLocked.Value)
        {
            if (other.TryGetComponent<Key>(out var key))
            {
                // Вызываем RPC для разблокировки двери
                UnlockDoorServerRpc();

                // Уничтожаем объект ключа
                Destroy(key.KeyObject);
                Debug.Log("Ключ использован для разблокировки двери");
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void UnlockDoorServerRpc()
    {
        if (IsLocked.Value)
        {
            Debug.Log("UnlockDoorServerRpc вызван");
            IsLocked.Value = false; // Разблокируем дверь на сервере
            audioSource?.Play(); // Проигрываем звук разблокировки
            UpdateClientsClientRpc(); // Уведомляем всех клиентов о разблокировке
            Debug.Log("Дверь разблокирована");
        }
    }

    [ClientRpc]
    private void UpdateClientsClientRpc()
    {
        IsLocked.Value = false; // Разблокируем дверь на клиентах
        audioSource?.Play(); // Проигрываем звук для всех клиентов
    }
}
