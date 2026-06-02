using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager instance;

    // Описание достижения
    public class Achievement
    {
        public string id;
        public string title;
        public string description;

        public Achievement(string id, string title, string description)
        {
            this.id = id;
            this.title = title;
            this.description = description;
        }
    }

    // База достижений
    private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>()
    {
        { "FIRST_SHIFT", new Achievement("FIRST_SHIFT", "ПЕРВЫЙ РУБЕЖ", "Успешно завершена первая смена.") },
        { "PERFECT_SHIFT", new Achievement("PERFECT_SHIFT", "ИДЕАЛЬНЫЙ ИНСПЕКТОР", "Смена пройдена без единой ошибки и штрафа.") },
        { "WELCOME_HOME", new Achievement("WELCOME_HOME", "ДОБРО ПОЖАЛОВАТЬ ДОМОЙ", "Допущена критическая ошибка, монстр проник в сектор.") },
        { "NERVES_OF_STEEL", new Achievement("NERVES_OF_STEEL", "СТАЛЬНЫЕ НЕРВЫ", "Атака монстра успешно отражена шокером.") },
        { "CLEAN_WINDOW", new Achievement("CLEAN_WINDOW", "КРОВАВАЯ РАБОТА", "Стекло блокпоста полностью очищено от крови.") },
        { "PANIC", new Achievement("PANIC", "ПАНИКЁР", "Отказано во въезде абсолютно честному гражданину.") },
        { "CYBORG", new Achievement("CYBORG", "КИБОРГ-УБИЙЦА", "Выявлен монстр с визуальной аномалией.") },
        { "SHERLOCK", new Achievement("SHERLOCK", "ШЕРЛОК БЛОКПОСТА", "Пойман шпион с микро-опечаткой в документах.") },
        { "TELEPHONIST", new Achievement("TELEPHONIST", "НА СВЯЗИ", "Вы выслушали все указания Управдома по телефону.") },
        { "PARANOID", new Achievement("PARANOID", "ПАРАНОЙЯ", "Допрошен абсолютно честный гражданин.") },
        { "IRON_SHIELD", new Achievement("IRON_SHIELD", "ЖЕЛЕЗНЫЙ ЩИТ", "Успешно пройдена финальная 7-я смена.") },
        { "CLOSE_CALL", new Achievement("CLOSE_CALL", "НА ВОЛОСКЕ", "Смена пройдена с ровно двумя ошибками.") }
    };

    private GameObject canvasObj;
    private RectTransform notificationPanel;
    private TextMeshProUGUI headerText;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI descText;

    private AudioSource audioSource;
    private AudioClip beepSound;
    private Queue<Achievement> notificationQueue = new Queue<Achievement>();
    private bool isDisplayingNotification = false;

    // Автоматическая инициализация при старте игры
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void AutoInitialize()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("AchievementsManager");
            go.AddComponent<AchievementsManager>();
            DontDestroyOnLoad(go);
            Debug.Log("<color=lime>[AchievementsManager] Успешно инициализирован в системе!</color>");
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Полностью 2D
        beepSound = CreateSynthSound();

        CreateUI();
    }

    // Разблокировать достижение по ID
    public void UnlockAchievement(string id)
    {
        if (!achievements.ContainsKey(id))
        {
            Debug.LogWarning($"[AchievementsManager] Достижение с ID '{id}' не найдено!");
            return;
        }

        // Если уже открыто — ничего не делаем
        if (PlayerPrefs.GetInt("Ach_" + id, 0) == 1)
        {
            return;
        }

        // Сохраняем прогресс
        PlayerPrefs.SetInt("Ach_" + id, 1);
        PlayerPrefs.Save();

        Debug.Log($"<color=lime>[AchievementsManager] ОТКРЫТО ДОСТИЖЕНИЕ: {achievements[id].title}</color>");

        // Добавляем уведомление в очередь
        notificationQueue.Enqueue(achievements[id]);

        // Если сейчас ничего не показывается, запускаем показ
        if (!isDisplayingNotification)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isDisplayingNotification = true;

        while (notificationQueue.Count > 0)
        {
            Achievement ach = notificationQueue.Dequeue();
            yield return StartCoroutine(AnimateNotification(ach));
        }

        isDisplayingNotification = false;
    }

    private IEnumerator AnimateNotification(Achievement ach)
    {
        // Динамически ищем шрифт из сцены, чтобы соответствовать стилю
        TMP_FontAsset font = FindFontInScene();
        if (font != null)
        {
            headerText.font = font;
            titleText.font = font;
            descText.font = font;
        }

        // Заполняем данные
        headerText.text = "[ ДОСТИЖЕНИЕ РАЗБЛОКИРОВАНО ]";
        titleText.text = ach.title;
        descText.text = ach.description;

        // Звуковой сигнал
        if (audioSource != null && beepSound != null)
        {
            // Подтягиваем громкость из настроек SFXVolume с коэффициентом 1.25x (для большей разборчивости)
            audioSource.volume = Mathf.Min(PlayerPrefs.GetFloat("SFXVolume", 0.5f) * 1.25f, 1f);
            audioSource.PlayOneShot(beepSound);
        }

        // Сброс позиции в невидимую зону слева (размер панели 450, сдвигаем до -470)
        notificationPanel.anchoredPosition = new Vector2(-470, -20);

        // 1. Анимация выезда слева направо
        float duration = 0.5f;
        float elapsed = 0f;
        Vector2 startPos = new Vector2(-470, -20);
        Vector2 targetPos = new Vector2(20, -20); // 20px от левого верхнего края

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Плавный выезд (Lerp с замедлением на конце)
            float t = elapsed / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease Out Sine
            notificationPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        notificationPanel.anchoredPosition = targetPos;

        // 2. Время отображения
        yield return new WaitForSeconds(3.5f);

        // 3. Анимация ухода обратно налево
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t; // Ease In Quad
            notificationPanel.anchoredPosition = Vector2.Lerp(targetPos, startPos, t);
            yield return null;
        }
        notificationPanel.anchoredPosition = startPos;

        // Небольшая задержка перед следующим
        yield return new WaitForSeconds(0.3f);
    }

    // Синтез ретро-звука (Sci-Fi double beep)
    private AudioClip CreateSynthSound()
    {
        int samplerate = 44100;
        float freq1 = 600f; // Первый низкий тон
        float freq2 = 1200f; // Второй высокий тон
        int length = (int)(samplerate * 0.35f); // 0.35 секунды
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / samplerate;
            float freq = (i < length / 2) ? freq1 : freq2;
            
            // Затухание громкости к концу тона
            float fade = 1f - (t / 0.35f);
            
            // Генерируем меандр (Square Wave) для классического ретро-звучания 8-бит
            float val = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t));
            
            // Увеличили базовую амплитуду с 0.08f до 0.16f для более сочного звука
            samples[i] = val * 0.16f * fade;
        }

        AudioClip clip = AudioClip.Create("Ach_Beep", length, 1, samplerate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // Динамический поиск шрифта в активной сцене (совместимо со всеми версиями Unity)
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

    // Процедурная генерация интерфейса (HUD Canvas)
    private void CreateUI()
    {
        // Создаем холст
        canvasObj = new GameObject("AchievementsCanvas");
        canvasObj.transform.SetParent(transform);
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Поверх абсолютно всего!

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Создаем панель уведомления (Левый Верхний Угол)
        GameObject panelObj = new GameObject("NotificationPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);

        notificationPanel = panelObj.AddComponent<RectTransform>();
        notificationPanel.anchorMin = new Vector2(0, 1);
        notificationPanel.anchorMax = new Vector2(0, 1);
        notificationPanel.pivot = new Vector2(0, 1);
        notificationPanel.sizeDelta = new Vector2(450, 115); // Сделали панель заметно больше и солиднее
        notificationPanel.anchoredPosition = new Vector2(-470, -20); // Изначально скрыта слева за экраном

        // Фон панели (Темно-зеленый полупрозрачный ретро-экран)
        Image bgImage = panelObj.AddComponent<Image>();
        bgImage.color = new Color(0.01f, 0.035f, 0.01f, 0.95f);

        // Зеленая неоновая рамка CRT
        Outline outline = panelObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 1f, 0f, 0.75f);
        outline.effectDistance = new Vector2(2f, -2f);

        // 1. Создаем Заголовок-Шапку ("ДОСТИЖЕНИЕ РАЗБЛОКИРОВАНО")
        GameObject headerObj = new GameObject("HeaderLabel");
        headerObj.transform.SetParent(panelObj.transform, false);
        RectTransform headerRt = headerObj.AddComponent<RectTransform>();
        headerRt.anchorMin = new Vector2(0, 1);
        headerRt.anchorMax = new Vector2(1, 1);
        headerRt.pivot = new Vector2(0.5f, 1);
        headerRt.anchoredPosition = new Vector2(20, -15);
        headerRt.sizeDelta = new Vector2(-40, 25);

        headerText = headerObj.AddComponent<TextMeshProUGUI>();
        headerText.fontSize = 15; // Увеличили размер шрифта
        headerText.fontStyle = FontStyles.Bold;
        headerText.color = new Color(0f, 1f, 0f, 0.95f); // Ярко-зеленый
        headerText.alignment = TextAlignmentOptions.Left;

        // 2. Создаем Название Ачивки
        GameObject titleObj = new GameObject("TitleLabel");
        titleObj.transform.SetParent(panelObj.transform, false);
        RectTransform titleRt = titleObj.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 1);
        titleRt.anchorMax = new Vector2(1, 1);
        titleRt.pivot = new Vector2(0.5f, 1);
        titleRt.anchoredPosition = new Vector2(20, -42);
        titleRt.sizeDelta = new Vector2(-40, 30);

        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 22; // Увеличили размер шрифта
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.white; // Белый
        titleText.alignment = TextAlignmentOptions.Left;

        // 3. Создаем Описание Ачивки
        GameObject descObj = new GameObject("DescLabel");
        descObj.transform.SetParent(panelObj.transform, false);
        RectTransform descRt = descObj.AddComponent<RectTransform>();
        descRt.anchorMin = new Vector2(0, 1);
        descRt.anchorMax = new Vector2(1, 1);
        descRt.pivot = new Vector2(0.5f, 1);
        descRt.anchoredPosition = new Vector2(20, -76);
        descRt.sizeDelta = new Vector2(-40, 30);

        descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.fontSize = 16; // Увеличили размер шрифта
        descText.color = new Color(0.7f, 0.9f, 0.7f, 0.85f); // Салатово-серый
        descText.alignment = TextAlignmentOptions.Left;
    }
}
