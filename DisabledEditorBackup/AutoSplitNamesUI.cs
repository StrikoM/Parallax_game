using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoSplitNamesUI
{
    static AutoSplitNamesUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Split Name Texts")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        if (EditorPrefs.GetBool("AutoSplitNames_v1", false)) return;
        EditorPrefs.SetBool("AutoSplitNames_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        bool changed = false;

        // Разделяем в Паспорте
        if (gm.passportNameText != null && gm.passportLastNameText == null)
        {
            GameObject newObj = Object.Instantiate(gm.passportNameText.gameObject, gm.passportNameText.transform.parent);
            newObj.name = "PassportLastNameText";
            
            RectTransform rt = newObj.GetComponent<RectTransform>();
            // Сдвигаем Фамилию чуть ниже Имени
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 35f);
            
            // Имя чуть-чуть приподнимаем
            RectTransform origRt = gm.passportNameText.GetComponent<RectTransform>();
            origRt.anchoredPosition = new Vector2(origRt.anchoredPosition.x, origRt.anchoredPosition.y + 15f);

            TMPro.TextMeshProUGUI txt = newObj.GetComponent<TMPro.TextMeshProUGUI>();
            txt.text = "ФАМИЛИЯ:\n...";
            gm.passportNameText.text = "ИМЯ:\n...";
            gm.passportLastNameText = txt;
            changed = true;
        }

        // Разделяем в Досье (если оно используется)
        if (gm.dossierNameText != null && gm.dossierLastNameText == null)
        {
            GameObject newObj = Object.Instantiate(gm.dossierNameText.gameObject, gm.dossierNameText.transform.parent);
            newObj.name = "DossierLastNameText";
            
            RectTransform rt = newObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 35f);
            
            RectTransform origRt = gm.dossierNameText.GetComponent<RectTransform>();
            origRt.anchoredPosition = new Vector2(origRt.anchoredPosition.x, origRt.anchoredPosition.y + 15f);

            TMPro.TextMeshProUGUI txt = newObj.GetComponent<TMPro.TextMeshProUGUI>();
            txt.text = "ФАМИЛИЯ:\n...";
            gm.dossierNameText.text = "ИМЯ:\n...";
            gm.dossierLastNameText = txt;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(gm);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Тексты Имени и Фамилии успешно разделены!</color>");
        }
    }
}
