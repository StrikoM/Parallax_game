using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixScreenPanels
{
    static AutoFixScreenPanels()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixScreenPanels_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixScreenPanels_v1", true);

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            // 1. Возвращаем монитор внутрь WindowFrame (как стол и телефон), если он оттуда выпал
            GameObject monitor = GameObject.Find("PhysicalMonitor");
            GameObject frame = GameObject.Find("WindowFrame");
            if (monitor != null && frame != null && monitor.transform.parent == canvas.transform)
            {
                monitor.transform.SetParent(frame.transform, false);
            }

            // 2. Выносим финальные экраны из WindowFrame прямо в корень Канваса.
            // Это ГАРАНТИРУЕТ, что они перекроют ВООБЩЕ ВСЁ, где бы оно ни находилось (даже монитор).
            string[] panelsToMove = new string[] {
                "ScreenFlash",
                "VictoryPanel",
                "GameOverPanel",
                "PausePanel",
                "PauseBtn",
                "RagCursor",
                "CRT_Overlay_Safe"
            };

            foreach (string pName in panelsToMove)
            {
                GameObject p = GameObject.Find(pName);
                if (p != null)
                {
                    p.transform.SetParent(canvas.transform, false);
                    p.transform.SetAsLastSibling(); // В самый низ Канваса (поверх всего)
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Полноэкранные панели вынесены в корень Канваса! Монитор больше не перекрывает экраны.</color>");
        }
    }
}
