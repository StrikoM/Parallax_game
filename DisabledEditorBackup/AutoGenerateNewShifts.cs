using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class AutoGenerateNewShifts
{
    static AutoGenerateNewShifts()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Сгенерировать Смены 3 и 4 (Газета)")]
    public static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (EditorPrefs.GetBool("AutoGenerateNewShifts_v1", false)) return;
        EditorPrefs.SetBool("AutoGenerateNewShifts_v1", true);

        string dbPath = "Assets/Database";
        if (!AssetDatabase.IsValidFolder(dbPath)) AssetDatabase.CreateFolder("Assets", "Database");
        
        string vPath = dbPath + "/Visitors";
        if (!AssetDatabase.IsValidFolder(vPath)) AssetDatabase.CreateFolder(dbPath, "Visitors");
        
        string sPath = dbPath + "/Shifts";
        if (!AssetDatabase.IsValidFolder(sPath)) AssetDatabase.CreateFolder(dbPath, "Shifts");

        // --- ГЕНЕРИРУЕМ НОВЫХ ПОСЕТИТЕЛЕЙ (КУЛЬТИСТЫ) ---
        VisitorData c1 = CreateVisitor("Cultist_1", "Виктор", "Мрачный", "2390-11", "Красные", "05.2085", true, "Красные", true, false, "Мы видим всё.");
        VisitorData c2 = CreateVisitor("Cultist_2", "Елена", "Тихая", "7712-99", "Красные", "11.2086", true, "Красные", true, true, "Они придут за вами.");
        
        // --- ГЕНЕРИРУЕМ ПРОСРОЧЕННЫЕ (УКРАДЕННЫЕ БЛАНКИ) ---
        VisitorData e1 = CreateVisitor("Expired_1", "Иван", "Петров", "1122-33", "Карие", "10.2084", true, "Карие", false, false, "Пустите меня быстрее.");
        VisitorData e2 = CreateVisitor("Expired_2", "Ольга", "Смирнова", "9988-77", "Зеленые", "08.2084", true, "Зеленые", false, true, "Я опаздываю!");
        
        // --- НОРМАЛЬНЫЕ ЛЮДИ ---
        VisitorData n1 = CreateVisitor("Normal_10", "Анна", "Белая", "4455-66", "Голубые", "12.2084", false, "Голубые", false, false, "Добрый день.");
        VisitorData n2 = CreateVisitor("Normal_11", "Борис", "Светлый", "3322-11", "Карие", "01.2085", false, "Карие", false, false, "Здравствуйте.");

        AssetDatabase.SaveAssets();

        // --- СОЗДАЕМ СМЕНУ 3: КУЛЬТ ---
        ShiftData shift3 = ScriptableObject.CreateInstance<ShiftData>();
        shift3.shiftName = "Смена 3";
        shift3.hasNewspaper = true;
        shift3.newspaperHeadline = "КУЛЬТ КРАСНОГО ГЛАЗА";
        shift3.newspaperBody = "Разыскиваются опасные сектанты. Особая примета: КРАСНЫЕ ГЛАЗА. Впускать их в дом строго запрещено, даже если документы в порядке!";
        shift3.directiveText = "Проверяйте цвет глаз. Красные = Отказ.";
        shift3.shiftVisitors = new VisitorData[] { n1, c1, n2, c2 };
        
        AssetDatabase.CreateAsset(shift3, sPath + "/Shift_3.asset");

        // --- СОЗДАЕМ СМЕНУ 4: КРАДЕНЫЕ ПАСПОРТА ---
        ShiftData shift4 = ScriptableObject.CreateInstance<ShiftData>();
        shift4.shiftName = "Смена 4";
        shift4.hasNewspaper = true;
        shift4.newspaperHeadline = "КРАЖА ПАСПОРТОВ";
        shift4.newspaperBody = "Полиция сообщает о краже пустых бланков старого образца. Все паспорта, срок действия которых заканчивается раньше 12.2084, считать недействительными.";
        shift4.directiveText = "Проверяйте дату. Меньше 12.2084 = Отказ.";
        shift4.shiftVisitors = new VisitorData[] { n2, e1, n1, c1, e2 };
        
        AssetDatabase.CreateAsset(shift4, sPath + "/Shift_4.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("[Antigravity] Смены 3 и 4 успешно сгенерированы! Не забудьте добавить их в базу смен.");
    }

    private static VisitorData CreateVisitor(string fName, string name, string last, string id, string eyes, string exp, bool isM, string docEyes, bool isImp, bool isMim, string speech)
    {
        VisitorData v = ScriptableObject.CreateInstance<VisitorData>();
        v.dossierName = name + "\n" + last;
        v.dossierId = "ID:\n" + id;
        v.dossierEyes = eyes;
        v.dossierExpDate = "12.2084"; // база данных
        
        v.passportName = name + " " + last;
        v.passportId = id;
        v.passportEyes = docEyes;
        v.passportExpDate = exp;
        
        v.isMonster = isM;
        v.isImpatient = isImp;
        v.isMimic = isMim;
        v.welcomeSpeech = speech;
        
        v.responseName = "С моим именем всё нормально.";
        v.responseEyes = "Это контактные линзы.";
        v.responseDate = "Я просто забыл его поменять.";

        AssetDatabase.CreateAsset(v, "Assets/Database/Visitors/" + fName + ".asset");
        return v;
    }
}
