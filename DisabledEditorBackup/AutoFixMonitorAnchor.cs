using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixMonitorAnchor
{
    static AutoFixMonitorAnchor()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        // Выполняем 1 раз
        if (EditorPrefs.GetBool("AutoFixMonitorAnchor_v1", false)) return;
        EditorPrefs.SetBool("AutoFixMonitorAnchor_v1", true);

        GameObject monitor = GameObject.Find("PhysicalMonitor");
        if (monitor != null)
        {
            RectTransform rt = monitor.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Самая частая проблема UI: кривые якоря (Anchors). 
                // Сохраняем реальную позицию, ставим якорь по центру и возвращаем позицию.
                Vector3 worldPos = rt.position;
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.position = worldPos;
            }

            // УНИЧТОЖАЕМ любые старые анимации (и Animator, и Legacy Animation), которые могут сбрасывать позицию
            if (monitor.GetComponent<Animator>() != null) Object.DestroyImmediate(monitor.GetComponent<Animator>());
            if (monitor.GetComponent<Animation>() != null) Object.DestroyImmediate(monitor.GetComponent<Animation>());

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=red>[Antigravity] МОНИТОР ПРИБИТ ГВОЗДЯМИ!</color>");
        }
    }
}
