using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoFixGameOverPanel
{
    static AutoFixGameOverPanel()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGameOverPanel_v2", false)) return;
        
        // ВАЖНО: Не запускаем скрипт в Play Mode, иначе Unity не сохранит изменения сцены!
        if (Application.isPlaying) return; 
        
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGameOverPanel_v2", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        FixPanel(gm.gameOverPanel, new Color(0.15f, 0.0f, 0.0f, 0.95f), true);
        FixPanel(gm.victoryPanel, new Color(0.0f, 0.15f, 0.0f, 0.95f), false);

        EditorUtility.SetDirty(gm);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=red>[Antigravity] Экраны Game Over перерисованы окончательно (Версия 2)!</color>");
    }

    static void FixPanel(GameObject panel, Color bgColor, bool isGameOver)
    {
        if (panel == null) return;

        // 1. Убираем ВСЕ серые фоны-кубики
        Image[] allImages = panel.GetComponentsInChildren<Image>(true);
        foreach(var img in allImages)
        {
            if (img.gameObject.GetComponent<Button>() == null)
            {
                img.color = new Color(0, 0, 0, 0); 
            }
        }

        // 2. Делаем ГЛАВНЫЙ фон на весь экран
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        Image panelBg = panel.GetComponent<Image>();
        if (panelBg == null) panelBg = panel.AddComponent<Image>();
        
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        panelBg.color = bgColor;

        // 3. Исправляем тексты
        TextMeshProUGUI[] texts = panel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var txt in texts)
        {
            // Используем новый метод TextWrappingModes.NoWrap, чтобы убрать warning'и
            txt.textWrappingMode = TextWrappingModes.NoWrap;
            txt.overflowMode = TextOverflowModes.Overflow;
            txt.alignment = TextAlignmentOptions.Center;

            if (txt.gameObject.transform.parent.GetComponent<Button>() != null)
            {
                // ЭТО ТЕКСТ КНОПКИ
                txt.fontSize = 35;
                txt.color = Color.white;
                txt.fontStyle = FontStyles.Bold;
                txt.characterSpacing = 2;
                
                RectTransform btnRT = txt.transform.parent.GetComponent<RectTransform>();
                if (btnRT != null)
                {
                    btnRT.sizeDelta = new Vector2(400, 70);
                    btnRT.anchoredPosition = new Vector2(0, -300); 
                }
                
                Button btn = txt.transform.parent.GetComponent<Button>();
                if (btn != null)
                {
                    Image btnImg = btn.GetComponent<Image>();
                    if (btnImg != null) btnImg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
                    
                    Outline outline = btn.gameObject.GetComponent<Outline>();
                    if (outline == null) outline = btn.gameObject.AddComponent<Outline>();
                    outline.effectColor = isGameOver ? new Color(0.8f, 0, 0, 0.8f) : new Color(0, 0.8f, 0, 0.8f);
                    outline.effectDistance = new Vector2(3, -3);
                }
            }
            else if (txt.text.Contains("УВОЛЕНЫ") || txt.text.Contains("ОКОНЧЕНА") || txt.text.Contains("ПРОШЛИ"))
            {
                // ЗАГОЛОВОК
                txt.fontSize = 110;
                txt.color = isGameOver ? new Color(0.9f, 0f, 0f, 1f) : new Color(0f, 0.9f, 0f, 1f);
                txt.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
                txt.characterSpacing = 10;
                
                Shadow shadow = txt.gameObject.GetComponent<Shadow>();
                if (shadow == null) shadow = txt.gameObject.AddComponent<Shadow>();
                shadow.effectColor = Color.black;
                shadow.effectDistance = new Vector2(8, -8);
                
                RectTransform rt = txt.GetComponent<RectTransform>();
                if (rt != null) rt.anchoredPosition = new Vector2(0, 250); 
            }
            else
            {
                // ТЕКСТ ПРИЧИНЫ
                txt.fontSize = 45;
                txt.color = Color.white;
                txt.textWrappingMode = TextWrappingModes.Normal; // Разрешаем перенос
                txt.fontStyle = FontStyles.Normal;
                
                RectTransform rt = txt.GetComponent<RectTransform>();
                if (rt != null) 
                {
                    rt.sizeDelta = new Vector2(1000, 300); 
                    rt.anchoredPosition = new Vector2(0, 0); 
                }
            }
        }
    }
}
