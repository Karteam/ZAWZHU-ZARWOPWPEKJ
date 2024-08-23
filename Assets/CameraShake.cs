using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // ���������� ��������� ��� ������ ������
    [Header("Shake Settings")]
    public float shakeDuration = 1f; // ������������ ������
    public float shakeMagnitude = 0.2f; // ��������� ������
    public float shakeFrequency = 1f; // ������� ������

    private Vector3 originalPosition; // �������� ��������� ������
    private float shakeTime = 0f; // ����� ������

    void Start()
    {
        originalPosition = transform.localPosition; // ��������� �������� �������
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            // �����������, ������� ������� �������� �� ��������� ������
            shakeTime -= Time.deltaTime;

            // ��������� ����� �������� � ����������� �� ������� � �������
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // ��������� ����� � ������
            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            // ���� �����, ���������� �� ������, �����������, ���������� ��������
            if (shakeTime <= 0)
            {
                shakeTime = 0;
                transform.localPosition = originalPosition; // ���������� � �������� ���������
            }
        }
    }

    // ��������� ����� ��� ������ ������ ������
    public void ShakeCamera()
    {
        shakeTime = shakeDuration; // ������������� ����� ������
    }
}
