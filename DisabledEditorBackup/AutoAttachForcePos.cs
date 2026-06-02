using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoAttachForcePos
{
    static AutoAttachForcePos()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAttachForcePos_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoAttachForcePos_v1", true);

        GameObject monitor = GameObject.Find("PhysicalMonitor");
        if (monitor != null)
        {
            if (monitor.GetComponent<MonitorForcePosition>() == null)
            {
                monitor.AddComponent<MonitorForcePosition>();
            }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=red>[Antigravity] ПОЗИЦИЯ МОНИТОРА ЗАБЛОКИРОВАНА НАМЕРТВО!</color>");
        }
    }
}
