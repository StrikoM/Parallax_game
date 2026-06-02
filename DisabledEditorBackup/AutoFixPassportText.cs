using UnityEngine;
using UnityEditor;

public class AutoFixPassportText
{
    [MenuItem("Parallax/Починить Тексты Паспорта")]
    public static void RunOnce()
    {
        if (Application.isPlaying) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        Transform expDate = canvas.transform.Find("WindowFrame/Desk/DocumentTray/Passport/PassportExpDateText");
        if (expDate == null) expDate = canvas.transform.Find("Desk/DocumentTray/Passport/PassportExpDateText");
        
        Transform idText = canvas.transform.Find("WindowFrame/Desk/DocumentTray/Passport/PassportIdText");
        if (idText == null) idText = canvas.transform.Find("Desk/DocumentTray/Passport/PassportIdText");

        if (expDate != null)
        {
            RectTransform rt = expDate.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Сдвигаем вниз на 15 пикселей от текущей позиции
                Vector2 pos = rt.anchoredPosition;
                pos.y = -20f; // Устанавливаем точную позицию пониже (обычно около -20 или -25)
                rt.anchoredPosition = pos;
            }
        }

        if (idText != null)
        {
            RectTransform rt = idText.GetComponent<RectTransform>();
            if (rt != null)
            {
                Vector2 pos = rt.anchoredPosition;
                pos.y = -20f; // Устанавливаем точную позицию пониже
                rt.anchoredPosition = pos;
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Antigravity] Тексты срока действия и ID в паспорте успешно сдвинуты вниз!");
    }
}
