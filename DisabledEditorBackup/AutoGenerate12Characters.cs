using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoGenerate12Characters
{
    static AutoGenerate12Characters()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoGen12_v1", false)) return;
        EditorPrefs.SetBool("AutoGen12_v1", true);

        // --- СМЕНА 1 (Ошибки в Имени или ID) ---
        VisitorData s1_1 = Create("Assets/Data/Shift1/Char1_Human.asset", "Алексей Смирнов", "Алексей Смирнов", "ID-100", "ID-100", "Карие", "Карие", "10.1985", "10.1985", false, "Здравствуйте, вот мои документы.");
        VisitorData s1_2 = Create("Assets/Data/Shift1/Char2_Monster.asset", "Мария Иванова", "Мария Петрова", "ID-101", "ID-101", "Зеленые", "Зеленые", "12.1986", "12.1986", true, "Впустите меня..."); // Ошибка: Фамилия не совпадает
        VisitorData s1_3 = Create("Assets/Data/Shift1/Char3_Human.asset", "Дмитрий Соколов", "Дмитрий Соколов", "ID-102", "ID-102", "Голубые", "Голубые", "01.1984", "01.1984", false, "Я очень устал после работы.");
        VisitorData s1_4 = Create("Assets/Data/Shift1/Char4_Monster.asset", "Елена Попова", "Елена Попова", "ID-103", "ID-666", "Серые", "Серые", "05.1988", "05.1988", true, "Мясо... то есть, здравствуйте."); // Ошибка: ID поддельный

        ShiftData shift1 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Data/Shift1/Shift1.asset");
        if (shift1 != null) {
            shift1.shiftVisitors = new VisitorData[] { s1_1, s1_2, s1_3, s1_4 };
            EditorUtility.SetDirty(shift1);
        }

        // --- СМЕНА 2 (Ошибки в ГЛАЗАХ) ---
        VisitorData s2_1 = Create("Assets/Data/Shift2/Char5_Human.asset", "Игорь Морозов", "Игорь Морозов", "ID-200", "ID-200", "Серые", "Серые", "08.1985", "08.1985", false, "Добрый вечер.");
        VisitorData s2_2 = Create("Assets/Data/Shift2/Char6_Monster.asset", "Анна Волкова", "Анна Волкова", "ID-201", "ID-201", "Голубые", "Красные", "02.1987", "02.1987", true, "Я всегда тут жила. Открой."); // Ошибка: глаза красные
        VisitorData s2_3 = Create("Assets/Data/Shift2/Char7_Human.asset", "Олег Лебедев", "Олег Лебедев", "ID-202", "ID-202", "Карие", "Карие", "03.1982", "03.1982", false, "Как же холодно на улице.");
        VisitorData s2_4 = Create("Assets/Data/Shift2/Char8_Monster.asset", "Виктория Новикова", "Виктория Новикова", "ID-203", "ID-203", "Зеленые", "Черные", "11.1989", "11.1989", true, "..."); // Ошибка: глаза черные

        ShiftData shift2 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Data/Shift2/Shift2.asset");
        if (shift2 != null) {
            shift2.shiftVisitors = new VisitorData[] { s2_1, s2_2, s2_3, s2_4 };
            EditorUtility.SetDirty(shift2);
        }

        // --- СМЕНА 3 (Ошибки в ДАТЕ) ---
        VisitorData s3_1 = Create("Assets/Data/Shift3/Char9_Human.asset", "Павел Козлов", "Павел Козлов", "ID-300", "ID-300", "Голубые", "Голубые", "12.1985", "12.1985", false, "Скоро Новый Год, вы не забыли?");
        VisitorData s3_2 = Create("Assets/Data/Shift3/Char10_Monster.asset", "Наталья Ильина", "Наталья Ильина", "ID-301", "ID-301", "Серые", "Серые", "10.1986", "01.1980", true, "Пусти меня, я замерзла."); // Ошибка: паспорт просрочен (1980 год)
        VisitorData s3_3 = Create("Assets/Data/Shift3/Char11_Human.asset", "Сергей Макаров", "Сергей Макаров", "ID-302", "ID-302", "Карие", "Карие", "07.1984", "07.1984", false, "Вот мой паспорт, всё должно быть в порядке.");
        VisitorData s3_4 = Create("Assets/Data/Shift3/Char12_Monster.asset", "Юлия Белова", "Юлия Белова", "ID-303", "ID-303", "Зеленые", "Зеленые", "09.1987", "09.1999", true, "Вам не нужно смотреть на дату. Просто нажмите зеленую кнопку."); // Ошибка: дата из будущего

        ShiftData shift3 = AssetDatabase.LoadAssetAtPath<ShiftData>("Assets/Data/Shift3/Shift3.asset");
        if (shift3 != null) {
            shift3.shiftVisitors = new VisitorData[] { s3_1, s3_2, s3_3, s3_4 };
            EditorUtility.SetDirty(shift3);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("<color=cyan>[Antigravity] 12 ИДЕАЛЬНЫХ ПЕРСОНАЖЕЙ СОЗДАНЫ И РАСПРЕДЕЛЕНЫ ПО СМЕНАМ!</color>");
    }

    static VisitorData Create(string path, string dName, string pName, string dId, string pId, string dEyes, string pEyes, string dDate, string pDate, bool isM, string speech)
    {
        VisitorData v = ScriptableObject.CreateInstance<VisitorData>();
        v.dossierName = dName; v.passportName = pName;
        v.dossierId = dId; v.passportId = pId;
        v.dossierEyes = dEyes; v.passportEyes = pEyes;
        v.dossierExpDate = dDate; v.passportExpDate = pDate;
        v.isMonster = isM;
        v.welcomeSpeech = speech;
        AssetDatabase.CreateAsset(v, path);
        return v;
    }
}
