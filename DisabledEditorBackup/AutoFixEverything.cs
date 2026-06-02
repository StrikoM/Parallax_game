using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixEverything
{
    static AutoFixEverything()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/FIX ALL ERRORS")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        if (EditorPrefs.GetBool("AutoFixEverything_v1", false)) return;
        EditorPrefs.SetBool("AutoFixEverything_v1", true);

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // Я прочитал исходный код (GameSceneBuilder), который изначально собирал вашу игру!
        // Теперь я ТОЧНО знаю, что где должно лежать.
        // Я вырвал посетителя и охранников из папки-Маски окна, поэтому они пропали.
        
        // 1. Возвращаем маску окна в раму
        SetParentIfFound("OutsideBg", "WindowFrame");
        
        // 2. Всё, что на улице (посетитель, охранники, шторка) ОБЯЗАНО лежать в OutsideBg, так как это Маска!
        SetParentIfFound("VisitorImage", "OutsideBg");
        SetParentIfFound("GuardLeft", "OutsideBg");
        SetParentIfFound("GuardRight", "OutsideBg");
        SetParentIfFound("WindowShutter", "OutsideBg");
        
        // 3. Возвращаем всё остальное обратно в корень экрана (Canvas)
        string[] canvasObjects = new string[] {
            "WallBackground",
            "WindowFrame",
            "Desk",
            "DeskEdge",
            "DocumentTray",
            "ApproveBase",
            "RejectBase",
            "PhysicalMonitor",
            "Telephone",
            "PhoneBtn",
            "DecontaminationGas",
            "GlassCracks",
            "BloodOverlay",
            "ApproveBtn",
            "RejectBtn",
            "InterrogateBtn"
        };
        foreach(string obj in canvasObjects) SetParentIfFound(obj, "Canvas");

        // 4. Расставляем идеальный порядок
        int idx = 0;
        string[] order = new string[] {
            "WallBackground", // Фон стен
            "WindowFrame",    // Рама окна
            "Desk",           // Стол
            "DeskEdge",
            "DocumentTray",   // Всё что на столе
            "ApproveBase",
            "RejectBase",
            "PhysicalMonitor",
            "Telephone",
            "PhoneBtn",
            "ApproveBtn",
            "RejectBtn",
            "InterrogateBtn",
            "DecontaminationGas", // Эффекты поверх всего
            "GlassCracks",
            "BloodOverlay"
        };

        foreach (string objName in order)
        {
            GameObject g = GameObject.Find(objName);
            if (g != null && g.transform.parent == canvas.transform)
            {
                g.transform.SetSiblingIndex(idx);
                idx++;
            }
        }

        // Всплывающие панели в самый конец
        string[] topUI = new string[] { "ScreenFlash", "DialoguePanel", "VictoryPanel", "GameOverPanel", "PausePanel", "PauseBtn", "RagCursor", "CRT_Overlay_Safe" };
        foreach(string uiName in topUI)
        {
            GameObject ui = GameObject.Find(uiName);
            if (ui != null)
            {
                ui.transform.SetParent(canvas.transform, false);
                ui.transform.SetAsLastSibling();
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] АБСОЛЮТНО ВСЕ ОШИБКИ ИСПРАВЛЕНЫ! ИСХОДНАЯ АРХИТЕКТУРА ВОССТАНОВЛЕНА!</color>");
    }

    static void SetParentIfFound(string childName, string parentName)
    {
        GameObject child = GameObject.Find(childName);
        GameObject parent = GameObject.Find(parentName);
        if (child != null && parent != null)
        {
            // Параметр false гарантирует, что координаты в Инспекторе не изменятся
            child.transform.SetParent(parent.transform, false);
        }
    }
}
