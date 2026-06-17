using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuButtonHoverEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private float lerpSpeed = 12f;

    private TextMeshProUGUI buttonText;
    private string originalText = "";
    private Color originalColor = Color.white;
    private bool hasStoredOriginals = false;

    private AudioSource audioSource;
    private static AudioClip hoverClickSound;
    private static AudioClip clickProcessSound;

    void Awake()
    {
        // Разрешаем работу в Главном Меню, а в Геймплейной сцене — только для кнопок Меню/Паузы/Настроек/Экранов окончания!
        bool isMainMenu = gameObject.scene.name == "MainMenu";
        bool isAllowedUI = transform.parent != null && 
            (transform.parent.name.Contains("Pause") || 
             transform.parent.name.Contains("Settings") || 
             transform.parent.name.Contains("Victory") || 
             transform.parent.name.Contains("GameOver") || 
             transform.parent.name.Contains("Newspaper") ||
             transform.parent.name.Contains("Clipboard") || // Clipboard is parent of end screen buttons
             transform.name.Contains("Resume") || 
             transform.name.Contains("Exit") || 
             transform.name.Contains("NextShift") || 
             transform.name.Contains("Restart") || 
             transform.name.Contains("MainMenu") || 
             transform.name.Contains("CloseNewspaper"));

        if (!isMainMenu && !isAllowedUI)
        {
            Destroy(this);
            return;
        }

        originalScale = transform.localScale;
        targetScale = originalScale;
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Создаем локальный источник звука для эффектов наведения
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D звук

        if (hoverClickSound == null)
        {
            hoverClickSound = CreateHoverSynthSound();
        }
        if (clickProcessSound == null)
        {
            clickProcessSound = CreateClickProcessSound();
        }

        // Подписываемся на клик кнопки для запуска сочной ретро-анимации (только если объект активен)
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => {
                if (this != null && gameObject != null && gameObject.activeInHierarchy && enabled)
                {
                    try
                    {
                        StartCoroutine(PlayClickAnimation());
                    }
                    catch (System.Exception)
                    {
                        // Игнорируем ошибку, если кнопка была деактивирована во время обработки клика
                    }
                }
            });
        }
    }

    void OnEnable()
    {
        transform.localScale = originalScale;
        targetScale = originalScale;
        if (buttonText != null && hasStoredOriginals && !string.IsNullOrEmpty(originalText))
        {
            buttonText.text = originalText;
            buttonText.color = originalColor;
        }
    }

    void Start()
    {
        StoreOriginalValues();
    }

    private void StoreOriginalValues()
    {
        if (hasStoredOriginals) return;

        if (buttonText != null)
        {
            originalText = buttonText.text;
            originalColor = buttonText.color;
            hasStoredOriginals = true;
        }
    }

    void Update()
    {
        // Плавная интерполяция размера кнопки
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * lerpSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StoreOriginalValues();

        targetScale = originalScale * 1.08f; // Увеличиваем на 8%

        // Воспроизводим приятный 8-битный ретро-щелчок
        if (audioSource != null && hoverClickSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f) * 0.35f;
            audioSource.PlayOneShot(hoverClickSound);
        }

        // Изменяем текст, добавляя надежные ретро-скобки (чтобы шрифт не выдавал пустые квадраты)
        if (buttonText != null)
        {
            if (string.IsNullOrEmpty(originalText))
            {
                originalText = buttonText.text;
                originalColor = buttonText.color;
                hasStoredOriginals = true;
            }

            // Добавляем ретро-скобки [] только если их там еще нет
            if (!originalText.Contains("["))
            {
                buttonText.text = "[ " + originalText + " ]";
            }
            buttonText.color = new Color(0f, 1f, 0f, 1f); // Насыщенный CRT зеленый цвет
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;

        // Возвращаем исходный текст и цвет
        if (buttonText != null && hasStoredOriginals && !string.IsNullOrEmpty(originalText))
        {
            buttonText.text = originalText;
            buttonText.color = originalColor;
        }
    }

    // Сочная ретро-анимация клика (эффект "расчета терминала")
    private IEnumerator PlayClickAnimation()
    {
        float duration = 0.35f;
        float elapsed = 0f;
        
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector3 startPos = rt.anchoredPosition3D;

        // Воспроизводим ретро-звук обработки данных
        if (audioSource != null && clickProcessSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f) * 0.65f;
            audioSource.PlayOneShot(clickProcessSound);
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            
            // Быстрое мерцание текста между зеленым и белым
            if (buttonText != null)
            {
                buttonText.color = (Random.value > 0.5f) ? new Color(0f, 1f, 0f, 1f) : Color.white;
            }

            // Ретро-тряска кнопки на CRT мониторе
            rt.anchoredPosition3D = startPos + new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0);

            yield return null;
        }

        // Восстанавливаем позицию и цвет
        rt.anchoredPosition3D = startPos;
        if (buttonText != null && hasStoredOriginals)
        {
            buttonText.color = originalColor;
        }
    }

    // Процедурный синтез очень короткого 8-битного ретро-щелчка (low-fi tick)
    private AudioClip CreateHoverSynthSound()
    {
        int samplerate = 44100;
        float freq = 800f; // Приятная частота для системного клика
        int length = (int)(samplerate * 0.04f); // 40 миллисекунд
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / samplerate;
            float fade = 1f - (t / 0.04f);
            
            // Меандр для ретро стиля
            float val = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t));
            
            samples[i] = val * 0.05f * fade;
        }

        AudioClip clip = AudioClip.Create("Hover_Click", length, 1, samplerate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // Процедурный синтез сочного звука работы процессора терминала при нажатии
    private AudioClip CreateClickProcessSound()
    {
        int samplerate = 44100;
        int length = (int)(samplerate * 0.35f); // 350 миллисекунд
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / samplerate;
            float fade = 1f - (t / 0.35f);
            
            // Быстро меняющаяся частота пилообразной волны (компьютерный просчет)
            float freq = 550f + Mathf.Sin(t * 60f) * 250f;
            
            // Генерируем меандр для классического 8-битного звучания
            float val = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t));
            
            samples[i] = val * 0.05f * fade;
        }

        AudioClip clip = AudioClip.Create("Click_Process", length, 1, samplerate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
