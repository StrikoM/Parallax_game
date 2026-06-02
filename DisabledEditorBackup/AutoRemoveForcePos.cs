using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoRemoveForcePos
{
    static AutoRemoveForcePos()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoRemoveForcePos_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoRemoveForcePos_v1", true);

        GameObject monitor = GameObject.Find("PhysicalMonitor");
        if (monitor != null)
        {
            // Снимаем жесткую блокировку, чтобы монитор мог увеличиваться на весь экран!
            MonitorForcePosition forceScript = monitor.GetComponent<MonitorForcePosition>();
            if (forceScript != null)
            {
                Object.DestroyImmediate(forceScript);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] МОНИТОР СНОВА МОЖЕТ ОТКРЫВАТЬСЯ НА ВЕСЬ ЭКРАН!</color>");
        }
    }
}
