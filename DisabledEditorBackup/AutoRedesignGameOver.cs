using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoRedesignGameOver
{
    static AutoRedesignGameOver()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoRedesignGameOver_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoRedesignGameOver_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        bool changed = false;

        // 1. Превращаем серый кубик Game Over в жуткий полноэкранный фон
        if (gm.gameOverPanel != null)
        {
            RectTransform rt = gm.gameOverPanel.GetComponent<RectTransform>();
            Image bg = gm.gameOverPanel.GetComponent<Image>();
            
            if (rt != null && bg != null)
            {
                // Растягиваем на весь экран
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                
                // Делаем зловещий темно-кровавый полупрозрачный цвет
                bg.color = new Color(0.15f, 0.0f, 0.0f, 0.95f);
                changed = true;
            }

            // Ищем заголовок и делаем его более жестким
            TextMeshProUGUI[] texts = gm.gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var txt in texts)
            {
                // Если это не текст причины (который меняется), а статичный заголовок
                if (txt != gm.gameOverReasonText)
                {
                    txt.fontSize = 120; // Огромный размер
                    txt.color = new Color(0.8f, 0f, 0f, 1f);
                    txt.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
                }
                else
                {
                    // Текст причины
                    txt.color = Color.white;
                    txt.fontSize = 50;
                }
            }
            
            // Если у кнопки серый фон, делаем его прозрачным, оставляем только текст и рамку
            Button btn = gm.gameOverPanel.GetComponentInChildren<Button>(true);
            if (btn != null)
            {
                Image btnImg = btn.GetComponent<Image>();
                if (btnImg != null) btnImg.color = new Color(0, 0, 0, 0.5f); // Черный полупрозрачный
            }
        }

        // 2. Делаем то же самое для панели Победы (но в терминальном зеленом стиле)
        if (gm.victoryPanel != null)
        {
            RectTransform rt = gm.victoryPanel.GetComponent<RectTransform>();
            Image bg = gm.victoryPanel.GetComponent<Image>();
            
            if (rt != null && bg != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                
                bg.color = new Color(0.0f, 0.1f, 0.0f, 0.95f);
                changed = true;
            }
        }

        // 3. Исправляем баг с печатью (текст переносился на новую строку)
        if (gm.stampText != null)
        {
            gm.stampText.enableWordWrapping = false;
            gm.stampText.overflowMode = TextOverflowModes.Overflow;
            
            if (gm.stampObject != null)
            {
                RectTransform stampRT = gm.stampObject.GetComponent<RectTransform>();
                if (stampRT != null)
                {
                    // Расширяем рамку печати
                    stampRT.sizeDelta = new Vector2(600, 150); 
                }
            }
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=red>[Antigravity] Панель смерти стала страшной! Печать исправлена.</color>");
        }
    }
}
