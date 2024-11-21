using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAspect", menuName = "Game/Aspect")]
public class Aspect1 : ScriptableObject
{
    public string AspectName; // �������� �������
    public List<Skill1> Skills; // ������ ������� �������
    public int Strength;       // ���� �������
}
