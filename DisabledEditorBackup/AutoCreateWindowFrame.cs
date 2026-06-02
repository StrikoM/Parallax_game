using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoRestorePerfect
{
    static AutoRestorePerfect()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/RESTORE HIERARCHY")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject windowFrame = GameObject.Find("WindowFrame");
        if (windowFrame == null) return;

        // Удаляем сломанный прямоугольник если он есть
        GameObject frontFrame = GameObject.Find("WindowFrameFront");
        if (frontFrame != null) Object.DestroyImmediate(frontFrame);

        // 1. OutsideBg и VisitorImage должны лежать прямо в Canvas (улица)
        GameObject outsideBg = GameObject.Find("OutsideBg");
        if (outsideBg != null)
        {
            outsideBg.transform.SetParent(canvas.transform, false);
            outsideBg.transform.SetAsFirstSibling(); // Индекс 0
        }

        GameObject visitorImage = GameObject.Find("VisitorImage");
        if (visitorImage != null)
        {
            visitorImage.transform.SetParent(canvas.transform, false);
            visitorImage.transform.SetSiblingIndex(1); // Индекс 1
        }

        // 2. Рама окна (WindowFrame) идет после посетителя
        windowFrame.transform.SetParent(canvas.transform, false);
        windowFrame.transform.SetSiblingIndex(2); // Индекс 2

        // 3. Охранники и стол должны лежать ВНУТРИ WindowFrame! 
        // Именно из-за того, что они выпали из WindowFrame, они стали огромными и сместились.
        string[] childrenOrder = new string[] {
            "GuardLeft",
            "GuardRight",
            "WindowShutter",
            "Desk",
            "DecontaminationGas",
            "GlassCracks",
            "BloodOverlay"
        };

        int idx = 0;
        foreach (string childName in childrenOrder)
        {
            GameObject g = GameObject.Find(childName);
            if (g != null)
            {
                g.transform.SetParent(windowFrame.transform, false);
                g.transform.SetSiblingIndex(idx);
                idx++;
            }
        }

        // Возвращаем все всплывающие окна в конец Canvas
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
        Debug.Log("<color=green>[Antigravity] ИЕРАРХИЯ ВОССТАНОВЛЕНА ИДЕАЛЬНО!</color>");
    }
}
