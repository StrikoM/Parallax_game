using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixPassportDate
{
    static AutoFixPassportDate()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixPassportDate_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixPassportDate_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        // Если дата так и не появилась, создадим её копированием текста Глаз
        if (gm.passportEyesText != null)
        {
            // Если текст даты уже есть, но спрятался или сломался - удалим старый
            if (gm.passportExpDateText != null)
            {
                Object.DestroyImmediate(gm.passportExpDateText.gameObject);
                gm.passportExpDateText = null;
            }

            // Дублируем объект Глаза
            GameObject newTextObj = Object.Instantiate(gm.passportEyesText.gameObject, gm.passportEyesText.transform.parent);
            newTextObj.name = "PassportExpDateText";
            
            RectTransform rt = newTextObj.GetComponent<RectTransform>();
            // Сдвигаем вниз на 40 пикселей от Глаз
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 45f);
            
            TMPro.TextMeshProUGUI txt = newTextObj.GetComponent<TMPro.TextMeshProUGUI>();
            txt.text = "ГОДЕН ДО:\n12.2084";
            
            gm.passportExpDateText = txt;
            
            // Если серая карточка слишком короткая, увеличим её высоту, чтобы текст влез
            RectTransform docRT = gm.passportEyesText.transform.parent.GetComponent<RectTransform>();
            if (docRT != null)
            {
                // Если якоря стоят по центру или сверху, мы можем просто увеличить размер
                docRT.sizeDelta = new Vector2(docRT.sizeDelta.x, docRT.sizeDelta.y + 50f);
            }

            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Текст 'Годен до' аккуратно скопирован под 'Глаза'!</color>");
        }
    }
}
