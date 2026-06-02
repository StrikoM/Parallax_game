using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixGasLayer_v3
{
    static AutoFixGasLayer_v3()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGasLayer_v3", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGasLayer_v3", true);

        GameObject gas = GameObject.Find("DecontaminationGas");
        GameObject outsideBg = GameObject.Find("OutsideBg");
        GameObject shutter = GameObject.Find("WindowShutter");

        if (gas != null && outsideBg != null)
        {
            // Главная ошибка была в том, что газ лежал не в той папке!
            // Переносим газ ВНУТРЬ фона окна (туда же, где стоит посетитель)
            gas.transform.SetParent(outsideBg.transform, false);
            
            // Ставим газ в самый низ списка (чтобы он был ПОВЕРХ посетителя)
            gas.transform.SetAsLastSibling();
            
            // Но железная шторка должна перекрывать газ! Ставим её ещё ниже.
            if (shutter != null)
            {
                shutter.transform.SetParent(outsideBg.transform, false);
                shutter.transform.SetAsLastSibling();
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Слой газа окончательно ИСПРАВЛЕН! Теперь он ПЕРЕД персонажем!</color>");
        }
    }
}
