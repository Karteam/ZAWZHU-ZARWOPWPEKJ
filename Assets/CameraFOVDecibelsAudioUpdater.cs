using UnityEngine;

public class CameraFOVDecibelsAudioUpdater : MonoBehaviour
{
    public Camera camera; // ������ �� ���� ������
    public AudioSource audioSource; // ������ �� �������� �����
    public float maxFOV = 100f; // ������������ �������� FOV
    public float minFOV = 60f; // ����������� �������� FOV
    public float smoothSpeed = 0.1f; // �������� ��������� FOV
    public float bassThreshold = -30f; // ����� ��� ��������� FOV �� ������ �����
    private float targetFOV; // ������� �������� FOV
    private float originalFOV; // ������������ �������� FOV

    void Start()
    {
        if (camera == null)
        {
            camera = Camera.main; // ������������ �������� ������, ���� �� �������
        }
        originalFOV = camera.fieldOfView; // ��������� ������������ �������� FOV
        targetFOV = originalFOV; // ���������� ������� �������� FOV �� ������������
    }

    void Update()
    {
        // �������� ������� ���� � ���������
        float bassDBValue = GetBassDBValue();

        // ���� ������� ��������� ���� ��������� �����, ������ FOV
        if (bassDBValue > bassThreshold)
        {
            // ����������� ������� ��������� � �������� FOV
            float normalizedBassVolume = Mathf.Clamp01((bassDBValue + 100f) / 100f); // ���������� � ��������� [0, 1]
            targetFOV = Mathf.Lerp(originalFOV, maxFOV, normalizedBassVolume); // �������� ������������ ��� FOV
        }
        else
        {
            targetFOV = originalFOV; // ��������� � ������������� �������� FOV ���� ��� �������
        }

        // ������� ����� �������� FOV
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime / smoothSpeed);
    }

    // ����� ��� ��������� ������ ���� � ���������
    private float GetBassDBValue()
    {
        float[] samples = new float[256];
        audioSource.GetOutputData(samples, 0);
        float currentBassVolume = 0f;

        // ��� ��������� ����, �� ����� ��������� ������ ������ �������
        for (int i = 0; i < samples.Length / 2; i++) // ��������� ������ �������� ������� ��� ������ ������
        {
            currentBassVolume += Mathf.Abs(samples[i]);
        }

        // ���������, ����� �� �� ������ �� ����
        if (currentBassVolume == 0)
            return -100f;

        return 20 * Mathf.Log10(currentBassVolume / (samples.Length / 2)); // ���������� �� 2, ����� �������� ������� ������ �� �����
    }
}
