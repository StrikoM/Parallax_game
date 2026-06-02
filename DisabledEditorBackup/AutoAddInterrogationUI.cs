using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class AutoAddInterrogationUI
{
    static AutoAddInterrogationUI()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoAddInterrogationUI_v1", false)) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        EditorPrefs.SetBool("AutoAddInterrogationUI_v1", true);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;

        // Ищем место для кнопки (например, рядом с кнопками Approve/Reject)
        GameObject approveBtn = GameObject.Find("ApproveBtn");
        GameObject rejectBtn = GameObject.Find("RejectBtn");
        Transform parent = null;
        
        if (approveBtn != null) 
        {
            parent = approveBtn.transform.parent;
        }

        if (parent != null)
        {
            Transform existing = parent.Find("InterrogateBtn");
            if (existing == null)
            {
                // Дублируем зеленую кнопку и делаем из нее синюю кнопку ДОПРОС
                GameObject btnObj = Object.Instantiate(approveBtn, parent);
                btnObj.name = "InterrogateBtn";
                
                RectTransform rt = btnObj.GetComponent<RectTransform>();
                // Ставим её ровно по центру (X = 0), между Одобрить и Отказать
                rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y); 
                
                Button b = btnObj.GetComponent<Button>();
                b.onClick.RemoveAllListeners(); // Убираем старые события Approve
                
                // Делаем её темно-синей
                Image img = btnObj.GetComponent<Image>();
                if (img != null) img.color = new Color(0.1f, 0.3f, 0.6f, 1f); 
                
                TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    txt.text = "ДОПРОС";
                    txt.color = Color.white;
                }
                
                gm.interrogateBtn = b;
                
                EditorUtility.SetDirty(gm);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                Debug.Log("<color=green>[Antigravity] Кнопка ДОПРОС добавлена по центру!</color>");
            }
        }
    }
}
