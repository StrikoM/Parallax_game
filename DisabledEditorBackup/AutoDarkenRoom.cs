using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoDarkenRoom
{
    static AutoDarkenRoom()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoDarkenRoom_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoDarkenRoom_v1", true);

        // 1. Делаем общую атмосферу темнее
        GameObject bg = GameObject.Find("Background");
        if (bg != null)
        {
            Image bgImg = bg.GetComponent<Image>();
            if (bgImg != null)
            {
                bgImg.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Уменьшили яркость на 70%
            }
        }

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            Transform existing = canvas.transform.Find("GlobalDarkness");
            if (existing == null)
            {
                GameObject darkObj = new GameObject("GlobalDarkness");
                darkObj.transform.SetParent(canvas.transform, false);
                darkObj.transform.SetSiblingIndex(1); // Сразу после Background
                
                RectTransform rt = darkObj.AddComponent<RectTransform>();
                rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
                
                Image darkImg = darkObj.AddComponent<Image>();
                darkImg.color = new Color(0.0f, 0.05f, 0.1f, 0.4f); // Жуткий синевато-черный фильтр
            }
        }

        // 2. Делаем нападение монстра более агрессивным визуально (раз картинки трещин пока нет)
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null && gm.glassCracksOverlay != null)
        {
            Image cracksBg = gm.glassCracksOverlay.GetComponent<Image>();
            if (cracksBg != null) cracksBg.color = new Color(0.5f, 0f, 0f, 0.4f); // Красный пульсирующий экран

            TMPro.TextMeshProUGUI txt = gm.glassCracksOverlay.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = "✖ МОНСТР ЛОМАЕТ СТЕКЛО! ✖";
                txt.fontSize = 100;
                txt.color = new Color(1f, 0.2f, 0.2f, 1f); // Ярко-красный
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=cyan>[Antigravity] Комната затемнена, а эффект нападения усилен!</color>");
    }
}
