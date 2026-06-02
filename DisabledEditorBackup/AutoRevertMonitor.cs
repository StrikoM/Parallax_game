using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoRevertMonitor
{
    static AutoRevertMonitor()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoRevertMonitor_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoRevertMonitor_v1", true);

        GameObject monitor = GameObject.Find("PhysicalMonitor");
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();

        if (monitor != null && canvas != null)
        {
            // 1. ВОЗВРАЩАЕМ МОНИТОР В КОРЕНЬ КАНВАСА (отменяем предыдущее перемещение в WindowFrame)
            // Это вернет его оригинальную логику открытия и якоря.
            monitor.transform.SetParent(canvas.transform, false);

            // 2. Ставим монитор до вспышек экрана, чтобы они его перекрывали при ударе шокером
            GameObject flash = GameObject.Find("ScreenFlash");
            if (flash != null)
            {
                monitor.transform.SetSiblingIndex(flash.transform.GetSiblingIndex());
            }
            else
            {
                monitor.transform.SetAsLastSibling();
            }

            // 3. Отменяем ручное опускание на 200 пикселей, которое мы сделали в прошлом скрипте
            RectTransform rt = monitor.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + 200f);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Монитор возвращен на свое изначальное идеальное место!</color>");
        }
    }
}
