using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Нужно для работы с текстом

public class MainMenu : MonoBehaviour
{
    [Header("Интерфейс (UI)")]
    public GameObject continueButton;
    public TextMeshProUGUI continueButtonText;
    public GameObject levelSelectButton; // Кнопка выбора уровней (размещена в редакторе)

    [Header("Звуки")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;

    [Header("Настройки")]
    public GameObject settingsPanel;
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;


    void Start()
    {
        // Безопасность: не запускаем логику главного меню, если мы на другой сцене!
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu")
        {
            Debug.LogWarning("[MainMenu] Предупреждение: Скрипт MainMenu запущен на сцене '" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "'. Отключаем.");
            enabled = false;
            return;
        }

        // ОЧЕНЬ ВАЖНО: При загрузке меню сбрасываем время и возвращаем курсор.
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Полная очистка остаточных UI-элементов из сцены геймплея
        DestroyPersistentUI();

        // Проверяем прогресс игрока
        int currentShift = PlayerPrefs.GetInt("CurrentShift", 0);
        int unlockedShift = PlayerPrefs.GetInt("UnlockedShift", 0);
        
        // Синхронизируем анлок, если текущая смена выше разблокированной
        if (currentShift > unlockedShift) 
        {
            unlockedShift = currentShift;
            PlayerPrefs.SetInt("UnlockedShift", unlockedShift);
            PlayerPrefs.Save();
        }

        // Если игра полностью пройдена (индекс смены равен 7), сбрасываем текущую смену для скрытия кнопки продолжения
        if (currentShift >= 7)
        {
            currentShift = 0;
            PlayerPrefs.SetInt("CurrentShift", 0);
            PlayerPrefs.Save();
        }
        
        if (currentShift > 0)
        {
            // Если есть сохранения, показываем кнопку "Продолжить" с номером смены
            if (continueButton != null) continueButton.SetActive(true);
            if (continueButtonText != null) continueButtonText.text = "ПРОДОЛЖИТЬ СМЕНУ (" + (currentShift + 1) + ")";
        }
        else
        {
            // Если игрок только скачал игру, скрываем "Продолжить"
            if (continueButton != null) continueButton.SetActive(false);
        }

        // Показываем кнопку "ВЫБОР СМЕНЫ", если пройдена хотя бы 1 смена
        if (levelSelectButton != null)
        {
            levelSelectButton.SetActive(unlockedShift > 0);
        }

        // Инициализируем громкость звуков UI
        if (uiAudioSource != null)
        {
            uiAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        }

        // Автоматически добавляем кнопку достижений
        CreateAchievementsButton();

        // Автоматически привязываем настройки звука в Главном Меню (на случай сброса ссылок)
        UnityEngine.UI.Slider[] allSliders = Object.FindObjectsByType<UnityEngine.UI.Slider>(FindObjectsInactive.Include);
        foreach (var slider in allSliders)
        {
            string sName = slider.name.ToLower();
            if (sName.Contains("music"))
            {
                musicSlider = slider;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(OnMusicVolumeChanged);
                slider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            }
            else if (sName.Contains("sfx"))
            {
                sfxSlider = slider;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(OnSFXVolumeChanged);
                slider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            }
        }

        Button[] allButtons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (var btn in allButtons)
        {
            string bName = btn.name.ToLower();
            if (bName == "settingsbtn" || bName == "settingsbutton")
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(ShowSettingsPanel);
            }
            else if (bName == "closesettingsbtn" || bName == "closesettingsbutton" || (bName == "closebtn" && btn.transform.IsChildOf(settingsPanel != null ? settingsPanel.transform : btn.transform)))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(HideSettingsPanel);
            }

            // Добавляем красивую ретро-анимацию наведения на все кнопки главного меню!
            if (btn.GetComponent<MenuButtonHoverEffects>() == null)
            {
                btn.gameObject.AddComponent<MenuButtonHoverEffects>();
            }
        }
    }

    public void ContinueGame()
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        StartCoroutine(LoadSceneWithDelay("GameScene"));
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("CurrentShift", 0);
        PlayerPrefs.Save();
        
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        StartCoroutine(LoadSceneWithDelay("GameScene"));
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        StartCoroutine(QuitWithDelay());
    }

    private System.Collections.IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // Небольшая пауза, чтобы звук успел проиграться до того, как сцена удалится
        yield return new WaitForSeconds(0.4f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    
    private System.Collections.IEnumerator QuitWithDelay()
    {
        yield return new WaitForSeconds(0.4f);
        Application.Quit();
    }

    // ==========================================
    // ЛОГИКА ВЫБОРА УРОВНЕЙ (Генерируется кодом)
    // ==========================================
    private GameObject levelsPanel;

    public void ShowLevelsPanel()
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        
        int unlockedShift = PlayerPrefs.GetInt("UnlockedShift", 0);

        if (levelsPanel == null)
        {
            CreateLevelsUIRuntime(unlockedShift);
        }
        else
        {
            levelsPanel.SetActive(true);
        }

        // Автоматически привязываем оверлейные кнопки к эффектам наведения
        Button[] allMenuButtons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (var btn in allMenuButtons)
        {
            if (btn.GetComponent<MenuButtonHoverEffects>() == null)
            {
                btn.gameObject.AddComponent<MenuButtonHoverEffects>();
            }
        }
    }

    public void HideLevelsPanel()
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        if (levelsPanel != null) levelsPanel.SetActive(false);
    }

    public void LoadSpecificShift(int shiftIndex)
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        PlayerPrefs.SetInt("CurrentShift", shiftIndex);
        PlayerPrefs.Save();
        StartCoroutine(LoadSceneWithDelay("GameScene"));
    }

    // Удален метод CreateLevelSelectButtonRuntime(), так как кнопка теперь в редакторе

    private void CreateLevelsUIRuntime(int maxUnlocked)
    {
        Canvas canvas = FindMainSceneCanvas();
        if (canvas == null) return;

        levelsPanel = new GameObject("RuntimeLevelsPanel");
        levelsPanel.transform.SetParent(canvas.transform, false);
        levelsPanel.transform.SetAsLastSibling();
        
        UnityEngine.UI.Image bg = levelsPanel.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0, 0, 0, 0.95f);
        bg.raycastTarget = true;

        RectTransform rt = levelsPanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        // Заголовок
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(levelsPanel.transform, false);
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "ВЫБЕРИТЕ СМЕНУ";
        title.fontSize = 50;
        title.color = Color.white;
        title.alignment = TextAlignmentOptions.Center;
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 0.85f); titleRt.anchorMax = new Vector2(1, 1);
        titleRt.offsetMin = Vector2.zero; titleRt.offsetMax = Vector2.zero;

        // Кнопка Назад
        GameObject closeObj = new GameObject("CloseBtn");
        closeObj.transform.SetParent(levelsPanel.transform, false);
        UnityEngine.UI.Image closeImg = closeObj.AddComponent<UnityEngine.UI.Image>();
        closeImg.color = new Color(0.6f, 0.1f, 0.1f);
        UnityEngine.UI.Button closeBtn = closeObj.AddComponent<UnityEngine.UI.Button>();
        closeBtn.onClick.AddListener(HideLevelsPanel);
        RectTransform closeRt = closeObj.GetComponent<RectTransform>();
        closeRt.sizeDelta = new Vector2(250, 70);
        closeRt.anchorMin = new Vector2(0.5f, 0.1f); closeRt.anchorMax = new Vector2(0.5f, 0.1f);
        closeRt.anchoredPosition = new Vector2(0, 0);

        GameObject closeTxtObj = new GameObject("Text");
        closeTxtObj.transform.SetParent(closeObj.transform, false);
        TextMeshProUGUI closeTxt = closeTxtObj.AddComponent<TextMeshProUGUI>();
        closeTxt.text = "НАЗАД";
        closeTxt.color = Color.white;
        closeTxt.fontSize = 30;
        closeTxt.alignment = TextAlignmentOptions.Center;
        RectTransform closeTxtRt = closeTxtObj.GetComponent<RectTransform>();
        closeTxtRt.anchorMin = Vector2.zero; closeTxtRt.anchorMax = Vector2.one;
        closeTxtRt.offsetMin = Vector2.zero; closeTxtRt.offsetMax = Vector2.zero;

        // Ограничиваем количество отображаемых уровней реальным числом смен (7 смен, индексы 0-6)
        int totalShifts = 7;
        int limit = Mathf.Min(maxUnlocked, totalShifts - 1);

        // Сетка кнопок смен
        for (int i = 0; i <= limit; i++)
        {
            int shiftIndex = i; // Обязательно для правильного замыкания в AddListener!
            
            GameObject btnObj = new GameObject("ShiftBtn_" + i);
            btnObj.transform.SetParent(levelsPanel.transform, false);
            UnityEngine.UI.Image btnImg = btnObj.AddComponent<UnityEngine.UI.Image>();
            btnImg.color = new Color(0.2f, 0.4f, 0.2f);
            UnityEngine.UI.Button btn = btnObj.AddComponent<UnityEngine.UI.Button>();
            
            btn.onClick.AddListener(() => LoadSpecificShift(shiftIndex));

            RectTransform btnRt = btnObj.GetComponent<RectTransform>();
            btnRt.sizeDelta = new Vector2(180, 180);
            
            // Простая сетка: 4 в ряд, по центру экрана
            int row = i / 4;
            int col = i % 4;
            btnRt.anchorMin = new Vector2(0.5f, 0.5f);
            btnRt.anchorMax = new Vector2(0.5f, 0.5f);
            
            float startX = -330f;
            float startY = 150f;
            btnRt.anchoredPosition = new Vector2(startX + col * 220f, startY - row * 220f);

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = "Смена\n" + (i + 1);
            txt.color = Color.white;
            txt.fontSize = 35;
            txt.alignment = TextAlignmentOptions.Center;
            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = Vector2.zero; txtRt.offsetMax = Vector2.zero;
        }
    }

    // ==========================================
    // ЛОГИКА НАСТРОЕК ЗВУКА
    // ==========================================
    public void ShowSettingsPanel()
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

            if (musicSlider != null) musicSlider.value = musicVol;
            if (sfxSlider != null) sfxSlider.value = sfxVol;
        }
    }

    public void HideSettingsPanel()
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        if (uiAudioSource != null)
        {
            uiAudioSource.volume = value;
        }
    }

    // ==========================================
    // ЛОГИКА ДОСТИЖЕНИЙ (Процедурный CRT интерфейс)
    // ==========================================
    private GameObject achievementsPanel;

    private struct MenuAchievement
    {
        public string id;
        public string title;
        public string description;
        public MenuAchievement(string id, string title, string description)
        {
            this.id = id;
            this.title = title;
            this.description = description;
        }
    }

    private MenuAchievement[] menuAchievements = new MenuAchievement[]
    {
        new MenuAchievement("FIRST_SHIFT", "ПЕРВЫЙ РУБЕЖ", "Успешно завершена первая смена."),
        new MenuAchievement("PERFECT_SHIFT", "ИДЕАЛЬНЫЙ ИНСПЕКТОР", "Смена пройдена без единой ошибки и штрафа."),
        new MenuAchievement("WELCOME_HOME", "ДОБРО ПОЖАЛОВАТЬ ДОМОЙ", "Допущена ошибка, монстр проник в сектор."),
        new MenuAchievement("NERVES_OF_STEEL", "СТАЛЬНЫЕ НЕРВЫ", "Атака монстра успешно отражена шокером."),
        new MenuAchievement("CLEAN_WINDOW", "КРОВАВАЯ РАБОТА", "Стекло блокпоста полностью очищено от крови."),
        new MenuAchievement("PANIC", "ПАНИКЁР", "Отказано во въезде абсолютно честному гражданину."),
        new MenuAchievement("CYBORG", "КИБОРГ-УБИЙЦА", "Выявлен монстр с визуальной аномалией."),
        new MenuAchievement("SHERLOCK", "ШЕРЛОК БЛОКПОСТА", "Пойман шпион с микро-опечаткой в документах."),
        new MenuAchievement("TELEPHONIST", "НА СВЯЗИ", "Вы выслушали все указания Управдома по телефону."),
        new MenuAchievement("PARANOID", "ПАРАНОЙЯ", "Допрошен абсолютно честный гражданин."),
        new MenuAchievement("IRON_SHIELD", "ЖЕЛЕЗНЫЙ ЩИТ", "Успешно пройдена финальная 7-я смена."),
        new MenuAchievement("CLOSE_CALL", "НА ВОЛОСКЕ", "Смена пройдена с ровно двумя ошибками.")
    };

    public void ShowAchievementsPanel()
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        
        if (achievementsPanel == null)
        {
            CreateAchievementsUIRuntime();
        }
        else
        {
            // Пересоздаем каждый раз при открытии, чтобы обновить статусы ачивок!
            Destroy(achievementsPanel);
            CreateAchievementsUIRuntime();
        }
        achievementsPanel.SetActive(true);

        // Автоматически привязываем динамически созданные кнопки достижений к эффектам наведения
        Button[] allMenuButtons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (var btn in allMenuButtons)
        {
            if (btn.GetComponent<MenuButtonHoverEffects>() == null)
            {
                btn.gameObject.AddComponent<MenuButtonHoverEffects>();
            }
        }
    }

    public void HideAchievementsPanel()
    {
        if (uiAudioSource != null && buttonClickSound != null) uiAudioSource.PlayOneShot(buttonClickSound);
        if (achievementsPanel != null) achievementsPanel.SetActive(false);
    }

    private TMP_FontAsset FindFontInScene()
    {
#if UNITY_2023_1_OR_NEWER
        TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include);
#else
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
#endif
        foreach (var txt in allTexts)
        {
            if (txt != null && txt.font != null)
            {
                return txt.font;
            }
        }
        return null;
    }

    private void CreateAchievementsButton()
    {
        // Если кнопка уже добавлена вручную в редакторе, не создаем дубликат!
        if (GameObject.Find("AchievementsButton") != null) return;

        GameObject playBtn = GameObject.Find("PlayButton");
        GameObject quitBtn = GameObject.Find("QuitButton");
        if (playBtn == null || quitBtn == null) return;

        // Клонируем PlayButton для сохранения стиля, анимаций и размера
        GameObject achBtnObj = Instantiate(playBtn, playBtn.transform.parent);
        achBtnObj.name = "AchievementsButton";

        RectTransform playRt = playBtn.GetComponent<RectTransform>();
        RectTransform quitRt = quitBtn.GetComponent<RectTransform>();
        RectTransform achRt = achBtnObj.GetComponent<RectTransform>();

        // Задаем Sibling Index, чтобы кнопка логически стояла перед кнопкой Выхода
        achBtnObj.transform.SetSiblingIndex(quitBtn.transform.GetSiblingIndex());

        // Распределяем место, если кнопки расположены вручную (без VerticalLayoutGroup)
        var layout = playBtn.transform.parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (layout == null)
        {
            Vector3 originalQuitPos = quitRt.anchoredPosition3D;
            // Сдвигаем кнопку Quit на 110 единиц вниз
            quitRt.anchoredPosition3D = originalQuitPos + new Vector3(0, -110f, 0);
            // Размещаем кнопку достижений на старом месте кнопки Quit
            achRt.anchoredPosition3D = originalQuitPos;
        }

        // Меняем текст кнопки
        TextMeshProUGUI txt = achBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.text = "ДОСТИЖЕНИЯ";
            TMP_FontAsset font = FindFontInScene();
            if (font != null) txt.font = font;
        }

        // Подключаем слушатель нажатия
        UnityEngine.UI.Button btn = achBtnObj.GetComponent<UnityEngine.UI.Button>();
        // Полностью очищаем скопированные из редактора события (чтобы кнопка не запускала Новую Игру!)
        btn.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        btn.onClick.AddListener(ShowAchievementsPanel);
    }

    private void CreateAchievementsUIRuntime()
    {
        Canvas canvas = FindMainSceneCanvas();
        if (canvas == null) return;

        // Создаем панель-фон
        achievementsPanel = new GameObject("RuntimeAchievementsPanel");
        achievementsPanel.transform.SetParent(canvas.transform, false);
        achievementsPanel.transform.SetAsLastSibling();
        
        UnityEngine.UI.Image bg = achievementsPanel.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.01f, 0.022f, 0.01f, 0.98f); // Темный зеленый CRT ретро-фон
        bg.raycastTarget = true;

        RectTransform rt = achievementsPanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        TMP_FontAsset activeFont = FindFontInScene();

        // 1. Заголовок
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(achievementsPanel.transform, false);
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "АРХИВ ДОСТИЖЕНИЙ ИНСПЕКТОРА";
        title.fontSize = 44;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(0f, 1f, 0f, 0.95f); // Светящийся зеленый
        title.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) title.font = activeFont;

        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 0.84f); titleRt.anchorMax = new Vector2(1, 0.98f);
        titleRt.offsetMin = Vector2.zero; titleRt.offsetMax = Vector2.zero;

        // Outline для свечения заголовка
        Outline titleOutline = titleObj.AddComponent<Outline>();
        titleOutline.effectColor = new Color(0f, 0.5f, 0f, 0.5f);
        titleOutline.effectDistance = new Vector2(2f, -2f);

        // 2. Кнопка закрытия ("НАЗАД")
        GameObject closeObj = new GameObject("CloseBtn");
        closeObj.transform.SetParent(achievementsPanel.transform, false);
        
        UnityEngine.UI.Image closeImg = closeObj.AddComponent<UnityEngine.UI.Image>();
        closeImg.color = new Color(0.35f, 0.1f, 0.1f);
        
        Outline closeOutline = closeObj.AddComponent<Outline>();
        closeOutline.effectColor = new Color(0.6f, 0.2f, 0.2f);
        closeOutline.effectDistance = new Vector2(2f, -2f);

        UnityEngine.UI.Button closeBtn = closeObj.AddComponent<UnityEngine.UI.Button>();
        closeBtn.onClick.AddListener(HideAchievementsPanel);

        RectTransform closeRt = closeObj.GetComponent<RectTransform>();
        closeRt.sizeDelta = new Vector2(300, 70);
        closeRt.anchorMin = new Vector2(0.5f, 0.08f); closeRt.anchorMax = new Vector2(0.5f, 0.08f);
        closeRt.anchoredPosition = new Vector2(0, 0);

        GameObject closeTxtObj = new GameObject("Text");
        closeTxtObj.transform.SetParent(closeObj.transform, false);
        TextMeshProUGUI closeTxt = closeTxtObj.AddComponent<TextMeshProUGUI>();
        closeTxt.text = "ВЕРНУТЬСЯ В МЕНЮ";
        closeTxt.color = Color.white;
        closeTxt.fontSize = 20;
        closeTxt.fontStyle = FontStyles.Bold;
        closeTxt.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) closeTxt.font = activeFont;

        RectTransform closeTxtRt = closeTxtObj.GetComponent<RectTransform>();
        closeTxtRt.anchorMin = Vector2.zero; closeTxtRt.anchorMax = Vector2.one;
        closeTxtRt.offsetMin = Vector2.zero; closeTxtRt.offsetMax = Vector2.zero;

        // 3. Создаем сетку из 12 ачивок (3 колонки, 4 строки)
        for (int i = 0; i < 12; i++)
        {
            int row = i / 3;
            int col = i % 3;
            bool unlocked = PlayerPrefs.GetInt("Ach_" + menuAchievements[i].id, 0) == 1;

            // Контейнер ячейки
            GameObject slotObj = new GameObject("AchSlot_" + i);
            slotObj.transform.SetParent(achievementsPanel.transform, false);

            RectTransform slotRt = slotObj.AddComponent<RectTransform>();
            slotRt.sizeDelta = new Vector2(530, 115);
            slotRt.anchorMin = new Vector2(0.5f, 0.5f);
            slotRt.anchorMax = new Vector2(0.5f, 0.5f);

            // 3 колонки: X positions = -550, 0, 550
            float posX = -550f + col * 550f;
            float posY = 175f - row * 135f;
            slotRt.anchoredPosition = new Vector2(posX, posY);

            // Фон ячейки (Зеленоватый если открыта, темно-серый если закрыта)
            UnityEngine.UI.Image slotBg = slotObj.AddComponent<UnityEngine.UI.Image>();
            slotBg.color = unlocked ? new Color(0f, 0.08f, 0f, 0.85f) : new Color(0.02f, 0.02f, 0.02f, 0.85f);

            Outline slotOutline = slotObj.AddComponent<Outline>();
            slotOutline.effectColor = unlocked ? new Color(0f, 1f, 0f, 0.65f) : new Color(0.18f, 0.18f, 0.18f, 0.6f);
            slotOutline.effectDistance = new Vector2(2f, -2f);

            // Иконка (Трофей/Замок)
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(slotObj.transform, false);
            TextMeshProUGUI iconTxt = iconObj.AddComponent<TextMeshProUGUI>();
            iconTxt.text = unlocked ? "[ ★ OK ]" : "[ 🔒 ЗАКРЫТО ]";
            iconTxt.fontSize = 15;
            iconTxt.fontStyle = FontStyles.Bold;
            iconTxt.color = unlocked ? new Color(0f, 1f, 0f, 1f) : new Color(0.4f, 0.4f, 0.4f, 1f);
            iconTxt.alignment = TextAlignmentOptions.Center;
            if (activeFont != null) iconTxt.font = activeFont;

            RectTransform iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0, 0); iconRt.anchorMax = new Vector2(0.24f, 1);
            iconRt.offsetMin = Vector2.zero; iconRt.offsetMax = Vector2.zero;

            // Контейнер с текстом
            GameObject detailsObj = new GameObject("Details");
            detailsObj.transform.SetParent(slotObj.transform, false);
            RectTransform detailsRt = detailsObj.AddComponent<RectTransform>();
            detailsRt.anchorMin = new Vector2(0.24f, 0); detailsRt.anchorMax = new Vector2(1, 1);
            detailsRt.offsetMin = Vector2.zero; detailsRt.offsetMax = Vector2.zero;

            // Название достижения
            GameObject tObj = new GameObject("Title");
            tObj.transform.SetParent(detailsObj.transform, false);
            TextMeshProUGUI tTxt = tObj.AddComponent<TextMeshProUGUI>();
            tTxt.text = menuAchievements[i].title;
            tTxt.fontSize = 18;
            tTxt.fontStyle = FontStyles.Bold;
            tTxt.color = unlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);
            tTxt.alignment = TextAlignmentOptions.Left;
            if (activeFont != null) tTxt.font = activeFont;

            RectTransform tRt = tObj.GetComponent<RectTransform>();
            tRt.anchorMin = new Vector2(0, 0.5f); tRt.anchorMax = new Vector2(1, 0.95f);
            tRt.offsetMin = Vector2.zero; tRt.offsetMax = Vector2.zero;

            // Описание достижения
            GameObject dObj = new GameObject("Desc");
            dObj.transform.SetParent(detailsObj.transform, false);
            TextMeshProUGUI dTxt = dObj.AddComponent<TextMeshProUGUI>();
            dTxt.text = menuAchievements[i].description;
            dTxt.fontSize = 13;
            dTxt.color = unlocked ? new Color(0.7f, 0.9f, 0.7f, 0.9f) : new Color(0.3f, 0.3f, 0.3f, 0.9f);
            dTxt.alignment = TextAlignmentOptions.Left;
            if (activeFont != null) dTxt.font = activeFont;

            RectTransform dRt = dObj.GetComponent<RectTransform>();
            dRt.anchorMin = new Vector2(0, 0.05f); dRt.anchorMax = new Vector2(1, 0.5f);
            dRt.offsetMin = Vector2.zero; dRt.offsetMax = Vector2.zero;
        }
    }

    private void DestroyPersistentUI()
    {
#if UNITY_2023_1_OR_NEWER
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
#else
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
#endif
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.scene.name != "MainMenu" && obj.scene.name != null)
            {
                string n = obj.name.ToLower();
                if (n.Contains("victory") || n.Contains("gameover") || n.Contains("pausepanel") || n.Contains("gamecanvas") || n.Contains("canvas(clone)"))
                {
                    Destroy(obj);
                }
            }
        }
    }

    private Canvas FindMainSceneCanvas()
    {
        Canvas canvas = null;
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include);
        foreach (var c in allCanvases)
        {
            if (c != null && c.gameObject.scene.name == gameObject.scene.name && (c.name == "Canvas" || c.name == "GameCanvas"))
            {
                canvas = c;
                break;
            }
        }
        if (canvas == null)
        {
            foreach (var c in allCanvases)
            {
                if (c != null && c.gameObject.scene.name == gameObject.scene.name)
                {
                    canvas = c;
                    break;
                }
            }
        }
        if (canvas == null)
        {
            foreach (var c in allCanvases)
            {
                if (c != null && c.name != "AchievementsCanvas")
                {
                    canvas = c;
                    break;
                }
            }
        }
        if (canvas == null)
        {
            canvas = Object.FindAnyObjectByType<Canvas>();
        }
        return canvas;
    }
}
