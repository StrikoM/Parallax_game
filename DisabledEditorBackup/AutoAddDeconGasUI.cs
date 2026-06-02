using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoAddDeconGasUI
{
    static AutoAddDeconGasUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddDeconGasUI_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoAddDeconGasUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        if (gm.deconGasOverlay == null)
        {
            // Ищем окно (WindowFrame)
            GameObject windowFrame = GameObject.Find("WindowFrame");
            if (windowFrame == null) return;

            // Создаем эффект газа внутри окна
            GameObject gasObj = new GameObject("DecontaminationGas");
            gasObj.transform.SetParent(windowFrame.transform, false);
            
            // Ставим его ПЕРЕД железной шторкой, чтобы газ был за шторкой, но перед посетителем
            gasObj.transform.SetSiblingIndex(windowFrame.transform.childCount - 2);
            
            // Растягиваем на всё окно
            RectTransform rt = gasObj.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            
            // Добавляем картинку (густой пар)
            Image img = gasObj.AddComponent<Image>();
            img.color = new Color(0.8f, 0.95f, 0.9f, 0.85f); // Бело-голубоватый густой дезинфицирующий пар
            
            // Добавляем CanvasGroup для анимации растворения
            CanvasGroup cg = gasObj.AddComponent<CanvasGroup>();
            cg.alpha = 0f; 
            cg.blocksRaycasts = false;
            
            gasObj.SetActive(false);

            gm.deconGasOverlay = cg;

            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Система дезинфекции (Пар) успешно установлена в будку!</color>");
        }
    }
}
