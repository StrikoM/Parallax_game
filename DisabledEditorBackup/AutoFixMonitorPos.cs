using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixMonitorPos
{
    static AutoFixMonitorPos()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixMonitorPos_v1", false)) return;
        
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;

        EditorPrefs.SetBool("AutoFixMonitorPos_v1", true);

        // Ищем монитор
        GameObject monitor = GameObject.Find("PhysicalMonitor");
        if (monitor != null)
        {
            RectTransform rt = monitor.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Сдвигаем на 120 пикселей вправо (чуть-чуть)
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + 120f, rt.anchoredPosition.y);
                
                // На всякий случай проверяем: если на нем висит Animator, он может сбрасывать позицию!
                Animator anim = monitor.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.enabled = false; // Отключаем, чтобы не мешал
                    Debug.LogWarning("[Antigravity] Я отключил Animator на мониторе, так как из-за него монитор возвращался назад!");
                }

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                Debug.Log("<color=green>[Antigravity] Монитор успешно сдвинут вправо!</color>");
            }
        }
    }
}
