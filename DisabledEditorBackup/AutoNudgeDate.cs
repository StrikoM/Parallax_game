using UnityEngine;
using UnityEditor;
using TMPro;

[InitializeOnLoad]
public class AutoNudgeDate
{
    static AutoNudgeDate()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Nudge Date Down")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        if (EditorPrefs.GetBool("AutoNudgeDate_v1", false)) return;
        EditorPrefs.SetBool("AutoNudgeDate_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        if (gm.passportExpDateText != null)
        {
            RectTransform rt = gm.passportExpDateText.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 12f);
            EditorUtility.SetDirty(gm.passportExpDateText);
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Срок годности сдвинут чуть ниже!</color>");
    }
}
