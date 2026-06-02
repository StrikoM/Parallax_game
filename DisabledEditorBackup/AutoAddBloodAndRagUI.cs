using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoAddBloodAndRagUI
{
    static AutoAddBloodAndRagUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddBloodAndRagUI_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoAddBloodAndRagUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // 1. Создаем пятно крови на окне
        if (gm.bloodOverlay == null)
        {
            GameObject bloodObj = new GameObject("BloodOverlay");
            bloodObj.transform.SetParent(canvas.transform, false);
            
            // Ставим кровь ПЕРЕД UI стола, но ПОСЛЕ посетителя
            int indexToSet = 2; // Примерно после посетителя
            if (gm.glassCracksOverlay != null) indexToSet = gm.glassCracksOverlay.transform.GetSiblingIndex();
            bloodObj.transform.SetSiblingIndex(indexToSet);
            
            RectTransform rt = bloodObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.2f); // Покрываем только окно примерно
            rt.anchorMax = new Vector2(0.9f, 0.8f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            
            Image img = bloodObj.AddComponent<Image>();
            img.color = new Color(0.6f, 0f, 0f, 0.95f); // Густая красная кровь
            
            // В будущем игрок заменит картинку на спрайт реальной крови
            GameObject txtObj = new GameObject("TextPlaceholder");
            txtObj.transform.SetParent(bloodObj.transform, false);
            RectTransform trt = txtObj.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            TMPro.TextMeshProUGUI txt = txtObj.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = "*КРОВАВОЕ ПЯТНО МОНСТРА*";
            txt.alignment = TMPro.TextAlignmentOptions.Center;
            txt.fontSize = 60;
            txt.color = new Color(0.2f, 0, 0, 1f);

            CanvasGroup cg = bloodObj.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false; // Кровь не должна мешать кликам
            
            bloodObj.SetActive(false);
            gm.bloodOverlay = cg;
        }

        // 2. Создаем Тряпку
        if (gm.ragCursor == null)
        {
            GameObject ragObj = new GameObject("RagCursor");
            ragObj.transform.SetParent(canvas.transform, false);
            ragObj.transform.SetAsLastSibling(); // Тряпка поверх всего
            
            RectTransform rt = ragObj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(150, 150);
            rt.anchoredPosition = new Vector2(300, -200); // Где-то на столе
            
            Image img = ragObj.AddComponent<Image>();
            img.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Грязно-серая тряпка
            
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(ragObj.transform, false);
            RectTransform trt = txtObj.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            TMPro.TextMeshProUGUI txt = txtObj.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = "ТРЯПКА";
            txt.alignment = TMPro.TextAlignmentOptions.Center;
            txt.fontSize = 30;
            txt.color = Color.black;

            CanvasGroup cg = ragObj.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false; // Чтобы клики проходили сквозь тряпку на кровь!
            
            ragObj.SetActive(false);
            gm.ragCursor = rt;
        }

        EditorUtility.SetDirty(gm);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=red>[Antigravity] Механика Грязного Стекла и Тряпки успешно установлена!</color>");
    }
}
