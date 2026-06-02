using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoBeautifyPauseMenu
{
    static AutoBeautifyPauseMenu()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoBeautifyPauseMenu_v3", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoBeautifyPauseMenu_v3", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null || gm.pausePanel == null) return;

        // 1. УНИЧТОЖАЕМ серый кубик. Ищем ВСЕ картинки внутри меню паузы.
        Image[] allImages = gm.pausePanel.GetComponentsInChildren<Image>(true);
        foreach(var img in allImages)
        {
            // Если это не кнопка, делаем ее полностью прозрачной (стираем кубик)
            if (img.gameObject.GetComponent<Button>() == null)
            {
                img.color = new Color(0, 0, 0, 0); 
            }
        }

        // 2. Делаем ГЛАВНЫЙ фон меню паузы на весь экран
        RectTransform panelRT = gm.pausePanel.GetComponent<RectTransform>();
        Image panelBg = gm.pausePanel.GetComponent<Image>();
        if (panelBg == null) panelBg = gm.pausePanel.AddComponent<Image>();
        
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        // Закрашиваем весь экран темным мрачным цветом
        panelBg.color = new Color(0.02f, 0.02f, 0.02f, 0.95f); 

        // 3. Жестко перекрашиваем кнопки (они не менялись из-за настроек Color Tint)
        Button[] buttons = gm.pausePanel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            Image btnImg = btn.GetComponent<Image>();
            TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            
            if (btnImg != null && txt != null)
            {
                // Фикс проблемы: Unity сама красила кнопки в зеленый и красный через Color Tint!
                // Меняем стандартные цвета кнопок на темно-серые
                ColorBlock cb = btn.colors;
                cb.normalColor = new Color(0.1f, 0.1f, 0.1f, 0.9f); 
                cb.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                cb.pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                cb.selectedColor = cb.normalColor;
                btn.colors = cb;
                
                btnImg.color = Color.white; // Сбрасываем базовый цвет картинки
                
                Outline outline = btn.gameObject.GetComponent<Outline>();
                if (outline == null) outline = btn.gameObject.AddComponent<Outline>();
                
                // Всю красоту переносим на текст и светящуюся рамку
                if (txt.text.ToLower().Contains("продолжить"))
                {
                    txt.color = new Color(0.2f, 1f, 0.2f, 1f); // Яркий зеленый
                    outline.effectColor = new Color(0.2f, 1f, 0.2f, 0.6f);
                }
                else if (txt.text.ToLower().Contains("главное") || txt.text.ToLower().Contains("меню"))
                {
                    txt.color = new Color(1f, 0.2f, 0.2f, 1f); // Яркий красный
                    outline.effectColor = new Color(1f, 0.2f, 0.2f, 0.6f);
                }
                
                outline.effectDistance = new Vector2(4, -4); // Толстая рамка
                txt.fontSize = 50;
                txt.fontStyle = FontStyles.Bold;
                txt.characterSpacing = 5; 
                
                // Делаем кнопки тонкими и широкими
                RectTransform btnRT = btn.GetComponent<RectTransform>();
                if (btnRT != null)
                {
                    btnRT.sizeDelta = new Vector2(500, 80); 
                }
            }
        }

        // Поднимаем заголовок ПАУЗА чуть выше, чтобы не наезжал на кнопки
        Transform titleTransform = gm.pausePanel.transform.Find("PauseTitle");
        if (titleTransform != null)
        {
            RectTransform trt = titleTransform.GetComponent<RectTransform>();
            trt.anchoredPosition = new Vector2(0, -200); 
        }

        EditorUtility.SetDirty(gm.pausePanel);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=yellow>[Antigravity] Серый кубик окончательно удален! Кнопки стилизованы.</color>");
    }
}
