using UnityEngine;

public enum SkillType1
{
    Passive,         // ��������� �������
    CooldownBased,   // ����� ��
    Chargeable,      // ����������� ������
    Consumable       // ������������ ����� �������
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill")]
public class Skill1 : ScriptableObject
{
    public string SkillName; // �������� ������
    public SkillType1 Type;   // ��� ������
    public KeyCode ActivationKey; // ������� ��� ��������� (��������, E, R, F)
    public bool Damage;      // ������� �� ����� ����
    public int DamageAmount; // ���� (���� Damage = true)
    public float Cooldown;   // ����������� (������ ��� CooldownBased � Chargeable)
    public int MaxCharges;   // ������������ ���������� ������� (��� Chargeable � Consumable)
    public int Level = 1;    // ������� ������
    public int MaxLevel = 3; // ������������ �������
    public string Description; // �������� ������

    // ���������
    public float RangeIncreasePerLevel;
    public int DamageIncreasePerLevel;
    public float CooldownReductionPerLevel;

    // ��� ����������� ������ � ����������
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Type == SkillType1.Passive || Type == SkillType1.Consumable)
        {
            Cooldown = 0;
        }

        if (!Damage)
        {
            DamageAmount = 0;
        }

        if (Type == SkillType1.Consumable || Type == SkillType1.Chargeable)
        {
            MaxCharges = Mathf.Max(1, MaxCharges);
        }
        else
        {
            MaxCharges = 0;
        }
    }
#endif
}
