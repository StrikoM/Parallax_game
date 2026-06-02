using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixMonitorPosFinal
{
    static AutoFixMonitorPosFinal()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixMonitorPosFinal_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoFixMonitorPosFinal_v1", true);

        GameObject monitor = GameObject.Find("PhysicalMonitor");
        if (monitor != null)
        {
            RectTransform rt = monitor.GetComponent<RectTransform>();
            if (rt != null)
            {
                // 1. Фиксируем якорь строго по центру
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                
                // 2. ЖЕСТКО задаем координаты, которые вы попросили!
                rt.anchoredPosition = new Vector2(650f, -90f);
            }

            // Убиваем любую анимацию
            if (monitor.GetComponent<Animator>() != null) Object.DestroyImmediate(monitor.GetComponent<Animator>());
            if (monitor.GetComponent<Animation>() != null) Object.DestroyImmediate(monitor.GetComponent<Animation>());

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] МОНИТОР ЖЕСТКО УСТАНОВЛЕН НА 650 и -90!</color>");
        }
    }
}
