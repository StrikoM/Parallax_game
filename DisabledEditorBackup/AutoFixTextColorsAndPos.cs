using UnityEngine;
using UnityEditor;
using TMPro;

[InitializeOnLoad]
public class AutoFixTextColorsAndPos
{
    static AutoFixTextColorsAndPos()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Fix Text Colors And Positions")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        if (EditorPrefs.GetBool("AutoFixTextColors_v3", false)) return;
        EditorPrefs.SetBool("AutoFixTextColors_v3", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        TextMeshProUGUI[] texts = new TextMeshProUGUI[] {
            gm.passportNameText,
            gm.passportLastNameText,
            gm.passportEyesText,
            gm.passportExpDateText,
            gm.passportIdText
        };

        foreach (var t in texts)
        {
            if (t != null)
            {
                // Делаем цвет темно-серым/черным (цвет ручки/печатных чернил), чтобы было хорошо видно на бумаге!
                t.color = new Color(0.15f, 0.15f, 0.15f, 1f);

                // Опускаем Имена чуть ниже, чтобы они легли точно на линию, а не висели в воздухе
                if (t == gm.passportNameText || t == gm.passportLastNameText)
                {
                    RectTransform rt = t.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 12f);
                }

                EditorUtility.SetDirty(t);
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Цвета текстов паспорта изменены на черные, имена сдвинуты вниз на линии!</color>");
    }
}
