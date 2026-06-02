using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoAddInterrogationQuestionsUI
{
    static AutoAddInterrogationQuestionsUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddInterrogationQuestionsUI_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoAddInterrogationQuestionsUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null || gm.interrogateBtn == null) return;

        Transform parent = gm.interrogateBtn.transform.parent;
        if (parent == null) return;

        Transform existing = parent.Find("QuestionsPanel");
        if (existing == null)
        {
            GameObject qPanel = new GameObject("QuestionsPanel");
            qPanel.transform.SetParent(parent, false);
            RectTransform qrt = qPanel.AddComponent<RectTransform>();
            qrt.anchorMin = new Vector2(0.5f, 0);
            qrt.anchorMax = new Vector2(0.5f, 0);
            qrt.anchoredPosition = new Vector2(0, 250); // Выше кнопки допроса
            qrt.sizeDelta = new Vector2(400, 220);

            Image img = qPanel.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.95f); // Темный фон
            
            Outline outl = qPanel.AddComponent<Outline>();
            outl.effectColor = new Color(0.3f, 0.5f, 0.8f, 1f);
            outl.effectDistance = new Vector2(2, -2);
            
            VerticalLayoutGroup vlg = qPanel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(10, 10, 10, 10);
            vlg.spacing = 10;
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            // Создаем 3 кнопки
            CreateQBtn(qPanel, "1. Спросить про Имя / ID", 0);
            CreateQBtn(qPanel, "2. Спросить про Цвет Глаз", 1);
            CreateQBtn(qPanel, "3. Спросить про Срок Действия", 2);

            qPanel.SetActive(false);
            gm.questionsPanel = qPanel;

            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Панель выбора вопросов успешно добавлена!</color>");
        }
    }

    static void CreateQBtn(GameObject parent, string text, int id)
    {
        GameObject btnObj = new GameObject("QBtn_" + id);
        btnObj.transform.SetParent(parent.transform, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 1f); // Темная кнопка
        Button b = btnObj.AddComponent<Button>(); // Слушатели добавятся в GameManager.Awake()

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        RectTransform trt = txtObj.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 24;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
    }
}
