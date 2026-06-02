using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixGasLayer_v4
{
    static AutoFixGasLayer_v4()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGasLayer_v4", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGasLayer_v4", true);

        GameObject gas = GameObject.Find("DecontaminationGas");
        GameObject outsideBg = GameObject.Find("OutsideBg");

        if (gas != null && outsideBg != null)
        {
            // Переносим газ обратно в WindowFrame, но ставим его СРАЗУ ПОСЛЕ OutsideBg.
            // Так он будет гарантированно поверх всего содержимого окна, но за столом (Desk).
            gas.transform.SetParent(outsideBg.transform.parent, false);
            gas.transform.SetSiblingIndex(outsideBg.transform.GetSiblingIndex() + 1);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Слой газа поставлен СРАЗУ ПОСЛЕ OutsideBg!</color>");
        }
    }
}
