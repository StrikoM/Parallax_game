using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoFixPauseMenuUI
{
    static AutoFixPauseMenuUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixPauseMenuUI_v1", false)) return;
        
        // Убедимся, что открыта сцена игры
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;

        EditorPrefs.SetBool("AutoFixPauseMenuUI_v1", true);

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        Transform pausePanel = canvas.transform.Find("PausePanel");
        if (pausePanel == null) return;

        // Делаем фон панели почти черным, чтобы хорошо читался текст
        Image panelImg = pausePanel.GetComponent<Image>();
        if (panelImg != null) panelImg.color = new Color(0, 0, 0, 0.95f);

        // Настраиваем кнопку ПРОДОЛЖИТЬ
        Transform resumeBtn = pausePanel.Find("ResumeBtn");
        if (resumeBtn != null)
        {
            Image rImg = resumeBtn.GetComponent<Image>();
            if (rImg != null) rImg.color = new Color(0, 0, 0, 0f); // Убираем уродливый сплошной цвет фона
            
            TextMeshProUGUI rTxt = resumeBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (rTxt != null) 
            {
                rTxt.color = new Color(0.3f, 0.8f, 0.3f); // Делаем текст стильным зеленым
                rTxt.fontSize = 50; 
                // Можно добавить легкую обводку, если нужен шрифт потолще
                rTxt.fontStyle = FontStyles.Bold;
            }
        }

        // Настраиваем кнопку ВЫХОД
        Transform exitBtn = pausePanel.Find("ExitBtn");
        if (exitBtn != null)
        {
            Image eImg = exitBtn.GetComponent<Image>();
            if (eImg != null) eImg.color = new Color(0, 0, 0, 0f); // Прозрачный фон
            
            TextMeshProUGUI eTxt = exitBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (eTxt != null) 
            {
                eTxt.color = new Color(0.8f, 0.3f, 0.3f); // Стилистический красный цвет
                eTxt.fontSize = 50; 
                eTxt.fontStyle = FontStyles.Bold;
            }
        }

        // Добавляем атмосферный заголовок ПАУЗА на фон
        Transform pauseTitle = pausePanel.Find("PauseTitle");
        if (pauseTitle == null)
        {
            GameObject titleObj = new GameObject("PauseTitle");
            titleObj.transform.SetParent(pausePanel, false);
            titleObj.transform.SetAsFirstSibling(); // Позади кнопок
            
            TextMeshProUGUI titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "ПАУЗА";
            titleTxt.color = new Color(1f, 1f, 1f, 0.15f); // Очень прозрачный белый (как тень)
            titleTxt.fontSize = 150;
            titleTxt.alignment = TextAlignmentOptions.Center;
            titleTxt.fontStyle = FontStyles.Bold;
            
            RectTransform titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 0.5f);
            titleRt.anchorMax = new Vector2(0.5f, 0.5f);
            titleRt.anchoredPosition = new Vector2(0, 200);
            titleRt.sizeDelta = new Vector2(800, 200);
        }

        // Сохраняем сцену
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("<color=cyan>[Antigravity] Дизайн меню паузы улучшен!</color>");
    }
}
