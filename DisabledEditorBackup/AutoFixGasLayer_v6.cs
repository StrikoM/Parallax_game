using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixGasLayer_v6
{
    static AutoFixGasLayer_v6()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGasLayer_v6", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGasLayer_v6", true);

        GameObject gas = GameObject.Find("DecontaminationGas");
        GameObject visitor = GameObject.Find("VisitorImage");

        if (gas != null && visitor != null)
        {
            // 1. Переносим газ в ту же папку, где лежит сам персонаж!
            gas.transform.SetParent(visitor.transform.parent, false);
            
            // 2. Ставим газ СРАЗУ ПОД персонажем в Иерархии.
            // В Unity это означает, что газ нарисуется ровно ПОВЕРХ персонажа.
            gas.transform.SetSiblingIndex(visitor.transform.GetSiblingIndex() + 1);
            
            // 3. Обнуляем Z-координату на всякий случай
            gas.transform.localPosition = new Vector3(gas.transform.localPosition.x, gas.transform.localPosition.y, 0f);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Газ жестко привязан к позиции ПОВЕРХ посетителя!</color>");
        }
    }
}
