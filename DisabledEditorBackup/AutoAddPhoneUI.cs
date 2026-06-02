using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoAddPhoneUI
{
    static AutoAddPhoneUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddPhoneUI_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoAddPhoneUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        // Заполняем реплики диспетчера для всех трех смен
        gm.shiftPhoneMessages = new string[]
        {
            "Алло? Это Диспетчер. Смена началась. Твоя задача проста: сверяй ИМЯ и ID в паспорте с базой данных в компьютере. Не впускай чужаков. Конец связи.",
            "Внимание! В городе участились случаи подделки документов. Теперь проверяй еще и ЦВЕТ ГЛАЗ. Если не совпадает — жми отказ. Будь бдителен.",
            "Слушай внимательно. Монстры крадут старые паспорта. Теперь обязательно проверяй СРОК ДЕЙСТВИЯ. Если просрочен — это не человек. Удачи."
        };

        // Создаем кнопку телефона на столе (слева внизу)
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null && gm.phoneButton == null)
        {
            GameObject phoneObj = new GameObject("PhoneButton");
            phoneObj.transform.SetParent(canvas.transform, false);
            
            RectTransform rt = phoneObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0); // Левый нижний угол
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(100, 100); 
            rt.sizeDelta = new Vector2(200, 150);

            Image img = phoneObj.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Пока это просто темный квадрат (вы потом замените на спрайт)

            // Подпись "ТЕЛЕФОН", чтобы вы поняли, что это
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(phoneObj.transform, false);
            RectTransform trt = txtObj.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            
            TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = "ТЕЛЕФОН";
            txt.alignment = TextAlignmentOptions.Center;
            txt.fontSize = 30;
            txt.color = Color.white;
            
            Button btn = phoneObj.AddComponent<Button>();
            gm.phoneButton = btn;

            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Телефон добавлен на стол!</color>");
        }
    }
}
