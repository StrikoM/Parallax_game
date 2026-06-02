using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixGasLayer_v7
{
    static AutoFixGasLayer_v7()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGasLayer_v7", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGasLayer_v7", true);

        GameObject gas = GameObject.Find("DecontaminationGas");
        GameObject shutter = GameObject.Find("WindowShutter");

        if (gas != null && shutter != null)
        {
            // Переносим газ в ту же папку, где лежит шторка
            gas.transform.SetParent(shutter.transform.parent, false);
            
            // Ставим газ СРАЗУ ПОСЛЕ шторки в списке.
            // Это значит, что газ нарисуется ПОВЕРХ железной шторки и ПОВЕРХ персонажа!
            gas.transform.SetSiblingIndex(shutter.transform.GetSiblingIndex() + 1);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Газ перенесен ПОВЕРХ железной шторки!</color>");
        }
    }
}
