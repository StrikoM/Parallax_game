using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixGasLayer_v8
{
    static AutoFixGasLayer_v8()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGasLayer_v8", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGasLayer_v8", true);

        GameObject gas = GameObject.Find("DecontaminationGas");
        GameObject outsideBg = GameObject.Find("OutsideBg");
        GameObject visitor = GameObject.Find("VisitorImage");
        GameObject shutter = GameObject.Find("WindowShutter");

        if (gas != null && outsideBg != null)
        {
            // 1. Вытаскиваем Газ из папки фона в основное окно (WindowFrame)
            gas.transform.SetParent(outsideBg.transform.parent, false);
            
            // 2. Ставим Газ ровно ПОСЛЕ папки OutsideBg (где сидит посетитель и шторка)
            // Это значит, что он будет рисоваться ПОВЕРХ всей будки, но ДО вашего стола (Desk)
            gas.transform.SetSiblingIndex(outsideBg.transform.GetSiblingIndex() + 1);

            // 3. ЖЕСТКО ОБНУЛЯЕМ Z-координату. 
            // Иногда в Unity картинка улетает "вглубь" экрана, и из-за этого рисуется сзади, несмотря на слои.
            if (visitor != null) visitor.transform.localPosition = new Vector3(visitor.transform.localPosition.x, visitor.transform.localPosition.y, 0);
            if (shutter != null) shutter.transform.localPosition = new Vector3(shutter.transform.localPosition.x, shutter.transform.localPosition.y, 0);
            outsideBg.transform.localPosition = new Vector3(outsideBg.transform.localPosition.x, outsideBg.transform.localPosition.y, 0);
            gas.transform.localPosition = new Vector3(gas.transform.localPosition.x, gas.transform.localPosition.y, 0);

            // Также обнуляем у самого газа (LeftSpray, RightSpray)
            foreach (Transform child in gas.transform)
            {
                child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, 0);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Газ 100% ПЕРЕД будкои и посетителем! Ось Z сброшена.</color>");
        }
    }
}
