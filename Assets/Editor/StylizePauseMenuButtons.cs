using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class StylizePauseMenuButtons : EditorWindow
{
    [MenuItem("Parallax/Стилизовать меню паузы в GameScene")]
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

        // Находим PausePanel
        Transform pausePanelTrans = canvas.transform.Find("PausePanel");
        if (pausePanelTrans == null) pausePanelTrans = canvas.transform.Find("PauseMenu");

        // Если не нашли на прямом Canvas, делаем глубокий поиск
        if (pausePanelTrans == null)
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var obj in allObjects)
            {
                if (obj != null && (obj.name == "PausePanel" || obj.name == "PauseMenu"))
                {
                    pausePanelTrans = obj.transform;
                    break;
                }
            }
        }

        if (pausePanelTrans == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти PausePanel в сцене! Попробуйте собрать интерфейс заново.", "ОК");
            return;
        }

        // Находим кнопки внутри PausePanel
        Transform resumeBtnTrans = pausePanelTrans.Find("ResumeBtn");
        Transform settingsBtnTrans = pausePanelTrans.Find("SettingsBtn");
        Transform exitBtnTrans = pausePanelTrans.Find("ExitBtn");

        if (resumeBtnTrans == null || settingsBtnTrans == null || exitBtnTrans == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось найти кнопки ResumeBtn, SettingsBtn или ExitBtn внутри PausePanel!", "ОК");
            return;
        }

        // Цвета
        Color neonGreen = new Color(0.2f, 1f, 0.2f, 1f);
        Color neonRed = new Color(1f, 0.2f, 0.2f, 1f);

        // Функция стилизации отдельной кнопки
        void StylizeButton(GameObject btnObj, Color neonColor)
        {
            Undo.RecordObject(btnObj, "Stylize Button Background");
            
            // 1. Делаем фон полностью прозрачным
            Image img = btnObj.GetComponent<Image>();
            if (img == null) img = btnObj.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0f); // Прозрачный фон
            img.sprite = null; // Убираем дефолтные спрайты кнопок, чтобы форма была чистым прямоугольником

            // 2. Настраиваем переходы цвета на кнопке (как в главном меню)
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                Undo.RecordObject(btn, "Stylize Button Colors");
                ColorBlock cb = btn.colors;
                cb.normalColor = new Color(1f, 1f, 1f, 0f);
                cb.highlightedColor = new Color(1f, 1f, 1f, 0.08f); // Легкое свечение при наведении
                cb.pressedColor = new Color(1f, 1f, 1f, 0.15f);
                cb.selectedColor = cb.normalColor;
                cb.colorMultiplier = 1f;
                btn.colors = cb;
            }

            // 3. Удаляем старые рамки/линии, если они есть
            foreach (Transform child in btnObj.transform)
            {
                if (child.name == "LineT" || child.name == "LineB" || child.name == "LineL" || child.name == "LineR")
                {
                    Undo.DestroyObjectImmediate(child.gameObject);
                }
            }

            // 4. Создаем красивые тонкие рамки
            // Верхняя
            GameObject tLine = new GameObject("LineT"); tLine.transform.SetParent(btnObj.transform, false);
            tLine.AddComponent<Image>().color = neonColor;
            RectTransform rT = tLine.GetComponent<RectTransform>(); rT.anchorMin = new Vector2(0, 1); rT.anchorMax = new Vector2(1, 1); rT.anchoredPosition = Vector2.zero; rT.sizeDelta = new Vector2(0, 5);
            Undo.RegisterCreatedObjectUndo(tLine, "Create Border LineT");

            // Нижняя
            GameObject bLine = new GameObject("LineB"); bLine.transform.SetParent(btnObj.transform, false);
            bLine.AddComponent<Image>().color = neonColor;
            RectTransform rB = bLine.GetComponent<RectTransform>(); rB.anchorMin = new Vector2(0, 0); rB.anchorMax = new Vector2(1, 0); rB.anchoredPosition = Vector2.zero; rB.sizeDelta = new Vector2(0, 5);
            Undo.RegisterCreatedObjectUndo(bLine, "Create Border LineB");

            // Левая
            GameObject lLine = new GameObject("LineL"); lLine.transform.SetParent(btnObj.transform, false);
            lLine.AddComponent<Image>().color = neonColor;
            RectTransform rL = lLine.GetComponent<RectTransform>(); rL.anchorMin = new Vector2(0, 0); rL.anchorMax = new Vector2(0, 1); rL.anchoredPosition = Vector2.zero; rL.sizeDelta = new Vector2(5, 0);
            Undo.RegisterCreatedObjectUndo(lLine, "Create Border LineL");

            // Правая
            GameObject rLine = new GameObject("LineR"); rLine.transform.SetParent(btnObj.transform, false);
            rLine.AddComponent<Image>().color = neonColor;
            RectTransform rR = rLine.GetComponent<RectTransform>(); rR.anchorMin = new Vector2(1, 0); rR.anchorMax = new Vector2(1, 1); rR.anchoredPosition = Vector2.zero; rR.sizeDelta = new Vector2(5, 0);
            Undo.RegisterCreatedObjectUndo(rLine, "Create Border LineR");

            // 5. Настраиваем текст
            TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                Undo.RecordObject(txt, "Stylize Button Text");
                txt.color = neonColor;
                txt.fontSize = 32;
                txt.fontStyle = FontStyles.Bold;
            }

            // 6. Добавляем компонент анимации наведения
            if (btnObj.GetComponent<MenuButtonHoverEffects>() == null)
            {
                btnObj.AddComponent<MenuButtonHoverEffects>();
            }
        }

        // Применяем стили к каждой кнопке
        StylizeButton(resumeBtnTrans.gameObject, neonGreen);
        StylizeButton(settingsBtnTrans.gameObject, neonGreen);
        StylizeButton(exitBtnTrans.gameObject, neonRed);

        // Помечаем сцену измененной, чтобы её можно было сохранить
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Успех!", "Кнопки меню паузы успешно стилизованы под единый CRT-дизайн главного меню!\n\nНажмите Ctrl + S, чтобы сохранить сцену.", "Отлично!");
    }
}
