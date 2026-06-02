using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class StylizeEndScreens : EditorWindow
{
    private static void SortAndCopyClipboards()
    {
        string spriteDir = "Assets/Sprites/";
        string img1Path = spriteDir + "media__1780306677196.jpg";
        string img2Path = spriteDir + "media__1780306679383.jpg";

        if (!System.IO.File.Exists(img1Path) || !System.IO.File.Exists(img2Path))
        {
            Debug.LogWarning("[StylizeEndScreens] Файлы clipboards от пользователя не найдены для автоматического переименования.");
            return;
        }

        // Настраиваем текстуры как readable временно для анализа
        ConfigureTextureForAnalysis(img1Path);
        ConfigureTextureForAnalysis(img2Path);

        Texture2D tex1 = AssetDatabase.LoadAssetAtPath<Texture2D>(img1Path);
        Texture2D tex2 = AssetDatabase.LoadAssetAtPath<Texture2D>(img2Path);

        if (tex1 != null && tex2 != null)
        {
            float green1 = 0, red1 = 0;
            float green2 = 0, red2 = 0;

            try
            {
                Color[] colors1 = tex1.GetPixels(tex1.width / 4, tex1.height / 4, tex1.width / 2, tex1.height / 2);
                foreach (var col in colors1)
                {
                    green1 += col.g;
                    red1 += col.r;
                }

                Color[] colors2 = tex2.GetPixels(tex2.width / 4, tex2.height / 4, tex2.width / 2, tex2.height / 2);
                foreach (var col in colors2)
                {
                    green2 += col.g;
                    red2 += col.r;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("[StylizeEndScreens] Ошибка чтения пикселей для сортировки: " + e.Message);
                // Резервный вариант по размеру файла
                long len1 = new System.IO.FileInfo(img1Path).Length;
                long len2 = new System.IO.FileInfo(img2Path).Length;
                if (len1 > len2)
                {
                    green1 = 100; red1 = 0;
                }
                else
                {
                    green2 = 100; red2 = 0;
                }
            }

            string approvedSrc = "";
            string firedSrc = "";

            if (green1 > red1)
            {
                approvedSrc = img1Path;
                firedSrc = img2Path;
            }
            else
            {
                approvedSrc = img2Path;
                firedSrc = img1Path;
            }

            string approvedDest = spriteDir + "clipboard_approved.jpg";
            string firedDest = spriteDir + "clipboard_fired.jpg";

            System.IO.File.Copy(approvedSrc, approvedDest, true);
            System.IO.File.Copy(firedSrc, firedDest, true);

            AssetDatabase.ImportAsset(approvedDest, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(firedDest, ImportAssetOptions.ForceUpdate);

            // Настраиваем их как Спрайты для интерфейса
            ConfigureTextureAsSprite(approvedDest);
            ConfigureTextureAsSprite(firedDest);

            Debug.Log("[StylizeEndScreens] Успешно скопированы и настроены clipboard_approved.jpg и clipboard_fired.jpg!");
        }
    }

    private static void ConfigureTextureForAnalysis(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            if (!importer.isReadable || importer.textureType != TextureImporterType.Default)
            {
                importer.isReadable = true;
                importer.textureType = TextureImporterType.Default;
                importer.SaveAndReimport();
            }
        }
    }

    private static bool IsValidTransform(Transform t)
    {
        if (t == null) return false;
        try
        {
            var testPos = t.localPosition;
            var testActive = t.gameObject.activeSelf;
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    private static Transform EnsureClipboardHierarchy(Transform panelTrans, bool isVictory)
    {
        // 1. Убеждаемся в наличии Background (затемняющий фон комнаты)
        Transform bgTrans = panelTrans.Find("Background");
        if (!IsValidTransform(bgTrans))
        {
            if (bgTrans != null)
            {
                try { Object.DestroyImmediate(bgTrans.gameObject); } catch {}
            }

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(panelTrans, false);
            bgTrans = bgObj.transform;
            
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.04f, 0.04f, 0.97f); // Темный атмосферный полупрозрачный фон
            
            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            bgRt.anchoredPosition = Vector2.zero;
        }
        else
        {
            Image bgImg = bgTrans.GetComponent<Image>();
            if (bgImg != null) bgImg.color = new Color(0.04f, 0.04f, 0.04f, 0.97f);
        }
        bgTrans.gameObject.SetActive(true);

        // 2. Убеждаемся в наличии Clipboard (планшет)
        Transform clipTrans = panelTrans.Find("Clipboard");
        if (!IsValidTransform(clipTrans))
        {
            if (clipTrans != null)
            {
                try { Object.DestroyImmediate(clipTrans.gameObject); } catch {}
            }

            GameObject clipObj = new GameObject("Clipboard");
            clipObj.transform.SetParent(panelTrans, false);
            clipTrans = clipObj.transform;
            
            Image clipImg = clipObj.AddComponent<Image>();
            clipImg.color = Color.white;
            clipImg.preserveAspect = true;
            
            RectTransform clipRt = clipObj.GetComponent<RectTransform>();
            clipRt.anchorMin = new Vector2(0.5f, 0.5f);
            clipRt.anchorMax = new Vector2(0.5f, 0.5f);
            clipRt.pivot = new Vector2(0.5f, 0.5f);
            clipRt.sizeDelta = new Vector2(560f, 840f); // Идеальный размер планшета
            clipRt.anchoredPosition = Vector2.zero;
        }
        clipTrans.gameObject.SetActive(true);

        // Назначаем спрайт планшета
        Image cImg = clipTrans.GetComponent<Image>();
        if (cImg == null) cImg = clipTrans.gameObject.AddComponent<Image>();
        if (cImg != null)
        {
            if (cImg.sprite == null)
            {
                string spritePath = isVictory ? "Assets/Sprites/clipboard_approved.jpg" : "Assets/Sprites/clipboard_fired.jpg";
                Sprite clipSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (clipSprite != null)
                {
                    cImg.sprite = clipSprite;
                    cImg.color = Color.white;
                    cImg.preserveAspect = true;
                }
            }
        }

        // 3. Переносим или создаем элементы внутри Clipboard
        // а) TitleText
        string titleName = isVictory ? "VictoryTitle" : "GameOverTitle";
        Transform titleTrans = clipTrans.Find("TitleText");
        if (!IsValidTransform(titleTrans)) titleTrans = clipTrans.Find(titleName);
        if (!IsValidTransform(titleTrans))
        {
            titleTrans = panelTrans.Find(titleName);
            if (!IsValidTransform(titleTrans)) titleTrans = panelTrans.Find("TitleText");
            if (!IsValidTransform(titleTrans))
            {
                foreach (Transform child in panelTrans)
                {
                    if (child != null && child.name.ToLower().Contains("title")) { titleTrans = child; break; }
                }
            }
        }
        if (!IsValidTransform(titleTrans))
        {
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(clipTrans, false);
            titleTrans = titleObj.transform;
            
            TextMeshProUGUI tmp = titleObj.AddComponent<TextMeshProUGUI>();
            tmp.text = isVictory ? "СМЕНА ОКОНЧЕНА" : "ВЫ УВОЛЕНЫ";
            tmp.color = new Color(0.12f, 0.12f, 0.12f);
            tmp.fontSize = 28f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            titleTrans.SetParent(clipTrans, false);
            titleTrans.name = "TitleText";
        }

        // б) StatsText
        string statsName = isVictory ? "VictoryStats" : "GameOverReason";
        Transform statsTrans = clipTrans.Find("StatsText");
        if (!IsValidTransform(statsTrans)) statsTrans = clipTrans.Find(statsName);
        if (!IsValidTransform(statsTrans))
        {
            statsTrans = panelTrans.Find(statsName);
            if (!IsValidTransform(statsTrans)) statsTrans = panelTrans.Find("StatsText");
            if (!IsValidTransform(statsTrans))
            {
                foreach (Transform child in panelTrans)
                {
                    if (child != null && (child.name.ToLower().Contains("stats") || child.name.ToLower().Contains("reason"))) { statsTrans = child; break; }
                }
            }
        }
        if (!IsValidTransform(statsTrans))
        {
            GameObject statsObj = new GameObject("StatsText");
            statsObj.transform.SetParent(clipTrans, false);
            statsTrans = statsObj.transform;
            
            TextMeshProUGUI tmp = statsObj.AddComponent<TextMeshProUGUI>();
            tmp.text = isVictory ? "Жильцы дома в безопасности." : "Слишком много ошибок.";
            tmp.color = new Color(0.15f, 0.15f, 0.15f);
            tmp.fontSize = 13f;
            tmp.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            statsTrans.SetParent(clipTrans, false);
            statsTrans.name = "StatsText";
        }

        // в) StampImage
        Transform stampTrans = clipTrans.Find("StampImage");
        if (!IsValidTransform(stampTrans)) stampTrans = clipTrans.Find("PassportStamp");
        if (!IsValidTransform(stampTrans))
        {
            stampTrans = panelTrans.Find("PassportStamp");
            if (!IsValidTransform(stampTrans)) stampTrans = panelTrans.Find("StampImage");
            if (!IsValidTransform(stampTrans))
            {
                foreach (Transform child in panelTrans)
                {
                    if (child != null && child.name.ToLower().Contains("stamp")) { stampTrans = child; break; }
                }
            }
        }
        if (!IsValidTransform(stampTrans))
        {
            GameObject stampObj = new GameObject("StampImage");
            stampObj.transform.SetParent(clipTrans, false);
            stampTrans = stampObj.transform;
            
            Image img = stampObj.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            stampTrans.SetParent(clipTrans, false);
            stampTrans.name = "StampImage";
        }

        // г) MainMenuBtn
        Transform menuBtnTrans = clipTrans.Find("MainMenuBtn");
        if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = clipTrans.Find("ExitBtn");
        if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = clipTrans.Find("ExitMenuBtn");
        if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = clipTrans.Find("MenuBtn");
        if (!IsValidTransform(menuBtnTrans))
        {
            menuBtnTrans = panelTrans.Find("ExitBtn");
            if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = panelTrans.Find("ExitMenuBtn");
            if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = panelTrans.Find("MainMenuBtn");
            if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = panelTrans.Find("MenuBtn");
            if (!IsValidTransform(menuBtnTrans))
            {
                foreach (Transform child in panelTrans)
                {
                    if (child != null && (child.name.ToLower().Contains("btn") || child.name.ToLower().Contains("button")))
                    {
                        if (child.name != "NextShiftBtn" && !child.name.ToLower().Contains("continue"))
                        {
                            menuBtnTrans = child;
                            break;
                        }
                    }
                }
            }
        }
        if (!IsValidTransform(menuBtnTrans))
        {
            GameObject btnObj = new GameObject("MainMenuBtn");
            btnObj.transform.SetParent(clipTrans, false);
            menuBtnTrans = btnObj.transform;
            
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            Outline outline = btnObj.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(2, -2);
            
            btnObj.AddComponent<Button>();
            
            GameObject txtObj = new GameObject("BtnText");
            txtObj.transform.SetParent(btnObj.transform, false);
            TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "В ГЛАВНОЕ МЕНЮ";
            tmp.fontSize = 20;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            
            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.sizeDelta = Vector2.zero;
        }
        else
        {
            menuBtnTrans.SetParent(clipTrans, false);
            menuBtnTrans.name = "MainMenuBtn";
        }

        return clipTrans;
    }

    [MenuItem("Parallax/Стилизовать экраны Победы и Поражения")]
    public static void Stylize()
    {
        // 1. Сначала распределяем и импортируем кастомные планшеты
        SortAndCopyClipboards();

        // 2. Проверяем сцену
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene")
        {
            EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, откройте сцену GameScene в Unity перед запуском этого скрипта!", "ОК");
            return;
        }

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти Canvas в сцене! Убедитесь, что сцена GameScene открыта.", "ОК");
            return;
        }

        Transform victoryPanelTrans = canvas.transform.Find("VictoryPanel");
        Transform gameOverPanelTrans = canvas.transform.Find("GameOverPanel");

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
        if (victoryPanelTrans == null)
        {
            foreach (var obj in allObjects)
            {
                if (obj != null && obj.name == "VictoryPanel") { victoryPanelTrans = obj.transform; break; }
            }
        }
        if (gameOverPanelTrans == null)
        {
            foreach (var obj in allObjects)
            {
                if (obj != null && obj.name == "GameOverPanel") { gameOverPanelTrans = obj.transform; break; }
            }
        }

        if (victoryPanelTrans == null || gameOverPanelTrans == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти VictoryPanel или GameOverPanel в сцене! Проверьте иерархию.", "ОК");
            return;
        }

        // Автоматически настраиваем текстуры как Спрайты в Unity
        ConfigureTextureAsSprite("Assets/Sprites/stamp_approved_ru.png");
        ConfigureTextureAsSprite("Assets/Sprites/stamp_fired_ru.png");

        Sprite approvedSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/stamp_approved_ru.png");
        Sprite firedSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/stamp_fired_ru.png");
        Sprite clipboardApprovedSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/clipboard_approved.jpg");
        Sprite clipboardFiredSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/clipboard_fired.jpg");

        // Привязываем спрайты к GameManager
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            Undo.RecordObject(gm, "Assign Russian End Game Stamps and Clipboards");
            gm.stampApprovedRu = approvedSprite;
            gm.stampFiredRu = firedSprite;
            gm.clipboardApprovedSprite = clipboardApprovedSprite;
            gm.clipboardFiredSprite = clipboardFiredSprite;
            EditorUtility.SetDirty(gm);
        }

        // Очищаем старые рамки/сканлайны
        RemoveLegacyBordersAndScanlines(victoryPanelTrans);
        RemoveLegacyBordersAndScanlines(gameOverPanelTrans);

        // ==========================================
        // 3. СТИЛИЗУЕМ VICTORYPANEL (СМЕНА ОКОНЧЕНА)
        // ==========================================
        // Делаем саму панель полностью прозрачной, так как за ней будет работать Background
        Image vicMainImg = victoryPanelTrans.GetComponent<Image>();
        if (vicMainImg != null)
        {
            Undo.RecordObject(vicMainImg, "Clear Victory Panel Background");
            vicMainImg.color = new Color(0f, 0f, 0f, 0f);
        }

        // Автоматически строим или корректируем иерархию планшета
        Transform vicClipboard = EnsureClipboardHierarchy(victoryPanelTrans, true);

        // Отключаем старые разбросанные элементы на корневой панели
        DisableOldDirectChildren(victoryPanelTrans);

        // Применяем идеальное пиксельное позиционирование внутри Clipboard
        if (vicClipboard != null)
        {
            PositionElement(vicClipboard, "TitleText", "VictoryTitle", 110f, 28f, isText: true, isButton: false);
            PositionElement(vicClipboard, "StatsText", "VictoryStats", 30f, 13f, isText: true, isButton: false);
            PositionElement(vicClipboard, "StampImage", "PassportStamp", -30f, 0f, isText: false, isButton: false, new Vector2(240f, 120f), -8f, approvedSprite);
            PositionElement(vicClipboard, "MainMenuBtn", "ExitMenuBtn", -130f, 0f, isText: false, isButton: true, new Vector2(260f, 50f));
        }

        // ==========================================
        // 4. СТИЛИЗУЕМ GAMEOVERPANEL (ВЫ УВОЛЕНЫ)
        // ==========================================
        Image goMainImg = gameOverPanelTrans.GetComponent<Image>();
        if (goMainImg != null)
        {
            Undo.RecordObject(goMainImg, "Clear Game Over Panel Background");
            goMainImg.color = new Color(0f, 0f, 0f, 0f);
        }

        Transform goClipboard = EnsureClipboardHierarchy(gameOverPanelTrans, false);

        DisableOldDirectChildren(gameOverPanelTrans);

        if (goClipboard != null)
        {
            PositionElement(goClipboard, "TitleText", "GameOverTitle", 110f, 28f, isText: true, isButton: false);
            PositionElement(goClipboard, "StatsText", "GameOverReason", 30f, 13f, isText: true, isButton: false);
            PositionElement(goClipboard, "StampImage", "PassportStamp", -30f, 0f, isText: false, isButton: false, new Vector2(240f, 120f), -12f, firedSprite);
            PositionElement(goClipboard, "MainMenuBtn", "ExitBtn", -130f, 0f, isText: false, isButton: true, new Vector2(260f, 50f));
        }

        // Помечаем сцену измененной для сохранения
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Успех!", "Иерархия планшетов успешно построена!\n\nДобавлены атмосферные фоны, твои кастомные планшеты настроены как Sprites и выровнены по пикселям с русскими шрифтами и штампами.\n\nНажмите Ctrl + S для сохранения.", "Отлично!");
    }

    private static void RemoveLegacyBordersAndScanlines(Transform panelTrans)
    {
        System.Collections.Generic.List<GameObject> toDestroy = new System.Collections.Generic.List<GameObject>();
        foreach (Transform child in panelTrans)
        {
            string cName = child.name;
            if (cName == "LineT" || cName == "LineB" || cName == "LineL" || cName == "LineR" || cName.StartsWith("CRT_Scanline") || cName == "RetroStampOverlay")
            {
                toDestroy.Add(child.gameObject);
            }
        }
        foreach (var obj in toDestroy)
        {
            Undo.DestroyObjectImmediate(obj);
        }
    }

    private static void DisableOldDirectChildren(Transform panelTrans)
    {
        foreach (Transform child in panelTrans)
        {
            string name = child.name;
            if (name != "Background" && name != "Clipboard")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private static void PositionElement(Transform parent, string primaryName, string secondaryName, float yPos, float sizeVal, bool isText, bool isButton, Vector2? sizeDelta = null, float? rotZ = null, Sprite spriteVal = null)
    {
        Transform target = parent.Find(primaryName);
        if (target == null && !string.IsNullOrEmpty(secondaryName)) target = parent.Find(secondaryName);
        if (target == null)
        {
            foreach (Transform child in parent)
            {
                if (child.name.ToLower().Contains(primaryName.ToLower()) || (!string.IsNullOrEmpty(secondaryName) && child.name.ToLower().Contains(secondaryName.ToLower())))
                {
                    target = child;
                    break;
                }
            }
        }

        if (target != null)
        {
            Undo.RecordObject(target.gameObject, "Position and Scale Clipboard Element");
            RectTransform rt = target.GetComponent<RectTransform>();
            if (rt != null)
            {
                Undo.RecordObject(rt, "Modify RectTransform");
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, yPos);
                
                if (sizeDelta.HasValue)
                {
                    rt.sizeDelta = sizeDelta.Value;
                }
                else if (isText)
                {
                    rt.sizeDelta = new Vector2(480f, primaryName.Contains("Title") ? 80f : 200f);
                }
                
                if (rotZ.HasValue)
                {
                    rt.localEulerAngles = new Vector3(0f, 0f, rotZ.Value);
                }
            }

            if (isText)
            {
                TextMeshProUGUI tmp = target.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    Undo.RecordObject(tmp, "Modify TMP Style");
                    tmp.fontSize = sizeVal;
                    tmp.fontStyle = primaryName.Contains("Title") || secondaryName.Contains("Title") ? FontStyles.Bold : FontStyles.Normal;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.color = new Color(0.15f, 0.15f, 0.15f); // Идеальный цвет печатной машинки!
                    tmp.enableWordWrapping = true;
                }
            }

            if (isButton)
            {
                TextMeshProUGUI btnTxt = target.GetComponentInChildren<TextMeshProUGUI>();
                if (btnTxt != null)
                {
                    Undo.RecordObject(btnTxt, "Modify Button text style");
                    btnTxt.fontSize = 20;
                    btnTxt.fontStyle = FontStyles.Bold;
                }
            }

            if (spriteVal != null)
            {
                Image img = target.GetComponent<Image>();
                if (img != null)
                {
                    Undo.RecordObject(img, "Modify Image Sprite");
                    img.sprite = spriteVal;
                    img.color = Color.white;
                    img.preserveAspect = true;
                }
            }
            
            // Гарантируем включение элемента
            Undo.RecordObject(target.gameObject, "Activate Element");
            target.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"[StylizeEndScreens] Не удалось найти элемент '{primaryName}' (или '{secondaryName}') в '{parent.name}'!");
        }
    }

    private static void ConfigureTextureAsSprite(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            if (importer.textureType != TextureImporterType.Sprite || importer.spritePixelsPerUnit != 100)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Point; // Для четкости пиксель-арта
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
                Debug.Log($"[StylizeEndScreens] Текстура {assetPath} успешно сконфигурирована как Sprite с Point фильтрацией!");
            }
        }
    }
}
