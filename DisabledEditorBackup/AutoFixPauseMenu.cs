using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;

[InitializeOnLoad]
public class AutoFixPauseMenu
{
    static AutoFixPauseMenu()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixPauseMenuRun_v5", false)) return;
        EditorPrefs.SetBool("AutoFixPauseMenuRun_v5", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // --- ТОТАЛЬНАЯ ПРОВЕРКА ВАЖНЫХ СИСТЕМ ---
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null) canvas.gameObject.AddComponent<GraphicRaycaster>();

        EventSystem es = Object.FindAnyObjectByType<EventSystem>();
        if (es == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        // Удаляем старые сломанные объекты
        Transform oldBtn = canvas.transform.Find("PauseBtn");
        if (oldBtn != null) GameObject.DestroyImmediate(oldBtn.gameObject);

        Transform oldPanel = canvas.transform.Find("PausePanel");
        if (oldPanel != null) GameObject.DestroyImmediate(oldPanel.gameObject);

        // 1. Создаем НОВУЮ, гарантированно рабочую кнопку паузы
        GameObject pauseBtnObj = new GameObject("PauseBtn");
        pauseBtnObj.transform.SetParent(canvas.transform, false);
        // Правый верхний угол
        RectTransform btnRt = pauseBtnObj.AddComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(1, 1);
        btnRt.anchorMax = new Vector2(1, 1);
        btnRt.pivot = new Vector2(1, 1);
        btnRt.anchoredPosition = new Vector2(-30, -30); 
        btnRt.sizeDelta = new Vector2(250, 80);

        Image btnImg = pauseBtnObj.AddComponent<Image>();
        btnImg.color = new Color(0, 0, 0, 0.7f); // Темный фон кнопки
        btnImg.raycastTarget = true;

        Button pauseBtn = pauseBtnObj.AddComponent<Button>();

        GameObject btnTxtObj = new GameObject("Text");
        btnTxtObj.transform.SetParent(pauseBtnObj.transform, false);
        TextMeshProUGUI btnTxt = btnTxtObj.AddComponent<TextMeshProUGUI>();
        btnTxt.text = "ПАУЗА";
        btnTxt.color = Color.white;
        btnTxt.fontSize = 35;
        btnTxt.alignment = TextAlignmentOptions.Center;
        RectTransform txtRt2 = btnTxtObj.GetComponent<RectTransform>();
        txtRt2.anchorMin = Vector2.zero; txtRt2.anchorMax = Vector2.one;
        txtRt2.offsetMin = Vector2.zero; txtRt2.offsetMax = Vector2.zero;

        // 2. Создаем новую панель паузы
        GameObject pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvas.transform, false);
        Image panelImg = pausePanel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.95f);
        panelImg.raycastTarget = true;
        RectTransform panelRt = pausePanel.GetComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;

        // Кнопка Продолжить
        GameObject rObj = new GameObject("ResumeBtn");
        rObj.transform.SetParent(pausePanel.transform, false);
        Image rImg = rObj.AddComponent<Image>();
        rImg.color = new Color(0.2f, 0.5f, 0.2f);
        rImg.raycastTarget = true;
        Button resumeBtn = rObj.AddComponent<Button>();
        RectTransform rRt = rObj.GetComponent<RectTransform>();
        rRt.sizeDelta = new Vector2(400, 100);
        rRt.anchoredPosition = new Vector2(0, 80);

        GameObject rTxtObj = new GameObject("Text");
        rTxtObj.transform.SetParent(rObj.transform, false);
        TextMeshProUGUI rTxt = rTxtObj.AddComponent<TextMeshProUGUI>();
        rTxt.text = "ПРОДОЛЖИТЬ";
        rTxt.color = Color.white;
        rTxt.fontSize = 35;
        rTxt.alignment = TextAlignmentOptions.Center;
        RectTransform rTxtRt = rTxtObj.GetComponent<RectTransform>();
        rTxtRt.anchorMin = Vector2.zero; rTxtRt.anchorMax = Vector2.one;
        rTxtRt.offsetMin = Vector2.zero; rTxtRt.offsetMax = Vector2.zero;

        // Кнопка Выход
        GameObject eObj = new GameObject("ExitBtn");
        eObj.transform.SetParent(pausePanel.transform, false);
        Image eImg = eObj.AddComponent<Image>();
        eImg.color = new Color(0.6f, 0.15f, 0.15f);
        eImg.raycastTarget = true;
        Button exitBtn = eObj.AddComponent<Button>();
        RectTransform eRt = eObj.GetComponent<RectTransform>();
        eRt.sizeDelta = new Vector2(400, 100);
        eRt.anchoredPosition = new Vector2(0, -80);

        GameObject eTxtObj = new GameObject("Text");
        eTxtObj.transform.SetParent(eObj.transform, false);
        TextMeshProUGUI eTxt = eTxtObj.AddComponent<TextMeshProUGUI>();
        eTxt.text = "ГЛАВНОЕ МЕНЮ";
        eTxt.color = Color.white;
        eTxt.fontSize = 35;
        eTxt.alignment = TextAlignmentOptions.Center;
        RectTransform eTxtRt = eTxtObj.GetComponent<RectTransform>();
        eTxtRt.anchorMin = Vector2.zero; eTxtRt.anchorMax = Vector2.one;
        eTxtRt.offsetMin = Vector2.zero; eTxtRt.offsetMax = Vector2.zero;

        // 3. ЖЕСТКАЯ ПРИВЯЗКА СОБЫТИЙ В ИНСПЕКТОРЕ
        UnityEditor.Events.UnityEventTools.AddPersistentListener(pauseBtn.onClick, new UnityEngine.Events.UnityAction(gm.TogglePause));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(resumeBtn.onClick, new UnityEngine.Events.UnityAction(gm.TogglePause));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(exitBtn.onClick, new UnityEngine.Events.UnityAction(gm.ReturnToMainMenu));

        // Привязка ссылок
        gm.pausePanel = pausePanel;
        gm.pauseButton = pauseBtn;
        gm.resumeButton = resumeBtn;
        gm.exitButton = exitBtn;

        // Отключаем лишние Raycast (например, телевизор или другие баги)
        Transform[] allT = canvas.GetComponentsInChildren<Transform>(true);
        foreach(var t in allT) {
            if (t.name == "CRT_Overlay_Safe" || t.name.Contains("Overlay")) {
                Image img = t.GetComponent<Image>();
                if (img != null) img.raycastTarget = false;
            }
        }

        pausePanel.SetActive(false);

        EditorUtility.SetDirty(gm);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("<color=green>[Antigravity] ПОЛНЫЙ СБРОС: Кнопка паузы и меню созданы с нуля и жестко привязаны к коду!</color>");
    }
}
