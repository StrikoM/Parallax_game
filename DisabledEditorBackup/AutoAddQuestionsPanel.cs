using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoAddQuestionsPanel
{
    static AutoAddQuestionsPanel()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Добавить Панель Вопросов")]
    public static void ManualRun()
    {
        EditorPrefs.DeleteKey("AutoAddQuestionsPanel_v1");
        RunOnce();
    }

    public static void RunOnce()
    {
        if (Application.isPlaying) return;

        // Исключаем повторные автоматические запуски, сохраняя ручной вызов через меню
        if (EditorPrefs.GetBool("AutoAddQuestionsPanel_v1", false)) return;
        EditorPrefs.SetBool("AutoAddQuestionsPanel_v1", true);

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[Antigravity] Canvas не найден. Панель вопросов не может быть создана.");
            return;
        }

        // 1. Находим и удаляем старую панель вопросов, если она существует
        Transform oldPanel = canvas.transform.Find("QuestionsPanel");
        if (oldPanel != null)
        {
            Object.DestroyImmediate(oldPanel.gameObject);
        }

        // 2. Создаем главную панель QuestionsPanel
        GameObject panelObj = new GameObject("QuestionsPanel");
        panelObj.transform.SetParent(canvas.transform, false);

        RectTransform panelRt = panelObj.AddComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        // Слева от посетителя, выше панели одобрять/отказывать
        panelRt.anchoredPosition = new Vector2(-280, 160);
        panelRt.sizeDelta = new Vector2(280, 240);

        // Стильный полупрозрачный темный фон
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.08f, 0.1f, 0.95f);

        // Контрастная рамка в стиле антиутопии (темно-красный)
        Outline panelOutline = panelObj.AddComponent<Outline>();
        panelOutline.effectColor = new Color(0.5f, 0.1f, 0.1f, 0.8f);
        panelOutline.effectDistance = new Vector2(2f, -2f);

        // 3. Добавляем заголовок "ДОПРОС: ВОПРОСЫ"
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform, false);

        RectTransform titleRt = titleObj.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 1);
        titleRt.anchorMax = new Vector2(1, 1);
        titleRt.anchoredPosition = new Vector2(0, -25);
        titleRt.sizeDelta = new Vector2(0, 30);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "ДОПРОС: ВОПРОСЫ";
        titleText.fontSize = 18;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.8f, 0.15f, 0.15f, 1f);

        // 4. Создаем контейнер для кнопок с автоматической вертикальной разметкой
        GameObject containerObj = new GameObject("ButtonsContainer");
        containerObj.transform.SetParent(panelObj.transform, false);

        RectTransform containerRt = containerObj.AddComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0, 0);
        containerRt.anchorMax = new Vector2(1, 1);
        containerRt.anchoredPosition = new Vector2(0, -25); // Ниже заголовка
        containerRt.sizeDelta = new Vector2(-20, -60); // Отступы по бокам и сверху/снизу

        VerticalLayoutGroup layout = containerObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 12;
        layout.padding = new RectOffset(5, 5, 5, 5);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        // 5. Описание вопросов и создание ровно трех кнопок
        string[] questionTexts = new string[]
        {
            "1. Несовпадение Имени / ID",
            "2. Несоответствие Глаз",
            "3. Истек Срок Паспорта"
        };

        string[] btnNames = new string[]
        {
            "NameQuestionBtn",
            "EyesQuestionBtn",
            "DateQuestionBtn"
        };

        for (int i = 0; i < 3; i++)
        {
            GameObject btnObj = new GameObject(btnNames[i]);
            btnObj.transform.SetParent(containerObj.transform, false);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.16f, 0.16f, 0.2f, 1f); // Темный металл

            Outline btnOutline = btnObj.AddComponent<Outline>();
            btnOutline.effectColor = new Color(0.3f, 0.3f, 0.35f, 1f);
            btnOutline.effectDistance = new Vector2(1f, -1f);

            btnObj.AddComponent<Button>();

            // Текст внутри кнопки
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);

            RectTransform txtRt = txtObj.AddComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = Vector2.zero;
            txtRt.offsetMax = Vector2.zero;

            TextMeshProUGUI txtTmp = txtObj.AddComponent<TextMeshProUGUI>();
            txtTmp.text = questionTexts[i];
            txtTmp.fontSize = 14;
            txtTmp.fontStyle = FontStyles.Bold;
            txtTmp.alignment = TextAlignmentOptions.Center;
            txtTmp.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            txtTmp.textWrappingMode = TextWrappingModes.Normal;
        }

        // 6. Подвязываем созданную панель к GameManager
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.questionsPanel = panelObj;
            EditorUtility.SetDirty(gm);
            Debug.Log("[Antigravity] Панель вопросов успешно привязана к GameManager!");
        }
        else
        {
            Debug.LogWarning("[Antigravity] GameManager не найден на сцене. Не удалось привязать панель автоматически.");
        }

        // 7. Скрываем панель по умолчанию, чтобы она не висела при старте игры
        panelObj.SetActive(false);

        // Помечаем сцену грязной, чтобы изменения сохранились в Unity
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Antigravity] Панель вопросов успешно создана, стилизована и добавлена на сцену!");
    }
}
