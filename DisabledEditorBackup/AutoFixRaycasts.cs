using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AutoFixRaycasts
{
    [MenuItem("Parallax/Починить Клики (Raycast)")]
    public static void RunOnce()
    {
        if (Application.isPlaying) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // Отключаем блокировку кликов у рамки окна и стекла
        Transform windowFrame = canvas.transform.Find("WindowFrame");
        if (windowFrame != null)
        {
            Image img = windowFrame.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;

            // Отключаем у всех декоративных элементов внутри окна, которые не являются кнопками
            foreach (Transform child in windowFrame)
            {
                if (child.GetComponent<Button>() == null)
                {
                    Image childImg = child.GetComponent<Image>();
                    if (childImg != null) childImg.raycastTarget = false;
                }
            }
        }

        // Находим посетителя и ГАРАНТИРУЕМ, что он кликабелен
        Transform visitor = canvas.transform.Find("VisitorImage");
        if (visitor == null && windowFrame != null) visitor = windowFrame.Find("VisitorImage");
        
        if (visitor != null)
        {
            Image visImg = visitor.GetComponent<Image>();
            if (visImg != null) visImg.raycastTarget = true; // Включаем прием кликов!
        }

        // Также отключаем клики на газе, крови и трещинах, чтобы они не мешали нажимать
        string[] overlays = { "DecontaminationGas", "BloodOverlay", "GlassCracksOverlay" };
        foreach (string o in overlays)
        {
            Transform t = canvas.transform.Find(o);
            if (t != null)
            {
                Image img = t.GetComponent<Image>();
                if (img != null) img.raycastTarget = false;
                
                CanvasGroup cg = t.GetComponent<CanvasGroup>();
                if (cg != null) cg.blocksRaycasts = false;
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Antigravity] Клики успешно починены! Рамка окна больше не блокирует посетителя.");
    }
}
