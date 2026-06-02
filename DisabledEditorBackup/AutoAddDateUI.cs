using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoAddDateUI
{
    static AutoAddDateUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddDateUI_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoAddDateUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        // 1. Создаем текст Срока годности на паспорте
        if (gm.passportNameText != null && gm.passportExpDateText == null)
        {
            Transform documentTray = gm.passportNameText.transform.parent; // DocumentTray
            
            GameObject expObj = new GameObject("PassportExpDateText");
            expObj.transform.SetParent(documentTray, false);
            
            RectTransform rt = expObj.AddComponent<RectTransform>();
            // Размещаем справа снизу
            rt.anchorMin = new Vector2(0.5f, 0.1f);
            rt.anchorMax = new Vector2(0.9f, 0.4f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            
            TMPro.TextMeshProUGUI txt = expObj.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = "ГОДЕН ДО:\n12.2084";
            txt.alignment = TMPro.TextAlignmentOptions.Center;
            txt.fontSize = gm.passportNameText.fontSize;
            txt.color = gm.passportNameText.color;
            txt.font = gm.passportNameText.font;

            gm.passportExpDateText = txt;
        }

        // 2. Создаем Календарь на стене, чтобы игрок знал текущую дату!
        GameObject calendarObj = GameObject.Find("CalendarPanel");
        if (calendarObj == null && gm.passportNameText != null)
        {
            Canvas canvas = gm.passportNameText.canvas;
            
            calendarObj = new GameObject("CalendarPanel");
            calendarObj.transform.SetParent(canvas.transform, false);
            calendarObj.transform.SetAsFirstSibling(); // На задний план (стена)
            // Ставим его повыше остальных
            calendarObj.transform.SetSiblingIndex(1); // Сразу после WallBackground
            
            RectTransform rt = calendarObj.AddComponent<RectTransform>();
            // Размещаем где-то слева вверху на стене
            rt.anchorMin = new Vector2(0.1f, 0.7f);
            rt.anchorMax = new Vector2(0.25f, 0.85f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            
            Image img = calendarObj.AddComponent<Image>();
            img.color = new Color(0.9f, 0.9f, 0.85f, 1f); // Желтоватая бумага
            
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(calendarObj.transform, false);
            RectTransform trt = titleObj.AddComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0.7f); trt.anchorMax = new Vector2(1, 1);
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            
            Image titleImg = titleObj.AddComponent<Image>();
            titleImg.color = new Color(0.7f, 0.2f, 0.2f, 1f); // Красная шапка календаря
            
            GameObject dateObj = new GameObject("DateText");
            dateObj.transform.SetParent(calendarObj.transform, false);
            RectTransform drt = dateObj.AddComponent<RectTransform>();
            drt.anchorMin = new Vector2(0, 0); drt.anchorMax = new Vector2(1, 0.7f);
            drt.offsetMin = Vector2.zero; drt.offsetMax = Vector2.zero;
            
            TMPro.TextMeshProUGUI dtxt = dateObj.AddComponent<TMPro.TextMeshProUGUI>();
            dtxt.text = "СЕГОДНЯ:\n<size=150%>11.2084</size>";
            dtxt.alignment = TMPro.TextAlignmentOptions.Center;
            dtxt.color = Color.black;
            dtxt.fontSize = 24;
            dtxt.fontStyle = TMPro.FontStyles.Bold;
        }

        EditorUtility.SetDirty(gm);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Поле 'Годен до' добавлено в паспорт, а на стену повешен Календарь!</color>");
    }
}
