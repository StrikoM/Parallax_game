using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoFixMonitorLock
{
    static AutoFixMonitorLock()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixMonitorLock_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoFixMonitorLock_v1", true);

        GameObject monitor = GameObject.Find("PhysicalMonitor");
        if (monitor != null)
        {
            // 1. Игнорируем авто-выравнивание (если оно есть)
            LayoutElement le = monitor.GetComponent<LayoutElement>();
            if (le == null) le = monitor.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
            
            // 2. Отсоединяем монитор от стола (Desk). 
            // Если на столе висит аниматор, он сбрасывает все дочерние объекты.
            // Теперь монитор независим!
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas != null && monitor.transform.parent != null && monitor.transform.parent.name == "Desk")
            {
                Transform desk = canvas.transform.Find("Desk");
                monitor.transform.SetParent(canvas.transform, true); // true сохраняет визуальную позицию
                
                // Ставим его в списке иерархии сразу после Desk, чтобы он правильно отображался (позади меню)
                if (desk != null)
                {
                    monitor.transform.SetSiblingIndex(desk.GetSiblingIndex() + 1);
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Монитор освобожден от стола и авто-выравнивания!</color>");
        }
    }
}
