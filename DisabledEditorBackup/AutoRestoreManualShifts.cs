using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoRestoreManualShifts
{
    static AutoRestoreManualShifts()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoRestoreManualShifts_v1", false)) return;
        if (Application.isPlaying) return;
        
        EditorPrefs.SetBool("AutoRestoreManualShifts_v1", true);

        // Загружаем созданные пользователем файлы из папки Visitors
        VisitorData v1 = AssetDatabase.LoadAssetAtPath<VisitorData>("Assets/Visitors/Visitor_01.asset");
        VisitorData v2 = AssetDatabase.LoadAssetAtPath<VisitorData>("Assets/Visitors/Visitor_02.asset");
        VisitorData v3 = AssetDatabase.LoadAssetAtPath<VisitorData>("Assets/Visitors/Visitor_03.asset");
        VisitorData v4 = AssetDatabase.LoadAssetAtPath<VisitorData>("Assets/Visitors/Visitor_04.asset");
        VisitorData v5 = AssetDatabase.LoadAssetAtPath<VisitorData>("Assets/Visitors/Visitor_05.asset");
        VisitorData v6 = AssetDatabase.LoadAssetAtPath<VisitorData>("Assets/Visitors/Visitor_06.asset");

        if (v1 == null) 
        {
            Debug.LogError("Не удалось найти Visitor_01 в папке Assets/Visitors!");
            return; 
        }

        // Создаем ручную смену 1
        ShiftData manualShift1 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Visitors/ManualShift_1.asset");
        if (manualShift1 == null)
        {
            manualShift1 = ScriptableObject.CreateInstance<ShiftData>();
            AssetDatabase.CreateAsset(manualShift1, "Assets/Visitors/ManualShift_1.asset");
        }
        manualShift1.shiftName = "Смена 1";
        manualShift1.directiveText = "Сверяйте данные и следите за окном.";
        manualShift1.shiftVisitors = new VisitorData[] { v1, v2, v3 };
        EditorUtility.SetDirty(manualShift1);

        // Создаем ручную смену 2
        ShiftData manualShift2 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Visitors/ManualShift_2.asset");
        if (manualShift2 == null)
        {
            manualShift2 = ScriptableObject.CreateInstance<ShiftData>();
            AssetDatabase.CreateAsset(manualShift2, "Assets/Visitors/ManualShift_2.asset");
        }
        manualShift2.shiftName = "Смена 2";
        manualShift2.directiveText = "Остерегайтесь подделок. Монстры уже здесь.";
        manualShift2.shiftVisitors = new VisitorData[] { v4, v5, v6 };
        EditorUtility.SetDirty(manualShift2);

        AssetDatabase.SaveAssets();

        // Обновляем GameManager
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.shiftsDatabase = new ShiftData[] { manualShift1, manualShift2 };
            EditorUtility.SetDirty(gm);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
            Debug.Log("<color=green>[Antigravity] Игра переведена на ваши 6 личных персонажей!</color>");
        }
    }
}
