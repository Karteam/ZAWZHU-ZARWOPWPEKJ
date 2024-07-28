using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public enum DoorState { Opened, Closed }

public class Door : NetworkBehaviour
{
    public float interactionDistance;
    public GameObject intText;
    public string doorOpenAnimName, doorCloseAnimName;
    public AudioClip doorOpen, doorClose;
    public float fill = 0f;
    float maxFill = 100f;
    public Image progressBar;
    public GameObject Bar;
    public bool canOpen = true;
    public GameObject LockText;

    private Animator an;
    private AudioSource doorSound;

    // Синхронизация состояния двери
    private NetworkVariable<DoorState> doorState = new NetworkVariable<DoorState>(DoorState.Closed);

    private void Awake()
    {
        fill = progressBar.fillAmount;
    }

    public IEnumerator WaitSec()
    {
        yield return new WaitForSeconds(1);
        canOpen = true;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (fill < 0)
        {
            fill = 0;
        }

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                if (hit.collider.TryGetComponent<DoorSt>(out var ds) && !ds.islocked)
                {
                   an = hit.collider.transform.GetComponent<Animator>();


                    doorSound = hit.collider.gameObject.GetComponent<AudioSource>();

                    intText.SetActive(true);

                    if (Input.GetKey(KeyCode.E) && fill < maxFill && canOpen)
                    {
                        fill += Time.deltaTime * 160f; // Заполнить
                        progressBar.fillAmount = fill / maxFill;
                    }
                    else
                    {
                        fill -= Time.deltaTime * 160f; // Уменьшить
                        progressBar.fillAmount = fill / maxFill;
                    }

                    if (fill >= maxFill)
                    {
                        canOpen = false;
                        StartCoroutine(WaitSec());

                        if (an.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                        {
                            CloseDoorServerRpc();
                        }
                        else if (an.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimName))
                        {
                            OpenDoorServerRpc();
                        }

                        fill = 0;
                        progressBar.fillAmount = fill;
                    }
                }
                else
                {
                    LockText.SetActive(true);
                    intText.SetActive(false);
                    fill = 0;
                    progressBar.fillAmount = fill;
                }
            }
            else
            {
                LockText.SetActive(false);
                intText.SetActive(false);
                fill = 0;
                progressBar.fillAmount = fill;
            }
        }
        else
        {
            LockText.SetActive(false);
            intText.SetActive(false);
            fill = 0;
            progressBar.fillAmount = fill;
        }

       
    }

    // RPC для открытия двери
    [ServerRpc(RequireOwnership = false)]
    public void OpenDoorServerRpc()
    {
        OpenDoor();
        NotifyDoorStateChange(DoorState.Opened);
    }

    // RPC для закрытия двери
    [ServerRpc(RequireOwnership = false)]
    public void CloseDoorServerRpc()
    {
        CloseDoor();
        NotifyDoorStateChange(DoorState.Closed);
    }

    private void OpenDoor()
    {
      
       

        doorSound.clip = doorOpen;
        doorSound.Play();
        an.ResetTrigger("Close");
        an.SetTrigger("Open");
        doorState.Value = DoorState.Opened;
    }

    private void CloseDoor()
    {
    
     

        doorSound.clip = doorClose;
        doorSound.Play();
        an.ResetTrigger("Open");
        an.SetTrigger("Close");
        doorState.Value = DoorState.Closed;
    }

    private void NotifyDoorStateChange(DoorState newState)
    {
        doorState.Value = newState;
        // Можно добавить дополнительную логику, если нужно
    }


}
