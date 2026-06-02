using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoForcePassportDateUI
{
    static AutoForcePassportDateUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoForcePassportDateUI_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoForcePassportDateUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        // Ищем серую картонку напрямую по имени
        GameObject tray = GameObject.Find("DocumentTray");
        if (tray == null) return;

        // Если дата уже есть, удаляем, чтобы пересоздать нормально
        if (gm.passportExpDateText != null)
        {
            Object.DestroyImmediate(gm.passportExpDateText.gameObject);
        }

        // Ищем текст 'Глаза', чтобы скопировать его стиль (шрифт, цвет, размер)
        TMPro.TextMeshProUGUI eyesText = gm.passportEyesText;
        if (eyesText == null)
        {
            // Пытаемся найти среди детей
            foreach (Transform child in tray.transform)
            {
                TMPro.TextMeshProUGUI t = child.GetComponent<TMPro.TextMeshProUGUI>();
                if (t != null && t.text.Contains("ГЛАЗА"))
                {
                    eyesText = t;
                    break;
                }
            }
        }

        // Создаем новый объект с текстом
        GameObject dateObj = new GameObject("PassportExpDateText");
        dateObj.transform.SetParent(tray.transform, false);
        
        TMPro.TextMeshProUGUI dateTxt = dateObj.AddComponent<TMPro.TextMeshProUGUI>();
        dateTxt.text = "ГОДЕН ДО:\n12.2084";
        dateTxt.alignment = TMPro.TextAlignmentOptions.Center;
        
        if (eyesText != null)
        {
            dateTxt.font = eyesText.font;
            dateTxt.fontSize = eyesText.fontSize;
            dateTxt.color = eyesText.color;
            
            RectTransform eyeRT = eyesText.GetComponent<RectTransform>();
            RectTransform dateRT = dateObj.GetComponent<RectTransform>();
            
            // Копируем размеры и позицию Глаз, затем смещаем ровно на 45 пикселей вниз!
            dateRT.anchorMin = eyeRT.anchorMin;
            dateRT.anchorMax = eyeRT.anchorMax;
            dateRT.pivot = eyeRT.pivot;
            dateRT.anchoredPosition = new Vector2(eyeRT.anchoredPosition.x, eyeRT.anchoredPosition.y - 45f);
            dateRT.sizeDelta = eyeRT.sizeDelta;
            
            // Чуть увеличим сам документ, чтобы текст влез
            RectTransform trayRT = tray.GetComponent<RectTransform>();
            trayRT.sizeDelta = new Vector2(trayRT.sizeDelta.x, trayRT.sizeDelta.y + 45f);
        }

        gm.passportExpDateText = dateTxt;

        EditorUtility.SetDirty(gm);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] ДАТА ЖЕСТКО ДОБАВЛЕНА НА ДОКУМЕНТ!</color>");
    }
}
