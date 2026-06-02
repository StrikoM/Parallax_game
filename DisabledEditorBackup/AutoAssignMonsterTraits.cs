using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoAssignMonsterTraits
{
    static AutoAssignMonsterTraits()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAssignMonsterTraits_v1", false)) return;
        
        EditorPrefs.SetBool("AutoAssignMonsterTraits_v1", true);

        string[] guids = AssetDatabase.FindAssets("t:VisitorData");
        int monsterCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            VisitorData vd = AssetDatabase.LoadAssetAtPath<VisitorData>(path);
            if (vd != null && vd.isMonster)
            {
                monsterCount++;
                
                // Сбрасываем старые трейты
                vd.isImpatient = false;
                vd.isMimic = false;

                // Чередуем: 1-й нетерпеливый, 2-й мимик, 3-й обычный, и так по кругу
                if (monsterCount % 3 == 1)
                {
                    vd.isImpatient = true;
                }
                else if (monsterCount % 3 == 2)
                {
                    vd.isMimic = true;
                }
                
                EditorUtility.SetDirty(vd);
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"<color=green>[Antigravity] Трейты Нетерпеливый/Мимик автоматически распределены среди {monsterCount} монстров!</color>");
    }
}
