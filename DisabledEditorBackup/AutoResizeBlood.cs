using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoResizeBlood
{
    static AutoResizeBlood()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoResizeBlood_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoResizeBlood_v1", true);

        GameObject bloodObj = GameObject.Find("BloodOverlay");
        GameObject visitorObj = GameObject.Find("VisitorImage"); // Посетитель стоит ровно в окне

        if (bloodObj != null && visitorObj != null)
        {
            RectTransform bloodRT = bloodObj.GetComponent<RectTransform>();
            RectTransform visitorRT = visitorObj.GetComponent<RectTransform>();

            // Привязываем пятно крови в точности к координатам и размерам окна посетителя
            bloodRT.SetParent(visitorRT.parent, true); // Переносим в ту же группу для точности
            bloodRT.SetSiblingIndex(visitorRT.GetSiblingIndex() + 1); // Ставим прямо поверх посетителя
            
            bloodRT.anchorMin = visitorRT.anchorMin;
            bloodRT.anchorMax = visitorRT.anchorMax;
            bloodRT.pivot = visitorRT.pivot;
            bloodRT.anchoredPosition = visitorRT.anchoredPosition;
            bloodRT.sizeDelta = visitorRT.sizeDelta;
            bloodRT.localScale = visitorRT.localScale;
            
            // Уменьшим шрифт, чтобы влез
            TMPro.TextMeshProUGUI txt = bloodObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (txt != null) txt.fontSize = 40;

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Кровь уменьшена ровно под размер окна!</color>");
        }
    }
}
