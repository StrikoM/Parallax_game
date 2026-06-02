using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoUpdateMultipleResponses
{
    static AutoUpdateMultipleResponses()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoUpdateMultipleResponses_v1", false)) return;
        EditorPrefs.SetBool("AutoUpdateMultipleResponses_v1", true);

        // --- Смена 1 (Люди) ---
        UpdateChar("Assets/Data/Shift1/Char1_Human.asset", 
            "С именем всё отлично.", 
            "Глаза как глаза.", 
            "Я недавно обновил паспорт.");
        UpdateChar("Assets/Data/Shift1/Char3_Human.asset", 
            "Это моя фамилия, да.", 
            "Просто устали глаза.", 
            "До конца срока еще долго.");
            
        // --- Смена 1 (Монстры с ошибкой в имени/ID) ---
        UpdateChar("Assets/Data/Shift1/Char2_Monster.asset", 
            "Имя... Я забрала его у прошлой хозяйки.", // Прокол
            "Глаза смотрят на вас.", 
            "Дата не имеет значения.");
        UpdateChar("Assets/Data/Shift1/Char4_Monster.asset", 
            "ID-666 — это число моей плоти.", // Прокол
            "Глаза полны боли.", 
            "Срок годности мяса истекает.");

        // --- Смена 2 (Люди) ---
        UpdateChar("Assets/Data/Shift2/Char5_Human.asset", 
            "Всё сходится.", 
            "Обычный цвет, просто освещение плохое.", 
            "Паспорт новый.");
        UpdateChar("Assets/Data/Shift2/Char7_Human.asset", 
            "Можете звать меня Олег.", 
            "Я ношу цветные линзы, извините.", 
            "Всё в норме.");
            
        // --- Смена 2 (Монстры с ошибкой в глазах) ---
        UpdateChar("Assets/Data/Shift2/Char6_Monster.asset", 
            "Имя Волкова...", 
            "Мои настоящие глаза сгнили. Пришлось взять красные.", // Прокол
            "Даты... скучно.");
        UpdateChar("Assets/Data/Shift2/Char8_Monster.asset", 
            "Виктория...", 
            "В моих глазах только вечная чернота.", // Прокол
            "Я тут давно.");

        // --- Смена 3 (Люди) ---
        UpdateChar("Assets/Data/Shift3/Char9_Human.asset", 
            "Павел, очень приятно.", 
            "Голубые, как небо.", 
            "С датой всё ок, паспорт еще действует.");
        UpdateChar("Assets/Data/Shift3/Char11_Human.asset", 
            "Всё сходится.", 
            "Карие.", 
            "Всё актуально. Проверьте внимательнее.");
            
        // --- Смена 3 (Монстры с ошибкой в дате) ---
        UpdateChar("Assets/Data/Shift3/Char10_Monster.asset", 
            "Наталья, да.", 
            "Серые глаза.", 
            "1980 год был отличным годом для кровавой жатвы."); // Прокол
        UpdateChar("Assets/Data/Shift3/Char12_Monster.asset", 
            "Юлия.", 
            "Зеленые.", 
            "Время не имеет значения для таких, как я. Будущее уже наступило."); // Прокол

        AssetDatabase.SaveAssets();
        Debug.Log("<color=cyan>[Antigravity] Все персонажи получили по 3 варианта ответов!</color>");
    }

    static void UpdateChar(string path, string nameResp, string eyesResp, string dateResp)
    {
        VisitorData v = AssetDatabase.LoadAssetAtPath<VisitorData>(path);
        if (v != null)
        {
            v.responseName = nameResp;
            v.responseEyes = eyesResp;
            v.responseDate = dateResp;
            EditorUtility.SetDirty(v);
        }
    }
}
