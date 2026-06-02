using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoResetProgress
{
    static AutoResetProgress()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoResetProgress_v1", false)) return;
        EditorPrefs.SetBool("AutoResetProgress_v1", true);

        // Полностью сбрасываем прогресс, чтобы игрок мог тестировать новую систему с 1-й смены!
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("CurrentShift", 0);
        PlayerPrefs.Save();
        
        Debug.Log("<color=magenta>[Antigravity] ПРОГРЕСС СБРОШЕН! Теперь вы начнете с 1-й смены, и телефон будет звонить!</color>");
    }
}
