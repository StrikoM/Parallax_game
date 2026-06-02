using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixCanvasOrder
{
    static AutoFixCanvasOrder()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixCanvasOrder_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixCanvasOrder_v1", true);

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // Порядок важен! Что вызывается позже - будет поверх всего.
        MoveToBottom(canvas.transform, "BloodOverlay");
        MoveToBottom(canvas.transform, "GlassCracks");
        MoveToBottom(canvas.transform, "ScreenFlash");
        
        // Диалоги поверх трещин и крови
        MoveToBottom(canvas.transform, "DialoguePanel");
        
        // Экраны конца игры поверх вообще всего геймплея
        MoveToBottom(canvas.transform, "VictoryPanel");
        MoveToBottom(canvas.transform, "GameOverPanel");
        
        // Пауза поверх экранов победы
        MoveToBottom(canvas.transform, "PausePanel");
        MoveToBottom(canvas.transform, "PauseBtn");
        
        // Тряпка для протирания крови (чтобы она не пряталась под экраном победы, хотя во время победы ее нет)
        MoveToBottom(canvas.transform, "RagCursor");

        // CRT фильтр - самый последний слой, накладывается на абсолютно весь экран
        MoveToBottom(canvas.transform, "CRT_Overlay_Safe");

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Порядок UI слоев исправлен! Телефон и монитор больше не будут вылезать.</color>");
    }

    static void MoveToBottom(Transform canvas, string name)
    {
        Transform t = canvas.Find(name);
        if (t != null)
        {
            t.SetAsLastSibling(); // Перемещаем в самый низ списка (чтобы рендерилось ПОВЕРХ остального)
        }
    }
}
