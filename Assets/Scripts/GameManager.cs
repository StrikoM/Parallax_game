using System.Collections;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("База данных уровней")]
    public ShiftData[] shiftsDatabase; 
    private ShiftData currentShift;
    private int currentShiftIndex = 0;
    private int currentVisitorIndex = 0;

    [Header("Интерфейс (UI)")]
    public Image visitorImageDisplay;
    public TextMeshProUGUI directiveTextDisplay;
    public DatabaseFolderUI databaseFolder; // Папка с базой данных

    [Header("Новая Анимация (Стиль Окна)")]
    public RectTransform windowShutter; // Металлическая шторка на окне (если есть)
    public RectTransform documentTray;  // Лоток/папка с документами (выезжает снизу)
    public RectTransform guardLeft;     // Охранник слева
    public RectTransform guardRight;    // Охранник справа
    [Header("Спрайты Охранников (Анимации)")]
    public Sprite guardStandingSprite;  // Стоит / Говорит
    public Sprite guardWalkingSprite;   // Идет
    public Sprite guardHoldingSprite;   // Держит монстра (изоляция)
    
    private Vector3 originalShutterPos;
    private Vector3 originalTrayPos;
    private Vector3 guardLeftStartPos;
    private Vector3 guardRightStartPos;
    private Vector3 originalGuardRightScale = Vector3.one;
    
    [Header("Досье (База Данных)")]
    public TextMeshProUGUI dossierNameText;
    public TextMeshProUGUI dossierLastNameText;
    public TextMeshProUGUI dossierIdText;
    public TextMeshProUGUI dossierEyesText;
    public Image dossierPhotoDisplay;

    [Header("Паспорт (Документ)")]
    public TextMeshProUGUI passportNameText;
    public TextMeshProUGUI passportLastNameText;
    public TextMeshProUGUI passportIdText;
    public TextMeshProUGUI passportEyesText;
    public TextMeshProUGUI passportExpDateText; // Новое поле для даты
    
    [Header("Правила игры")]
    public TextMeshProUGUI strikesTextDisplay; // Теперь это штрафы, а не лояльность
    public TextMeshProUGUI quotaTextDisplay;   // Очередь вместо таймера
    public int strikes = 0;                    // 3 штрафа = увольнение
    private bool isShiftActive = true;
    private bool isAnimating = false;          // Блокировка кнопок во время анимации
    private Sprite defaultGuardSprite;         // Обычная картинка охранника

    [Header("Экраны завершения")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryStatsText;

    [Header("Русские Штампы Конца Игры")]
    public Sprite stampApprovedRu;
    public Sprite stampFiredRu;
    public Sprite clipboardApprovedSprite;
    public Sprite clipboardFiredSprite;

    [Header("Система Печатей")]
    public GameObject stampObject;
    public TextMeshProUGUI stampText;
    public UnityEngine.UI.Outline stampOutline;

    [Header("Диалоги (Аниме стиль)")]
    public GameObject dialoguePanel;
    public Image dialoguePortrait;
    public Sprite dispatcherSprite; // Спрайт для Службы безопасности (Диспетчера)
    public TextMeshProUGUI dialogueNameText;
    public TextMeshProUGUI dialogueContentText;

    [Header("Звуки")]
    public AudioSource sfxAudioSource;     // Источник звуков (SFX)
    public AudioClip shutterCloseSound;    // Звук падения шторки
    public AudioClip shutterOpenSound;     // Звук открытия шторки (необязательно)

    [Header("Пауза и Меню")]
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button exitButton;
    private bool isPaused = false;
    private float lastToggleTime = -1f;
    private bool loggedPausePanelNull = false;
    private GameObject pauseTapePlayerPanel;

    [Header("Кнопка Допроса")]
    public Button interrogateBtn;
    public GameObject questionsPanel; // Панель с вариантами вопросов
    
    [Header("Газета смены")]
    public GameObject newspaperPanel;
    public TMPro.TextMeshProUGUI newspaperHeadlineText;
    public TMPro.TextMeshProUGUI newspaperBodyText;

    [Header("Настройки")]
    public GameObject settingsPanel;
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;


    [Header("Телефон (Сюжет)")]
    public Button phoneButton;
    public string[] shiftPhoneMessages; 
    public AudioClip phoneRingSound;
    public AudioClip phonePickupSound;
    private bool isPhoneRinging = false;
    private bool phoneAnswered = false;

    [Header("Защита (Шокер)")]
    public GameObject glassCracksOverlay;
    public GameObject stunGunDrawer;
    public Button stunGunButton;
    public Image screenFlashOverlay;
    public int maxStunCharges = 1;
    private int currentStunCharges = 1;

    [Header("Кровь и Тряпка")]
    public CanvasGroup bloodOverlay;
    public RectTransform ragCursor;
    private bool isBloodOnGlass = false;
    private float cleaningProgress = 0f;
    private Vector2 lastMousePos;
    private bool isBloodNext = false;

    [Header("Дезинфекция")]
    public CanvasGroup deconGasOverlay;
    public AudioClip deconGasSound;

    [Header("Монстры: Трейты")]
    public AudioClip glassKnockSound;
    private bool isVisitorWaiting = false;
    private float visitorTimer = 0f;
    private int knockStage = 0;

    // Данные для анимации
    private Vector3 originalVisitorPos;

    void Start()
    {
        // Создание текста для охранника удалено, так как теперь он использует общую панель диалогов.

        // Читаем из сохранений, на каком мы дне
        currentShiftIndex = PlayerPrefs.GetInt("CurrentShift", 0);
        currentStunCharges = maxStunCharges;

        // Прячем экраны при старте
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Предварительно стилизуем экраны при старте
        StylizeEndScreensRuntime();
        CreateRulesButtonRuntime();

        // Гарантируем, что оверлеи не будут блокировать мышь на протяжении всей игры
        DisableRaycastOnOverlay("DecontaminationGas");
        DisableRaycastOnOverlay("BloodOverlay");
        DisableRaycastOnOverlay("GlassCracks");
        DisableRaycastOnOverlay("ScreenFlash");
        DisableRaycastOnOverlay("CRT_Overlay_Safe");
        DisableRaycastOnOverlay("GlobalDarkness");

        if (shiftsDatabase == null || shiftsDatabase.Length == 0)
        {
            Debug.LogError("База Смен пуста! Добавьте смены в GameManager.");
            return;
        }

        if (currentShiftIndex >= shiftsDatabase.Length)
        {
            // Игрок прошел все уровни! 
            // Чтобы он не застревал на экране победы навсегда, сбросим прогресс
            // и дадим возможность играть снова с 1-й смены!
            PlayerPrefs.SetInt("CurrentShift", 0);
            PlayerPrefs.Save();
            currentShiftIndex = 0;
            Debug.Log("[GameManager] Прогресс сброшен на 1 смену для повторной игры.");
        }

        currentShift = shiftsDatabase[currentShiftIndex];
        if (currentShift == null)
        {
            Debug.LogError("ОШИБКА: Смена " + currentShiftIndex + " не назначена в GameManager (пустое поле)! Пожалуйста, перетащите файл смены в массив Shifts Database.");
            return;
        }
        
        if (directiveTextDisplay != null) directiveTextDisplay.text = "ЗАМЕТКА УПРАВДОМА:\n" + currentShift.directiveText;

        // Загружаем граждан в папку
        if (databaseFolder != null)
            databaseFolder.LoadShiftCitizens(currentShift.shiftVisitors);
        Debug.Log("Загружена: " + currentShift.shiftName + ". Директива: " + currentShift.directiveText);

        if (visitorImageDisplay != null)
        {
            originalVisitorPos = visitorImageDisplay.rectTransform.anchoredPosition;
            // Делаем посетителя всегда полностью видимым по прозрачности
            Color c = visitorImageDisplay.color;
            c.a = 1f;
            visitorImageDisplay.color = c;
        }
        
        if (windowShutter != null) originalShutterPos = windowShutter.anchoredPosition;
        if (documentTray != null) originalTrayPos = documentTray.anchoredPosition;
        if (guardLeft != null) 
        {
            guardLeftStartPos = guardLeft.anchoredPosition;
            defaultGuardSprite = guardStandingSprite != null ? guardStandingSprite : guardLeft.GetComponent<Image>().sprite;
            if (guardStandingSprite != null) guardLeft.GetComponent<Image>().sprite = guardStandingSprite;
        }
        if (guardRight != null) 
        {
            guardRightStartPos = guardRight.anchoredPosition;
            originalGuardRightScale = guardRight.localScale;
            if (guardStandingSprite != null) guardRight.GetComponent<Image>().sprite = guardStandingSprite;
        }

        UpdateUI();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        
        // Показываем газету, если она настроена для смены
        if (currentShift != null && currentShift.hasNewspaper && newspaperPanel != null)
        {
            if (newspaperHeadlineText != null) newspaperHeadlineText.text = currentShift.newspaperHeadline;
            if (newspaperBodyText != null) newspaperBodyText.text = currentShift.newspaperBody;
            newspaperPanel.SetActive(true);
            newspaperPanel.transform.SetAsLastSibling(); // Гарантируем, что газета будет поверх монитора!
        }
        else
        {
            StartShiftAfterNewspaper();
        }
    }

    public void StartShiftAfterNewspaper()
    {
        if (newspaperPanel != null) newspaperPanel.SetActive(false);
        
        // Проверяем, есть ли сообщение от босса для этой смены
        if (shiftPhoneMessages != null && currentShiftIndex < shiftPhoneMessages.Length && !string.IsNullOrEmpty(shiftPhoneMessages[currentShiftIndex]))
        {
            StartCoroutine(PhoneRingRoutine());
        }
        else
        {
            ShowVisitor(currentVisitorIndex);
        }
    }

    void UpdateUI()
    {
        // Если у нас в инспекторе все еще висит старый текст лояльности/таймера,
        // мы можем временно использовать их под новые нужды.
        if (strikesTextDisplay != null)
            strikesTextDisplay.text = "Штрафы: " + strikes + "/3";
            
        if (quotaTextDisplay != null && currentShift != null)
        {
            int left = currentShift.shiftVisitors.Length - currentVisitorIndex;
            quotaTextDisplay.text = "Очередь: " + Mathf.Max(0, left);
        }
    }

    void ShowVisitor(int index)
    {
        GameObject isolateBtn = GameObject.Find("EmergencyIsolateBtn");
        if (isolateBtn != null)
        {
            Image img = isolateBtn.GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }

        if (currentShift == null || currentShift.shiftVisitors.Length == 0) return;

        if (index >= currentShift.shiftVisitors.Length)
        {
            // Прошли уровень успешно! Сохраняем прогресс (переход на следующий день)
            PlayerPrefs.SetInt("CurrentShift", currentShiftIndex + 1);
            PlayerPrefs.Save();
            
            if (quotaTextDisplay != null) quotaTextDisplay.text = "Очередь: 0";
            
            EndShift("Смена окончена! Все проверены.");
            return;
        }

        VisitorData currentVisitor = currentShift.shiftVisitors[index];

        if (visitorImageDisplay != null)
        {
            visitorImageDisplay.sprite = currentVisitor.visitorSprite;
            visitorImageDisplay.color = new Color(1f, 1f, 1f, 1f); // Возвращаем видимость!
        }
        
        // Убираем Досье (2-я информация), чтобы не дублировать
        if (dossierNameText != null) dossierNameText.text = "";
        if (dossierLastNameText != null) dossierLastNameText.text = "";
        if (dossierIdText != null) dossierIdText.text = "";
        if (dossierEyesText != null) dossierEyesText.text = "";
        if (dossierPhotoDisplay != null) 
        {
            dossierPhotoDisplay.sprite = currentVisitor.dossierSprite;
            dossierPhotoDisplay.color = currentVisitor.dossierSprite == null ? new Color(1,1,1,0) : Color.white;
        }

        // Заполняем только Паспорт (ИМЯ и ФАМИЛИЯ разделены)
        string pName = currentVisitor.passportName;
        string pFirst = pName;
        string pLast = "";
        if (!string.IsNullOrEmpty(pName))
        {
            int spaceIdx = pName.IndexOf(' ');
            if (spaceIdx > 0)
            {
                pFirst = pName.Substring(0, spaceIdx);
                pLast = pName.Substring(spaceIdx + 1);
            }
        }
        
        if (passportNameText != null) passportNameText.text = pFirst;
        if (passportLastNameText != null) passportLastNameText.text = pLast;
        if (passportIdText != null) passportIdText.text = "ID:\n" + currentVisitor.passportId;
        
        // Показываем глаза только если это Смена 2 или выше (Индекс 1+)
        if (passportEyesText != null) 
        {
            if (currentShiftIndex >= 1) passportEyesText.text = currentVisitor.passportEyes;
            else passportEyesText.text = ""; // Прячем для первой смены
        }
        
        // Показываем срок действия всегда на бумажке
        if (passportExpDateText != null) 
        {
            string exp = currentVisitor.passportExpDate;
            if (string.IsNullOrEmpty(exp)) exp = "12.2084"; // Заглушка, если дата не указана в файле
            passportExpDateText.text = exp;
        }
        
        // В новом стиле посетитель не выезжает слева. Он просто стоит на месте за закрытой шторкой.
        if (visitorImageDisplay != null)
        {
            visitorImageDisplay.rectTransform.anchoredPosition = originalVisitorPos;
        }
        
        UpdateUI();
        
        if (isBloodNext)
        {
            isBloodNext = false;
            isBloodOnGlass = true;
            cleaningProgress = 0f;
            if (bloodOverlay != null)
            {
                bloodOverlay.gameObject.SetActive(true);
                bloodOverlay.alpha = 1f;
            }
            lastMousePos = Input.mousePosition;
        }
        
        // Запускаем анимацию входа
        StartCoroutine(AnimateVisitorWalkIn());
    }

    void Update()
    {
        if (pausePanel == null && !loggedPausePanelNull)
        {
            loggedPausePanelNull = true;
            Debug.LogError("[GameManager] [" + GetHashCode() + "] Update: pausePanel стал NULL!");
            
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            string foundObjects = "";
            foreach (var obj in allObjects)
            {
                if (obj != null && obj.name.ToLower().Contains("pause"))
                {
                    foundObjects += $"{obj.name} (Active: {obj.activeInHierarchy}, Scene: {obj.scene.name}), ";
                }
            }
            Debug.LogError("[GameManager] Найденные объекты со словом 'pause': " + foundObjects);
        }

        // Синхронизируем громкость эффектов в реальном времени
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        }

        // Поддержка нажатия Escape для паузы
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Чит-код F10 для быстрого завершения смены и тестирования скриншотов диплома
        if (Input.GetKeyDown(KeyCode.F10))
        {
            EndShift("Смена завершена чит-кодом (F10)");
        }

        VisitorData currentVisitor = (isShiftActive && currentShift != null && currentVisitorIndex < currentShift.shiftVisitors.Length) 
            ? currentShift.shiftVisitors[currentVisitorIndex] : null;

        // Анимация дыхания (когда персонаж стоит на месте)
        if (visitorImageDisplay != null && isShiftActive && !isAnimating && currentVisitor != null)
        {
            float breathSpeed = currentVisitor.isMimic ? 8f : 2f;
            float breathAmp = currentVisitor.isMimic ? 0.05f : 0.015f;
            float breathScale = 1f + Mathf.Sin(Time.time * breathSpeed) * breathAmp;
            visitorImageDisplay.transform.localScale = new Vector3(1f, breathScale, 1f);
        }
        else if (visitorImageDisplay != null)
        {
            visitorImageDisplay.transform.localScale = Vector3.one;
        }

        if (isVisitorWaiting && !isAnimating && !isPaused && isShiftActive && currentVisitor != null)
        {
            visitorTimer += Time.deltaTime;
            
            if (currentVisitor.isImpatient)
            {
                if (visitorTimer > 10f && knockStage == 0)
                {
                    knockStage = 1;
                    StartCoroutine(KnockRoutine(1));
                }
                else if (visitorTimer > 18f && knockStage == 1)
                {
                    knockStage = 2;
                    StartCoroutine(KnockRoutine(2));
                }
                else if (visitorTimer > 25f && knockStage == 2)
                {
                    knockStage = 3;
                    StartCoroutine(KnockRoutine(3));
                    if (glassCracksOverlay != null) glassCracksOverlay.SetActive(true); // Появляются трещины
                }
                else if (visitorTimer > 30f && knockStage == 3)
                {
                    knockStage = 4;
                    isVisitorWaiting = false;
                    // Автоматическая смерть! Монстр пробивает стекло из-за долгого ожидания
                    StartCoroutine(MonsterAttackRoutine()); 
                }
            }
        }

        // Логика протирания стекла от крови
        if (isBloodOnGlass)
        {
            if (Input.GetMouseButton(0))
            {
                if (ragCursor != null)
                {
                    ragCursor.gameObject.SetActive(true);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(ragCursor.parent as RectTransform, Input.mousePosition, null, out Vector2 localPoint);
                    ragCursor.localPosition = localPoint;
                }

                float delta = Vector2.Distance(Input.mousePosition, lastMousePos);
                cleaningProgress += delta * 0.0003f; // Скорость стирания
                
                if (bloodOverlay != null)
                {
                    bloodOverlay.alpha = 1f - cleaningProgress;
                }

                if (cleaningProgress >= 1f)
                {
                    isBloodOnGlass = false;
                    if (bloodOverlay != null) bloodOverlay.gameObject.SetActive(false);
                    if (ragCursor != null) ragCursor.gameObject.SetActive(false);
                    
                    // ДОСТИЖЕНИЕ: КРОВАВАЯ РАБОТА
                    if (AchievementsManager.instance != null)
                    {
                        AchievementsManager.instance.UnlockAchievement("CLEAN_WINDOW");
                    }
                }
            }
            else
            {
                // Возвращаем тряпку на стол, если отпустили кнопку
                if (ragCursor != null) 
                {
                    ragCursor.gameObject.SetActive(true);
                    ragCursor.anchoredPosition = new Vector2(250, 100); // Позиция на столе
                }
            }
            lastMousePos = Input.mousePosition;
        }
    }

    private IEnumerator KnockRoutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (sfxAudioSource != null && glassKnockSound != null) sfxAudioSource.PlayOneShot(glassKnockSound);
            
            if (visitorImageDisplay != null)
            {
                Vector3 origPos = originalVisitorPos;
                visitorImageDisplay.rectTransform.anchoredPosition = origPos + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
                yield return new WaitForSeconds(0.1f);
                visitorImageDisplay.rectTransform.anchoredPosition = origPos;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    void GameOver(string reason)
    {
        isShiftActive = false;
        Debug.LogError("ИГРА ОКОНЧЕНА! Причина: " + reason);
        
        // Воспроизводим ретро-звук при увольнении (тяжелый грохот)
        if (sfxAudioSource != null && shutterCloseSound != null)
        {
            sfxAudioSource.PlayOneShot(shutterCloseSound);
        }

        // Прячем кнопку паузы, чтобы она не перекрывала экран поражения
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);

        // Прячем кнопку правил, чтобы она не перекрывала экран поражения
        if (rulesButtonObj != null) rulesButtonObj.SetActive(false);

        // Применяем динамическую стилизацию панелей
        StylizeEndScreensRuntime();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling(); // Гарантируем, что панель поражения будет поверх монитора!
            if (gameOverReasonText != null)
            {
                gameOverReasonText.text = reason; // Возвращаем чистый простой текст
            }
        }
    }

    void EndShift(string message)
    {
        isShiftActive = false;
        Debug.Log("ПОБЕДА! " + message + " Ошибок: " + strikes);

        // ДОСТИЖЕНИЯ
        if (AchievementsManager.instance != null)
        {
            if (currentShiftIndex == 0)
            {
                AchievementsManager.instance.UnlockAchievement("FIRST_SHIFT");
            }
            if (strikes == 0)
            {
                AchievementsManager.instance.UnlockAchievement("PERFECT_SHIFT");
            }
            if (currentShiftIndex == 6) // Финальная 7-я смена
            {
                AchievementsManager.instance.UnlockAchievement("IRON_SHIELD");
            }
            if (strikes == 2) // Пройдено с ровно двумя ошибками
            {
                AchievementsManager.instance.UnlockAchievement("CLOSE_CALL");
            }
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            victoryPanel.transform.SetAsLastSibling(); // Гарантируем, что панель победы будет поверх монитора!
            
            // Воспроизводим ретро-звук при победе (фанфары!)
            if (sfxAudioSource != null)
            {
                AudioClip fanfare = CreateVictoryFanfare();
                sfxAudioSource.PlayOneShot(fanfare, 0.8f);
            }

            // Прячем кнопку паузы, чтобы она не перекрывала экран победы
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);

            // Прячем кнопку правил, чтобы она не перекрывала экран победы
            if (rulesButtonObj != null) rulesButtonObj.SetActive(false);

            // Применяем динамическую стилизацию панелей (перед скрытием кнопки NextShiftBtn, если она выключается!)
            StylizeEndScreensRuntime();

            if (victoryStatsText != null) 
            {
                                if (currentShiftIndex + 1 >= shiftsDatabase.Length)
                {
                    victoryStatsText.text = "ВЫ ПРОШЛИ ИГРУ!\n\nВсе смены завершены.\nМонстры не прошли.";
                    
                    // Поменяем заголовок на "ИГРА ПРОЙДЕНА"
                    Transform titleTrans = victoryPanel.transform.Find("Clipboard/TitleText");
                    if (titleTrans != null)
                    {
                        TextMeshProUGUI titleTmp = titleTrans.GetComponent<TextMeshProUGUI>();
                        if (titleTmp != null) titleTmp.text = "ИГРА ПРОЙДЕНА";
                    }

                    // Прячем кнопку "Продолжить смену" (проверяем оба варианта имени кнопки)
                    Button[] btns = victoryPanel.GetComponentsInChildren<Button>(true);
                    foreach(var b in btns) {
                        if(b.name == "NextShiftBtn" || b.name == "ContinueShiftBtn") b.gameObject.SetActive(false);
                    }

                    // Перестраиваем кнопки после сокрытия NextShiftBtn, чтобы кнопка ExitMenu была в центре
                    StylizeEndScreensRuntime();

                    // Запускаем секретную финальную анимацию поздравления!
                    StartCoroutine(FinalVictorySequence(victoryPanel));
                }
                else
                {
                    victoryStatsText.text = "СМЕНА ОКОНЧЕНА\n\nОшибок: " + strikes + "/3\nЖильцы дома в безопасности.";
                }
            }
        }
    }

    public void OnApproveClicked()
    {
        if (!isShiftActive || isAnimating || currentShift == null) return; 

        VisitorData currentVisitor = currentShift.shiftVisitors[currentVisitorIndex];
        
        if (currentVisitor.isMonster == true)
        {
            // Очень редкий шанс (20%), что монстр решит напасть на стекло
            if (Random.value < 0.20f)
            {
                // Нападение! (Нужен шокер)
                StartCoroutine(StampRoutine(true, false, "", true));
            }
            else
            {
                // ДОСТИЖЕНИЕ: ДОБРО ПОЖАЛОВАТЬ ДОМОЙ
                if (AchievementsManager.instance != null)
                {
                    AchievementsManager.instance.UnlockAchievement("WELCOME_HOME");
                }

                // В 80% случаев монстр просто проходит внутрь. Мы мгновенно проигрываем с камерным числом жертв!
                int victims = Random.Range(5, 45);
                StartCoroutine(StampRoutine(true, true, $"Вы впустили монстра в здание. Число погибших жильцов: {victims} человек. Жильцы мертвы. ИГРА ОКОНЧЕНА.", false));
            }
            return;
        }

        StartCoroutine(StampRoutine(true, false, "", false));
    }

    public void OnRejectClicked()
    {
        if (!isShiftActive || isAnimating || currentShift == null) return; 

        GameObject isolateBtn = GameObject.Find("EmergencyIsolateBtn");
        if (isolateBtn != null)
        {
            Image img = isolateBtn.GetComponent<Image>();
            if (img != null) img.color = new Color(0.9f, 0.1f, 0.1f, 1f); // Красный цвет
        }

        VisitorData currentVisitor = currentShift.shiftVisitors[currentVisitorIndex];
        
        if (currentVisitor.isMonster == false)
        {
            strikes++;
            UpdateUI();
            
            // ДОСТИЖЕНИЕ: ПАНИКЁР
            if (AchievementsManager.instance != null)
            {
                AchievementsManager.instance.UnlockAchievement("PANIC");
            }
            
            if (strikes >= 3) { 
                StartCoroutine(StampRoutine(false, true, "Слишком много ложных обвинений. Вы уволены.", false));
                return; 
            }
        }
        else
        {
            // Мы успешно изолировали монстра! Готовим кровь на стекло
            isBloodNext = true;

            // ДОСТИЖЕНИЯ: КИБОРГ-УБИЙЦА и ШЕРЛОК БЛОКПОСТА
            if (AchievementsManager.instance != null)
            {
                bool isVisualAnomaly = currentVisitor.dossierName == currentVisitor.passportName && 
                                       currentVisitor.dossierId == currentVisitor.passportId && 
                                       currentVisitor.dossierEyes == currentVisitor.passportEyes && 
                                       currentVisitor.dossierExpDate == currentVisitor.passportExpDate;
                                       
                if (isVisualAnomaly)
                {
                    AchievementsManager.instance.UnlockAchievement("CYBORG");
                }
                else
                {
                    AchievementsManager.instance.UnlockAchievement("SHERLOCK");
                }
            }
        }

        StartCoroutine(StampRoutine(false, false, "", false));
    }

    private IEnumerator StampRoutine(bool isApprove, bool isGameOver, string gameOverReason, bool isMonsterAttack)
    {
        isAnimating = true;
        isVisitorWaiting = false;

        // Прячем старую экранную печать и пропускаем ее анимацию, так как у нас есть реальные тактильные 2D-штампы
        if (stampObject != null) stampObject.SetActive(false);
        
        // Небольшая пауза, чтобы игрок мог увидеть печать на паспорте перед тем, как тот уедет
        yield return new WaitForSeconds(0.8f);

        if (isMonsterAttack)
        {
            StartCoroutine(MonsterAttackRoutine());
            yield break;
        }

        if (isGameOver)
        {
            if (isApprove)
            {
                yield return StartCoroutine(AnimateApproveBeforeGameOver(gameOverReason));
            }
            else
            {
                yield return StartCoroutine(AnimateRejectBeforeGameOver(gameOverReason));
            }
            yield break;
        }

        // После печати запускаем стандартную анимацию ухода
        if (isApprove)
        {
            StartCoroutine(AnimateApproveAndLoadNext());
        }
        else
        {
            StartCoroutine(AnimateRejectAndLoadNext());
        }
    }

    private IEnumerator AnimateApproveBeforeGameOver(string reason)
    {
        isAnimating = true;
        lastActionWasApprove = true;
        
        float duration = 1.0f;
        float elapsed = 0f;
        
        Vector3 trayOpenPos = originalTrayPos;
        Vector3 trayClosedPos = originalTrayPos + new Vector3(0f, -600f, 0f);
        
        Vector3 visitorStart = originalVisitorPos;
        Vector3 visitorTarget = originalVisitorPos + new Vector3(800f, 0f, 0f);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (visitorImageDisplay != null) visitorImageDisplay.rectTransform.anchoredPosition = Vector3.Lerp(visitorStart, visitorTarget, t);
            if (documentTray != null) documentTray.anchoredPosition = Vector3.Lerp(trayOpenPos, trayClosedPos, t);
            
            yield return null;
        }
        
        GameOver(reason);
    }

    private IEnumerator AnimateRejectBeforeGameOver(string reason)
    {
        isAnimating = true;
        lastActionWasApprove = false;
        
        float duration = 1.0f;
        float elapsed = 0f;
        
        Vector3 shutterOpenPos = originalShutterPos + new Vector3(0f, 800f, 0f);
        Vector3 shutterClosedPos = originalShutterPos; 
        
        Vector3 trayOpenPos = originalTrayPos;
        Vector3 trayClosedPos = originalTrayPos + new Vector3(0f, -600f, 0f);
        
        Vector3 guardLeftTarget = guardLeftStartPos + new Vector3(400f, 0f, 0f);
        Vector3 guardRightTarget = guardRightStartPos + new Vector3(-400f, 0f, 0f);
        
        if (guardLeft != null && guardWalkingSprite != null) guardLeft.GetComponent<Image>().sprite = guardWalkingSprite;
        if (guardRight != null && guardWalkingSprite != null) 
        {
            guardRight.GetComponent<Image>().sprite = guardWalkingSprite;
            guardRight.localScale = new Vector3(-Mathf.Abs(originalGuardRightScale.x), originalGuardRightScale.y, originalGuardRightScale.z);
        }

        float guardDuration = 0.8f;
        while (elapsed < guardDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / guardDuration; 
            
            if (documentTray != null) documentTray.anchoredPosition = Vector3.Lerp(trayOpenPos, trayClosedPos, t);
            if (guardLeft != null) guardLeft.anchoredPosition = Vector3.Lerp(guardLeftStartPos, guardLeftTarget, t);
            if (guardRight != null) guardRight.anchoredPosition = Vector3.Lerp(guardRightStartPos, guardRightTarget, t);
            
            yield return null; 
        }

        if (guardLeft != null && guardHoldingSprite != null) guardLeft.GetComponent<Image>().sprite = guardHoldingSprite;
        if (guardRight != null && guardHoldingSprite != null) guardRight.GetComponent<Image>().sprite = guardHoldingSprite;

        elapsed = 0f;
        if (sfxAudioSource != null && shutterCloseSound != null)
        {
            sfxAudioSource.PlayOneShot(shutterCloseSound);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; 
            
            if (windowShutter != null) windowShutter.anchoredPosition = Vector3.Lerp(shutterOpenPos, shutterClosedPos, t);
            
            yield return null; 
        }

        yield return new WaitForSeconds(1.5f);
        
        if (guardLeft != null) 
        {
            guardLeft.anchoredPosition = guardLeftStartPos;
            if (defaultGuardSprite != null) guardLeft.GetComponent<Image>().sprite = defaultGuardSprite;
        }
        if (guardRight != null) 
        {
            guardRight.anchoredPosition = guardRightStartPos;
            guardRight.localScale = originalGuardRightScale;
            if (defaultGuardSprite != null) guardRight.GetComponent<Image>().sprite = defaultGuardSprite;
        }

        GameOver(reason);
    }

    // Флаг, чтобы знать, как вводить следующего посетителя
    private bool lastActionWasApprove = true;

    // Анимация ПРОПУСКА (человек уходит вправо)
    private IEnumerator AnimateApproveAndLoadNext()
    {
        isAnimating = true;
        lastActionWasApprove = true;
        
        float duration = 1.0f; // Увеличил время, чтобы он уходил медленнее
        float elapsed = 0f;
        
        Vector3 trayOpenPos = originalTrayPos;
        Vector3 trayClosedPos = originalTrayPos + new Vector3(0f, -600f, 0f);
        
        Vector3 visitorStart = originalVisitorPos;
        Vector3 visitorTarget = originalVisitorPos + new Vector3(800f, 0f, 0f); // Уходит вправо

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; 
            
            if (visitorImageDisplay != null) visitorImageDisplay.rectTransform.anchoredPosition = Vector3.Lerp(visitorStart, visitorTarget, t);
            if (documentTray != null) documentTray.anchoredPosition = Vector3.Lerp(trayOpenPos, trayClosedPos, t);
            
            yield return null; 
        }

        currentVisitorIndex++;
        ShowVisitor(currentVisitorIndex);
    }

    // Анимация ИЗОЛЯЦИИ (закрывается железная шторка)
    private IEnumerator AnimateRejectAndLoadNext()
    {
        isAnimating = true;
        lastActionWasApprove = false;
        
        float duration = 1.0f; // Шторка падает медленнее, создавая ощущение тяжести
        float elapsed = 0f;
        
        Vector3 shutterOpenPos = originalShutterPos + new Vector3(0f, 800f, 0f);
        Vector3 shutterClosedPos = originalShutterPos; 
        
        Vector3 trayOpenPos = originalTrayPos;
        Vector3 trayClosedPos = originalTrayPos + new Vector3(0f, -600f, 0f);
        
        // 1. Сначала убираем лоток и выводим охранников
        Vector3 guardLeftTarget = guardLeftStartPos + new Vector3(400f, 0f, 0f); // Едет дальше вправо (для широкого окна)
        Vector3 guardRightTarget = guardRightStartPos + new Vector3(-400f, 0f, 0f); // Едет дальше влево
        
        if (guardLeft != null && guardWalkingSprite != null) guardLeft.GetComponent<Image>().sprite = guardWalkingSprite;
        if (guardRight != null && guardWalkingSprite != null) 
        {
            guardRight.GetComponent<Image>().sprite = guardWalkingSprite;
            guardRight.localScale = new Vector3(-Mathf.Abs(originalGuardRightScale.x), originalGuardRightScale.y, originalGuardRightScale.z); // Поворачиваем налево
        }

        float guardDuration = 0.8f;
        while (elapsed < guardDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / guardDuration; 
            
            if (documentTray != null) documentTray.anchoredPosition = Vector3.Lerp(trayOpenPos, trayClosedPos, t);
            if (guardLeft != null) guardLeft.anchoredPosition = Vector3.Lerp(guardLeftStartPos, guardLeftTarget, t);
            if (guardRight != null) guardRight.anchoredPosition = Vector3.Lerp(guardRightStartPos, guardRightTarget, t);
            
            yield return null; 
        }

        // Схватили монстра! Меняем на фото "Держит"
        if (guardLeft != null && guardHoldingSprite != null) guardLeft.GetComponent<Image>().sprite = guardHoldingSprite;
        if (guardRight != null && guardHoldingSprite != null) guardRight.GetComponent<Image>().sprite = guardHoldingSprite;

        // 2. Затем падает шторка
        elapsed = 0f;
        
        // Воспроизводим звук падения железной двери!
        if (sfxAudioSource != null && shutterCloseSound != null)
        {
            sfxAudioSource.PlayOneShot(shutterCloseSound);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; 
            
            if (windowShutter != null) windowShutter.anchoredPosition = Vector3.Lerp(shutterOpenPos, shutterClosedPos, t);
            
            yield return null; 
        }

        // Шторка упала. Делаем паузу, чтобы создать напряжение! (Монстра забирают)
        yield return new WaitForSeconds(1.5f);
        
        // Возвращаем охранников на места за кадром (их не видно, т.к. шторка закрыта)
        if (guardLeft != null) 
        {
            guardLeft.anchoredPosition = guardLeftStartPos;
            if (defaultGuardSprite != null) guardLeft.GetComponent<Image>().sprite = defaultGuardSprite;
        }
        if (guardRight != null) 
        {
            guardRight.anchoredPosition = guardRightStartPos;
            guardRight.localScale = originalGuardRightScale; // Возвращаем оригинальный масштаб
            if (defaultGuardSprite != null) guardRight.GetComponent<Image>().sprite = defaultGuardSprite;
        }

        currentVisitorIndex++;
        ShowVisitor(currentVisitorIndex);
    }

    // Анимация ВХОДА (Зависит от прошлого действия)
    private IEnumerator AnimateVisitorWalkIn()
    {
        isAnimating = true;
        float duration = 1.0f; // Плавное открытие и приход
        float elapsed = 0f;
        
        Vector3 shutterOpenPos = originalShutterPos + new Vector3(0f, 800f, 0f);
        Vector3 shutterClosedPos = originalShutterPos; 
        
        Vector3 trayOpenPos = originalTrayPos;
        Vector3 trayClosedPos = originalTrayPos + new Vector3(0f, -600f, 0f);

        Vector3 visitorStart = originalVisitorPos + new Vector3(-800f, 0f, 0f); // Восстанавливаем переменную!
        Vector3 visitorTarget = originalVisitorPos;

        // Прячем печать от предыдущего посетителя!
        if (stampObject != null) stampObject.SetActive(false);

        // Уничтожаем все динамические отпечатки штампов на паспорте и въездном талоне (DocumentTray)
        if (documentTray != null)
        {
            foreach (Transform child in documentTray.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name == "DynamicStampMark")
                {
                    Object.Destroy(child.gameObject);
                }
            }
        }

        // Автоматически закрываем выдвижной ящик со штампами при смене посетителя
        StampDrawerController drawer = Object.FindAnyObjectByType<StampDrawerController>();
        if (drawer != null) drawer.ForceClose();

        if (lastActionWasApprove)
        {
            // Если прошлого мы пропустили (шторка уже открыта) -> новый заходит слева
            if (windowShutter != null) windowShutter.anchoredPosition = shutterOpenPos; 
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration; 
                float easeT = 1f - Mathf.Pow(1f - t, 3f); 
                
                if (visitorImageDisplay != null) visitorImageDisplay.rectTransform.anchoredPosition = Vector3.Lerp(visitorStart, visitorTarget, easeT);
                if (documentTray != null) documentTray.anchoredPosition = Vector3.Lerp(trayClosedPos, trayOpenPos, easeT);
                
                yield return null; 
            }
        }
        else
        {
            // 1. Сначала поднимается шторка (окно пустое).
            if (visitorImageDisplay != null) visitorImageDisplay.rectTransform.anchoredPosition = visitorStart;
            
            // Воспроизводим звук открытия железной двери!
            if (sfxAudioSource != null && shutterOpenSound != null)
            {
                sfxAudioSource.PlayOneShot(shutterOpenSound);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration; 
                float easeT = 1f - Mathf.Pow(1f - t, 3f); 
                if (windowShutter != null) windowShutter.anchoredPosition = Vector3.Lerp(shutterClosedPos, shutterOpenPos, easeT);
                yield return null; 
            }
            if (windowShutter != null) windowShutter.anchoredPosition = shutterOpenPos;

            // 2. Охранник (левый) выходит в центр окна
            if (guardLeft != null)
            {
                guardLeft.anchoredPosition = guardLeftStartPos;
                if (guardWalkingSprite != null) guardLeft.GetComponent<Image>().sprite = guardWalkingSprite;

                elapsed = 0f;
                float walkDuration = 0.8f;
                while (elapsed < walkDuration)
                {
                    elapsed += Time.deltaTime;
                    guardLeft.anchoredPosition = Vector3.Lerp(guardLeftStartPos, originalVisitorPos, elapsed / walkDuration);
                    yield return null;
                }
                guardLeft.anchoredPosition = originalVisitorPos;
            }

            // 3. Охранник говорит текст (меняем картинку на "стоящего/говорящего")
            if (guardLeft != null && guardStandingSprite != null)
            {
                guardLeft.GetComponent<Image>().sprite = guardStandingSprite;
            }
            else if (guardLeft != null && defaultGuardSprite != null)
            {
                guardLeft.GetComponent<Image>().sprite = defaultGuardSprite;
            }

            StartGuardDialogue("УГРОЗА ИЗОЛИРОВАНА. ПРОДОЛЖАЙ В ТОМ ЖЕ ДУХЕ.");
            while (dialoguePanel != null && dialoguePanel.activeSelf)
            {
                yield return null;
            }

            // 4. Охранник уходит вправо
            if (guardLeft != null)
            {
                if (guardWalkingSprite != null) guardLeft.GetComponent<Image>().sprite = guardWalkingSprite;

                Vector3 guardExitPos = originalVisitorPos + new Vector3(800f, 0f, 0f);
                elapsed = 0f;
                float walkDuration = 1.6f;
                while (elapsed < walkDuration)
                {
                    elapsed += Time.deltaTime;
                    guardLeft.anchoredPosition = Vector3.Lerp(originalVisitorPos, guardExitPos, elapsed / walkDuration);
                    yield return null;
                }
                guardLeft.anchoredPosition = guardLeftStartPos; // возвращаем его на базу слева
                if (defaultGuardSprite != null) guardLeft.GetComponent<Image>().sprite = defaultGuardSprite; // возвращаем в обычное состояние
            }

            // 5. Теперь новый посетитель заходит слева, а лоток выезжает на стол
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration; 
                float easeT = 1f - Mathf.Pow(1f - t, 3f); 
                
                if (visitorImageDisplay != null) visitorImageDisplay.rectTransform.anchoredPosition = Vector3.Lerp(visitorStart, visitorTarget, easeT);
                if (documentTray != null) documentTray.anchoredPosition = Vector3.Lerp(trayClosedPos, trayOpenPos, easeT);
                
                yield return null; 
            }
        }

        if (windowShutter != null) windowShutter.anchoredPosition = shutterOpenPos;
        if (documentTray != null) documentTray.anchoredPosition = trayOpenPos;
        if (visitorImageDisplay != null) visitorImageDisplay.rectTransform.anchoredPosition = originalVisitorPos;
        
        // --- ДЕЗИНФЕКЦИЯ (Пшик газом с ДВУХ СТОРОН) ---
        if (deconGasOverlay == null)
        {
            GameObject gasObj = GameObject.Find("DecontaminationGas");
            if (gasObj != null) deconGasOverlay = gasObj.GetComponent<CanvasGroup>();
        }

        if (deconGasOverlay != null) 
        {
            // ФОРСИРУЕМ ДИНАМИЧЕСКИ: Находим шторку и ставим газ ровно перед ней в Иерархии!
            if (windowShutter != null)
            {
                deconGasOverlay.transform.SetParent(windowShutter.parent, false);
                deconGasOverlay.transform.SetSiblingIndex(windowShutter.GetSiblingIndex() + 1);
            }

            deconGasOverlay.gameObject.SetActive(true);
            deconGasOverlay.alpha = 1f;
            if (sfxAudioSource != null && deconGasSound != null) sfxAudioSource.PlayOneShot(deconGasSound);
            
            if (deconGasOverlay.transform.childCount >= 2)
            {
                RectTransform leftSpray = deconGasOverlay.transform.GetChild(0).GetComponent<RectTransform>();
                RectTransform rightSpray = deconGasOverlay.transform.GetChild(1).GetComponent<RectTransform>();
                
                Vector2 leftStart = new Vector2(-300f, 0f);
                Vector2 leftEnd = new Vector2(0f, 0f);
                Vector2 rightStart = new Vector2(300f, 0f);
                Vector2 rightEnd = new Vector2(0f, 0f);
                
                // 1. Резкий впрыск к центру (0.3 сек)
                float gasElapsed = 0f;
                while(gasElapsed < 0.3f)
                {
                    gasElapsed += Time.deltaTime;
                    float t = gasElapsed / 0.3f;
                    float easeT = 1f - Mathf.Pow(1f - t, 3f);
                    if (leftSpray != null) leftSpray.anchoredPosition = Vector2.Lerp(leftStart, leftEnd, easeT);
                    if (rightSpray != null) rightSpray.anchoredPosition = Vector2.Lerp(rightStart, rightEnd, easeT);
                    yield return null;
                }
                
                // 2. Медленное растворение (1.2 сек)
                gasElapsed = 0f;
                while(gasElapsed < 1.2f) 
                {
                    gasElapsed += Time.deltaTime;
                    deconGasOverlay.alpha = 1f - (gasElapsed / 1.2f);
                    // Немного продолжают ползти вперед
                    if (leftSpray != null) leftSpray.anchoredPosition = Vector2.Lerp(leftEnd, new Vector2(100f, 0f), gasElapsed / 1.2f);
                    if (rightSpray != null) rightSpray.anchoredPosition = Vector2.Lerp(rightEnd, new Vector2(-100f, 0f), gasElapsed / 1.2f);
                    yield return null;
                }
            }
            else
            {
                // Запасной старый вариант (если нет 2 детей)
                float gasElapsed = 0f;
                while(gasElapsed < 1.0f)
                {
                    gasElapsed += Time.deltaTime;
                    deconGasOverlay.alpha = 1f - (gasElapsed / 1.0f);
                    yield return null;
                }
            }
            
            deconGasOverlay.alpha = 0f;
            deconGasOverlay.gameObject.SetActive(false);
        }
        
        // ЗАПУСКАЕМ ДИАЛОГ после входа и дезинфекции
        StartDialogue(currentShift.shiftVisitors[currentVisitorIndex]);

        isVisitorWaiting = true;
        visitorTimer = 0f;
        knockStage = 0;

        isAnimating = false; 
    }

    void Awake()
    {
        // Принудительный поиск панелей, если сброшены ссылки в инспекторе (отсекаем DontDestroyOnLoad холст достижений!)
        Canvas canvas = FindMainSceneCanvas();
        if (canvas != null)
        {
            Debug.Log("[GameManager] [" + GetHashCode() + "] Awake: Найден холст " + canvas.name);
            if (pausePanel == null)
            {
                Transform t = canvas.transform.Find("PausePanel");
                if (t == null) t = canvas.transform.Find("PauseMenu");
                if (t != null) pausePanel = t.gameObject;
            }
            // Глубокий поиск-бекап, если direct child Find не нашел
            if (pausePanel == null)
            {
                GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
                foreach (var obj in allObjects)
                {
                    if (obj != null && (obj.name == "PausePanel" || obj.name == "PauseMenu"))
                    {
                        pausePanel = obj;
                        break;
                    }
                }
            }
            Debug.Log("[GameManager] [" + GetHashCode() + "] Awake: pausePanel после поиска = " + (pausePanel != null ? pausePanel.name : "NULL"));

            if (settingsPanel == null)
            {
                Transform t = canvas.transform.Find("SettingsPanel");
                if (t == null) t = canvas.transform.Find("SettingsMenu");
                if (t != null) settingsPanel = t.gameObject;
            }
            // Глубокий поиск-бекап для settingsPanel
            if (settingsPanel == null)
            {
                GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
                foreach (var obj in allObjects)
                {
                    if (obj != null && (obj.name == "SettingsPanel" || obj.name == "SettingsMenu"))
                    {
                        settingsPanel = obj;
                        break;
                    }
                }
            }
            Debug.Log("[GameManager] [" + GetHashCode() + "] Awake: settingsPanel после поиска = " + (settingsPanel != null ? settingsPanel.name : "NULL"));
        }
        else
        {
            Debug.LogError("[GameManager] [" + GetHashCode() + "] Awake: ОШИБКА! Главный холст не найден!");
        }

        // 0. Жестко фиксируем слои интерфейса на старте игры (НАХОДИМ ДАЖЕ ВЫКЛЮЧЕННЫЕ ПАНЕЛИ)
        if (canvas != null)
        {
            if (screenFlashOverlay != null) { screenFlashOverlay.transform.SetParent(canvas.transform, false); screenFlashOverlay.transform.SetAsLastSibling(); }
            if (dialoguePanel != null) { dialoguePanel.transform.SetParent(canvas.transform, false); dialoguePanel.transform.SetAsLastSibling(); }
            if (victoryPanel != null) { victoryPanel.transform.SetParent(canvas.transform, false); victoryPanel.transform.SetAsLastSibling(); }
            if (gameOverPanel != null) { gameOverPanel.transform.SetParent(canvas.transform, false); gameOverPanel.transform.SetAsLastSibling(); }
            if (pausePanel != null) { pausePanel.transform.SetParent(canvas.transform, false); pausePanel.transform.SetAsLastSibling(); }
            
            Transform pBtn = canvas.transform.Find("WindowFrame/PauseBtn");
            if (pBtn == null) pBtn = canvas.transform.Find("PauseBtn");
            if (pBtn != null) { pBtn.SetParent(canvas.transform, false); pBtn.SetAsLastSibling(); }
            
            Transform crt = canvas.transform.Find("WindowFrame/CRT_Overlay_Safe");
            if (crt == null) crt = canvas.transform.Find("CRT_Overlay_Safe");
            if (crt != null) { crt.SetParent(canvas.transform, false); crt.SetAsLastSibling(); }
            
            // Если фото досье не привязано в инспекторе, ищем его
            if (dossierPhotoDisplay == null)
            {
                GameObject photoObj = GameObject.Find("DossierPhoto");
                if (photoObj != null) dossierPhotoDisplay = photoObj.GetComponent<Image>();
            }
        }

        // 1. Автоматически ищем и привязываем основные кнопки геймплея
        // Ищем на всем Canvas кнопки по именам, которые дает наш GameSceneBuilder
        Button[] allButtons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (var btn in allButtons)
        {
            if (btn.name == "ApproveBtn") { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(OnApproveClicked); }
            if (btn.name == "RejectBtn" || btn.name == "EmergencyIsolateBtn") { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(OnRejectClicked); }
            if (btn.name == "NextShiftBtn" || btn.name == "ContinueShiftBtn") { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(LoadNextShift); }
            if (btn.name == "InterrogateBtn") { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(OnInterrogateClicked); }
        }

        if (interrogateBtn != null) 
        {
            interrogateBtn.onClick.RemoveAllListeners();
            interrogateBtn.onClick.AddListener(OnInterrogateClicked);
        }

        if (questionsPanel != null)
        {
            Button[] qBtns = questionsPanel.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < qBtns.Length; i++)
            {
                int index = i; // capture index
                qBtns[i].onClick.RemoveAllListeners();
                qBtns[i].onClick.AddListener(() => AskQuestion(index));
            }
        }

        if (phoneButton == null)
        {
            GameObject phoneObj = GameObject.Find("PhoneButton");
            if (phoneObj == null) phoneObj = GameObject.Find("PhoneBtn");
            if (phoneObj == null) phoneObj = GameObject.Find("Telephone");
            if (phoneObj != null)
            {
                phoneButton = phoneObj.GetComponent<Button>();
            }
        }

        if (phoneButton != null)
        {
            phoneButton.onClick.RemoveAllListeners();
            phoneButton.onClick.AddListener(OnPhoneClicked);
        }

        // Полная автоматическая привязка кнопок меню паузы и панели настроек (сверхнадежный поиск по подстрокам)
        Button[] allSceneButtons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (var btn in allSceneButtons)
        {
            string bName = btn.name.ToLower();
            
            // Ищем только реальную кнопку паузы (не смешиваем с кнопками меню паузы типа PauseExitBtn, PauseResumeBtn и т.д.)
            if (bName.Contains("pause") && !bName.Contains("exit") && !bName.Contains("settings") && !bName.Contains("resume") && !bName.Contains("quit") && !bName.Contains("menu"))
            {
                pauseButton = btn;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(TogglePause);
                MakeElementSuperClickable(btn.gameObject, 10000);
                Debug.Log("[GameManager] Успешно привязана основная кнопка паузы: " + btn.name);
            }
            else if (bName.Contains("resume") || bName.Contains("continue"))
            {
                resumeButton = btn;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(TogglePause);
                Debug.Log("[GameManager] Успешно привязана кнопка продолжения: " + btn.name);
            }
            else if (bName.Contains("exit") || bName.Contains("mainmenu") || bName.Contains("quit"))
            {
                exitButton = btn;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(ReturnToMainMenu);
                Debug.Log("[GameManager] Успешно привязана кнопка выхода: " + btn.name);
            }
            else if (bName.Contains("settings") && (bName.Contains("close") || bName.Contains("hide") || bName.Contains("exit")))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(CloseSettings);
                Debug.Log("[GameManager] Успешно привязана кнопка закрытия настроек: " + btn.name);
            }
            else if (bName.Contains("settings"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OpenSettings);
                Debug.Log("[GameManager] Успешно привязана кнопка открытия настроек: " + btn.name);
            }
            else if (bName == "closebtn" && settingsPanel != null && btn.transform.IsChildOf(settingsPanel.transform))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(CloseSettings);
                Debug.Log("[GameManager] Успешно привязана кнопка закрытия настроек (CloseBtn): " + btn.name);
            }
        }

        // Автоматически привязываем слайдеры настроек громкости
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

        if (dialoguePanel != null)
        {
            Image panelImage = dialoguePanel.GetComponent<Image>();
            if (panelImage == null) 
            {
                panelImage = dialoguePanel.AddComponent<Image>();
                panelImage.color = new Color(0, 0, 0, 0); // Прозрачный фон, если его нет
            }
            Button dialogueBtn = dialoguePanel.GetComponent<Button>();
            if (dialogueBtn == null) dialogueBtn = dialoguePanel.AddComponent<Button>();
            dialogueBtn.onClick.RemoveAllListeners();
            dialogueBtn.onClick.AddListener(OnDialogueClicked);
        }

        Debug.Log("[GameManager] Все кнопки найдены и подключены автоматически.");
        
        // Создаем кнопку магнитофона внутри меню паузы
        CreatePauseTapePlayerButton();
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Переход в Главное Меню...");
        Time.timeScale = 1f;
        
        // Проверяем, есть ли сцена в билде
        if (Application.CanStreamedLevelBeLoaded("MainMenu"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogError("ОШИБКА: Сцена 'MainMenu' не добавлена в Build Settings! Нажми File -> Build Settings и перетащи туда сцену MainMenu.");
        }
    }

    // Алиас для обратной совместимости со старыми скриптами генерации (EndScreenBuilder и т.д.)
    public void ReturnToMenu()
    {
        ReturnToMainMenu();
    }

    public void LoadNextShift()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезагружаем текущую сцену для новой смены
    }

    public void TogglePause()
    {
        // Предотвращаем двойной вызов (например, из-за инспектора + кода или двойного клика) в одном кадре
        if (Time.unscaledTime - lastToggleTime < 0.1f)
        {
            return;
        }
        lastToggleTime = Time.unscaledTime;

        if (!isShiftActive && !isPaused) return; 

        isPaused = !isPaused;

        // Динамический глубокий поиск-бекап по всей сцене (включая неактивные объекты!)
        if (pausePanel == null)
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var obj in allObjects)
            {
                if (obj != null && (obj.name == "PausePanel" || obj.name == "PauseMenu"))
                {
                    pausePanel = obj;
                    break;
                }
            }
        }

        // Динамический глубокий поиск-бекап для settingsPanel
        if (settingsPanel == null)
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var obj in allObjects)
            {
                if (obj != null && (obj.name == "SettingsPanel" || obj.name == "SettingsMenu"))
                {
                    settingsPanel = obj;
                    break;
                }
            }
        }

        Debug.Log("[GameManager] [" + GetHashCode() + "] TogglePause: isPaused = " + isPaused + ", pausePanel = " + (pausePanel != null ? pausePanel.name : "NULL"));

        if (pausePanel != null) 
        {
            if (isPaused) pausePanel.transform.SetAsLastSibling(); // Гарантирует, что панель будет ПОВЕРХ монитора и всего остального!
            pausePanel.SetActive(isPaused);

            // Сбрасываем CanvasGroup, если он блокировал видимость
            CanvasGroup cg = pausePanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1f;
                cg.blocksRaycasts = isPaused;
                cg.interactable = isPaused;
            }
        }
        else
        {
            Debug.LogError("[GameManager] [" + GetHashCode() + "] ОШИБКА: pausePanel равен NULL в TogglePause!");
        }

        // При закрытии паузы закрываем и настройки
        if (!isPaused && settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            PlayerPrefs.Save();
        }

        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log(isPaused ? "Пауза ВКЛ" : "Пауза ВЫКЛ");
    }

    // ==========================================
    // ЛОГИКА НАСТРОЕК ЗВУКА
    // ==========================================
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            settingsPanel.transform.SetAsLastSibling(); // Поверх всего!
            
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

            if (musicSlider != null) musicSlider.value = musicVol;
            if (sfxSlider != null) sfxSlider.value = sfxVol;
        }
    }

    public void CloseSettings()
    {
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
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = value;
        }
        if (phoneButton != null)
        {
            AudioSource phoneAudio = phoneButton.GetComponent<AudioSource>();
            if (phoneAudio != null)
            {
                phoneAudio.volume = value;
            }
        }
    }
    private bool isTypingDialogue = false;
    private string fullDialogueText = "";
    private Coroutine typewriterCoroutine;

    private IEnumerator TypewriterRoutine(string text)
    {
        isTypingDialogue = true;
        dialogueContentText.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogueContentText.text += c;
            yield return new WaitForSeconds(0.03f); // Скорость печати
        }
        isTypingDialogue = false;
    }

    private void StartGuardDialogue(string text)
    {
        if (dialoguePanel == null) return;
        
        dialoguePanel.SetActive(true);
        if (dialoguePortrait != null)
        {
            dialoguePortrait.sprite = guardStandingSprite != null ? guardStandingSprite : defaultGuardSprite;
            dialoguePortrait.gameObject.SetActive(dialoguePortrait.sprite != null);
        }
        if (dialogueNameText != null) dialogueNameText.text = "Служба зачистки";
        
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        fullDialogueText = text;
        typewriterCoroutine = StartCoroutine(TypewriterRoutine(fullDialogueText));
    }

    private void StartDialogue(VisitorData visitor)
    {
        if (dialoguePanel == null) return;
        
        dialoguePanel.SetActive(true);
        if (dialoguePortrait != null)
        {
            dialoguePortrait.sprite = visitor.visitorSprite;
            dialoguePortrait.gameObject.SetActive(dialoguePortrait.sprite != null);
        }
        if (dialogueNameText != null) dialogueNameText.text = visitor.passportName.Split('\n')[0]; // Берем только имя
        
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        fullDialogueText = string.IsNullOrEmpty(visitor.welcomeSpeech) ? "..." : visitor.welcomeSpeech;
        typewriterCoroutine = StartCoroutine(TypewriterRoutine(fullDialogueText));
    }

    public void OnDialogueClicked()
    {
        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;

        if (isTypingDialogue)
        {
            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            dialogueContentText.text = fullDialogueText;
            isTypingDialogue = false;
        }
        else
        {
            dialoguePanel.SetActive(false);

            if (phoneAnswered)
            {
                phoneAnswered = false;
                ShowVisitor(currentVisitorIndex); // Исправил ошибку
            }
        }
    }

    private IEnumerator PhoneRingRoutine()
    {
        isPhoneRinging = true;
        phoneAnswered = false;
        
        if (phoneButton != null)
        {
            AudioSource phoneAudio = phoneButton.GetComponent<AudioSource>();
            if (phoneAudio == null) phoneAudio = phoneButton.gameObject.AddComponent<AudioSource>();
            
            float ringTimer = 0f;
            Vector3 origPos = phoneButton.transform.localPosition;
            
            while(isPhoneRinging)
            {
                if (phoneAudio != null)
                {
                    phoneAudio.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
                }
                
                // Проигрываем звук звонка и устанавливаем таймер (длина звука + 1.5 сек тишины)
                if (ringTimer <= 0f)
                {
                    if (phoneAudio != null && phoneRingSound != null) 
                    {
                        phoneAudio.PlayOneShot(phoneRingSound);
                        ringTimer = phoneRingSound.length + 1.5f; 
                    }
                    else
                    {
                        ringTimer = 2.0f; // Запасной таймер
                    }
                }
                
                ringTimer -= Time.deltaTime;

                // Трясем телефон только во время звучания самого звонка
                if (phoneRingSound != null && ringTimer > 1.5f)
                {
                    phoneButton.transform.localPosition = origPos + new Vector3(Mathf.Sin(Time.time * 30f) * 10f, 0, 0);
                }
                else if (phoneRingSound == null)
                {
                    phoneButton.transform.localPosition = origPos + new Vector3(Mathf.Sin(Time.time * 30f) * 10f, 0, 0);
                }
                else
                {
                    phoneButton.transform.localPosition = origPos; // Тишина - не трясется
                }

                yield return null;
            }
            
            phoneButton.transform.localPosition = origPos;
        }
    }

    public void OnPhoneClicked()
    {
        Debug.Log("[GameManager] OnPhoneClicked triggered! isPhoneRinging = " + isPhoneRinging + ", phoneAnswered = " + phoneAnswered + ", currentShiftIndex = " + currentShiftIndex);
        
        bool hasMessage = shiftPhoneMessages != null && currentShiftIndex < shiftPhoneMessages.Length && !string.IsNullOrEmpty(shiftPhoneMessages[currentShiftIndex]);
        
        // Разрешаем взять трубку, если телефон звонит, ЛИБО если он не звонит, но сообщение есть и оно еще не было прослушано!
        if (!isPhoneRinging && (!hasMessage || phoneAnswered))
        {
            Debug.Log("[GameManager] Взятие трубки отклонено: телефон не звонит и нет непрослушанного сообщения.");
            return;
        }

        Debug.Log("[GameManager] Трубка успешно снята!");
        
        // ДОСТИЖЕНИЕ: НА СВЯЗИ
        if (AchievementsManager.instance != null)
        {
            AchievementsManager.instance.UnlockAchievement("TELEPHONIST");
        }

        isPhoneRinging = false;
        phoneAnswered = true;
        
        if (phoneButton != null)
        {
            AudioSource phoneAudio = phoneButton.GetComponent<AudioSource>();
            if (phoneAudio != null)
            {
                phoneAudio.Stop(); // Мгновенно останавливаем звонок
                
                phoneAudio.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
                if (phonePickupSound != null)
                {
                    phoneAudio.PlayOneShot(phonePickupSound); // Проигрываем звук снятия трубки
                }
            }
        }
        
        if (dialoguePanel == null) return;
        
        dialoguePanel.SetActive(true);
        if (dialoguePortrait != null)
        {
            if (dispatcherSprite != null)
            {
                dialoguePortrait.sprite = dispatcherSprite;
                dialoguePortrait.gameObject.SetActive(true);
            }
            else
            {
                dialoguePortrait.sprite = null;
                dialoguePortrait.gameObject.SetActive(false);
            }
        }
        if (dialogueNameText != null) dialogueNameText.text = "СЛУЖБА БЕЗОПАСНОСТИ"; 
        
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        fullDialogueText = hasMessage ? shiftPhoneMessages[currentShiftIndex] : "...";
        typewriterCoroutine = StartCoroutine(TypewriterRoutine(fullDialogueText));
    }

    public void OnInterrogateClicked()
    {
        if (!isShiftActive || isAnimating || currentShift == null) return;
        
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(!questionsPanel.activeSelf);
        }
    }

    public void AskQuestion(int questionType)
    {
        if (!isShiftActive || isAnimating || currentShift == null) return;
        if (questionsPanel != null) questionsPanel.SetActive(false); // Прячем кнопки вопросов
        
        VisitorData currentVisitor = currentShift.shiftVisitors[currentVisitorIndex];
        
        // ДОСТИЖЕНИЕ: ПАРАНОЙЯ
        if (AchievementsManager.instance != null && currentVisitor.isMonster == false)
        {
            AchievementsManager.instance.UnlockAchievement("PARANOID");
        }

        string response = "...";
        
        if (currentVisitor.isMimic)
        {
            // Мимик повторяет вопросы искаженным образом
            if (questionType == 0) response = "П-почему... не с-совпадает ИМЯ?";
            else if (questionType == 1) response = "Ч-что с... м-моими ГЛАЗАМИ?";
            else if (questionType == 2) response = "М-мой ПАСПОРТ... просрочен?";
        }
        else
        {
            // Выбираем случайный индекс для разнообразия ответов
            int randIndex = Random.Range(0, 3);
            
            if (questionType == 0) // Имя или ID
            {
                if (!string.IsNullOrEmpty(currentVisitor.responseName) && 
                    currentVisitor.responseName != "С моим именем всё нормально." && 
                    currentVisitor.responseName != "С моими данными всё в порядке.")
                {
                    response = currentVisitor.responseName;
                }
                else
                {
                    if (currentVisitor.isMonster)
                    {
                        string[] monsterReplies = new string[] {
                            "Имя... моё имя... это просто имя. Оно правильное. Пропустите меня.",
                            "В базе данных старая версия меня... Я изменился... Пропустите.",
                            "Я спешу. Не задавайте глупых вопросов. Я живу здесь."
                        };
                        response = monsterReplies[randIndex];
                    }
                    else
                    {
                        string[] humanReplies = new string[] {
                            "Ой, в паспортном столе опечатались... Мне обещали исправить на следующей неделе.",
                            "Это моя девичья фамилия, я совсем недавно вышла замуж!",
                            "О господи, опять принтер смазал буквы? Это точно цифра 8, а не 3."
                        };
                        response = humanReplies[randIndex];
                    }
                }
            }
            else if (questionType == 1) // Глаза
            {
                if (!string.IsNullOrEmpty(currentVisitor.responseEyes) && 
                    currentVisitor.responseEyes != "Обычные глаза." && 
                    currentVisitor.responseEyes != "Это контактные линзы.")
                {
                    response = currentVisitor.responseEyes;
                }
                else
                {
                    if (currentVisitor.isMonster)
                    {
                        string[] monsterReplies = new string[] {
                            "Мои глаза... видят вас. Они нормальные. Смотрите на них.",
                            "Свет... здесь слишком яркий свет. Мои зрачки в порядке.",
                            "Это просто контактные линзы... человеческие линзы... да..."
                        };
                        response = monsterReplies[randIndex];
                    }
                    else
                    {
                        string[] humanReplies = new string[] {
                            "Ой, я сегодня забыл надеть линзы... или наоборот, надел цветные!",
                            "У меня сильная аллергия на тополиный пух, глаза ужасно опухли.",
                            "Я просто очень сильно не выспался... работаю на трех работах."
                        };
                        response = humanReplies[randIndex];
                    }
                }
            }
            else if (questionType == 2) // Срок действия паспорта
            {
                if (!string.IsNullOrEmpty(currentVisitor.responseDate) && 
                    currentVisitor.responseDate != "С датами всё верно." && 
                    currentVisitor.responseDate != "Я просто забыл его поменять.")
                {
                    response = currentVisitor.responseDate;
                }
                else
                {
                    if (currentVisitor.isMonster)
                    {
                        string[] monsterReplies = new string[] {
                            "Дата... это просто цифры на бумаге. Время не имеет значения.",
                            "Паспорт свежий... он пахнет человеком... он не просрочен.",
                            "Я должен войти. Моя семья ждет меня внутри. Дата верна."
                        };
                        response = monsterReplies[randIndex];
                    }
                    else
                    {
                        string[] humanReplies = new string[] {
                            "О нет! Я совсем закрутился с работой и забыл продлить... Пожалуйста, пропустите!",
                            "Я уже подал документы на замену, вот справка... Ой, я забыл её дома.",
                            "Черт, неужели уже 2084 год? Как быстро летит время..."
                        };
                        response = humanReplies[randIndex];
                    }
                }
            }
        }
        
        if (dialoguePanel == null) return;
        
        dialoguePanel.SetActive(true);
        if (dialoguePortrait != null)
        {
            dialoguePortrait.sprite = currentVisitor.visitorSprite;
            dialoguePortrait.gameObject.SetActive(dialoguePortrait.sprite != null);
        }
        if (dialogueNameText != null) dialogueNameText.text = currentVisitor.passportName.Split('\n')[0]; 
        
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        
        fullDialogueText = response;
        typewriterCoroutine = StartCoroutine(TypewriterRoutine(fullDialogueText));
    }

    private IEnumerator MonsterAttackRoutine()
    {
        isAnimating = true;
        if (stampObject != null) stampObject.SetActive(false);
        
        // Резко увеличиваем монстра и красим в красный
        Vector3 origScale = Vector3.one;
        if (visitorImageDisplay != null) 
        {
            origScale = visitorImageDisplay.transform.localScale;
            visitorImageDisplay.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            visitorImageDisplay.color = new Color(1f, 0.5f, 0.5f, 1f);
        }
        
        // Показываем трещины на стекле и отключаем на них блокировку мыши
        if (glassCracksOverlay != null)
        {
            glassCracksOverlay.SetActive(true);
            Image img = glassCracksOverlay.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;
            foreach (var child in glassCracksOverlay.GetComponentsInChildren<Image>(true))
            {
                child.raycastTarget = false;
            }
        }

        // Отключаем блокировку кликов мыши на оверлее крови
        if (bloodOverlay != null)
        {
            bloodOverlay.blocksRaycasts = false;
            Image img = bloodOverlay.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;
            foreach (var child in bloodOverlay.GetComponentsInChildren<Image>(true))
            {
                child.raycastTarget = false;
            }
        }
        
        // Выдвигаем ящик с шокером и форсируем его поверх трещин/крови в иерархии
        if (stunGunDrawer != null)
        {
            stunGunDrawer.SetActive(true);
            stunGunDrawer.transform.SetAsLastSibling();
            CanvasGroup cg = stunGunDrawer.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = true;
        }
        
        // Автоматически находим кнопку шокера, если она не привязана в инспекторе
        if (stunGunButton == null && stunGunDrawer != null)
        {
            stunGunButton = stunGunDrawer.GetComponentInChildren<Button>(true);
        }

        float timeLeft = 2.0f;
        bool shocked = false;
        
        if (stunGunButton != null)
        {
            TextMeshProUGUI bTxt = stunGunButton.GetComponentInChildren<TextMeshProUGUI>(true);
            
            if (currentStunCharges > 0)
            {
                if (bTxt != null) bTxt.text = "УДАРИТЬ ШОКЕРОМ!";
                stunGunButton.onClick.RemoveAllListeners();
                stunGunButton.onClick.AddListener(() => { 
                    shocked = true; 
                    currentStunCharges--; // Тратим заряд
                });
            }
            else
            {
                if (bTxt != null) bTxt.text = "ШОКЕР РАЗРЯЖЕН!";
                stunGunButton.onClick.RemoveAllListeners(); // Ничего не делает, игрок обречен
            }
        }

        Vector3 origVisitorPos = visitorImageDisplay != null ? visitorImageDisplay.rectTransform.anchoredPosition : Vector3.zero;

        // Ждем 2 секунды, трясем монстра
        while(timeLeft > 0)
        {
            if (shocked) break;
            
            timeLeft -= Time.deltaTime;
            
            if (visitorImageDisplay != null)
            {
                visitorImageDisplay.rectTransform.anchoredPosition = origVisitorPos + new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);
            }
            
            yield return null;
        }
        
        if (visitorImageDisplay != null) visitorImageDisplay.rectTransform.anchoredPosition = origVisitorPos;
        
        if (shocked)
        {
            // Игрок успел ударить шокером!
            if (AchievementsManager.instance != null)
            {
                AchievementsManager.instance.UnlockAchievement("NERVES_OF_STEEL");
            }

            if (screenFlashOverlay != null)
            {
                screenFlashOverlay.gameObject.SetActive(true);
                screenFlashOverlay.color = new Color(0.5f, 0.8f, 1f, 1f); // Синяя вспышка тока
            }
            
            if (glassCracksOverlay != null) glassCracksOverlay.SetActive(false);
            if (stunGunDrawer != null) stunGunDrawer.SetActive(false);
            
            // Монстр оглушен и обуглен! Оставляем его видимым, чтобы охранники могли его утащить.
            if (visitorImageDisplay != null) 
            {
                visitorImageDisplay.transform.localScale = origScale;
                visitorImageDisplay.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Темный (обгоревший)
            }
            
            // Плавное затухание вспышки
            float t = 1f;
            while(t > 0)
            {
                t -= Time.deltaTime * 2f;
                if (screenFlashOverlay != null) screenFlashOverlay.color = new Color(0.5f, 0.8f, 1f, t);
                yield return null;
            }
            if (screenFlashOverlay != null) screenFlashOverlay.gameObject.SetActive(false);
            
            // Штраф за ошибку (игрок ведь изначально нажал Одобрить)
            strikes++;
            UpdateUI();
            
            if (strikes >= 3) 
            {
                GameOver("Вы оглушили монстра, но до этого допустили слишком много ошибок. Вы уволены.");
                yield break;
            }
            
            // Даем игроку секунду посмотреть на пустое окно, чтобы насладиться победой
            yield return new WaitForSeconds(1.0f);
            
            isBloodNext = true; // После шокера стекло тоже в крови монстра!
            
            // Переходим к следующему посетителю
            StartCoroutine(AnimateRejectAndLoadNext());
        }
        else
        {
            // Время вышло
            if (glassCracksOverlay != null) glassCracksOverlay.SetActive(false);
            if (stunGunDrawer != null) stunGunDrawer.SetActive(false);
            
            GameOver("ВЫ НЕ УСПЕЛИ ДОСТАТЬ ШОКЕР. МОНСТР ВЫБИЛ СТЕКЛО.");
        }
    }

    private void DisableRaycastOnOverlay(string objName)
    {
        GameObject obj = GameObject.Find(objName);
        if (obj != null)
        {
            Image img = obj.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;
            
            foreach (var child in obj.GetComponentsInChildren<Image>(true))
            {
                child.raycastTarget = false;
            }

            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = false;
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

    private void MakeElementSuperClickable(GameObject obj, int sortingOrder)
    {
        if (obj == null) return;
        
        Canvas c = obj.GetComponent<Canvas>();
        if (c == null) c = obj.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = sortingOrder;

        UnityEngine.UI.GraphicRaycaster gr = obj.GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (gr == null) gr = obj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    }

    // =========================================================================
    // ДИНАМИЧЕСКИЙ СТИЛИЗАТОР ЭКРАНОВ ПОБЕДЫ И ПОРАЖЕНИЯ НА СТАРТЕ И В РАНТАЙМЕ
    // =========================================================================
    private Sprite LoadSpriteFromFile(string fileName)
    {
        // Проверяем пути в проекте
        string[] paths = new string[] {
            System.IO.Path.Combine(Application.dataPath, "Sprites", fileName),
            System.IO.Path.Combine(Application.dataPath, "3", "Sprites", fileName),
            System.IO.Path.Combine(Application.dataPath, fileName)
        };

        foreach (string path in paths)
        {
            if (System.IO.File.Exists(path))
            {
                try
                {
                    byte[] data = System.IO.File.ReadAllBytes(path);
                    Texture2D tex = new Texture2D(2, 2);
                    if (tex.LoadImage(data))
                    {
                        tex.filterMode = FilterMode.Point;
                        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[GameManager] Ошибка загрузки спрайта из файла: " + e.Message);
                }
            }
        }

#if UNITY_EDITOR
        // Попытка загрузить из базы ассетов в редакторе
        string assetPath = "Assets/Sprites/" + fileName;
        Sprite editorSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (editorSprite != null) return editorSprite;
#endif

        return null;
    }

    private bool IsValidTransform(Transform t)
    {
        if (t == null) return false;
        try
        {
            var testPos = t.localPosition;
            var testActive = t.gameObject.activeSelf;
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    private Transform EnsureClipboardHierarchyRuntime(GameObject panelObj, bool isVictory)
    {
        if (panelObj == null) return null;

        // 1. Убеждаемся в наличии Background (затемняющий фон комнаты)
        Transform bgTrans = panelObj.transform.Find("Background");
        if (!IsValidTransform(bgTrans))
        {
            if (bgTrans != null)
            {
                try { Destroy(bgTrans.gameObject); } catch {}
            }

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(panelObj.transform, false);
            bgTrans = bgObj.transform;
            
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.04f, 0.04f, 0.97f);
            
            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            bgRt.anchoredPosition = Vector2.zero;
        }
        else
        {
            Image bgImg = bgTrans.GetComponent<Image>();
            if (bgImg != null) bgImg.color = new Color(0.04f, 0.04f, 0.04f, 0.97f);
        }
        bgTrans.gameObject.SetActive(true);

        // 2. Убеждаемся в наличии Clipboard (планшет)
        Transform clipTrans = panelObj.transform.Find("Clipboard");
        if (!IsValidTransform(clipTrans))
        {
            if (clipTrans != null)
            {
                try { Destroy(clipTrans.gameObject); } catch {}
            }

            GameObject clipObj = new GameObject("Clipboard");
            clipObj.transform.SetParent(panelObj.transform, false);
            clipTrans = clipObj.transform;
            
            Image clipImg = clipObj.AddComponent<Image>();
            clipImg.color = Color.white;
            clipImg.preserveAspect = true;
            
            RectTransform clipRt = clipObj.GetComponent<RectTransform>();
            clipRt.anchorMin = new Vector2(0.5f, 0.5f);
            clipRt.anchorMax = new Vector2(0.5f, 0.5f);
            clipRt.pivot = new Vector2(0.5f, 0.5f);
            clipRt.sizeDelta = new Vector2(560f, 840f);
            clipRt.anchoredPosition = Vector2.zero;
        }
        clipTrans.gameObject.SetActive(true);

        // Очистка дубликатов дочерних элементов в Clipboard, чтобы избежать наложения старых кнопок и текстов
        {
            System.Collections.Generic.HashSet<string> seenKeys = new System.Collections.Generic.HashSet<string>();
            System.Collections.Generic.List<GameObject> duplicatesToDestroy = new System.Collections.Generic.List<GameObject>();
            
            for (int i = 0; i < clipTrans.childCount; i++)
            {
                Transform child = clipTrans.GetChild(i);
                if (child == null) continue;
                
                string childName = child.name.ToLower();
                string key = "";
                
                if (childName.Contains("title"))
                {
                    key = "title";
                }
                else if (childName.Contains("stats") || childName.Contains("reason"))
                {
                    key = "stats";
                }
                else if (childName.Contains("stamp"))
                {
                    key = "stamp";
                }
                else if (childName.Contains("next") || childName.Contains("continue"))
                {
                    key = "nextbtn";
                }
                else if (childName.Contains("restart") || childName.Contains("retry"))
                {
                    key = "restartbtn";
                }
                else if (childName.Contains("menu") || childName.Contains("exit") || childName.Contains("btn") || childName.Contains("button"))
                {
                    key = "menubtn";
                }
                
                if (!string.IsNullOrEmpty(key))
                {
                    if (seenKeys.Contains(key))
                    {
                        duplicatesToDestroy.Add(child.gameObject);
                    }
                    else
                    {
                        seenKeys.Add(key);
                    }
                }
            }
            foreach (var duplicate in duplicatesToDestroy)
            {
                DestroyImmediate(duplicate);
            }
        }

        // Назначаем спрайт планшета
        Image cImg = clipTrans.GetComponent<Image>();
        if (cImg == null) cImg = clipTrans.gameObject.AddComponent<Image>();
        if (cImg != null)
        {
            if (cImg.sprite == null)
            {
                Sprite clipSprite = isVictory ? clipboardApprovedSprite : clipboardFiredSprite;
                if (clipSprite != null)
                {
                    cImg.sprite = clipSprite;
                    cImg.color = Color.white;
                    cImg.preserveAspect = true;
                }
            }
        }

        // 3. Переносим или создаем элементы внутри Clipboard
        // а) TitleText
        string titleName = isVictory ? "VictoryTitle" : "GameOverTitle";
        Transform titleTrans = clipTrans.Find("TitleText");
        if (!IsValidTransform(titleTrans)) titleTrans = clipTrans.Find(titleName);
        if (!IsValidTransform(titleTrans))
        {
            titleTrans = panelObj.transform.Find(titleName);
            if (!IsValidTransform(titleTrans)) titleTrans = panelObj.transform.Find("TitleText");
            if (!IsValidTransform(titleTrans))
            {
                titleTrans = null;
                foreach (Transform child in panelObj.transform)
                {
                    if (child != null && child.name.ToLower().Contains("title")) { titleTrans = child; break; }
                }
            }
        }
        if (!IsValidTransform(titleTrans))
        {
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(clipTrans, false);
            titleTrans = titleObj.transform;
            
            TextMeshProUGUI tmp = titleObj.AddComponent<TextMeshProUGUI>();
            tmp.text = isVictory ? "СМЕНА ОКОНЧЕНА" : "ВЫ УВОЛЕНЫ";
            tmp.color = new Color(0.12f, 0.12f, 0.12f);
            tmp.fontSize = 28f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            titleTrans.SetParent(clipTrans, false);
            titleTrans.name = "TitleText";
        }

        // б) StatsText
        string statsName = isVictory ? "VictoryStats" : "GameOverReason";
        Transform statsTrans = clipTrans.Find("StatsText");
        if (!IsValidTransform(statsTrans)) statsTrans = clipTrans.Find(statsName);
        if (!IsValidTransform(statsTrans))
        {
            statsTrans = panelObj.transform.Find(statsName);
            if (!IsValidTransform(statsTrans)) statsTrans = panelObj.transform.Find("StatsText");
            if (!IsValidTransform(statsTrans))
            {
                statsTrans = null;
                foreach (Transform child in panelObj.transform)
                {
                    if (child != null && (child.name.ToLower().Contains("stats") || child.name.ToLower().Contains("reason"))) { statsTrans = child; break; }
                }
            }
        }
        if (!IsValidTransform(statsTrans))
        {
            GameObject statsObj = new GameObject("StatsText");
            statsObj.transform.SetParent(clipTrans, false);
            statsTrans = statsObj.transform;
            
            TextMeshProUGUI tmp = statsObj.AddComponent<TextMeshProUGUI>();
            tmp.text = isVictory ? "Жильцы дома в безопасности." : "Слишком много ошибок.";
            tmp.color = new Color(0.15f, 0.15f, 0.15f);
            tmp.fontSize = 13f;
            tmp.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            statsTrans.SetParent(clipTrans, false);
            statsTrans.name = "StatsText";
        }

        // в) StampImage
        Transform stampTrans = clipTrans.Find("StampImage");
        if (!IsValidTransform(stampTrans)) stampTrans = clipTrans.Find("PassportStamp");
        if (!IsValidTransform(stampTrans))
        {
            stampTrans = panelObj.transform.Find("PassportStamp");
            if (!IsValidTransform(stampTrans)) stampTrans = panelObj.transform.Find("StampImage");
            if (!IsValidTransform(stampTrans))
            {
                stampTrans = null;
                foreach (Transform child in panelObj.transform)
                {
                    if (child != null && child.name.ToLower().Contains("stamp")) { stampTrans = child; break; }
                }
            }
        }
        if (!IsValidTransform(stampTrans))
        {
            GameObject stampObj = new GameObject("StampImage");
            stampObj.transform.SetParent(clipTrans, false);
            stampTrans = stampObj.transform;
            
            Image img = stampObj.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            stampTrans.SetParent(clipTrans, false);
            stampTrans.name = "StampImage";
        }

        // г) MainMenuBtn
        Transform menuBtnTrans = clipTrans.Find("MainMenuBtn");
        if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = clipTrans.Find("ExitBtn");
        if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = clipTrans.Find("ExitMenuBtn");
        if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = clipTrans.Find("MenuBtn");
        if (!IsValidTransform(menuBtnTrans))
        {
            menuBtnTrans = panelObj.transform.Find("ExitBtn");
            if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = panelObj.transform.Find("ExitMenuBtn");
            if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = panelObj.transform.Find("MainMenuBtn");
            if (!IsValidTransform(menuBtnTrans)) menuBtnTrans = panelObj.transform.Find("MenuBtn");
            if (!IsValidTransform(menuBtnTrans))
            {
                menuBtnTrans = null;
                foreach (Transform child in panelObj.transform)
                {
                    if (child != null && (child.name.ToLower().Contains("btn") || child.name.ToLower().Contains("button")))
                    {
                        if (child.name != "NextShiftBtn" && !child.name.ToLower().Contains("continue"))
                        {
                            menuBtnTrans = child;
                            break;
                        }
                    }
                }
            }
        }
        if (!IsValidTransform(menuBtnTrans))
        {
            GameObject btnObj = new GameObject("MainMenuBtn");
            btnObj.transform.SetParent(clipTrans, false);
            menuBtnTrans = btnObj.transform;
            
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            Outline outline = btnObj.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(2, -2);
            
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(ReturnToMenu);
            btnObj.AddComponent<MenuButtonHoverEffects>();
            
            GameObject txtObj = new GameObject("BtnText");
            txtObj.transform.SetParent(btnObj.transform, false);
            TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "В ГЛАВНОЕ МЕНЮ";
            tmp.fontSize = 20;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            
            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.sizeDelta = Vector2.zero;
        }
        else
        {
            menuBtnTrans.SetParent(clipTrans, false);
            menuBtnTrans.name = "MainMenuBtn";
            Button btn = menuBtnTrans.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(ReturnToMenu);
                if (btn.GetComponent<MenuButtonHoverEffects>() == null)
                {
                    btn.gameObject.AddComponent<MenuButtonHoverEffects>();
                }
            }
        }

        // д) RestartBtn (только для GameOverPanel)
        if (!isVictory)
        {
            Transform restartTrans = clipTrans.Find("RestartBtn");
            if (!IsValidTransform(restartTrans)) restartTrans = clipTrans.Find("RetryBtn");
            if (!IsValidTransform(restartTrans))
            {
                restartTrans = panelObj.transform.Find("RestartBtn");
                if (!IsValidTransform(restartTrans)) restartTrans = panelObj.transform.Find("RetryBtn");
                if (!IsValidTransform(restartTrans))
                {
                    restartTrans = null;
                    foreach (Transform child in panelObj.transform)
                    {
                        if (child != null && (child.name.ToLower().Contains("restart") || child.name.ToLower().Contains("retry")))
                        {
                            restartTrans = child;
                            break;
                        }
                    }
                }
            }
            if (IsValidTransform(restartTrans))
            {
                restartTrans.SetParent(clipTrans, false);
                restartTrans.name = "RestartBtn";
                Button btn = restartTrans.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(LoadNextShift);
                    if (btn.GetComponent<MenuButtonHoverEffects>() == null)
                    {
                        btn.gameObject.AddComponent<MenuButtonHoverEffects>();
                    }
                }
            }
        }

        // е) NextShiftBtn (только для VictoryPanel)
        if (isVictory)
        {
            Transform nextBtnTrans = clipTrans.Find("NextShiftBtn");
            if (!IsValidTransform(nextBtnTrans)) nextBtnTrans = clipTrans.Find("ContinueShiftBtn");
            if (!IsValidTransform(nextBtnTrans))
            {
                nextBtnTrans = panelObj.transform.Find("NextShiftBtn");
                if (!IsValidTransform(nextBtnTrans)) nextBtnTrans = panelObj.transform.Find("ContinueShiftBtn");
                if (!IsValidTransform(nextBtnTrans))
                {
                    nextBtnTrans = null;
                    foreach (Transform child in panelObj.transform)
                    {
                        if (child != null && (child.name.ToLower().Contains("next") || child.name.ToLower().Contains("continue")))
                        {
                            nextBtnTrans = child;
                            break;
                        }
                    }
                }
            }
            if (IsValidTransform(nextBtnTrans))
            {
                nextBtnTrans.SetParent(clipTrans, false);
                nextBtnTrans.name = "NextShiftBtn";
                Button btn = nextBtnTrans.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(LoadNextShift);
                    if (btn.GetComponent<MenuButtonHoverEffects>() == null)
                    {
                        btn.gameObject.AddComponent<MenuButtonHoverEffects>();
                    }
                }
            }
        }

        return clipTrans;
    }

    private void StylizeEndScreensRuntime()
    {
        // Гарантируем загрузку русских спрайтов штампов на лету
        if (stampApprovedRu == null) stampApprovedRu = LoadSpriteFromFile("stamp_approved_ru.png");
        if (stampFiredRu == null) stampFiredRu = LoadSpriteFromFile("stamp_fired_ru.png");
        if (clipboardApprovedSprite == null) clipboardApprovedSprite = LoadSpriteFromFile("clipboard_approved.jpg");
        if (clipboardFiredSprite == null) clipboardFiredSprite = LoadSpriteFromFile("clipboard_fired.jpg");

        // 1. Стилизуем VictoryPanel (Экран победы)
        if (victoryPanel != null)
        {
            // Полностью убираем зеленый фон самой панели
            Image vicMainImg = victoryPanel.GetComponent<Image>();
            if (vicMainImg != null)
            {
                vicMainImg.color = new Color(0f, 0f, 0f, 0f);
            }

            // Автоматически строим или корректируем иерархию планшета
            Transform clipboard = EnsureClipboardHierarchyRuntime(victoryPanel, true);

            // Отключаем старые разбросанные по панели элементы (кнопки, тексты), кроме фона и планшета
            foreach (Transform child in victoryPanel.transform)
            {
                if (child != null)
                {
                    string cName = child.name;
                    if (cName != "Background" && cName != "Clipboard")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            if (IsValidTransform(clipboard))
            {
                // Проверяем, активна ли кнопка продолжения смены
                Transform nextBtn = clipboard.Find("NextShiftBtn");
                bool isNextActive = nextBtn != null && nextBtn.gameObject.activeSelf;

                PositionElementRuntime(clipboard, "TitleText", "VictoryTitle", 280f, 28f, isText: true, isButton: false);
                PositionElementRuntime(clipboard, "StatsText", "VictoryStats", 100f, 13f, isText: true, isButton: false);
                PositionElementRuntime(clipboard, "StampImage", "PassportStamp", 0f, 0f, isText: false, isButton: false, new Vector2(240f, 120f), -8f, stampApprovedRu);

                if (isNextActive)
                {
                    PositionElementRuntime(clipboard, "NextShiftBtn", "ContinueShiftBtn", -220f, 20f, isText: false, isButton: true, new Vector2(260f, 50f));
                    PositionElementRuntime(clipboard, "MainMenuBtn", "ExitMenuBtn", -320f, 20f, isText: false, isButton: true, new Vector2(260f, 50f));
                }
                else
                {
                    PositionElementRuntime(clipboard, "MainMenuBtn", "ExitMenuBtn", -220f, 20f, isText: false, isButton: true, new Vector2(260f, 50f));
                }
            }
        }

        // 2. Стилизуем GameOverPanel (Экран поражения)
        if (gameOverPanel != null)
        {
            // Полностью убираем красный фон самой панели
            Image goMainImg = gameOverPanel.GetComponent<Image>();
            if (goMainImg != null)
            {
                goMainImg.color = new Color(0f, 0f, 0f, 0f);
            }

            // Автоматически строим или корректируем иерархию планшета
            Transform clipboard = EnsureClipboardHierarchyRuntime(gameOverPanel, false);

            // Отключаем старые элементы
            foreach (Transform child in gameOverPanel.transform)
            {
                if (child != null)
                {
                    string cName = child.name;
                    if (cName != "Background" && cName != "Clipboard")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            if (IsValidTransform(clipboard))
            {
                PositionElementRuntime(clipboard, "TitleText", "GameOverTitle", 280f, 28f, isText: true, isButton: false);
                PositionElementRuntime(clipboard, "StatsText", "GameOverReason", 40f, 13f, isText: true, isButton: false);
                PositionElementRuntime(clipboard, "StampImage", "PassportStamp", 20f, 0f, isText: false, isButton: false, new Vector2(240f, 120f), -12f, stampFiredRu);
                PositionElementRuntime(clipboard, "RestartBtn", "RetryBtn", -220f, 20f, isText: false, isButton: true, new Vector2(260f, 50f));
                PositionElementRuntime(clipboard, "MainMenuBtn", "ExitBtn", -320f, 20f, isText: false, isButton: true, new Vector2(260f, 50f));
            }
        }
    }

    private void PositionElementRuntime(Transform parent, string primaryName, string secondaryName, float yPos, float sizeVal, bool isText, bool isButton, Vector2? sizeDelta = null, float? rotZ = null, Sprite spriteVal = null)
    {
        if (!IsValidTransform(parent)) return;

        Transform target = parent.Find(primaryName);
        if (!IsValidTransform(target) && !string.IsNullOrEmpty(secondaryName)) target = parent.Find(secondaryName);
        if (!IsValidTransform(target))
        {
            target = null;
            foreach (Transform child in parent)
            {
                if (child != null && (child.name.ToLower().Contains(primaryName.ToLower()) || (!string.IsNullOrEmpty(secondaryName) && child.name.ToLower().Contains(secondaryName.ToLower()))))
                {
                    target = child;
                    break;
                }
            }
        }

        if (IsValidTransform(target))
        {
            RectTransform rt = target.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, yPos);
                
                if (sizeDelta.HasValue)
                {
                    rt.sizeDelta = sizeDelta.Value;
                }
                else if (isText)
                {
                    rt.sizeDelta = new Vector2(480f, primaryName.Contains("Title") ? 80f : 200f);
                }
                
                if (rotZ.HasValue)
                {
                    rt.localEulerAngles = new Vector3(0f, 0f, rotZ.Value);
                }
            }

            if (isText)
            {
                TextMeshProUGUI tmp = target.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.fontSize = sizeVal;
                    tmp.fontStyle = primaryName.Contains("Title") || secondaryName.Contains("Title") ? FontStyles.Bold : FontStyles.Normal;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.color = new Color(0.15f, 0.15f, 0.15f); // Идеальный цвет пишущей машинки на бумаге планшета!
                    tmp.enableWordWrapping = true;
                }
            }

            if (isButton)
            {
                TextMeshProUGUI btnTxt = target.GetComponentInChildren<TextMeshProUGUI>();
                if (btnTxt != null)
                {
                    btnTxt.fontSize = 20;
                    btnTxt.fontStyle = FontStyles.Bold;
                }
            }

            if (spriteVal != null)
            {
                Image img = target.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = spriteVal;
                    img.color = Color.white;
                    img.preserveAspect = true;
                }
            }
            
            // Включаем спозиционированный элемент
            target.gameObject.SetActive(true);
        }
    }

    // ==========================================
    // КАНВАС И ПРАВИЛА (СУРЕТ 3.4)
    // ==========================================
    private GameObject rulesPanelObj;
    private GameObject rulesButtonObj;
    private int rulesCurrentPage = 0;
    private TextMeshProUGUI rulesContentText;
    private TextMeshProUGUI rulesPageLabel;
    private Button rulesPrevBtn;
    private Button rulesNextBtn;

    public void ShowRulesPanel()
    {
        if (sfxAudioSource != null && phonePickupSound != null) sfxAudioSource.PlayOneShot(phonePickupSound, 0.4f);

        if (rulesPanelObj == null)
        {
            CreateRulesPanelRuntime();
        }
        
        rulesPanelObj.SetActive(true);
        rulesCurrentPage = 0;
        UpdateRulesPageContent();
        
        // Пауза игры во время чтения правил
        Time.timeScale = 0f;
    }

    public void HideRulesPanel()
    {
        if (sfxAudioSource != null && phonePickupSound != null) sfxAudioSource.PlayOneShot(phonePickupSound, 0.4f);
        if (rulesPanelObj != null) rulesPanelObj.SetActive(false);
        
        // Снимаем паузу
        Time.timeScale = 1f;
    }

    private void UpdateRulesPageContent()
    {
        if (rulesContentText == null || rulesPageLabel == null) return;

        rulesPageLabel.text = $"Страница {rulesCurrentPage + 1} / 3";

        switch (rulesCurrentPage)
        {
            case 0:
                rulesContentText.text = 
                    "<color=#dd8822><size=24><b>1. ОБЯЗАННОСТИ ИНСПЕКТОРА</b></size></color>\n\n" +
                    "1. Тщательно проверяйте документы каждого гражданина.\n" +
                    "2. Сверяйте имя, фамилию, номер паспорта и срок действия.\n" +
                    "3. Сверяйте фото в паспорте с лицом посетителя.\n" +
                    "4. В случае выявления несоответствий или отсутствия документов — отказывайте во въезде.\n" +
                    "5. При обнаружении монстра/мимика немедленно используйте защитные средства.";
                if (rulesPrevBtn != null) rulesPrevBtn.gameObject.SetActive(false);
                if (rulesNextBtn != null) rulesNextBtn.gameObject.SetActive(true);
                break;
            case 1:
                rulesContentText.text = 
                    "<color=#dd8822><size=24><b>2. ИНСТРУКЦИЯ ПО БЕЗОПАСНОСТИ</b></size></color>\n\n" +
                    "1. Двойные стальные ставни защищают вас от прямых атак. Держите их исправными.\n" +
                    "2. В случае угрозы или нападения активируйте шокер.\n" +
                    "3. Мимики могут маскироваться под обычных жителей. Изучайте досье на наличие аномалий.\n" +
                    "4. Никогда не открывайте дверь подозрительным лицам без полной проверки документов.";
                if (rulesPrevBtn != null) rulesPrevBtn.gameObject.SetActive(true);
                if (rulesNextBtn != null) rulesNextBtn.gameObject.SetActive(true);
                break;
            case 2:
                rulesContentText.text = 
                    "<color=#dd8822><size=24><b>3. РЕГЛАМЕНТ ШТАМПОВАНИЯ</b></size></color>\n\n" +
                    "1. Зеленый штамп <color=#22dd22><b>«ОДОБРЕНО»</b></color> ставится на паспорт гражданина при успешном прохождении всех проверок.\n" +
                    "2. Красный штамп <color=#dd2222><b>«ОТКАЗАНО»</b></color> ставится при любых нарушениях протокола или подозрении на мимикрию.\n" +
                    "3. Неправильно поставленный штамп ведет к немедленному начислению штрафа.";
                if (rulesPrevBtn != null) rulesPrevBtn.gameObject.SetActive(true);
                if (rulesNextBtn != null) rulesNextBtn.gameObject.SetActive(false);
                break;
        }
    }

    private void CreateRulesPanelRuntime()
    {
        Canvas canvas = FindMainSceneCanvas();
        if (canvas == null) return;

        // Вспомогательный метод для безопасного создания UI-объектов
        System.Func<string, Transform, GameObject> createUI = (name, parent) =>
        {
            GameObject obj = new GameObject(name);
            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            return obj;
        };

        rulesPanelObj = createUI("RulesPanel", canvas.transform);
        rulesPanelObj.transform.SetAsLastSibling();

        RectTransform panelRt = rulesPanelObj.GetComponent<RectTransform>();
        panelRt.sizeDelta = new Vector2(700f, 500f);
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.anchoredPosition = Vector2.zero;

        Image bg = rulesPanelObj.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.05f, 0.98f);

        Outline outline = rulesPanelObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.8f, 0.35f, 0.1f, 1f);
        outline.effectDistance = new Vector2(4f, -4f);

        TMP_FontAsset activeFont = null;
        TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include);
        foreach (var txt in allTexts)
        {
            if (txt != null && txt.font != null) { activeFont = txt.font; break; }
        }

        // Заголовок
        GameObject titleObj = createUI("TitleText", rulesPanelObj.transform);
        TextMeshProUGUI titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
        titleTxt.text = "ИНСТРУКЦИЯ И ПРАВИЛА ИНСПЕКТОРА";
        titleTxt.fontSize = 22;
        titleTxt.fontStyle = FontStyles.Bold;
        titleTxt.color = new Color(0.9f, 0.4f, 0.1f);
        titleTxt.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) titleTxt.font = activeFont;
        
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 0.85f);
        titleRt.anchorMax = new Vector2(1f, 0.95f);
        titleRt.offsetMin = new Vector2(20f, 0f);
        titleRt.offsetMax = new Vector2(-20f, 0f);

        // Текст содержания
        GameObject contentObj = createUI("ContentText", rulesPanelObj.transform);
        rulesContentText = contentObj.AddComponent<TextMeshProUGUI>();
        rulesContentText.fontSize = 18;
        rulesContentText.color = Color.white;
        rulesContentText.alignment = TextAlignmentOptions.TopLeft;
        rulesContentText.enableWordWrapping = true;
        if (activeFont != null) rulesContentText.font = activeFont;

        RectTransform contentRt = contentObj.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0.05f, 0.2f);
        contentRt.anchorMax = new Vector2(0.95f, 0.8f);
        contentRt.offsetMin = Vector2.zero;
        contentRt.offsetMax = Vector2.zero;

        // Лейбл страниц
        GameObject pageLabelObj = createUI("PageLabel", rulesPanelObj.transform);
        rulesPageLabel = pageLabelObj.AddComponent<TextMeshProUGUI>();
        rulesPageLabel.fontSize = 16;
        rulesPageLabel.color = Color.gray;
        rulesPageLabel.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) rulesPageLabel.font = activeFont;

        RectTransform pageLabelRt = pageLabelObj.GetComponent<RectTransform>();
        pageLabelRt.anchorMin = new Vector2(0.4f, 0.05f);
        pageLabelRt.anchorMax = new Vector2(0.6f, 0.12f);
        pageLabelRt.offsetMin = Vector2.zero;
        pageLabelRt.offsetMax = Vector2.zero;

        // Вспомогательный метод для кнопок
        System.Action<string, Vector2, Vector2, string, System.Action> setupButton = (btnName, size, pos, btnText, action) =>
        {
            GameObject btnObj = createUI(btnName, rulesPanelObj.transform);
            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            rt.anchorMin = new Vector2(0.5f, 0.05f);
            rt.anchorMax = new Vector2(0.5f, 0.05f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;

            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            Outline btnOutline = btnObj.AddComponent<Outline>();
            btnOutline.effectColor = Color.white;
            btnOutline.effectDistance = new Vector2(1f, -1f);

            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(() => action());

            GameObject txtObj = createUI("Text", btnObj.transform);
            TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = btnText;
            tmp.fontSize = 14;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            if (activeFont != null) tmp.font = activeFont;

            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = Vector2.zero;
            txtRt.offsetMax = Vector2.zero;

            btnObj.AddComponent<MenuButtonHoverEffects>();

            if (btnName == "PrevBtn") rulesPrevBtn = btn;
            else if (btnName == "NextBtn") rulesNextBtn = btn;
        };

        setupButton("PrevBtn", new Vector2(100f, 35f), new Vector2(-150f, 15f), "НАЗАД", () => {
            if (rulesCurrentPage > 0) {
                rulesCurrentPage--;
                UpdateRulesPageContent();
            }
        });

        setupButton("NextBtn", new Vector2(100f, 35f), new Vector2(150f, 15f), "ДАЛЕЕ", () => {
            if (rulesCurrentPage < 2) {
                rulesCurrentPage++;
                UpdateRulesPageContent();
            }
        });

        setupButton("CloseBtn", new Vector2(120f, 35f), new Vector2(0f, 15f), "ЗАКРЫТЬ", HideRulesPanel);
    }

    private void CreateRulesButtonRuntime()
    {
        if (GameObject.Find("RulesButton") != null)
        {
            rulesButtonObj = GameObject.Find("RulesButton");
            return;
        }

        GameObject pauseBtnObj = GameObject.Find("PauseBtn");
        if (pauseBtnObj == null) pauseBtnObj = GameObject.Find("PauseButton");
        if (pauseBtnObj == null) return;

        GameObject rulesBtnObj = Instantiate(pauseBtnObj, pauseBtnObj.transform.parent);
        rulesBtnObj.name = "RulesButton";
        rulesButtonObj = rulesBtnObj;

        RectTransform pauseRt = pauseBtnObj.GetComponent<RectTransform>();
        RectTransform rulesRt = rulesBtnObj.GetComponent<RectTransform>();

        // Настраиваем размер кнопки правил, делая её компактнее
        rulesRt.sizeDelta = new Vector2(130f, 45f);

        // Сдвигаем влево относительно паузы и выравниваем по центру Y
        rulesRt.anchoredPosition = pauseRt.anchoredPosition + new Vector2(-270f, -17.5f);

        // Настраиваем текст на кнопке
        TextMeshProUGUI txt = rulesBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.text = "ПРАВИЛА";
            txt.fontSize = 14f;
        }

        Button btn = rulesBtnObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick = new Button.ButtonClickedEvent();
            btn.onClick.AddListener(ShowRulesPanel);
        }
    }

    private AudioClip CreateVictoryFanfare()
    {
        int samplerate = 44100;
        
        // Мажорный праздничный фанфарный арпеджио на 4 секунды
        float[] freqs = new float[] { 
            523.25f, 659.25f, 783.99f, 1046.50f, 1318.51f, 1567.98f, 1318.51f, 1046.50f, 
            698.46f, 880.00f, 1046.50f, 1396.91f, 1760.00f, 1396.91f, 1046.50f, 1567.98f 
        };
        
        float[] durs = new float[] { 
            0.15f, 0.15f, 0.15f, 0.25f, 0.15f, 0.15f, 0.15f, 0.25f,
            0.15f, 0.15f, 0.15f, 0.25f, 0.15f, 0.15f, 0.15f, 0.50f
        };
        
        float length = 0f;
        foreach (float d in durs) length += d;
        
        int totalSamples = (int)(samplerate * length);
        float[] samples = new float[totalSamples];
        
        int sampleIdx = 0;
        for (int note = 0; note < freqs.Length; note++)
        {
            float freq = freqs[note];
            float duration = durs[note];
            int noteSamples = (int)(samplerate * duration);
            
            for (int s = 0; s < noteSamples && sampleIdx < totalSamples; s++)
            {
                float t = (float)s / samplerate;
                
                // Смесь синусоиды и меандра для теплого ретро-звука (8-бит чиптюн)
                float sine = Mathf.Sin(2 * Mathf.PI * freq * t);
                float square = Mathf.Sign(sine);
                float val = (sine * 0.7f + square * 0.3f);
                
                // Плавное затухание в конце ноты (Envelope)
                float decay = 1f - ((float)s / noteSamples);
                samples[sampleIdx++] = val * decay * 0.18f;
            }
        }
        
        AudioClip clip = AudioClip.Create("Victory_Fanfare", totalSamples, 1, samplerate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private IEnumerator ShakeTransform(RectTransform rt, float duration, float magnitude)
    {
        Vector3 startPos = rt.anchoredPosition3D;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            rt.anchoredPosition3D = startPos + new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), 0);
            yield return null;
        }
        rt.anchoredPosition3D = startPos;
    }

    private IEnumerator BinaryConfettiRoutine(Canvas canvas, float duration)
    {
        float elapsed = 0f;
        System.Collections.Generic.List<RectTransform> activeConfetti = new System.Collections.Generic.List<RectTransform>();
        float spawnTimer = 0f;

        RectTransform canvasRt = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRt != null ? canvasRt.rect.width : 1920f;
        float canvasHeight = canvasRt != null ? canvasRt.rect.height : 1080f;

        while (elapsed < duration)
        {
            float dt = Time.unscaledDeltaTime;
            elapsed += dt;
            spawnTimer += dt;

            if (spawnTimer >= 0.04f)
            {
                spawnTimer = 0f;
                GameObject conf = new GameObject("MatrixConfetti");
                conf.transform.SetParent(canvas.transform, false);

                TextMeshProUGUI tmp = conf.AddComponent<TextMeshProUGUI>();
                tmp.text = Random.value > 0.5f ? "0" : "1";
                tmp.fontSize = Random.Range(18, 30);
                tmp.color = new Color(0f, Random.Range(0.6f, 1f), 0f, Random.Range(0.6f, 0.95f));
                tmp.fontStyle = FontStyles.Bold;

                if (directiveTextDisplay != null) tmp.font = directiveTextDisplay.font;

                RectTransform rt = conf.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                float rx = Random.Range(-canvasWidth / 2f, canvasWidth / 2f);
                rt.anchoredPosition = new Vector2(rx, 30f);

                activeConfetti.Add(rt);
            }

            float speed = 400f;
            for (int i = activeConfetti.Count - 1; i >= 0; i--)
            {
                RectTransform rt = activeConfetti[i];
                if (rt == null)
                {
                    activeConfetti.RemoveAt(i);
                    continue;
                }

                rt.anchoredPosition += new Vector2(0f, -speed * dt);
                if (rt.anchoredPosition.y < -canvasHeight - 50f)
                {
                    activeConfetti.RemoveAt(i);
                    Destroy(rt.gameObject);
                }
            }

            yield return null;
        }

        foreach (var rt in activeConfetti)
        {
            if (rt != null) Destroy(rt.gameObject);
        }
    }

    private IEnumerator FinalVictorySequence(GameObject panelObj)
    {
        // 1. Находим Clipboard и выключаем его на время анимации
        Transform clipboard = panelObj.transform.Find("Clipboard");
        if (clipboard != null) clipboard.gameObject.SetActive(false);

        // 2. Создаем терминальный контейнер
        GameObject terminalContainer = new GameObject("TerminalContainer");
        terminalContainer.transform.SetParent(panelObj.transform, false);
        
        RectTransform terminalRt = terminalContainer.AddComponent<RectTransform>();
        terminalRt.anchorMin = Vector2.zero;
        terminalRt.anchorMax = Vector2.one;
        terminalRt.sizeDelta = Vector2.zero;

        // Создаем фон терминала
        GameObject termBg = new GameObject("TerminalBg");
        termBg.transform.SetParent(terminalContainer.transform, false);
        Image bgImg = termBg.AddComponent<Image>();
        bgImg.color = new Color(0.01f, 0.03f, 0.01f, 0.98f); // Глубокий темно-зеленый терминал
        RectTransform bgRt = termBg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;

        // Зеленая обводка экрана
        UnityEngine.UI.Outline outline = termBg.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = new Color(0f, 0.8f, 0f, 0.5f);
        outline.effectDistance = new Vector2(4f, -4f);

        // Текст терминала
        GameObject termTextObj = new GameObject("TerminalText");
        termTextObj.transform.SetParent(terminalContainer.transform, false);
        RectTransform txtRt = termTextObj.AddComponent<RectTransform>();
        txtRt.anchorMin = new Vector2(0.1f, 0.15f);
        txtRt.anchorMax = new Vector2(0.9f, 0.85f);
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;

        TextMeshProUGUI terminalText = termTextObj.AddComponent<TextMeshProUGUI>();
        terminalText.fontSize = 28f;
        terminalText.color = new Color(0f, 1f, 0f, 1f); // Яркий зеленый
        terminalText.alignment = TextAlignmentOptions.TopLeft;
        terminalText.fontStyle = FontStyles.Bold;
        terminalText.enableWordWrapping = true;

        if (directiveTextDisplay != null) terminalText.font = directiveTextDisplay.font;

        // Формируем текст
        string titleStr = currentShift != null ? currentShift.shiftName.ToUpper() : "СМЕНА 7";
        string fullText = 
            $"\n" +
            $"[СИСТЕМНЫЙ СИГНАЛ: {titleStr} ЗАВЕРШЕНА]\n" +
            $"[ИНСПЕКТОР, СВЯЗЬ УСТАНОВЛЕНА...]\n\n" +
            $"АНАЛИЗ БЕЗОПАСНОСТИ СЕКТОРА:\n" +
            $"------------------------------------------------\n" +
            $"- Штрафы / Ошибки: {strikes} / 3\n" +
            $"- Граждане сектора: Безопасность подтверждена\n" +
            $"- Угрозы проникновения: Нейтрализованы\n" +
            $"------------------------------------------------\n\n" +
            $"ПОЗДРАВЛЯЕМ, ИНСПЕКТОР!\n" +
            $"Вы успешно защитили наш блокпост и жильцов.\n" +
            $"Ни один монстр не прошел.\n\n" +
            $"ЗВАНИЕ: ЖЕЛЕЗНЫЙ ЩИТ РОДИНЫ\n" +
            $"СТАТУС: ПОЧЕТНАЯ ОТСТАВКА\n\n" +
            $"[КОНЕЦ ПЕРЕДАЧИ]";

        // Печатаем посимвольно (быстрая печать)
        terminalText.text = "";
        float charDelay = 0.008f;
        int charCount = 0;
        foreach (char c in fullText)
        {
            terminalText.text += c;
            if (c != ' ' && c != '\n')
            {
                charCount++;
                if (charCount % 2 == 0)
                {
                    if (sfxAudioSource != null && phonePickupSound != null)
                    {
                        sfxAudioSource.PlayOneShot(phonePickupSound, 0.025f); 
                    }
                }
            }
            yield return new WaitForSecondsRealtime(charDelay);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // СТЕМПИНГ: Спавним МНОГО неоновых штампов "ОДОБРЕНО" по всему экрану терминала!
        int finalStampsCount = Random.Range(35, 46); // Увеличено до 35-45 штампов
        for (int i = 0; i < finalStampsCount; i++)
        {
            GameObject stampObj = new GameObject("FinalVictoryStamp_" + i);
            stampObj.transform.SetParent(terminalContainer.transform, false);
            
            RectTransform stampRt = stampObj.AddComponent<RectTransform>();
            stampRt.anchorMin = new Vector2(0.5f, 0.5f);
            stampRt.anchorMax = new Vector2(0.5f, 0.5f);
            stampRt.pivot = new Vector2(0.5f, 0.5f);
            
            // Разбрасываем по всему терминалу
            float rx = Random.Range(-550f, 550f);
            float ry = Random.Range(-400f, 400f);
            stampRt.anchoredPosition = new Vector2(rx, ry);
            
            // Случайный размер и наклон
            float randW = Random.Range(300f, 420f);
            float randH = randW * 0.375f;
            stampRt.sizeDelta = new Vector2(randW, randH);
            stampRt.localEulerAngles = new Vector3(0f, 0f, Random.Range(-35f, 35f));
            
            Image stampImg = stampObj.AddComponent<Image>();
            stampImg.color = new Color(0f, Random.Range(0.8f, 1f), 0f, Random.Range(0.7f, 0.95f));
            
            if (stampApprovedRu != null)
            {
                stampImg.sprite = stampApprovedRu;
                stampImg.preserveAspect = true;
            }
            
            // Звук удара
            if (sfxAudioSource != null && shutterCloseSound != null)
            {
                sfxAudioSource.PlayOneShot(shutterCloseSound, 0.85f);
            }
            
            // Анимация быстрого "удара" штампа
            float stampDuration = 0.08f;
            float stampElapsed = 0f;
            Vector3 startScale = Vector3.one * 4f;
            Vector3 targetScale = Vector3.one;
            
            while (stampElapsed < stampDuration)
            {
                stampElapsed += Time.unscaledDeltaTime;
                float t = stampElapsed / stampDuration;
                stampRt.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            stampRt.localScale = targetScale;
            
            yield return new WaitForSecondsRealtime(Random.Range(0.06f, 0.12f));
        }

        // Тряска экрана
        Canvas canvas = panelObj.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRt = canvas.GetComponent<RectTransform>();
            if (canvasRt != null)
            {
                yield return StartCoroutine(ShakeTransform(canvasRt, 0.25f, 15f));
            }
        }

        // Запуск бинарного конфетти (дождя матричного)
        Canvas termCanvas = terminalContainer.GetComponentInParent<Canvas>();
        if (termCanvas != null)
        {
            StartCoroutine(BinaryConfettiRoutine(termCanvas, 4.5f));
        }

        yield return new WaitForSecondsRealtime(4.5f);

        // 3. Плавно скрываем терминал и показываем бумажный планшет Victory
        float fadeDuration = 0.8f;
        float fadeElapsed = 0f;
        
        CanvasGroup cg = terminalContainer.AddComponent<CanvasGroup>();
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.unscaledDeltaTime;
            cg.alpha = 1f - (fadeElapsed / fadeDuration);
            yield return null;
        }

        Destroy(terminalContainer);

        // Включаем Clipboard обратно
        if (clipboard != null)
        {
            clipboard.gameObject.SetActive(true);
            CanvasGroup clipCg = clipboard.gameObject.GetComponent<CanvasGroup>();
            if (clipCg == null) clipCg = clipboard.gameObject.AddComponent<CanvasGroup>();
            clipCg.alpha = 0f;

            float showDuration = 0.5f;
            float showElapsed = 0f;
            while (showElapsed < showDuration)
            {
                showElapsed += Time.unscaledDeltaTime;
                clipCg.alpha = showElapsed / showDuration;
                yield return null;
            }
            clipCg.alpha = 1f;
        }
    }

    private IEnumerator SpawnMultipleVictoryStamps(GameObject panelObj)
    {
        yield return new WaitForSeconds(0.35f);

        Transform clipboard = panelObj.transform.Find("Clipboard");
        Transform stampParent = (clipboard != null) ? clipboard : panelObj.transform;

        foreach (Transform child in stampParent.GetComponentsInChildren<Transform>(true))
        {
            if (child != null && child.name.StartsWith("MultiVictoryStamp"))
            {
                Object.Destroy(child.gameObject);
            }
        }

        int stampCount = Random.Range(6, 9);
        for (int i = 0; i < stampCount; i++)
        {
            GameObject stampObj = new GameObject("MultiVictoryStamp_" + i);
            stampObj.transform.SetParent(stampParent, false);

            RectTransform stampRt = stampObj.AddComponent<RectTransform>();
            stampRt.anchorMin = new Vector2(0.5f, 0.5f);
            stampRt.anchorMax = new Vector2(0.5f, 0.5f);
            stampRt.pivot = new Vector2(0.5f, 0.5f);

            float randX = Random.Range(-180f, 180f);
            float randY = Random.Range(-210f, 210f);
            stampRt.anchoredPosition = new Vector2(randX, randY);

            float randSizeWidth = Random.Range(180f, 220f);
            float randSizeHeight = randSizeWidth * 0.375f;
            stampRt.sizeDelta = new Vector2(randSizeWidth, randSizeHeight);
            stampRt.localEulerAngles = new Vector3(0f, 0f, Random.Range(-28f, 28f));

            Image stampImg = stampObj.AddComponent<Image>();
            stampImg.color = new Color(Random.Range(0f, 0.15f), Random.Range(0.7f, 1f), Random.Range(0.1f, 0.3f), Random.Range(0.75f, 0.95f));

            if (stampApprovedRu != null)
            {
                stampImg.sprite = stampApprovedRu;
                stampImg.preserveAspect = true;
            }

            if (sfxAudioSource != null && shutterCloseSound != null)
            {
                sfxAudioSource.PlayOneShot(shutterCloseSound, 0.75f);
            }

            float stampDuration = 0.08f;
            float stampElapsed = 0f;
            Vector3 startScale = Vector3.one * 3.5f;
            Vector3 targetScale = Vector3.one;

            while (stampElapsed < stampDuration)
            {
                stampElapsed += Time.unscaledDeltaTime;
                float t = stampElapsed / stampDuration;
                stampRt.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            stampRt.localScale = targetScale;

            yield return new WaitForSecondsRealtime(Random.Range(0.06f, 0.12f));
        }
    }

    private void CreatePauseTapePlayerButton()
    {
        if (pausePanel == null) return;
        if (pausePanel.transform.Find("TapePlayerButton") != null) return;

        // Ищем кнопку выхода или настроек для шаблона стиля и положения
        Transform exitRt = pausePanel.transform.Find("ExitBtn");
        if (exitRt == null) exitRt = pausePanel.transform.Find("ExitMenuBtn");
        if (exitRt == null) exitRt = pausePanel.transform.Find("ExitButton");
        
        // Если не нашли кнопку выхода, берем любую кнопку как шаблон
        Button templateBtn = pausePanel.GetComponentInChildren<Button>(true);
        if (templateBtn == null) return;

        // Клонируем кнопку выхода (или шаблон)
        GameObject tapeBtnObj = Instantiate(templateBtn.gameObject, pausePanel.transform);
        tapeBtnObj.name = "TapePlayerButton";

        RectTransform tapeRt = tapeBtnObj.GetComponent<RectTransform>();
        
        // Если нашли кнопку выхода, позиционируем красивой стопкой
        if (exitRt != null)
        {
            RectTransform exitRect = exitRt.GetComponent<RectTransform>();
            Vector3 originalExitPos = exitRect.anchoredPosition3D;
            
            // Сдвигаем кнопку Выхода на 80 единиц вниз
            exitRect.anchoredPosition3D = originalExitPos + new Vector3(0, -80f, 0);
            
            // Нашу кнопку магнитофона ставим на старое место кнопки Выхода
            tapeRt.anchoredPosition3D = originalExitPos;
            
            // Убеждаемся в правильном порядке рендеринга (Sibling Index)
            tapeBtnObj.transform.SetSiblingIndex(exitRt.GetSiblingIndex());
        }

        // Меняем текст кнопки
        TextMeshProUGUI txt = tapeBtnObj.GetComponentInChildren<TextMeshProUGUI>(true);
        if (txt != null)
        {
            txt.text = "МАГНИТОФОН";
            txt.fontSize = 18f;
        }

        Button btn = tapeBtnObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick = new Button.ButtonClickedEvent();
            btn.onClick.AddListener(ShowPauseTapePlayerPanel);
        }

        tapeBtnObj.SetActive(true);
    }

    public void ShowPauseTapePlayerPanel()
    {
        if (sfxAudioSource != null && phonePickupSound != null) sfxAudioSource.PlayOneShot(phonePickupSound, 0.4f);

        if (pauseTapePlayerPanel != null)
        {
            Destroy(pauseTapePlayerPanel);
        }

        Canvas canvas = FindMainSceneCanvas();
        if (canvas == null) return;

        pauseTapePlayerPanel = new GameObject("RuntimeTapePlayerPanel");
        pauseTapePlayerPanel.transform.SetParent(canvas.transform, false);
        pauseTapePlayerPanel.transform.SetAsLastSibling();

        RectTransform panelRt = pauseTapePlayerPanel.AddComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(500f, 520f);
        panelRt.anchoredPosition = Vector2.zero;

        Image panelBg = pauseTapePlayerPanel.AddComponent<Image>();
        panelBg.color = new Color(0.08f, 0.08f, 0.08f, 0.98f);

        Outline panelOutline = pauseTapePlayerPanel.AddComponent<Outline>();
        panelOutline.effectColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
        panelOutline.effectDistance = new Vector2(3f, -3f);

        TMP_FontAsset activeFont = null;
        TextMeshProUGUI[] allTexts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include);
        foreach (var txt in allTexts)
        {
            if (txt != null && txt.font != null) { activeFont = txt.font; break; }
        }

        // 1. Заголовок
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(pauseTapePlayerPanel.transform, false);
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "РЕГЕНЕРАТОР ЗВУКОВОГО ЭФИРА";
        title.fontSize = 22;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        title.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) title.font = activeFont;

        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 0.88f); titleRt.anchorMax = new Vector2(1, 0.98f);
        titleRt.offsetMin = Vector2.zero; titleRt.offsetMax = Vector2.zero;

        // Находим или создаем постоянный GameObject для воспроизведения аудио в фоне
        Transform persistentTpTrans = transform.Find("PersistentTapePlayer");
        GameObject persistentTpObj;
        if (persistentTpTrans == null)
        {
            persistentTpObj = new GameObject("PersistentTapePlayer");
            persistentTpObj.transform.SetParent(transform, false);
        }
        else
        {
            persistentTpObj = persistentTpTrans.gameObject;
        }
        TapePlayer tp = persistentTpObj.GetComponent<TapePlayer>();
        if (tp == null)
        {
            tp = persistentTpObj.AddComponent<TapePlayer>();
        }

        // 2. Левая катушка
        tp.spoolLeft = CreatePauseVisualSpool(pauseTapePlayerPanel, "SpoolLeft", new Vector2(-120f, 50f));

        // 3. Правая катушка
        tp.spoolRight = CreatePauseVisualSpool(pauseTapePlayerPanel, "SpoolRight", new Vector2(120f, 50f));

        // 4. Дисплей частоты / канала
        GameObject displayObj = new GameObject("Display");
        displayObj.transform.SetParent(pauseTapePlayerPanel.transform, false);
        
        Image displayBg = displayObj.AddComponent<Image>();
        displayBg.color = new Color(0.02f, 0.05f, 0.02f, 0.95f); // ЖК дисплей
        
        Outline displayOutline = displayObj.AddComponent<Outline>();
        displayOutline.effectColor = new Color(0f, 0.6f, 0f, 0.5f);
        displayOutline.effectDistance = new Vector2(2f, -2f);

        RectTransform displayRt = displayObj.GetComponent<RectTransform>();
        displayRt.sizeDelta = new Vector2(340f, 65f);
        displayRt.anchorMin = new Vector2(0.5f, 0.5f);
        displayRt.anchorMax = new Vector2(0.5f, 0.5f);
        displayRt.anchoredPosition = new Vector2(0f, -70f);

        GameObject dispTxtObj = new GameObject("Text");
        dispTxtObj.transform.SetParent(displayObj.transform, false);
        TextMeshProUGUI dispTxt = dispTxtObj.AddComponent<TextMeshProUGUI>();
        dispTxt.text = "МАГНИТОФОН ВЫКЛЮЧЕН";
        dispTxt.color = new Color(0f, 0.9f, 0f, 1f); // Зеленый текст
        dispTxt.fontSize = 18;
        dispTxt.fontStyle = FontStyles.Bold;
        dispTxt.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) dispTxt.font = activeFont;

        RectTransform dispTxtRt = dispTxtObj.GetComponent<RectTransform>();
        dispTxtRt.anchorMin = Vector2.zero; dispTxtRt.anchorMax = Vector2.one;
        dispTxtRt.offsetMin = Vector2.zero; dispTxtRt.offsetMax = Vector2.zero;

        tp.channelTextDisplay = dispTxt;

        // 5. Светодиодный индикатор
        GameObject ledObj = new GameObject("LedIndicator");
        ledObj.transform.SetParent(pauseTapePlayerPanel.transform, false);
        
        Image ledImg = ledObj.AddComponent<Image>();
        ledImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        
        RectTransform ledRt = ledObj.GetComponent<RectTransform>();
        ledRt.sizeDelta = new Vector2(20f, 20f);
        ledRt.anchorMin = new Vector2(0.5f, 0.5f);
        ledRt.anchorMax = new Vector2(0.5f, 0.5f);
        ledRt.anchoredPosition = new Vector2(-190f, -70f);

        tp.ledIndicator = ledImg;

        // 6. Кнопка ВКЛ / ВЫКЛ
        GameObject playBtnObj = new GameObject("PlayBtn");
        playBtnObj.transform.SetParent(pauseTapePlayerPanel.transform, false);

        Image playBtnImg = playBtnObj.AddComponent<Image>();
        playBtnImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Outline playBtnOutline = playBtnObj.AddComponent<Outline>();
        playBtnOutline.effectColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        playBtnOutline.effectDistance = new Vector2(2f, -2f);

        Button playBtn = playBtnObj.AddComponent<Button>();
        playBtn.onClick.AddListener(tp.TogglePlay);

        RectTransform playBtnRt = playBtnObj.GetComponent<RectTransform>();
        playBtnRt.sizeDelta = new Vector2(160f, 50f);
        playBtnRt.anchorMin = new Vector2(0.5f, 0.5f);
        playBtnRt.anchorMax = new Vector2(0.5f, 0.5f);
        playBtnRt.anchoredPosition = new Vector2(-95f, -145f);

        GameObject playBtnTxtObj = new GameObject("Text");
        playBtnTxtObj.transform.SetParent(playBtnObj.transform, false);
        TextMeshProUGUI playBtnTxt = playBtnTxtObj.AddComponent<TextMeshProUGUI>();
        playBtnTxt.text = "СТАРТ / СТОП";
        playBtnTxt.color = Color.white;
        playBtnTxt.fontSize = 16;
        playBtnTxt.fontStyle = FontStyles.Bold;
        playBtnTxt.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) playBtnTxt.font = activeFont;

        RectTransform playBtnTxtRt = playBtnTxtObj.GetComponent<RectTransform>();
        playBtnTxtRt.anchorMin = Vector2.zero; playBtnTxtRt.anchorMax = Vector2.one;
        playBtnTxtRt.offsetMin = Vector2.zero; playBtnTxtRt.offsetMax = Vector2.zero;

        // 7. Кнопка СЛЕДУЮЩИЙ КАНАЛ
        GameObject channelBtnObj = new GameObject("ChannelBtn");
        channelBtnObj.transform.SetParent(pauseTapePlayerPanel.transform, false);
        
        Image channelBtnImg = channelBtnObj.AddComponent<Image>();
        channelBtnImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        Outline channelBtnOutline = channelBtnObj.AddComponent<Outline>();
        channelBtnOutline.effectColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        channelBtnOutline.effectDistance = new Vector2(2f, -2f);

        Button channelBtn = channelBtnObj.AddComponent<Button>();
        channelBtn.onClick.AddListener(tp.NextChannel);

        RectTransform channelBtnRt = channelBtnObj.GetComponent<RectTransform>();
        channelBtnRt.sizeDelta = new Vector2(160f, 50f);
        channelBtnRt.anchorMin = new Vector2(0.5f, 0.5f);
        channelBtnRt.anchorMax = new Vector2(0.5f, 0.5f);
        channelBtnRt.anchoredPosition = new Vector2(95f, -145f);

        GameObject channelBtnTxtObj = new GameObject("Text");
        channelBtnTxtObj.transform.SetParent(channelBtnObj.transform, false);
        TextMeshProUGUI channelBtnTxt = channelBtnTxtObj.AddComponent<TextMeshProUGUI>();
        channelBtnTxt.text = "СЛЕД. ЭФИР";
        channelBtnTxt.color = Color.white;
        channelBtnTxt.fontSize = 16;
        channelBtnTxt.fontStyle = FontStyles.Bold;
        channelBtnTxt.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) channelBtnTxt.font = activeFont;

        RectTransform channelBtnTxtRt = channelBtnTxtObj.GetComponent<RectTransform>();
        channelBtnTxtRt.anchorMin = Vector2.zero; channelBtnTxtRt.anchorMax = Vector2.one;
        channelBtnTxtRt.offsetMin = Vector2.zero; channelBtnTxtRt.offsetMax = Vector2.zero;

        // 8. Кнопка ЗАКРЫТЬ
        GameObject closeBtnObj = new GameObject("CloseBtn");
        closeBtnObj.transform.SetParent(pauseTapePlayerPanel.transform, false);
        
        Image closeBtnImg = closeBtnObj.AddComponent<Image>();
        closeBtnImg.color = new Color(0.5f, 0.15f, 0.15f, 1f); // Красная кнопка назад

        Outline closeBtnOutline = closeBtnObj.AddComponent<Outline>();
        closeBtnOutline.effectColor = new Color(0.7f, 0.2f, 0.2f, 1f);
        closeBtnOutline.effectDistance = new Vector2(2f, -2f);

        Button closeBtn = closeBtnObj.AddComponent<Button>();
        closeBtn.onClick.AddListener(HidePauseTapePlayerPanel);

        RectTransform closeBtnRt = closeBtnObj.GetComponent<RectTransform>();
        closeBtnRt.sizeDelta = new Vector2(350f, 50f);
        closeBtnRt.anchorMin = new Vector2(0.5f, 0.5f);
        closeBtnRt.anchorMax = new Vector2(0.5f, 0.5f);
        closeBtnRt.anchoredPosition = new Vector2(0f, -215f);

        GameObject closeBtnTxtObj = new GameObject("Text");
        closeBtnTxtObj.transform.SetParent(closeBtnObj.transform, false);
        TextMeshProUGUI closeBtnTxt = closeBtnTxtObj.AddComponent<TextMeshProUGUI>();
        closeBtnTxt.text = "НАЗАД В МЕНЮ ПАУЗЫ";
        closeBtnTxt.color = Color.white;
        closeBtnTxt.fontSize = 16;
        closeBtnTxt.fontStyle = FontStyles.Bold;
        closeBtnTxt.alignment = TextAlignmentOptions.Center;
        if (activeFont != null) closeBtnTxt.font = activeFont;

        RectTransform closeBtnTxtRt = closeBtnTxtObj.GetComponent<RectTransform>();
        closeBtnTxtRt.anchorMin = Vector2.zero; closeBtnTxtRt.anchorMax = Vector2.one;
        closeBtnTxtRt.offsetMin = Vector2.zero; closeBtnTxtRt.offsetMax = Vector2.zero;
        
        pauseTapePlayerPanel.SetActive(true);

        // Инициализируем UI из сохраненного состояния магнитофона
        tp.RefreshUI(tp.spoolLeft, tp.spoolRight, tp.ledIndicator, tp.channelTextDisplay);
    }

    public void HidePauseTapePlayerPanel()
    {
        if (sfxAudioSource != null && phonePickupSound != null) sfxAudioSource.PlayOneShot(phonePickupSound, 0.4f);
        if (pauseTapePlayerPanel != null)
        {
            Destroy(pauseTapePlayerPanel);
        }
    }

    private RectTransform CreatePauseVisualSpool(GameObject parent, string name, Vector2 pos)
    {
        GameObject spoolObj = new GameObject(name);
        spoolObj.transform.SetParent(parent.transform, false);
        
        RectTransform rt = spoolObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(130f, 130f);
        rt.anchoredPosition = pos;

        // Base circle
        Image img = spoolObj.AddComponent<Image>();
        img.color = new Color(0.22f, 0.22f, 0.22f, 1f);

        // Outline
        Outline outline = spoolObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        outline.effectDistance = new Vector2(2f, -2f);

        // Spokes
        for (int i = 0; i < 4; i++)
        {
            GameObject spoke = new GameObject("Spoke_" + i);
            spoke.transform.SetParent(spoolObj.transform, false);
            
            RectTransform spokeRt = spoke.AddComponent<RectTransform>();
            spokeRt.sizeDelta = new Vector2(12f, 110f);
            spokeRt.localEulerAngles = new Vector3(0, 0, i * 45f);

            Image spokeImg = spoke.AddComponent<Image>();
            spokeImg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }

        // Center hub
        GameObject hub = new GameObject("Hub");
        hub.transform.SetParent(spoolObj.transform, false);
        RectTransform hubRt = hub.AddComponent<RectTransform>();
        hubRt.sizeDelta = new Vector2(30f, 30f);
        Image hubImg = hub.AddComponent<Image>();
        hubImg.color = new Color(0.5f, 0.5f, 0.5f, 1f);

        return rt;
    }
}