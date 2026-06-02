using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoAddDeconGasUI_v2
{
    static AutoAddDeconGasUI_v2()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddDeconGasUI_v2", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoAddDeconGasUI_v2", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        // Удаляем старый газ (если есть), чтобы сделать новый двойной!
        GameObject windowFrame = GameObject.Find("WindowFrame");
        if (windowFrame == null) return;

        Transform oldGas = windowFrame.transform.Find("DecontaminationGas");
        if (oldGas != null)
        {
            Object.DestroyImmediate(oldGas.gameObject);
        }

        // 1. Создаем Контейнер
        GameObject gasContainerObj = new GameObject("DecontaminationGas");
        gasContainerObj.transform.SetParent(windowFrame.transform, false);
        gasContainerObj.transform.SetSiblingIndex(windowFrame.transform.childCount - 2); // За шторкой
        
        RectTransform rtContainer = gasContainerObj.AddComponent<RectTransform>();
        rtContainer.anchorMin = Vector2.zero; rtContainer.anchorMax = Vector2.one;
        rtContainer.offsetMin = Vector2.zero; rtContainer.offsetMax = Vector2.zero;
        
        CanvasGroup cg = gasContainerObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f; 
        cg.blocksRaycasts = false;

        // 2. Левый пар (выстреливает слева)
        GameObject leftGas = new GameObject("LeftSpray");
        leftGas.transform.SetParent(gasContainerObj.transform, false);
        RectTransform leftRt = leftGas.AddComponent<RectTransform>();
        leftRt.anchorMin = new Vector2(0f, 0f); leftRt.anchorMax = new Vector2(0.6f, 1f); // Левая половина окна + немного дальше
        leftRt.offsetMin = Vector2.zero; leftRt.offsetMax = Vector2.zero;
        
        Image imgLeft = leftGas.AddComponent<Image>();
        imgLeft.color = new Color(0.8f, 0.95f, 0.9f, 0.85f); // Густой пар

        // 3. Правый пар (выстреливает справа)
        GameObject rightGas = new GameObject("RightSpray");
        rightGas.transform.SetParent(gasContainerObj.transform, false);
        RectTransform rightRt = rightGas.AddComponent<RectTransform>();
        rightRt.anchorMin = new Vector2(0.4f, 0f); rightRt.anchorMax = new Vector2(1f, 1f); // Правая половина окна + немного дальше
        rightRt.offsetMin = Vector2.zero; rightRt.offsetMax = Vector2.zero;
        
        Image imgRight = rightGas.AddComponent<Image>();
        imgRight.color = new Color(0.8f, 0.95f, 0.9f, 0.85f); // Густой пар

        gasContainerObj.SetActive(false);
        gm.deconGasOverlay = cg;

        EditorUtility.SetDirty(gm);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Двухсторонняя система дезинфекции успешно установлена!</color>");
    }
}
