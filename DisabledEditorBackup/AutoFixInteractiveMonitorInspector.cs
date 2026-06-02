using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixInteractiveMonitorInspector
{
    static AutoFixInteractiveMonitorInspector()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixInteractiveMonitorInspector_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoFixInteractiveMonitorInspector_v1", true);

        InteractiveMonitor im = Object.FindAnyObjectByType<InteractiveMonitor>();
        if (im != null)
        {
            // Перезаписываем сохраненные значения прямо в Инспекторе
            im.deskPosition = new Vector2(650f, -90f);
            
            RectTransform rt = im.GetComponent<RectTransform>();
            if (rt != null) rt.anchoredPosition = new Vector2(650f, -90f);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Значения Desk Position в Инспекторе ИСПРАВЛЕНЫ на 650, -90!</color>");
        }
    }
}
