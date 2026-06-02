using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoResizeBloodOverlay
{
    static AutoResizeBloodOverlay()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Resize Blood and Cracks")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;

        GameObject outside = GameObject.Find("OutsideBg");
        if (outside == null) return;

        RectTransform outsideRT = outside.GetComponent<RectTransform>();

        string[] overlays = new string[] { "BloodOverlay", "GlassCracks" };

        foreach (string oName in overlays)
        {
            GameObject overlay = GameObject.Find(oName);
            if (overlay != null)
            {
                RectTransform rt = overlay.GetComponent<RectTransform>();
                if (rt != null)
                {
                    // Делаем якоря, размер и позицию идентичными фону окна
                    rt.anchorMin = outsideRT.anchorMin;
                    rt.anchorMax = outsideRT.anchorMax;
                    rt.pivot = outsideRT.pivot;
                    rt.sizeDelta = outsideRT.sizeDelta;
                    rt.anchoredPosition = outsideRT.anchoredPosition;

                    EditorUtility.SetDirty(overlay);
                }
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Кровь и Трещины подогнаны под размер окна!</color>");
    }
}
