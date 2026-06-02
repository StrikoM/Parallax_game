using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public class AutoAddDossierPhotoUI
{
    static AutoAddDossierPhotoUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddDossierPhotoUI_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoAddDossierPhotoUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        // Ищем объект досье по тексту имени
        if (gm.dossierNameText != null && gm.dossierPhotoDisplay == null)
        {
            Transform dossierPanel = gm.dossierNameText.transform.parent; // Панель базы данных
            
            GameObject photoObj = new GameObject("DossierPhoto");
            photoObj.transform.SetParent(dossierPanel, false);
            
            RectTransform rt = photoObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.5f);
            rt.anchorMax = new Vector2(0.9f, 0.95f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            
            Image img = photoObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Темный квадрат-заглушка
            img.preserveAspect = true;

            gm.dossierPhotoDisplay = img;
            
            // Немного сдвигаем текст базы данных вниз, чтобы уступить место фото
            RectTransform nameRT = gm.dossierNameText.GetComponent<RectTransform>();
            nameRT.anchorMin = new Vector2(0, 0.35f); nameRT.anchorMax = new Vector2(1, 0.45f);
            
            RectTransform idRT = gm.dossierIdText.GetComponent<RectTransform>();
            idRT.anchorMin = new Vector2(0, 0.25f); idRT.anchorMax = new Vector2(1, 0.35f);
            
            RectTransform eyesRT = gm.dossierEyesText.GetComponent<RectTransform>();
            eyesRT.anchorMin = new Vector2(0, 0.15f); eyesRT.anchorMax = new Vector2(1, 0.25f);

            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Поле для фото на мониторе добавлено!</color>");
        }
    }
}
