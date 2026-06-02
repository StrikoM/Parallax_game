using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixRaycastBlockers
{
    static AutoFixRaycastBlockers()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixRaycastBlockers_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixRaycastBlockers_v1", true);

        // 1. Отключаем "Raycast Target" у полноэкранных эффектов, чтобы сквозь них проходили клики мыши!
        string[] blockers = new string[] { 
            "BloodOverlay", 
            "GlassCracks", 
            "ScreenFlash", 
            "CRT_Overlay_Safe",
            "GlobalDarkness" 
        };

        foreach (string bName in blockers)
        {
            GameObject obj = GameObject.Find(bName);
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                if (img != null) img.raycastTarget = false;

                Image[] childImages = obj.GetComponentsInChildren<Image>(true);
                foreach(var child in childImages) child.raycastTarget = false;

                CanvasGroup cg = obj.GetComponent<CanvasGroup>();
                if (cg != null) cg.blocksRaycasts = false;
            }
        }

        // 2. Ставим Шокер ПОВЕРХ трещин в иерархии, чтобы он рисовался впереди!
        GameObject stunGun = GameObject.Find("StunGunDrawer");
        if (stunGun != null)
        {
            stunGun.transform.SetAsLastSibling();
            
            // Затем ставим экраны окончания игры еще ниже, чтобы они всё перекрывали
            MoveToBottom("DialoguePanel");
            MoveToBottom("VictoryPanel");
            MoveToBottom("GameOverPanel");
            MoveToBottom("PausePanel");
            MoveToBottom("PauseBtn");
            MoveToBottom("RagCursor");
            MoveToBottom("CRT_Overlay_Safe");
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Проблема с непрожатием шокера решена! Трещины больше не блокируют мышь.</color>");
    }

    static void MoveToBottom(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null) obj.transform.SetAsLastSibling();
    }
}
