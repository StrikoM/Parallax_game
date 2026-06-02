using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoRescueMission
{
    static AutoRescueMission()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/RESCUE MISSION")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        if (EditorPrefs.GetBool("AutoRescue_v1", false)) return;
        EditorPrefs.SetBool("AutoRescue_v1", true);

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // Я понял в чем дело! Телефон, монитор и документы не исчезли — они остались на месте, 
        // а Стол и Охранники "улетели" вверх!
        // Это доказывает, что изначально ВСЕ они лежали прямо в Canvas, а не в WindowFrame.
        // Сейчас я вытащу их всех обратно в Canvas, и они МГНОВЕННО соберутся обратно как пазл!

        string[] allObjects = new string[] {
            "OutsideBg",
            "VisitorImage",
            "WindowFrame", // Стена
            "GuardLeft",
            "GuardRight",
            "WindowShutter",
            "Desk",
            "DecontaminationGas",
            "GlassCracks",
            "BloodOverlay",
            "Telephone",
            "PhoneBtn",
            "TerminalMonitor",
            "PhysicalMonitor",
            "DocumentTray",
            "ApproveBtn",
            "RejectBtn",
            "InterrogateBtn",
            "StunGunDrawer"
        };

        // 1. Возвращаем всех в Canvas
        foreach (string objName in allObjects)
        {
            GameObject g = GameObject.Find(objName);
            if (g != null)
            {
                g.transform.SetParent(canvas.transform, false);
            }
        }

        // 2. Расставляем идеальный порядок слоев (сзади наперед)
        int orderIdx = 0;
        
        // Найдем, где в Canvas начинается наша игра, чтобы не сбить другие панели
        Transform firstObj = canvas.transform.Find("OutsideBg");
        if (firstObj != null) orderIdx = firstObj.GetSiblingIndex();

        string[] correctOrder = new string[] {
            "OutsideBg",          // 1. Улица
            "VisitorImage",       // 2. Посетитель
            "WindowShutter",      // 3. Железная шторка
            "WindowFrame",        // 4. Стена с окном
            "GuardLeft",          // 5. Охранники в комнате
            "GuardRight",
            "Desk",               // 6. Стол
            "TerminalMonitor",    // 7. Объекты на столе
            "PhysicalMonitor",
            "Telephone",
            "PhoneBtn",
            "DocumentTray",
            "ApproveBtn",
            "RejectBtn",
            "InterrogateBtn",
            "StunGunDrawer",
            "DecontaminationGas", // 8. Эффекты
            "GlassCracks",
            "BloodOverlay"
        };

        foreach (string objName in correctOrder)
        {
            GameObject g = GameObject.Find(objName);
            if (g != null)
            {
                g.transform.SetSiblingIndex(orderIdx);
                orderIdx++;
            }
        }

        // Перекидываем служебные окна в самый конец, чтобы они перекрывали всё
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
        Debug.Log("<color=magenta>[Antigravity] ОПЕРАЦИЯ СПАСЕНИЯ: Пазл собран, все объекты в Canvas!</color>");
    }
}
