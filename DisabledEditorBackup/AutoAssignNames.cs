using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoAssignNames
{
    static AutoAssignNames()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAssignNames_v2", false)) return;
        if (Application.isPlaying) return;
        EditorPrefs.SetBool("AutoAssignNames_v2", true);

        // СНГ имена
        string[] names = new string[] {
            "Алексей Смирнов",    // 1 (М)
            "Елена Воронина",     // 2 (Ж, Монстр)
            "Дмитрий Соколов",    // 3 (М)
            "Артем Лебедев",      // 4 (М, Монстр)
            "Анна Морозова",      // 5 (Ж)
            "Мария Волкова",      // 6 (Ж, Монстр)
            "Иван Новиков",       // 7 (М)
            "Сергей Попов",       // 8 (М)
            "Наталья Орлова",     // 9 (Ж, Монстр, ошибка в глазах)
            "Виктор Зайцев",      // 10 (М, Монстр, ошибка в имени)
            "Максим Козлов",      // 11 (М)
            "Дарья Ильина"        // 12 (Ж, Монстр, ошибка в дате)
        };

        for(int i = 0; i < 12; i++)
        {
            int num = i + 1;
            string path = "Assets/Visitors/Visitor_" + num.ToString("00") + ".asset";
            VisitorData v = AssetDatabase.LoadAssetAtPath<VisitorData>(path);
            if (v != null)
            {
                v.dossierName = names[i];
                v.passportName = names[i];

                // Сохраняем логику опечатки для 10-го монстра
                if (num == 10) 
                {
                    v.passportName = "Виктар Зяйцев"; // Специальная опечатка
                }

                EditorUtility.SetDirty(v);
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>[Antigravity] Имена успешно назначены всем 12 персонажам!</color>");
    }
}
