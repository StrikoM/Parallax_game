using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoFixMainMenuBtn
{
    static AutoFixMainMenuBtn()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixMainMenuBtn_v5", false)) return;
        
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu") return;

        EditorPrefs.SetBool("AutoFixMainMenuBtn_v5", true);

        MainMenu menuScript = Object.FindAnyObjectByType<MainMenu>();
        if (menuScript == null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        Transform continueTransform = null;
        Transform newGameTransform = null;
        Transform selectShiftTransform = null;
        Transform exitTransform = null;

        TextMeshProUGUI[] texts = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach(var t in texts) {
            string txt = t.text.Trim().ToUpper();
            if (txt.Contains("ПРОДОЛЖИТЬ") || txt.Contains("CONTINUE")) continueTransform = t.transform.parent;
            if (txt == "NEW GAME") newGameTransform = t.transform.parent;
            if (txt == "SELECT SHIFT" || txt == "ВЫБОР СМЕНЫ") selectShiftTransform = t.transform.parent;
            if (txt == "EXIT") exitTransform = t.transform.parent;
        }

        if (continueTransform != null && newGameTransform != null && selectShiftTransform != null && exitTransform != null)
        {
            RectTransform[] rts = new RectTransform[] { 
                continueTransform.GetComponent<RectTransform>(),
                newGameTransform.GetComponent<RectTransform>(),
                selectShiftTransform.GetComponent<RectTransform>(),
                exitTransform.GetComponent<RectTransform>()
            };

            float startY = 120f; // Приподнимаем еще выше
            float step = 90f;    // Увеличиваем расстояние между ними

            for (int i = 0; i < rts.Length; i++)
            {
                RectTransform rt = rts[i];
                
                // Якоря в центр
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                
                // ДЕЛАЕМ КНОПКУ ШИРОКОЙ И ВЫСОКОЙ, ЧТОБЫ ТЕКСТ ВЛЕЗ
                rt.sizeDelta = new Vector2(900f, 100f);

                rt.anchoredPosition = new Vector2(0f, startY - (step * i));

                // ЖЕСТКО ЧИНИМ САМ ТЕКСТ ВНУТРИ КНОПКИ
                TextMeshProUGUI txt = rt.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    RectTransform txtRt = txt.GetComponent<RectTransform>();
                    // Растягиваем текст по размеру кнопки
                    txtRt.anchorMin = Vector2.zero;
                    txtRt.anchorMax = Vector2.one;
                    txtRt.offsetMin = Vector2.zero;
                    txtRt.offsetMax = Vector2.zero;
                    
                    // Запрещаем тексту переноситься на новую строку!
                    txt.enableWordWrapping = false;
                    txt.alignment = TextAlignmentOptions.Center;
                    
                    // Если текст все равно не влезает, пусть немного уменьшит шрифт, но не ломается
                    txt.enableAutoSizing = true;
                    txt.fontSizeMin = 20;
                    txt.fontSizeMax = 50;
                }
            }
        }

        EditorUtility.SetDirty(menuScript);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("<color=green>[Antigravity] Кнопки расширены, перенос строк отключен!</color>");
    }
}
