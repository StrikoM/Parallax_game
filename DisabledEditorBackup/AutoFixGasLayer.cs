using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixGasLayer
{
    static AutoFixGasLayer()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixGasLayer_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixGasLayer_v1", true);

        GameObject gasContainerObj = GameObject.Find("DecontaminationGas");
        if (gasContainerObj != null)
        {
            // 1. Помещаем газ в самый низ списка WindowFrame (чтобы он был ПОВЕРХ персонажа)
            gasContainerObj.transform.SetAsLastSibling();
            
            // 2. А железную шторку помещаем ЕЩЕ ниже, чтобы она перекрывала газ
            GameObject shutter = GameObject.Find("WindowShutter");
            if (shutter != null)
            {
                shutter.transform.SetAsLastSibling();
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Слой газа исправлен! Теперь он ПЕРЕД персонажем!</color>");
        }
    }
}
