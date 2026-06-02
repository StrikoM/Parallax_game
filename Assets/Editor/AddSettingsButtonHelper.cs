using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class AddSettingsButtonHelper : EditorWindow
{
    [MenuItem("Parallax/Добавить кнопку Настроек в главное меню")]
    public static void AddButton()
    {
        // Проверяем, открыта ли нужная сцена
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu")
        {
            EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, откройте сцену MainMenu в Unity перед запуском этого скрипта!", "ОК");
            return;
        }

        GameObject playBtn = GameObject.Find("PlayButton");
        GameObject achBtn = GameObject.Find("AchievementsButton");
        GameObject quitBtn = GameObject.Find("QuitButton");

        if (playBtn == null || quitBtn == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти PlayButton или QuitButton в сцене! Убедитесь, что сцена MainMenu открыта.", "ОК");
            return;
        }

        // Проверяем, нет ли уже кнопки настроек
        if (GameObject.Find("SettingsButton") != null)
        {
            EditorUtility.DisplayDialog("Инфо", "Кнопка SettingsButton уже присутствует на сцене!", "ОК");
            return;
        }

        // Клонируем кнопку. Если есть кнопка достижений, клонируем её, иначе PlayButton
        GameObject sourceBtn = achBtn != null ? achBtn : playBtn;
        GameObject setBtnObj = Object.Instantiate(sourceBtn, sourceBtn.transform.parent);
        setBtnObj.name = "SettingsButton";

        // Регистрируем создание объекта для Undo (чтобы можно было нажать Ctrl+Z)
        Undo.RegisterCreatedObjectUndo(setBtnObj, "Create Settings Button");

        RectTransform quitRt = quitBtn.GetComponent<RectTransform>();
        RectTransform setRt = setBtnObj.GetComponent<RectTransform>();

        // Сдвигаем кнопку настроек на место перед кнопкой выхода
        setBtnObj.transform.SetSiblingIndex(quitBtn.transform.GetSiblingIndex());

        var layout = playBtn.transform.parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (layout == null)
        {
            Vector3 originalQuitPos = quitRt.anchoredPosition3D;
            
            // Записываем состояние для Undo
            Undo.RecordObject(quitRt, "Move Quit Button");
            
            // Сдвигаем QuitButton вниз
            quitRt.anchoredPosition3D = originalQuitPos + new Vector3(0, -110f, 0);

            // SettingsButton ставим на старое место QuitButton
            setRt.anchoredPosition3D = originalQuitPos;
        }

        // Меняем текст кнопки
        TextMeshProUGUI txt = setBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.text = "НАСТРОЙКИ";
            // Копируем шрифт из источника
            TextMeshProUGUI sourceTxt = sourceBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (sourceTxt != null) txt.font = sourceTxt.font;
        }

        // Настраиваем клик
        UnityEngine.UI.Button btn = setBtnObj.GetComponent<UnityEngine.UI.Button>();
        btn.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        
        MainMenu mm = Object.FindAnyObjectByType<MainMenu>();
        if (mm != null)
        {
            // Подвязываем вызов ShowSettingsPanel через UnityEvent в инспекторе
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, mm.ShowSettingsPanel);
        }

        // Помечаем сцену измененной, чтобы её можно было сохранить
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Успех!", "Кнопка «НАСТРОЙКИ» успешно добавлена на сцену навсегда!\n\nНажмите Ctrl + S, чтобы сохранить изменения в сцене.", "Отлично!");
    }
}
