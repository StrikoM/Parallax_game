using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class StylizeSettingsPanel : EditorWindow
{
    [MenuItem("Parallax/Стилизовать панель настроек в GameScene")]
    public static void Stylize()
    {
        // Проверяем, открыта ли нужная сцена
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene")
        {
            EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, откройте сцену GameScene в Unity перед запуском этого скрипта!", "ОК");
            return;
        }

        // Находим Canvas в сцене
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти Canvas в сцене! Убедитесь, что сцена GameScene открыта.", "ОК");
            return;
        }

        // Находим SettingsPanel
        Transform settingsPanelTrans = canvas.transform.Find("SettingsPanel");
        if (settingsPanelTrans == null) settingsPanelTrans = canvas.transform.Find("SettingsMenu");

        // Если не нашли на прямом Canvas, делаем глубокий поиск
        if (settingsPanelTrans == null)
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var obj in allObjects)
            {
                if (obj != null && (obj.name == "SettingsPanel" || obj.name == "SettingsMenu"))
                {
                    settingsPanelTrans = obj.transform;
                    break;
                }
            }
        }

        if (settingsPanelTrans == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти SettingsPanel в сцене! Попробуйте собрать интерфейс заново.", "ОК");
            return;
        }

        Color neonGreen = new Color(0.2f, 1f, 0.2f, 1f);

        // 1. Делаем саму панель центрированным красивым терминальным окном (а не на весь экран)
        GameObject panelObj = settingsPanelTrans.gameObject;
        Undo.RecordObject(panelObj, "Stylize Settings Panel Background");
        
        RectTransform panelRt = panelObj.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.anchoredPosition = Vector2.zero;
        panelRt.sizeDelta = new Vector2(750f, 580f); // Компактный и аккуратный размер

        Image panelImg = panelObj.GetComponent<Image>();
        if (panelImg == null) panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0.02f, 0.05f, 0.02f, 0.98f); // Очень темный, почти черный зеленый (непрозрачный, чтобы скрыть паузу!)
        panelImg.sprite = null;

        // 2. Стилизуем и уменьшаем рамки вокруг панели
        // Удаляем старые рамки
        foreach (Transform child in settingsPanelTrans)
        {
            if (child.name == "LineT" || child.name == "LineB" || child.name == "LineL" || child.name == "LineR")
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }

        // Добавляем новые аккуратные тонкие рамки (толщина 5)
        // Верхняя
        GameObject tLine = new GameObject("LineT"); tLine.transform.SetParent(settingsPanelTrans, false);
        tLine.AddComponent<Image>().color = neonGreen;
        RectTransform rT = tLine.GetComponent<RectTransform>(); rT.anchorMin = new Vector2(0, 1); rT.anchorMax = new Vector2(1, 1); rT.anchoredPosition = Vector2.zero; rT.sizeDelta = new Vector2(0, 5);
        Undo.RegisterCreatedObjectUndo(tLine, "Create Border LineT");

        // Нижняя
        GameObject bLine = new GameObject("LineB"); bLine.transform.SetParent(settingsPanelTrans, false);
        bLine.AddComponent<Image>().color = neonGreen;
        RectTransform rB = bLine.GetComponent<RectTransform>(); rB.anchorMin = new Vector2(0, 0); rB.anchorMax = new Vector2(1, 0); rB.anchoredPosition = Vector2.zero; rB.sizeDelta = new Vector2(0, 5);
        Undo.RegisterCreatedObjectUndo(bLine, "Create Border LineB");

        // Левая
        GameObject lLine = new GameObject("LineL"); lLine.transform.SetParent(settingsPanelTrans, false);
        lLine.AddComponent<Image>().color = neonGreen;
        RectTransform rL = lLine.GetComponent<RectTransform>(); rL.anchorMin = new Vector2(0, 0); rL.anchorMax = new Vector2(0, 1); rL.anchoredPosition = Vector2.zero; rL.sizeDelta = new Vector2(5, 0);
        Undo.RegisterCreatedObjectUndo(lLine, "Create Border LineL");

        // Правая
        GameObject rLine = new GameObject("LineR"); rLine.transform.SetParent(settingsPanelTrans, false);
        rLine.AddComponent<Image>().color = neonGreen;
        RectTransform rR = rLine.GetComponent<RectTransform>(); rR.anchorMin = new Vector2(1, 0); rR.anchorMax = new Vector2(1, 1); rR.anchoredPosition = Vector2.zero; rR.sizeDelta = new Vector2(5, 0);
        Undo.RegisterCreatedObjectUndo(rLine, "Create Border LineR");


        // 3. Стилизуем элементы внутри
        // Заголовок "НАСТРОЙКИ ЗВУКА"
        Transform titleTrans = settingsPanelTrans.Find("SettingsTitle");
        if (titleTrans != null)
        {
            TextMeshProUGUI titleTxt = titleTrans.GetComponent<TextMeshProUGUI>();
            if (titleTxt != null)
            {
                Undo.RecordObject(titleTxt, "Stylize Settings Title Text");
                titleTxt.fontSize = 44; // Чуть компактнее
                titleTxt.fontStyle = FontStyles.Bold;
                titleTxt.color = neonGreen;
            }
            RectTransform titleRt = titleTrans.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 1f);
            titleRt.anchorMax = new Vector2(0.5f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0f, -40f);
            titleRt.sizeDelta = new Vector2(600f, 70f);
        }

        // Позиционируем контейнеры слайдеров
        Transform musicContainer = null;
        Transform sfxContainer = null;

        foreach (Transform child in settingsPanelTrans)
        {
            if (child.name.Contains("SliderContainer"))
            {
                if (child.name.Contains("MUSIC") || child.name.Contains("МУЗЫКА"))
                {
                    musicContainer = child;
                }
                else if (child.name.Contains("SFX") || child.name.Contains("ЭФФЕКТЫ"))
                {
                    sfxContainer = child;
                }
            }
        }

        // Стилизуем контейнер музыки
        if (musicContainer != null)
        {
            RectTransform contRt = musicContainer.GetComponent<RectTransform>();
            contRt.anchorMin = new Vector2(0.5f, 0.5f);
            contRt.anchorMax = new Vector2(0.5f, 0.5f);
            contRt.pivot = new Vector2(0.5f, 0.5f);
            contRt.anchoredPosition = new Vector2(0f, 85f);
            contRt.sizeDelta = new Vector2(600f, 120f);

            // Метка музыки
            TextMeshProUGUI label = musicContainer.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                Undo.RecordObject(label, "Stylize Music Label");
                label.fontSize = 24;
                label.fontStyle = FontStyles.Bold;
                label.color = neonGreen;
                
                RectTransform labelRt = label.GetComponent<RectTransform>();
                labelRt.anchorMin = new Vector2(0.5f, 1f);
                labelRt.anchorMax = new Vector2(0.5f, 1f);
                labelRt.anchoredPosition = new Vector2(0f, -10f);
                labelRt.sizeDelta = new Vector2(600f, 30f);
            }

            // Слайдер
            Slider sld = musicContainer.GetComponentInChildren<Slider>();
            if (sld != null)
            {
                RectTransform sldRt = sld.GetComponent<RectTransform>();
                sldRt.anchorMin = new Vector2(0.5f, 0f);
                sldRt.anchorMax = new Vector2(0.5f, 0f);
                sldRt.pivot = new Vector2(0.5f, 0f);
                sldRt.anchoredPosition = new Vector2(0f, 15f);
                sldRt.sizeDelta = new Vector2(500f, 25f);
            }
        }

        // Стилизуем контейнер SFX
        if (sfxContainer != null)
        {
            RectTransform contRt = sfxContainer.GetComponent<RectTransform>();
            contRt.anchorMin = new Vector2(0.5f, 0.5f);
            contRt.anchorMax = new Vector2(0.5f, 0.5f);
            contRt.pivot = new Vector2(0.5f, 0.5f);
            contRt.anchoredPosition = new Vector2(0f, -50f);
            contRt.sizeDelta = new Vector2(600f, 120f);

            // Метка SFX
            TextMeshProUGUI label = sfxContainer.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                Undo.RecordObject(label, "Stylize SFX Label");
                label.fontSize = 24;
                label.fontStyle = FontStyles.Bold;
                label.color = neonGreen;
                
                RectTransform labelRt = label.GetComponent<RectTransform>();
                labelRt.anchorMin = new Vector2(0.5f, 1f);
                labelRt.anchorMax = new Vector2(0.5f, 1f);
                labelRt.anchoredPosition = new Vector2(0f, -10f);
                labelRt.sizeDelta = new Vector2(600f, 30f);
            }

            // Слайдер
            Slider sld = sfxContainer.GetComponentInChildren<Slider>();
            if (sld != null)
            {
                RectTransform sldRt = sld.GetComponent<RectTransform>();
                sldRt.anchorMin = new Vector2(0.5f, 0f);
                sldRt.anchorMax = new Vector2(0.5f, 0f);
                sldRt.pivot = new Vector2(0.5f, 0f);
                sldRt.anchoredPosition = new Vector2(0f, 15f);
                sldRt.sizeDelta = new Vector2(500f, 25f);
            }
        }

        // 4. Стилизуем кнопку "НАЗАД"
        Transform backBtnTrans = settingsPanelTrans.Find("BackBtn");
        if (backBtnTrans == null) backBtnTrans = settingsPanelTrans.Find("BackButton");

        if (backBtnTrans != null)
        {
            GameObject backObj = backBtnTrans.gameObject;
            Undo.RecordObject(backObj, "Stylize Back Button Background");

            // Задаем аккуратный размер и положение кнопке
            RectTransform backRt = backObj.GetComponent<RectTransform>();
            backRt.anchorMin = new Vector2(0.5f, 0f);
            backRt.anchorMax = new Vector2(0.5f, 0f);
            backRt.pivot = new Vector2(0.5f, 0f);
            backRt.anchoredPosition = new Vector2(0f, 40f);
            backRt.sizeDelta = new Vector2(300f, 65f);

            // Делаем фон прозрачным
            Image img = backObj.GetComponent<Image>();
            if (img == null) img = backObj.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0f); // Прозрачный фон
            img.sprite = null;

            // Настраиваем переходы цвета на кнопке (как в главном меню)
            Button btn = backObj.GetComponent<Button>();
            if (btn != null)
            {
                Undo.RecordObject(btn, "Stylize Back Button Colors");
                ColorBlock cb = btn.colors;
                cb.normalColor = new Color(1f, 1f, 1f, 0f);
                cb.highlightedColor = new Color(1f, 1f, 1f, 0.08f); // Легкое свечение при наведении
                cb.pressedColor = new Color(1f, 1f, 1f, 0.15f);
                cb.selectedColor = cb.normalColor;
                cb.colorMultiplier = 1f;
                btn.colors = cb;
            }

            // Удаляем старые рамки
            foreach (Transform child in backBtnTrans)
            {
                if (child.name == "LineT" || child.name == "LineB" || child.name == "LineL" || child.name == "LineR")
                {
                    Undo.DestroyObjectImmediate(child.gameObject);
                }
            }

            // Добавляем новые тонкие неоновые рамки
            // Верхняя
            GameObject btLine = new GameObject("LineT"); btLine.transform.SetParent(backBtnTrans, false);
            btLine.AddComponent<Image>().color = neonGreen;
            RectTransform brT = btLine.GetComponent<RectTransform>(); brT.anchorMin = new Vector2(0, 1); brT.anchorMax = new Vector2(1, 1); brT.anchoredPosition = Vector2.zero; brT.sizeDelta = new Vector2(0, 5);
            Undo.RegisterCreatedObjectUndo(btLine, "Create Back Border LineT");

            // Нижняя
            GameObject bbLine = new GameObject("LineB"); bbLine.transform.SetParent(backBtnTrans, false);
            bbLine.AddComponent<Image>().color = neonGreen;
            RectTransform brB = bbLine.GetComponent<RectTransform>(); brB.anchorMin = new Vector2(0, 0); brB.anchorMax = new Vector2(1, 0); brB.anchoredPosition = Vector2.zero; brB.sizeDelta = new Vector2(0, 5);
            Undo.RegisterCreatedObjectUndo(bbLine, "Create Back Border LineB");

            // Левая
            GameObject blLine = new GameObject("LineL"); blLine.transform.SetParent(backBtnTrans, false);
            blLine.AddComponent<Image>().color = neonGreen;
            RectTransform brL = blLine.GetComponent<RectTransform>(); brL.anchorMin = new Vector2(0, 0); brL.anchorMax = new Vector2(0, 1); brL.anchoredPosition = Vector2.zero; brL.sizeDelta = new Vector2(5, 0);
            Undo.RegisterCreatedObjectUndo(blLine, "Create Back Border LineL");

            // Правая
            GameObject brLine = new GameObject("LineR"); brLine.transform.SetParent(backBtnTrans, false);
            brLine.AddComponent<Image>().color = neonGreen;
            RectTransform brR = brLine.GetComponent<RectTransform>(); brR.anchorMin = new Vector2(1, 0); brR.anchorMax = new Vector2(1, 1); brR.anchoredPosition = Vector2.zero; brR.sizeDelta = new Vector2(5, 0);
            Undo.RegisterCreatedObjectUndo(brLine, "Create Back Border LineR");

            // Настраиваем текст
            TextMeshProUGUI txt = backObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                Undo.RecordObject(txt, "Stylize Back Button Text");
                txt.color = neonGreen;
                txt.fontSize = 28;
                txt.fontStyle = FontStyles.Bold;
            }

            // Добавляем анимацию наведения
            if (backObj.GetComponent<MenuButtonHoverEffects>() == null)
            {
                backObj.AddComponent<MenuButtonHoverEffects>();
            }
        }

        // Помечаем сцену измененной, чтобы её можно было сохранить
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Успех!", "Панель настроек звука успешно стилизована под аккуратное CRT-окно!\n\nНажмите Ctrl + S, чтобы сохранить сцену.", "Отлично!");
    }
}
