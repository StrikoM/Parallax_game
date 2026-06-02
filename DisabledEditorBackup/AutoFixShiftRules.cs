using UnityEngine;
using UnityEditor;
using TMPro;

[InitializeOnLoad]
public class AutoFixShiftRules
{
    static AutoFixShiftRules()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixShiftRules_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoFixShiftRules_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        DatabaseFolderUI db = Object.FindAnyObjectByType<DatabaseFolderUI>();

        bool changed = false;

        // Создаем поле СРОК ДЕЙСТВИЯ для Паспорта (на столе)
        if (gm != null && gm.passportEyesText != null && gm.passportExpDateText == null)
        {
            GameObject newObj = Object.Instantiate(gm.passportEyesText.gameObject, gm.passportEyesText.transform.parent);
            newObj.name = "PassportExpDateText";
            
            RectTransform rt = newObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 120f); // Сдвигаем ниже глаз

            TextMeshProUGUI textComp = newObj.GetComponent<TextMeshProUGUI>();
            textComp.text = "ГОДЕН ДО:\n---";
            
            gm.passportExpDateText = textComp;
            EditorUtility.SetDirty(gm);
            changed = true;
        }

        // Создаем поле СРОК ДЕЙСТВИЯ для Базы данных (в мониторе)
        if (db != null && db.folderEyesText != null && db.folderExpDateText == null)
        {
            GameObject newObj = Object.Instantiate(db.folderEyesText.gameObject, db.folderEyesText.transform.parent);
            newObj.name = "FolderExpDateText";
            
            RectTransform rt = newObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 120f); // Сдвигаем ниже глаз

            TextMeshProUGUI textComp = newObj.GetComponent<TextMeshProUGUI>();
            textComp.text = "ГОДЕН ДО:\n---";
            
            db.folderExpDateText = textComp;
            EditorUtility.SetDirty(db);
            changed = true;
        }

        if (changed)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=cyan>[Antigravity] Добавлены новые поля 'Годен до' для 3-ей смены!</color>");
        }
    }
}
