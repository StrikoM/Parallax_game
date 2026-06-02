using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class AddAchievementsButtonHelper : EditorWindow
{
    [MenuItem("Parallax/Добавить кнопку Достижений в сцену")]
    public static void AddButton()
    {
        // Проверяем, открыта ли нужная сцена
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu")
        {
            EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, откройте сцену MainMenu в Unity перед запуском этого скрипта!", "ОК");
            return;
        }

        GameObject playBtn = GameObject.Find("PlayButton");
        GameObject quitBtn = GameObject.Find("QuitButton");

        if (playBtn == null || quitBtn == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти PlayButton или QuitButton в сцене! Убедитесь, что сцена MainMenu открыта.", "ОК");
            return;
        }

        // Проверяем, нет ли уже кнопки достижений
        if (GameObject.Find("AchievementsButton") != null)
        {
            EditorUtility.DisplayDialog("Инфо", "Кнопка AchievementsButton уже присутствует на сцене!", "ОК");
            return;
        }

        // Клонируем кнопку
        GameObject achBtnObj = Object.Instantiate(playBtn, playBtn.transform.parent);
        achBtnObj.name = "AchievementsButton";

        // Регистрируем создание объекта для Undo (чтобы можно было нажать Ctrl+Z)
        Undo.RegisterCreatedObjectUndo(achBtnObj, "Create Achievements Button");

        RectTransform quitRt = quitBtn.GetComponent<RectTransform>();
        RectTransform achRt = achBtnObj.GetComponent<RectTransform>();

        // Сдвигаем кнопку достижений на место
        achBtnObj.transform.SetSiblingIndex(quitBtn.transform.GetSiblingIndex());

        var layout = playBtn.transform.parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (layout == null)
        {
            Vector3 originalQuitPos = quitRt.anchoredPosition3D;
            
            // Записываем состояние для Undo
            Undo.RecordObject(quitRt, "Move Quit Button");
            
            // Сдвигаем QuitButton вниз
            quitRt.anchoredPosition3D = originalQuitPos + new Vector3(0, -110f, 0);

            // AchievementsButton ставим на место QuitButton
            achRt.anchoredPosition3D = originalQuitPos;
        }

        // Меняем текст кнопки
        TextMeshProUGUI txt = achBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.text = "ДОСТИЖЕНИЯ";
            // Копируем шрифт из PlayButton
            TextMeshProUGUI playTxt = playBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (playTxt != null) txt.font = playTxt.font;
        }

        // Настраиваем клик
        UnityEngine.UI.Button btn = achBtnObj.GetComponent<UnityEngine.UI.Button>();
        btn.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        
        MainMenu mm = Object.FindAnyObjectByType<MainMenu>();
        if (mm != null)
        {
            // Подвязываем вызов ShowAchievementsPanel через UnityEvent в инспекторе
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, mm.ShowAchievementsPanel);
        }

        // Помечаем сцену измененной, чтобы её можно было сохранить
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Успех!", "Кнопка «ДОСТИЖЕНИЯ» успешно добавлена на сцену навсегда!\n\nНажмите Ctrl + S, чтобы сохранить изменения в сцене.", "Отлично!");
    }

    // ==========================================
    // ТЕСТИРОВАНИЕ: ЧИТ-МЕНЮ ДЛЯ ДОСТИЖЕНИЙ
    // ==========================================
    private static string[] achievementIds = new string[]
    {
        "FIRST_SHIFT", "PERFECT_SHIFT", "WELCOME_HOME", "NERVES_OF_STEEL",
        "CLEAN_WINDOW", "PANIC", "CYBORG", "SHERLOCK",
        "TELEPHONIST", "PARANOID", "IRON_SHIELD", "CLOSE_CALL"
    };

    [MenuItem("Parallax/Открыть ВСЕ достижения")]
    public static void UnlockAllAchievements()
    {
        foreach (string id in achievementIds)
        {
            PlayerPrefs.SetInt("Ach_" + id, 1);
        }
        PlayerPrefs.Save();
        EditorUtility.DisplayDialog("Чит-Меню", "Все 12 достижений успешно РАЗБЛОКИРОВАНЫ!\n\nЗапустите игру и откройте список, чтобы оценить их вид!", "Круто");
    }

    [MenuItem("Parallax/Сбросить ВСЕ достижения")]
    public static void ResetAllAchievements()
    {
        foreach (string id in achievementIds)
        {
            PlayerPrefs.DeleteKey("Ach_" + id);
        }
        PlayerPrefs.Save();
        EditorUtility.DisplayDialog("Чит-Меню", "Прогресс всех 12 достижений успешно СБРОШЕН!", "ОК");
    }
}
