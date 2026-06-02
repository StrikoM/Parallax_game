using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoFixStunGun
{
    static AutoFixStunGun()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool("AutoFixStunGun_v1", false)) return;
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;
        
        EditorPrefs.SetBool("AutoFixStunGun_v1", true);

        GameObject drawer = GameObject.Find("StunGunDrawer");
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();

        if (drawer != null && canvas != null)
        {
            // 1. Вытаскиваем шокер в корень Канваса, чтобы его точно никто не перекрыл
            drawer.transform.SetParent(canvas.transform, false);
            
            // 2. Ставим его прямо перед красной вспышкой
            GameObject flash = GameObject.Find("ScreenFlash");
            if (flash != null)
            {
                drawer.transform.SetSiblingIndex(flash.transform.GetSiblingIndex());
            }

            // 3. Убеждаемся, что кнопка может нажиматься
            CanvasGroup cg = drawer.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = true;

            // 4. ГЛАВНОЕ: жестко привязываем кнопку шокера к GameManager
            GameManager gm = Object.FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                UnityEngine.UI.Button btn = drawer.GetComponentInChildren<UnityEngine.UI.Button>(true);
                if (btn != null)
                {
                    gm.stunGunButton = btn;
                    EditorUtility.SetDirty(gm);
                    Debug.Log("<color=green>[Antigravity] Кнопка шокера успешно найдена и привязана!</color>");
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Antigravity] Слой шокера исправлен!</color>");
        }
    }
}
