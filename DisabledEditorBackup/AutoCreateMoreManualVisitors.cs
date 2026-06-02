using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoCreateMoreManualVisitors
{
    static AutoCreateMoreManualVisitors()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoCreateMoreManualVisitors_v1", false)) return;
        if (Application.isPlaying) return;
        EditorPrefs.SetBool("AutoCreateMoreManualVisitors_v1", true);

        // Создаем новых посетителей 7-12
        VisitorData[] newV = new VisitorData[6];
        for(int i = 0; i < 6; i++)
        {
            int num = i + 7;
            string path = "Assets/Visitors/Visitor_" + num.ToString("00") + ".asset";
            VisitorData v = AssetDatabase.LoadAssetAtPath<VisitorData>(path);
            if (v == null)
            {
                v = ScriptableObject.CreateInstance<VisitorData>();
                
                v.dossierName = "Персонаж " + num;
                v.dossierId = "ID-" + Random.Range(1000, 9999);
                v.dossierEyes = (num % 2 == 0) ? "Карие" : "Зеленые";
                v.dossierExpDate = "12.2084";
                
                v.passportName = v.dossierName;
                v.passportId = v.dossierId;
                v.passportEyes = v.dossierEyes;
                v.passportExpDate = v.dossierExpDate;

                // Назначаем монстров: 9, 10, 12
                if (num == 9 || num == 10 || num == 12)
                {
                    v.isMonster = true;
                    // Делаем какую-то ошибку в документах
                    if (num == 9) v.passportEyes = "Черные";
                    if (num == 10) v.passportName = "Персoнaж 10"; // Опечатка (латиница)
                    if (num == 12) v.passportExpDate = "12.2080"; // Просрочен
                }
                else
                {
                    v.isMonster = false;
                }
                
                AssetDatabase.CreateAsset(v, path);
            }
            newV[i] = v;
        }

        // Создаем Смену 3
        ShiftData manualShift3 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Visitors/ManualShift_3.asset");
        if (manualShift3 == null)
        {
            manualShift3 = ScriptableObject.CreateInstance<ShiftData>();
            AssetDatabase.CreateAsset(manualShift3, "Assets/Visitors/ManualShift_3.asset");
        }
        manualShift3.shiftName = "Смена 3";
        manualShift3.directiveText = "Сверяйте цвет глаз. Монстры плохо их подделывают.";
        manualShift3.shiftVisitors = new VisitorData[] { newV[0], newV[1], newV[2] };
        EditorUtility.SetDirty(manualShift3);

        // Создаем Смену 4
        ShiftData manualShift4 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Visitors/ManualShift_4.asset");
        if (manualShift4 == null)
        {
            manualShift4 = ScriptableObject.CreateInstance<ShiftData>();
            AssetDatabase.CreateAsset(manualShift4, "Assets/Visitors/ManualShift_4.asset");
        }
        manualShift4.shiftName = "Смена 4";
        manualShift4.directiveText = "Внимательно смотрите на сроки действия документов.";
        manualShift4.shiftVisitors = new VisitorData[] { newV[3], newV[4], newV[5] };
        EditorUtility.SetDirty(manualShift4);

        AssetDatabase.SaveAssets();

        // Обновляем GameManager
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            ShiftData s1 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Visitors/ManualShift_1.asset");
            ShiftData s2 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Visitors/ManualShift_2.asset");
            
            gm.shiftsDatabase = new ShiftData[] { s1, s2, manualShift3, manualShift4 };
            EditorUtility.SetDirty(gm);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
            Debug.Log("<color=green>[Antigravity] Добавлены персонажи 7-12 и Смены 3, 4!</color>");
        }
    }
}
