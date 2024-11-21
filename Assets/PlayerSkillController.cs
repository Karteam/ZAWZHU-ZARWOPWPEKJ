using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public Aspect PlayerAspect;

    private float[] skillCooldownTimers;

    private void Start()
    {
        if (PlayerAspect != null)
        {
            skillCooldownTimers = new float[PlayerAspect.Skills.Length];
        }
    }

    private void Update()
    {
        // ��������� ������� ��
        for (int i = 0; i < skillCooldownTimers.Length; i++)
        {
            if (skillCooldownTimers[i] > 0)
                skillCooldownTimers[i] -= Time.deltaTime;
        }

        // ��������� ������� ������
        for (int i = 0; i < PlayerAspect.Skills.Length; i++)
        {
            var skill = PlayerAspect.Skills[i];
            if (Input.GetKeyDown(skill.activationKey) && skillCooldownTimers[i] <= 0)
            {
                ActivateSkill(skill, i);
            }
        }
    }

    private void ActivateSkill(Skill skill, int skillIndex)
    {
        // ��������� ��� ������ � ����������
        if (skill.skillType == SkillType.Passive) return;

        // ������ ���������
        if (skill is HackerHealSkill hackerHeal)
        {
            hackerHeal.ActivateSkill();
        }

        // ������������� ��
        skillCooldownTimers[skillIndex] = skill.cooldownTime;
    }
}
