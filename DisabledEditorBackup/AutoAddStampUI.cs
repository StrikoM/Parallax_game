using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoAddStampUI
{
    static AutoAddStampUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddStampUI_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoAddStampUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        Transform tray = gm.documentTray != null ? gm.documentTray : GameObject.Find("DocumentTray")?.transform;

        if (tray != null)
        {
            Transform existingStamp = tray.Find("PassportStamp");
            if (existingStamp == null)
            {
                // Создаем объект печати
                GameObject stampObj = new GameObject("PassportStamp");
                stampObj.transform.SetParent(tray, false);
                
                RectTransform rt = stampObj.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0, 50); // Чуть выше центра паспорта
                rt.sizeDelta = new Vector2(400, 100);
                
                // Добавляем полупрозрачный фон (как у чернил)
                Image img = stampObj.AddComponent<Image>();
                img.color = new Color(0, 0, 0, 0);
                
                // Обводка печати
                Outline outline = stampObj.AddComponent<Outline>();
                outline.effectColor = new Color(0.1f, 0.8f, 0.1f, 0.8f);
                outline.effectDistance = new Vector2(5, -5);
                
                // Текст печати
                GameObject textObj = new GameObject("StampText");
                textObj.transform.SetParent(stampObj.transform, false);
                RectTransform trt = textObj.AddComponent<RectTransform>();
                trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
                trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
                
                TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
                txt.text = "ОДОБРЕНО";
                txt.fontSize = 70;
                txt.fontStyle = FontStyles.Bold;
                txt.alignment = TextAlignmentOptions.Center;
                txt.color = new Color(0.1f, 0.8f, 0.1f, 0.8f);

                // Наклоняем печать для реализма (криво поставлена)
                stampObj.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(5f, 15f));
                stampObj.SetActive(false); // Скрыта по умолчанию
                
                // Привязываем к GameManager
                gm.stampObject = stampObj;
                gm.stampText = txt;
                gm.stampOutline = outline;
                
                EditorUtility.SetDirty(gm);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                Debug.Log("<color=green>[Antigravity] Система штампов добавлена в UI!</color>");
            }
        }
    }
}
