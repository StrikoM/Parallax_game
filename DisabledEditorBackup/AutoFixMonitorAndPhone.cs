using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixMonitorAndPhone
{
    static AutoFixMonitorAndPhone()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixMonitorAndPhone_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixMonitorAndPhone_v1", true);

        // 1. Опускаем манитор
        GameObject monitor = GameObject.Find("PhysicalMonitor");
        if (monitor != null)
        {
            RectTransform rt = monitor.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Опускаем монитор на 200 пикселей вниз, чтобы не закрывать календарь
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 200f);
            }
        }

        // 2. Настраиваем текст звонков телефона
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            // Если массив звонков пустой или в нём мало смен, увеличиваем его
            if (gm.shiftPhoneMessages == null || gm.shiftPhoneMessages.Length < 4)
            {
                string[] newMsgs = new string[4];
                if (gm.shiftPhoneMessages != null)
                {
                    for (int i = 0; i < gm.shiftPhoneMessages.Length; i++) newMsgs[i] = gm.shiftPhoneMessages[i];
                }
                gm.shiftPhoneMessages = newMsgs;
            }

            // Заполняем пустые тексты звонков, чтобы телефон ОБЯЗАТЕЛЬНО звонил в начале смены
            if (string.IsNullOrEmpty(gm.shiftPhoneMessages[0])) gm.shiftPhoneMessages[0] = "Служба безопасности на связи. Будь внимателен, сегодня могут прийти мимики. Обращай внимание на их странное дыхание.";
            if (string.IsNullOrEmpty(gm.shiftPhoneMessages[1])) gm.shiftPhoneMessages[1] = "Осторожно, стекло хрупкое. Некоторые монстры очень нетерпеливы. Если услышишь стук - сразу бей шокером!";
            if (string.IsNullOrEmpty(gm.shiftPhoneMessages[2])) gm.shiftPhoneMessages[2] = "Ты отлично справляешься. Но угроза растет. Не верь никому и проверяй документы.";
            if (string.IsNullOrEmpty(gm.shiftPhoneMessages[3])) gm.shiftPhoneMessages[3] = "Это твоя последняя смена. Они попытаются прорваться. Удачи.";

            EditorUtility.SetDirty(gm);
            Debug.Log("<color=green>[Antigravity] Тексты для звонков настроены! Телефон будет звонить.</color>");
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
