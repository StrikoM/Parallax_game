using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class AutoAddNewspaperToDesk
{
    [MenuItem("Parallax/Добавить Газету на Стол")]
    public static void RunOnce()
    {
        if (Application.isPlaying) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        Transform desk = canvas.transform.Find("WindowFrame/Desk");
        if (desk == null) desk = canvas.transform.Find("Desk"); // Резервный поиск
        
        if (desk == null)
        {
            Debug.LogError("Стол (Desk) не найден!");
            return;
        }

        // Удаляем старую иконку газеты, если есть
        Transform old = desk.Find("NewspaperDeskBtn");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // Создаем кнопку-иконку
        GameObject iconObj = new GameObject("NewspaperDeskBtn");
        iconObj.transform.SetParent(desk, false);
        
        RectTransform rt = iconObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        // Размещаем справа от лотка (Лоток на 0, телефон на -250)
        rt.anchoredPosition = new Vector2(300, 80); 
        rt.sizeDelta = new Vector2(160, 100);
        
        // Немного повернем для небрежности
        rt.localRotation = Quaternion.Euler(0, 0, 10);

        Image img = iconObj.AddComponent<Image>();
        img.color = new Color(0.9f, 0.88f, 0.8f, 1f); // Желтоватая старая бумага

        Button btn = iconObj.AddComponent<Button>();

        // Добавляем текст на свернутую газету
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(iconObj.transform, false);
        RectTransform txtRt = txtObj.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = "ГАЗЕТА\n(Читать)";
        txt.fontSize = 20;
        txt.fontStyle = FontStyles.Bold;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Antigravity] Иконка газеты успешно добавлена на стол!");
    }
}
