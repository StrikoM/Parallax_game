using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixStampLayer
{
    static AutoFixStampLayer()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixStampLayer_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixStampLayer_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null && gm.stampObject != null)
        {
            // Помещаем печать в самый низ списка детей документа (чтобы она была ПОВЕРХ всего, включая фото)
            gm.stampObject.transform.SetAsLastSibling();

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Слой печати 'ОДОБРЕНО' исправлен! Теперь она ПОВЕРХ фото!</color>");
        }
    }
}
