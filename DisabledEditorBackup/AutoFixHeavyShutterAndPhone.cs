using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

[InitializeOnLoad]
public class AutoFixHeavyShutterAndPhone
{
    static AutoFixHeavyShutterAndPhone()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Restore Premium Door & Phone")]
    public static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene") return;

        Debug.Log("<color=yellow>[Antigravity] Настраиваю премиальную железную дверь и телефон...</color>");

        // 1. Проверяем и исправляем настройки импорта спрайтов
        FixSpriteImportSettings("Assets/Sprites/heavy_iron_shutter_asset_1778255440075.png");
        FixSpriteImportSettings("Assets/Visitors/Phone_GuyCN-removebg-preview.png");

        // Загружаем спрайты
        Sprite shutterSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/heavy_iron_shutter_asset_1778255440075.png");
        Sprite phoneSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Visitors/Phone_GuyCN-removebg-preview.png");

        if (shutterSprite == null)
        {
            // Попробуем альтернативный путь
            shutterSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Visitors/heavy_iron_shutter_asset_1778255440075.png");
        }

        // 2. Восстанавливаем шторку (дверь)
        GameObject shutterObj = GameObject.Find("WindowShutter");
        if (shutterObj != null)
        {
            shutterObj.SetActive(true);
            Image img = shutterObj.GetComponent<Image>();
            if (img != null)
            {
                if (shutterSprite != null)
                {
                    img.sprite = shutterSprite;
                    img.color = Color.white;
                    Debug.Log("[Antigravity] Спрайт железной двери успешно назначен!");
                }
                else
                {
                    img.color = new Color(0.25f, 0.28f, 0.3f, 1f);
                    Debug.LogWarning("[Antigravity] Предупреждение: Спрайт железной двери не найден в Assets!");
                }
            }

            RectTransform rt = shutterObj.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition3D = Vector3.zero; // Гарантирует Z = 0
                rt.sizeDelta = new Vector2(820, 620);
                rt.localScale = Vector3.one;
            }

            // Удаляем временные полоски-жалюзи, если они есть
            for (int i = shutterObj.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = shutterObj.transform.GetChild(i);
                if (child.name == "Line")
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
        else
        {
            Debug.LogError("[Antigravity] Ошибка: Объект WindowShutter не найден в сцене!");
        }

        // 3. Восстанавливаем телефон
        GameObject phoneObj = GameObject.Find("PhoneButton");
        if (phoneObj == null)
        {
            phoneObj = GameObject.Find("PhoneBtn");
        }
        if (phoneObj == null)
        {
            phoneObj = GameObject.Find("Telephone");
        }

        if (phoneObj != null)
        {
            phoneObj.SetActive(true);
            // Переименовываем в PhoneButton для унификации
            phoneObj.name = "PhoneButton";

            Image img = phoneObj.GetComponent<Image>();
            if (img != null)
            {
                if (phoneSprite != null)
                {
                    img.sprite = phoneSprite;
                    img.color = Color.white;
                    Debug.Log("[Antigravity] Спрайт телефона успешно назначен!");
                }
                else
                {
                    img.color = new Color(0.15f, 0.15f, 0.15f, 1f);
                    Debug.LogWarning("[Antigravity] Предупреждение: Спрайт телефона не найден в Assets!");
                }
            }

            RectTransform rt = phoneObj.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(0f, 0f);
                rt.pivot = new Vector2(0f, 0f);
                rt.anchoredPosition3D = new Vector3(60f, 40f, 0f); // Гарантирует Z = 0
                rt.sizeDelta = new Vector2(250f, 180f);      // Премиальный размер
                rt.localScale = Vector3.one;
            }

            // Удаляем или скрываем текстовую подпись "ТЕЛЕФОН"
            for (int i = phoneObj.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = phoneObj.transform.GetChild(i);
                if (child.name == "Text" || child.GetComponent<TMPro.TextMeshProUGUI>() != null)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            // Убедимся, что кнопка привязана к GameManager
            GameManager gm = Object.FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                Button btn = phoneObj.GetComponent<Button>();
                if (btn != null)
                {
                    gm.phoneButton = btn;
                    EditorUtility.SetDirty(gm);
                }
            }
        }
        else
        {
            Debug.LogWarning("[Antigravity] Предупреждение: Кнопка телефона не найдена в сцене. Она будет добавлена при следующем восстановлении.");
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[Antigravity] Атмосферная железная дверь и телефон полностью настроены!</color>");
    }

    private static void FixSpriteImportSettings(string path)
    {
        if (!File.Exists(path)) return;

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            if (importer.textureType != TextureImporterType.Sprite || importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Point; // Для пиксель-арта
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
                Debug.Log($"[Antigravity] Настройки импорта для {path} изменены на Sprite!");
            }
        }
    }
}
