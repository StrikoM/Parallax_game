using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixGasLayer_v5
{
    static AutoFixGasLayer_v5()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGasLayer_v5", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGasLayer_v5", true);

        GameObject gas = GameObject.Find("DecontaminationGas");
        GameObject shutter = GameObject.Find("WindowShutter");

        if (gas != null && shutter != null)
        {
            // Переносим газ в ту же папку, где находится шторка (внутри OutsideBg)
            gas.transform.SetParent(shutter.transform.parent, false);
            
            // Ставим газ прямо ПЕРЕД шторкой в списке
            // В Unity это значит, что газ нарисуется раньше шторки.
            // Итог: Газ поверх посетителя, но ЗА железной шторкой!
            gas.transform.SetSiblingIndex(shutter.transform.GetSiblingIndex());

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Газ поставлен за шторку (перед ней в Иерархии)!</color>");
        }
    }
}
