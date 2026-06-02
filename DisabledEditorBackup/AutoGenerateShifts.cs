using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoGenerateShifts
{
    static AutoGenerateShifts()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoGenerateShifts_v1", false)) return;
        EditorPrefs.SetBool("AutoGenerateShifts_v1", true);

        // 1. Создаем папки
        if (!AssetDatabase.IsValidFolder("Assets/Data/Shift2")) AssetDatabase.CreateFolder("Assets/Data", "Shift2");
        if (!AssetDatabase.IsValidFolder("Assets/Data/Shift3")) AssetDatabase.CreateFolder("Assets/Data", "Shift3");

        // Пытаемся найти любую картинку, чтобы посетители не были невидимыми
        VisitorData template = AssetDatabase.LoadAssetAtPath<VisitorData>("Assets/Data/Shift1/ElenaMarkova.asset");
        Sprite defaultSprite = template != null ? template.visitorSprite : null;

        // --- СМЕНА 2 (ГЛАЗА) ---
        VisitorData s2_v1 = CreateVisitor("Assets/Data/Shift2/Ivan.asset", "Иван Иванов", "ID-111", "Серые", "Серые", false, defaultSprite);
        VisitorData s2_m1 = CreateVisitor("Assets/Data/Shift2/MonsterEyes.asset", "Олег Петров", "ID-666", "Зеленые", "Красные", true, defaultSprite);
        
        ShiftData shift2 = ScriptableObject.CreateInstance<ShiftData>();
        shift2.shiftName = "Смена 2";
        shift2.directiveText = "ВНИМАНИЕ! Участились случаи подделки цвета глаз. Сверяйте глаза в паспорте с базой!";
        shift2.shiftVisitors = new VisitorData[] { s2_v1, s2_m1 };
        AssetDatabase.CreateAsset(shift2, "Assets/Data/Shift2/Shift2.asset");

        // --- СМЕНА 3 (ДАТЫ) ---
        VisitorData s3_v1 = CreateVisitorWithDates("Assets/Data/Shift3/Anna.asset", "Анна Смирнова", "ID-222", "Голубые", "Голубые", "11.1985", "11.1985", false, defaultSprite);
        VisitorData s3_m1 = CreateVisitorWithDates("Assets/Data/Shift3/MonsterDate.asset", "Сергей Жуков", "ID-999", "Карие", "Карие", "08.1986", "01.1980", true, defaultSprite);
        
        ShiftData shift3 = ScriptableObject.CreateInstance<ShiftData>();
        shift3.shiftName = "Смена 3";
        shift3.directiveText = "ВНИМАНИЕ! Проверяйте СРОК ДЕЙСТВИЯ паспорта. Если даты не совпадают — изолировать!";
        shift3.shiftVisitors = new VisitorData[] { s3_v1, s3_m1 };
        AssetDatabase.CreateAsset(shift3, "Assets/Data/Shift3/Shift3.asset");

        // --- ОБНОВЛЯЕМ GameManager ---
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            ShiftData shift1 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Data/Shift1/Shift1.asset");
            if (shift1 != null) {
                gm.shiftsDatabase = new ShiftData[] { shift1, shift2, shift3 };
            } else {
                gm.shiftsDatabase = new ShiftData[] { shift2, shift3 };
            }
            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>[Antigravity] СМЕНА 2 и СМЕНА 3 УСПЕШНО СГЕНЕРИРОВАНЫ И ПОДКЛЮЧЕНЫ!</color>");
    }

    static VisitorData CreateVisitor(string path, string name, string id, string dossierEyes, string passportEyes, bool isMonster, Sprite s)
    {
        VisitorData v = ScriptableObject.CreateInstance<VisitorData>();
        v.dossierName = name; v.passportName = name;
        v.dossierId = id; v.passportId = id;
        v.dossierEyes = dossierEyes; v.passportEyes = passportEyes;
        v.isMonster = isMonster;
        v.visitorSprite = s;
        AssetDatabase.CreateAsset(v, path);
        return v;
    }

    static VisitorData CreateVisitorWithDates(string path, string name, string id, string eyes, string pEyes, string dDate, string pDate, bool isMonster, Sprite s)
    {
        VisitorData v = ScriptableObject.CreateInstance<VisitorData>();
        v.dossierName = name; v.passportName = name;
        v.dossierId = id; v.passportId = id;
        v.dossierEyes = eyes; v.passportEyes = pEyes;
        v.dossierExpDate = dDate; v.passportExpDate = pDate;
        v.isMonster = isMonster;
        v.visitorSprite = s;
        AssetDatabase.CreateAsset(v, path);
        return v;
    }
}
