using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixZOrder
{
    static AutoFixZOrder()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Fix UI Z-Order")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            // 1. Находим монитор и убеждаемся, что он в корне Canvas
            GameObject monitor = GameObject.Find("PhysicalMonitor");
            if (monitor != null && monitor.transform.parent != canvas.transform)
            {
                monitor.transform.SetParent(canvas.transform, false);
            }

            // 2. Берем экраны Победы, Поражения и Паузы, вытаскиваем их в корень Canvas 
            // и ставим В САМЫЙ НИЗ списка (то есть ПОВЕРХ монитора)
            string[] topPanels = new string[] {
                "ScreenFlash",
                "DialoguePanel",
                "VictoryPanel",
                "GameOverPanel",
                "PausePanel",
                "PauseBtn",
                "RagCursor",
                "CRT_Overlay_Safe"
            };

            foreach (string pName in topPanels)
            {
                GameObject p = GameObject.Find(pName);
                if (p != null)
                {
                    p.transform.SetParent(canvas.transform, false);
                    p.transform.SetAsLastSibling(); // Гарантирует, что панель нарисуется поверх всего!
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Панель смены теперь ПОВЕРХ монитора!</color>");
        }
    }
}
