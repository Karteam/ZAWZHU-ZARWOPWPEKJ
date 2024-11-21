using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Skill skill = (Skill)target;

        // ��������� ��� Skill
        EditorGUILayout.LabelField("Skill Configuration", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // ���������� ���� skillName
        skill.skillName = EditorGUILayout.TextField("Skill Name", skill.skillName);

        // ���������� ���� ��� ������ ������
        skill.skillIcon = (Sprite)EditorGUILayout.ObjectField("Skill Icon", skill.skillIcon, typeof(Sprite), false);

        // ���������� skillType � ��������� ��� ��������
        skill.skillType = (SkillType)EditorGUILayout.EnumPopup("Skill Type", skill.skillType);
        EditorGUILayout.Space();

        // ���������� activationKey � ��������� ��� ��������
        skill.activationKey = (KeyCode)EditorGUILayout.EnumPopup("Activation Key", skill.activationKey);
        EditorGUILayout.Space();

        // ���� skillType ����� Passive ��� Cooldown, ������� ������ � Charges Settings
        if (skill.skillType == SkillType.Cooldown || skill.skillType == SkillType.Passive)
        {
            // �� ���������� Charges Settings
        }
        else
        {
            // HEADER Charges Settings
            EditorGUILayout.LabelField("Charges Settings", EditorStyles.boldLabel);

            // ���� skillType ����� Charging, �������� maxCharges
            if (skill.skillType != SkillType.Charging)
            {
                skill.maxCharges = EditorGUILayout.IntField("Max Charges", skill.maxCharges);
            }

            skill.chargesAmount = EditorGUILayout.IntField("Charges Amount", skill.chargesAmount);
            EditorGUILayout.Space();
        }

        // HEADER Cooldown Settings
        if (skill.skillType != SkillType.Passive && skill.skillType != SkillType.Charging) // ��������� ������� ��� Charging
        {
            EditorGUILayout.LabelField("Cooldown Settings", EditorStyles.boldLabel);

            // ���� skillType ����� Cooldown, ������ ������������� has_A_Cooldown � true � ��������� ����
            if (skill.skillType == SkillType.Cooldown)
            {
                skill.has_A_Cooldown = true; // ������������� has_A_Cooldown � true
                EditorGUI.BeginDisabledGroup(true); // ��������� ����
                EditorGUILayout.Toggle("Has A Cooldown", skill.has_A_Cooldown);
                EditorGUI.EndDisabledGroup(); // �������� ��� �������
            }
            else
            {
                skill.has_A_Cooldown = EditorGUILayout.Toggle("Has A Cooldown", skill.has_A_Cooldown);
            }

            if (skill.has_A_Cooldown)
            {
                skill.cooldownTime = EditorGUILayout.FloatField("Cooldown Time", skill.cooldownTime);
            }

            EditorGUILayout.Space();
        }

        // HEADER Damage Settings
        if (skill.skillType != SkillType.Passive)
        {
            EditorGUILayout.LabelField("Damage Settings", EditorStyles.boldLabel);
            skill.dealsDamage = EditorGUILayout.Toggle("Deals Damage", skill.dealsDamage);

            if (skill.dealsDamage)
            {
                skill.damage = EditorGUILayout.FloatField("Damage", skill.damage);
            }
            EditorGUILayout.Space();
        }

        // Header Leveling Up
        EditorGUILayout.LabelField("Leveling Up", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // ������������ ���������� ��������� �� 3
        if (skill.upgrades.Length > 3)
        {
            Array.Resize(ref skill.upgrades, 3);
        }

        for (int i = 0; i < skill.upgrades.Length; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // �������� ���� ��������� � ����������� �� skillType
            if (skill.skillType == SkillType.Passive)
            {
                // ����������� ������ ��� UpgradeType
                UpgradeType selectedUpgradeType = (UpgradeType)EditorGUILayout.EnumPopup($"Upgrade Type {i + 1}", skill.upgrades[i].upgradeType);
                if (selectedUpgradeType == UpgradeType.Damage)
                {
                    EditorGUILayout.HelpBox("Cannot select Damage upgrade type for Passive skills.", MessageType.Warning);
                }
                else
                {
                    skill.upgrades[i].upgradeType = selectedUpgradeType;
                }
            }
            else
            {
                // ��� ���� �����, ����� Passive
                skill.upgrades[i].upgradeType = (UpgradeType)EditorGUILayout.EnumPopup($"Upgrade Type {i + 1}", skill.upgrades[i].upgradeType);
            }

            // ������ ������� ����������� ��� Damage upgrade, ���� dealsDamage true � �� Passive
            if (skill.skillType != SkillType.Passive && skill.dealsDamage)
            {
                // �������� �� ��� ����������� "Damage upgrade"
                if (skill.upgrades[i].upgradeType == UpgradeType.Damage)
                {
                    skill.upgrades[i].newValue = EditorGUILayout.FloatField("New Value", skill.upgrades[i].newValue);
                }
                else
                {
                    // ���� ��� ��������� �� "Damage", ���������� ���� ��� ����� New Value
                    skill.upgrades[i].newValue = EditorGUILayout.FloatField("New Value", skill.upgrades[i].newValue);
                }
            }
            else
            {
                // ���� �� ��������� ��������� Damage, ������ ���������� N/A
                EditorGUILayout.LabelField("New Value", "N/A", EditorStyles.label);
            }

            // ������ ��� �������� ���������
            if (GUILayout.Button($"Remove Upgrade {i + 1}"))
            {
                Array.Copy(skill.upgrades, i + 1, skill.upgrades, i, skill.upgrades.Length - i - 1);
                Array.Resize(ref skill.upgrades, skill.upgrades.Length - 1);
                break; // ��������� ����, ����� �������� ��������� ������� ������� ��� ��������
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // ���� ����� ������� ������ 3, ���������� ������ ��� ���������� ������ ���������
        if (skill.upgrades.Length < 3)
        {
            if (GUILayout.Button("Add Upgrade"))
            {
                Array.Resize(ref skill.upgrades, skill.upgrades.Length + 1);
                skill.upgrades[skill.upgrades.Length - 1] = new SkillUpgrade(); // �������������� ����� ���������
            }
        }

        // ��������� ��������� � �������
        if (GUI.changed)
        {
            EditorUtility.SetDirty(skill);
        }
    }
}