using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoAddDefenseUI
{
    static AutoAddDefenseUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddDefenseUI_v1", false)) return;
        
        // Только если открыта GameScene и игра не запущена
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        if (Application.isPlaying) return;
        
        EditorPrefs.SetBool("AutoAddDefenseUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // 1. Трещины на стекле (Cracks Overlay)
        if (gm.glassCracksOverlay == null)
        {
            GameObject cracksObj = new GameObject("GlassCracks");
            cracksObj.transform.SetParent(canvas.transform, false);
            cracksObj.transform.SetSiblingIndex(canvas.transform.childCount - 2); 
            
            RectTransform rt = cracksObj.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            
            Image img = cracksObj.AddComponent<Image>();
            img.color = new Color(1, 1, 1, 0.4f); 
            
            GameObject txtObj = new GameObject("TextPlaceholder");
            txtObj.transform.SetParent(cracksObj.transform, false);
            RectTransform trt = txtObj.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = "ТРЕЩИНЫ И СТУК!";
            txt.alignment = TextAlignmentOptions.Center;
            txt.fontSize = 80;
            txt.color = new Color(1, 0, 0, 0.5f);
            
            cracksObj.SetActive(false);
            gm.glassCracksOverlay = cracksObj;
        }

        // 2. Вспышка от шокера (Flash Overlay)
        if (gm.screenFlashOverlay == null)
        {
            GameObject flashObj = new GameObject("ScreenFlash");
            flashObj.transform.SetParent(canvas.transform, false);
            flashObj.transform.SetAsLastSibling(); 
            
            RectTransform rt = flashObj.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            
            Image img = flashObj.AddComponent<Image>();
            img.color = new Color(0,0,0,0);
            
            flashObj.SetActive(false);
            gm.screenFlashOverlay = img;
        }

        // 3. Ящик с Шокером (Stun Gun Drawer)
        if (gm.stunGunDrawer == null)
        {
            GameObject drawerObj = new GameObject("StunGunDrawer");
            drawerObj.transform.SetParent(canvas.transform, false);
            drawerObj.transform.SetAsLastSibling();
            
            RectTransform rt = drawerObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0); 
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(600, 250);
            rt.anchoredPosition = new Vector2(0, 0);
            
            Image img = drawerObj.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.98f);
            
            Outline outl = drawerObj.AddComponent<Outline>();
            outl.effectColor = new Color(0.8f, 0f, 0f, 0.8f);
            outl.effectDistance = new Vector2(5, 5);

            // Кнопка Шокера
            GameObject btnObj = new GameObject("StunGunButton");
            btnObj.transform.SetParent(drawerObj.transform, false);
            
            RectTransform brt = btnObj.AddComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.5f);
            brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.anchoredPosition = Vector2.zero;
            brt.sizeDelta = new Vector2(300, 150);
            
            Image bImg = btnObj.AddComponent<Image>();
            bImg.color = new Color(0.8f, 0.8f, 0f, 1f); 
            
            Button btn = btnObj.AddComponent<Button>();
            gm.stunGunButton = btn;
            
            GameObject bTxtObj = new GameObject("Text");
            bTxtObj.transform.SetParent(btnObj.transform, false);
            RectTransform bTrt = bTxtObj.AddComponent<RectTransform>();
            bTrt.anchorMin = Vector2.zero; bTrt.anchorMax = Vector2.one;
            bTrt.offsetMin = Vector2.zero; bTrt.offsetMax = Vector2.zero;
            
            TextMeshProUGUI bTxt = bTxtObj.AddComponent<TextMeshProUGUI>();
            bTxt.text = "УДАРИТЬ ШОКЕРОМ!";
            bTxt.alignment = TextAlignmentOptions.Center;
            bTxt.fontSize = 28;
            bTxt.color = Color.black;
            bTxt.fontStyle = FontStyles.Bold;
            
            drawerObj.SetActive(false);
            gm.stunGunDrawer = drawerObj;
        }

        EditorUtility.SetDirty(gm);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=red>[Antigravity] Система Защиты (Шокер) успешно установлена!</color>");
    }
}
