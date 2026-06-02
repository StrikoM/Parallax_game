using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class AutoAddNewspaperUI
{
    [MenuItem("Parallax/Добавить Газету")]
    public static void RunOnce()
    {
        if (Application.isPlaying) return;

        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        // Удаляем старую панель, если есть
        Transform old = canvas.transform.Find("NewspaperPanel");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // 1. Создаем Главную Панель Газеты
        GameObject panelObj = new GameObject("NewspaperPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        panelObj.transform.SetAsLastSibling(); // Поверх всего

        RectTransform rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(720, 820); // Слегка шире для двух колонок
        rt.localEulerAngles = new Vector3(0, 0, -1.2f); // Слегка небрежный поворот на столе инспектора

        // Состаренный газетный оттенок бумаги
        Image img = panelObj.AddComponent<Image>();
        img.color = new Color(0.88f, 0.85f, 0.76f, 1f); 

        // Массивная темная рамка в газетном стиле
        Outline panelOutline = panelObj.AddComponent<Outline>();
        panelOutline.effectColor = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        panelOutline.effectDistance = new Vector2(4f, -4f);

        // 2. Создаем Внутреннюю тонкую рамку (стиль классической печати)
        GameObject innerBorderObj = new GameObject("InnerBorder");
        innerBorderObj.transform.SetParent(panelObj.transform, false);
        RectTransform innerBorderRt = innerBorderObj.AddComponent<RectTransform>();
        innerBorderRt.anchorMin = Vector2.zero;
        innerBorderRt.anchorMax = Vector2.one;
        innerBorderRt.offsetMin = new Vector2(12, 12);
        innerBorderRt.offsetMax = new Vector2(-12, -12);
        Image innerBorderImg = innerBorderObj.AddComponent<Image>();
        innerBorderImg.color = new Color(0, 0, 0, 0); // Прозрачный центр
        Outline innerBorderOutline = innerBorderObj.AddComponent<Outline>();
        innerBorderOutline.effectColor = new Color(0.12f, 0.12f, 0.12f, 0.45f);
        innerBorderOutline.effectDistance = new Vector2(1.5f, -1.5f);

        // ================= ФИЗИЧЕСКИЕ ДЕТАЛИ ГАЗЕТЫ (СКЛАДКИ И ПЯТНА) =================
        
        // Масляное пятно / след от чашки кофе (в левом нижнем углу)
        GameObject coffeeStain = new GameObject("CoffeeStain");
        coffeeStain.transform.SetParent(panelObj.transform, false);
        RectTransform coffeeStainRt = coffeeStain.AddComponent<RectTransform>();
        coffeeStainRt.anchorMin = new Vector2(0.15f, 0.08f);
        coffeeStainRt.anchorMax = new Vector2(0.15f, 0.08f);
        coffeeStainRt.sizeDelta = new Vector2(130, 90);
        coffeeStainRt.localEulerAngles = new Vector3(0, 0, 35f);
        Image coffeeStainImg = coffeeStain.AddComponent<Image>();
        coffeeStainImg.color = new Color(0.38f, 0.28f, 0.18f, 0.06f);

        // Отпечаток грязного пальца / сажа (в правом нижнем углу)
        GameObject fingerSmudge = new GameObject("FingerSmudge");
        fingerSmudge.transform.SetParent(panelObj.transform, false);
        RectTransform smudgeRt = fingerSmudge.AddComponent<RectTransform>();
        smudgeRt.anchorMin = new Vector2(0.88f, 0.05f);
        smudgeRt.anchorMax = new Vector2(0.88f, 0.05f);
        smudgeRt.sizeDelta = new Vector2(90, 60);
        smudgeRt.localEulerAngles = new Vector3(0, 0, -15f);
        Image smudgeImg = fingerSmudge.AddComponent<Image>();
        smudgeImg.color = new Color(0.1f, 0.08f, 0.08f, 0.05f);

        // Линия горизонтального сгиба бумаги (посередине)
        GameObject horizCrease = new GameObject("HorizCrease");
        horizCrease.transform.SetParent(panelObj.transform, false);
        RectTransform horizCreaseRt = horizCrease.AddComponent<RectTransform>();
        horizCreaseRt.anchorMin = new Vector2(0f, 0.5f);
        horizCreaseRt.anchorMax = new Vector2(1f, 0.5f);
        horizCreaseRt.anchoredPosition = Vector2.zero;
        horizCreaseRt.sizeDelta = new Vector2(0, 2);
        Image horizCreaseImg = horizCrease.AddComponent<Image>();
        horizCreaseImg.color = new Color(0f, 0f, 0f, 0.09f); // Тень сгиба

        GameObject horizHighlight = new GameObject("HorizHighlight");
        horizHighlight.transform.SetParent(panelObj.transform, false);
        RectTransform horizHighlightRt = horizHighlight.AddComponent<RectTransform>();
        horizHighlightRt.anchorMin = new Vector2(0f, 0.5f);
        horizHighlightRt.anchorMax = new Vector2(1f, 0.5f);
        horizHighlightRt.anchoredPosition = new Vector2(0, -2);
        horizHighlightRt.sizeDelta = new Vector2(0, 1.5f);
        Image horizHighlightImg = horizHighlight.AddComponent<Image>();
        horizHighlightImg.color = new Color(1f, 1f, 1f, 0.18f); // Блик сгиба

        // Линия вертикального сгиба бумаги (посередине)
        GameObject vertCrease = new GameObject("VertCrease");
        vertCrease.transform.SetParent(panelObj.transform, false);
        RectTransform vertCreaseRt = vertCrease.AddComponent<RectTransform>();
        vertCreaseRt.anchorMin = new Vector2(0.5f, 0f);
        vertCreaseRt.anchorMax = new Vector2(0.5f, 1f);
        vertCreaseRt.anchoredPosition = Vector2.zero;
        vertCreaseRt.sizeDelta = new Vector2(2, 0);
        Image vertCreaseImg = vertCrease.AddComponent<Image>();
        vertCreaseImg.color = new Color(0f, 0f, 0f, 0.06f); // Тень вертикального сгиба

        GameObject vertHighlight = new GameObject("VertHighlight");
        vertHighlight.transform.SetParent(panelObj.transform, false);
        RectTransform vertHighlightRt = vertHighlight.AddComponent<RectTransform>();
        vertHighlightRt.anchorMin = new Vector2(0.5f, 0f);
        vertHighlightRt.anchorMax = new Vector2(0.5f, 1f);
        vertHighlightRt.anchoredPosition = new Vector2(2, 0);
        vertHighlightRt.sizeDelta = new Vector2(1.5f, 0);
        Image vertHighlightImg = vertHighlight.AddComponent<Image>();
        vertHighlightImg.color = new Color(1f, 1f, 1f, 0.14f); // Блик вертикального сгиба

        // ================= ШАПКА ГАЗЕТЫ (HEADER BLOCK - БЕЗ СЛОМАННЫХ СИМВОЛОВ) =================

        // Боковое левое ухо "РАБОЧИЙ КЛАСС ЕДИН"
        GameObject leftEar = new GameObject("LeftEar");
        leftEar.transform.SetParent(panelObj.transform, false);
        RectTransform leftEarRt = leftEar.AddComponent<RectTransform>();
        leftEarRt.anchorMin = new Vector2(0.04f, 0.88f);
        leftEarRt.anchorMax = new Vector2(0.17f, 0.97f);
        leftEarRt.offsetMin = Vector2.zero;
        leftEarRt.offsetMax = Vector2.zero;
        
        Image leftEarImg = leftEar.AddComponent<Image>();
        leftEarImg.color = new Color(0, 0, 0, 0);
        Outline leftEarOutline = leftEar.AddComponent<Outline>();
        leftEarOutline.effectColor = new Color(0.12f, 0.12f, 0.12f, 0.4f);
        leftEarOutline.effectDistance = new Vector2(1f, -1f);

        GameObject leftEarTxtObj = new GameObject("Text");
        leftEarTxtObj.transform.SetParent(leftEar.transform, false);
        RectTransform leftEarTxtRt = leftEarTxtObj.AddComponent<RectTransform>();
        leftEarTxtRt.anchorMin = Vector2.zero;
        leftEarTxtRt.anchorMax = Vector2.one;
        leftEarTxtRt.offsetMin = Vector2.zero;
        leftEarTxtRt.offsetMax = Vector2.zero;
        TextMeshProUGUI leftEarTxt = leftEarTxtObj.AddComponent<TextMeshProUGUI>();
        leftEarTxt.text = "<align=center><b>* * *\nРАБОЧИЙ\nКЛАСС\nЕДИН</b></align>";
        leftEarTxt.fontSize = 9;
        leftEarTxt.lineSpacing = -8;
        leftEarTxt.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Боковое правое ухо "ПОРЯДОК ДОЛГ СЛУЖБА"
        GameObject rightEar = new GameObject("RightEar");
        rightEar.transform.SetParent(panelObj.transform, false);
        RectTransform rightEarRt = rightEar.AddComponent<RectTransform>();
        rightEarRt.anchorMin = new Vector2(0.83f, 0.88f);
        rightEarRt.anchorMax = new Vector2(0.96f, 0.97f);
        rightEarRt.offsetMin = Vector2.zero;
        rightEarRt.offsetMax = Vector2.zero;
        
        Image rightEarImg = rightEar.AddComponent<Image>();
        rightEarImg.color = new Color(0, 0, 0, 0);
        Outline rightEarOutline = rightEar.AddComponent<Outline>();
        rightEarOutline.effectColor = new Color(0.12f, 0.12f, 0.12f, 0.4f);
        rightEarOutline.effectDistance = new Vector2(1f, -1f);

        GameObject rightEarTxtObj = new GameObject("Text");
        rightEarTxtObj.transform.SetParent(rightEar.transform, false);
        RectTransform rightEarTxtRt = rightEarTxtObj.AddComponent<RectTransform>();
        rightEarTxtRt.anchorMin = Vector2.zero;
        rightEarTxtRt.anchorMax = Vector2.one;
        rightEarTxtRt.offsetMin = Vector2.zero;
        rightEarTxtRt.offsetMax = Vector2.zero;
        TextMeshProUGUI rightEarTxt = rightEarTxtObj.AddComponent<TextMeshProUGUI>();
        rightEarTxt.text = "<align=center><b>* * *\nПОРЯДОК\nДОЛГ\nСЛУЖБА</b></align>";
        rightEarTxt.fontSize = 9;
        rightEarTxt.lineSpacing = -8;
        rightEarTxt.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // 3. Заголовок газеты "УТРЕННИЙ ВЕСТНИК" (Сделан шире, чтобы не переносился на 2 строчки!)
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform, false);
        RectTransform titleRt = titleObj.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.18f, 0.87f);
        titleRt.anchorMax = new Vector2(0.82f, 0.98f);
        titleRt.offsetMin = Vector2.zero;
        titleRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "УТРЕННИЙ ВЕСТНИК";
        titleText.fontSize = 42; // Чуть меньше для идеального умещения в одну строку
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        titleText.characterSpacing = 1.5f;

        // Линия под заголовком (толстая печатная полоса)
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(panelObj.transform, false);
        RectTransform lineRt = lineObj.AddComponent<RectTransform>();
        lineRt.anchorMin = new Vector2(0.04f, 0.865f);
        lineRt.anchorMax = new Vector2(0.96f, 0.865f);
        lineRt.anchoredPosition = Vector2.zero;
        lineRt.sizeDelta = new Vector2(0, 4);
        Image lineImg = lineObj.AddComponent<Image>();
        lineImg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        // 4. Метаданные газеты (Выпуск, Город, Цена - Смещены вниз для предотвращения перекрытия!)
        GameObject metaObj = new GameObject("MetaText");
        metaObj.transform.SetParent(panelObj.transform, false);
        RectTransform metaRt = metaObj.AddComponent<RectTransform>();
        metaRt.anchorMin = new Vector2(0.04f, 0.825f);
        metaRt.anchorMax = new Vector2(0.96f, 0.855f);
        metaRt.offsetMin = Vector2.zero;
        metaRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI metaText = metaObj.AddComponent<TextMeshProUGUI>();
        metaText.text = "Выпуск № 108   |   21 мая 1984 г.   |   Город Карис   |   Цена: 5 копеек";
        metaText.fontSize = 13;
        metaText.fontStyle = FontStyles.Bold;
        metaText.alignment = TextAlignmentOptions.Center;
        metaText.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        // Вторая линия под метаданными (тонкая линия - Смещена вниз!)
        GameObject line2Obj = new GameObject("Line2");
        line2Obj.transform.SetParent(panelObj.transform, false);
        RectTransform line2Rt = line2Obj.AddComponent<RectTransform>();
        line2Rt.anchorMin = new Vector2(0.04f, 0.82f);
        line2Rt.anchorMax = new Vector2(0.96f, 0.82f);
        line2Rt.anchoredPosition = Vector2.zero;
        line2Rt.sizeDelta = new Vector2(0, 1.5f);
        Image line2Img = line2Obj.AddComponent<Image>();
        line2Img.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        // ================= КОНТЕНТ (ДВУХКОЛОНОЧНЫЙ МАКЕТ) =================

        // 5. Динамический Заголовок Новости
        GameObject headlineObj = new GameObject("HeadlineText");
        headlineObj.transform.SetParent(panelObj.transform, false);
        RectTransform headRt = headlineObj.AddComponent<RectTransform>();
        headRt.anchorMin = new Vector2(0.04f, 0.77f);
        headRt.anchorMax = new Vector2(0.60f, 0.81f);
        headRt.offsetMin = Vector2.zero;
        headRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI headText = headlineObj.AddComponent<TextMeshProUGUI>();
        headText.text = "НОВОСТИ ДНЯ";
        headText.fontSize = 32;
        headText.fontStyle = FontStyles.Bold;
        headText.alignment = TextAlignmentOptions.Center;
        headText.color = new Color(0.55f, 0.12f, 0.12f, 1f); // Насыщенный печатный бордово-красный

        // 6. Левая колонка - Динамический Текст Новости (ГИГАНТСКИЕ ЧИТАЕМЫЕ БУКВЫ!)
        GameObject bodyObj = new GameObject("BodyText");
        bodyObj.transform.SetParent(panelObj.transform, false);
        RectTransform bodyRt = bodyObj.AddComponent<RectTransform>();
        bodyRt.anchorMin = new Vector2(0.04f, 0.49f);
        bodyRt.anchorMax = new Vector2(0.60f, 0.76f);
        bodyRt.offsetMin = Vector2.zero;
        bodyRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI bodyText = bodyObj.AddComponent<TextMeshProUGUI>();
        bodyText.text = "В городе всё спокойно.";
        bodyText.fontSize = 27; // Увеличено еще сильнее с 23 до 27 для максимальной четкости!
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        bodyText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        bodyText.textWrappingMode = TextWrappingModes.Normal;
        bodyText.lineSpacing = 10f;

        // Разделительная горизонтальная линия в левой колонке
        GameObject leftDivider = new GameObject("LeftDivider");
        leftDivider.transform.SetParent(panelObj.transform, false);
        RectTransform leftDividerRt = leftDivider.AddComponent<RectTransform>();
        leftDividerRt.anchorMin = new Vector2(0.04f, 0.47f);
        leftDividerRt.anchorMax = new Vector2(0.60f, 0.47f);
        leftDividerRt.anchoredPosition = Vector2.zero;
        leftDividerRt.sizeDelta = new Vector2(0, 1.5f);
        Image leftDividerImg = leftDivider.AddComponent<Image>();
        leftDividerImg.color = new Color(0.1f, 0.1f, 0.1f, 0.3f);

        // ================= ЗАПОЛНЕНИЕ ПУСТОТЫ (ФОТО ДНЯ И ДЕКРЕТ) =================

        // Контейнер «Фото дня»
        GameObject photoBox = new GameObject("PhotoBox");
        photoBox.transform.SetParent(panelObj.transform, false);
        RectTransform photoBoxRt = photoBox.AddComponent<RectTransform>();
        photoBoxRt.anchorMin = new Vector2(0.04f, 0.12f);
        photoBoxRt.anchorMax = new Vector2(0.31f, 0.44f);
        photoBoxRt.offsetMin = Vector2.zero;
        photoBoxRt.offsetMax = Vector2.zero;

        Image photoBg = photoBox.AddComponent<Image>();
        photoBg.color = new Color(0.18f, 0.18f, 0.18f, 1f); // Угольно-черный газетный фотоблок
        Outline photoOutline = photoBox.AddComponent<Outline>();
        photoOutline.effectColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        photoOutline.effectDistance = new Vector2(1.5f, -1.5f);

        // Элемент пиксель-арта внутри фото (Используем текст МГБ вместо звезды-квадрата)
        GameObject photoGraphic = new GameObject("PhotoGraphic");
        photoGraphic.transform.SetParent(photoBox.transform, false);
        RectTransform photoGraphicRt = photoGraphic.AddComponent<RectTransform>();
        photoGraphicRt.anchorMin = new Vector2(0, 0.2f);
        photoGraphicRt.anchorMax = new Vector2(1, 1);
        photoGraphicRt.offsetMin = Vector2.zero;
        photoGraphicRt.offsetMax = Vector2.zero;
        TextMeshProUGUI photoGraphicTxt = photoGraphic.AddComponent<TextMeshProUGUI>();
        photoGraphicTxt.text = "<align=center><size=32><color=#333333><b>МГБ</b></color></size>\n<size=12><color=#666666><b>СЕКТОР-4</b></color></size></align>";
        photoGraphicTxt.fontSize = 14;
        photoGraphicTxt.color = Color.white;

        // Подпись под фото
        GameObject photoCaption = new GameObject("PhotoCaption");
        photoCaption.transform.SetParent(photoBox.transform, false);
        RectTransform photoCaptionRt = photoCaption.AddComponent<RectTransform>();
        photoCaptionRt.anchorMin = new Vector2(0, 0);
        photoCaptionRt.anchorMax = new Vector2(1, 0.2f);
        photoCaptionRt.offsetMin = Vector2.zero;
        photoCaptionRt.offsetMax = Vector2.zero;
        TextMeshProUGUI photoCaptionTxt = photoCaption.AddComponent<TextMeshProUGUI>();
        photoCaptionTxt.text = "<align=center><i>Рис. 1. Заводской КПП №3</i></align>";
        photoCaptionTxt.fontSize = 11;
        photoCaptionTxt.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);

        // Блок «Официальный Указ Комендатуры»
        GameObject decreeBox = new GameObject("DecreeBox");
        decreeBox.transform.SetParent(panelObj.transform, false);
        RectTransform decreeRt = decreeBox.AddComponent<RectTransform>();
        decreeRt.anchorMin = new Vector2(0.33f, 0.12f);
        decreeRt.anchorMax = new Vector2(0.60f, 0.44f);
        decreeRt.offsetMin = Vector2.zero;
        decreeRt.offsetMax = Vector2.zero;

        Image decreeBg = decreeBox.AddComponent<Image>();
        decreeBg.color = new Color(0, 0, 0, 0); // Прозрачный центр
        Outline decreeOutline = decreeBox.AddComponent<Outline>();
        decreeOutline.effectColor = new Color(0.12f, 0.12f, 0.12f, 0.7f);
        decreeOutline.effectDistance = new Vector2(1.5f, -1.5f);

        GameObject decreeTxtObj = new GameObject("Text");
        decreeTxtObj.transform.SetParent(decreeBox.transform, false);
        RectTransform decreeTxtRt = decreeTxtObj.AddComponent<RectTransform>();
        decreeTxtRt.anchorMin = Vector2.zero;
        decreeTxtRt.anchorMax = Vector2.one;
        decreeTxtRt.offsetMin = new Vector2(8, 8);
        decreeTxtRt.offsetMax = new Vector2(-8, -8);
        
        TextMeshProUGUI decreeTxt = decreeTxtObj.AddComponent<TextMeshProUGUI>();
        decreeTxt.text = "<b><align=center><color=#901515><size=15>УКАЗ № 42</size></color></align></b>\n" +
                          "<size=13.5><line-height=110%>Гражданам предписывается сдать любые личные радиоприёмники в фонд Комендатуры до пятницы. Саботаж карается трибуналом.</size>";
        decreeTxt.fontSize = 12.5f;
        decreeTxt.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        // Разделительная вертикальная линия между колонками
        GameObject dividerObj = new GameObject("DividerLine");
        dividerObj.transform.SetParent(panelObj.transform, false);
        RectTransform dividerRt = dividerObj.AddComponent<RectTransform>();
        dividerRt.anchorMin = new Vector2(0.63f, 0.12f);
        dividerRt.anchorMax = new Vector2(0.63f, 0.82f);
        dividerRt.anchoredPosition = Vector2.zero;
        dividerRt.sizeDelta = new Vector2(2, 0);
        Image dividerImg = dividerObj.AddComponent<Image>();
        dividerImg.color = new Color(0.1f, 0.1f, 0.1f, 0.35f);

        // 7. Правая колонка - Пропаганда, Погода, Объявления (ОЧЕНЬ КРУПНЫЙ ШРИФТ!)
        GameObject rightColObj = new GameObject("RightColumnText");
        rightColObj.transform.SetParent(panelObj.transform, false);
        RectTransform rightColRt = rightColObj.AddComponent<RectTransform>();
        rightColRt.anchorMin = new Vector2(0.66f, 0.12f);
        rightColRt.anchorMax = new Vector2(0.96f, 0.82f);
        rightColRt.offsetMin = Vector2.zero;
        rightColRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI rightColText = rightColObj.AddComponent<TextMeshProUGUI>();
        rightColText.text = "<color=#901515><b>БДИТЕЛЬНОСТЬ!</b></color>\n" +
                            "Гражданин, помни: бдительность — твой священный долг перед Партией. Сообщай о любых подозрительных лицах.\n\n" +
                            "<b><color=#222222>ПОГОДА В КАРИСЕ</color></b>\n" +
                            "Густой смог, северный ветер. Токсичные осадки: в пределах нормы (15%). Носите маски К-3.\n\n" +
                            "<b><color=#222222>ОБЪЯВЛЕНИЯ</color></b>\n" +
                            "• Продам шинель армейскую, б/у, разм. 50. Барак №3.\n" +
                            "• Утерян партийный билет №382. Вернуть коменданту.";
        rightColText.fontSize = 19f; // Увеличено еще сильнее с 16.5 до 19 для идеальной читаемости!
        rightColText.alignment = TextAlignmentOptions.TopLeft;
        rightColText.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        rightColText.textWrappingMode = TextWrappingModes.Normal;
        rightColText.lineSpacing = 5f;

        // ================= ОФИЦИАЛЬНЫЕ ШТАМПЫ И ПОМЕТКИ =================

        // Рукописная синяя заметка от руки (в левом верхнем углу под шапкой)
        GameObject handwrittenNote = new GameObject("HandwrittenNote");
        handwrittenNote.transform.SetParent(panelObj.transform, false);
        RectTransform handRt = handwrittenNote.AddComponent<RectTransform>();
        handRt.anchorMin = new Vector2(0.24f, 0.81f);
        handRt.anchorMax = new Vector2(0.24f, 0.81f);
        handRt.anchoredPosition = Vector2.zero;
        handRt.sizeDelta = new Vector2(250, 30);
        handRt.localEulerAngles = new Vector3(0, 0, -4f);
        TextMeshProUGUI handTxt = handwrittenNote.AddComponent<TextMeshProUGUI>();
        handTxt.text = "<i>Сверить списки! — Майор П.</i>";
        handTxt.fontSize = 15;
        handTxt.color = new Color(0.12f, 0.32f, 0.75f, 0.85f); 

        // 8. Наклонный красный штамп цензуры (Без сломанных unicode символов)
        GameObject stampObj = new GameObject("CensorStamp");
        stampObj.transform.SetParent(panelObj.transform, false);
        RectTransform stampRt = stampObj.AddComponent<RectTransform>();
        stampRt.anchorMin = new Vector2(0.85f, 0.82f); // Слегка опущен, чтобы не перекрывать шапку!
        stampRt.anchorMax = new Vector2(0.85f, 0.82f);
        stampRt.anchoredPosition = Vector2.zero;
        stampRt.sizeDelta = new Vector2(160, 48);
        stampRt.localEulerAngles = new Vector3(0, 0, 14f);

        Image stampImg = stampObj.AddComponent<Image>();
        stampImg.color = new Color(0.75f, 0.12f, 0.12f, 0.08f);
        Outline stampOutline = stampObj.AddComponent<Outline>();
        stampOutline.effectColor = new Color(0.75f, 0.12f, 0.12f, 0.85f);
        stampOutline.effectDistance = new Vector2(2.5f, -2.5f);

        GameObject stampTxtObj = new GameObject("StampText");
        stampTxtObj.transform.SetParent(stampObj.transform, false);
        RectTransform stampTxtRt = stampTxtObj.AddComponent<RectTransform>();
        stampTxtRt.anchorMin = Vector2.zero;
        stampTxtRt.anchorMax = Vector2.one;
        stampTxtRt.offsetMin = Vector2.zero;
        stampTxtRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI stampTxt = stampTxtObj.AddComponent<TextMeshProUGUI>();
        stampTxt.text = "* ПРОЙДЕНО *\nЦЕНЗУРОЙ";
        stampTxt.fontSize = 13;
        stampTxt.fontStyle = FontStyles.Bold;
        stampTxt.alignment = TextAlignmentOptions.Center;
        stampTxt.color = new Color(0.75f, 0.12f, 0.12f, 0.85f);
        stampTxt.lineSpacing = -10;

        // ВТОРОЙ ШТАМП: Синий штамп "КПП КАРИС / РАЗРЕШЕНО" в правом нижнем углу (Без сломанных unicode символов)
        GameObject kppStampObj = new GameObject("KppStamp");
        kppStampObj.transform.SetParent(panelObj.transform, false);
        RectTransform kppStampRt = kppStampObj.AddComponent<RectTransform>();
        kppStampRt.anchorMin = new Vector2(0.80f, 0.20f);
        kppStampRt.anchorMax = new Vector2(0.80f, 0.20f);
        kppStampRt.anchoredPosition = Vector2.zero;
        kppStampRt.sizeDelta = new Vector2(150, 48);
        kppStampRt.localEulerAngles = new Vector3(0, 0, -18f);

        Image kppStampImg = kppStampObj.AddComponent<Image>();
        kppStampImg.color = new Color(0.12f, 0.22f, 0.7f, 0.07f);
        Outline kppStampOutline = kppStampObj.AddComponent<Outline>();
        kppStampOutline.effectColor = new Color(0.12f, 0.22f, 0.7f, 0.8f);
        kppStampOutline.effectDistance = new Vector2(2f, -2f);

        GameObject kppStampTxtObj = new GameObject("StampText");
        kppStampTxtObj.transform.SetParent(kppStampObj.transform, false);
        RectTransform kppStampTxtRt = kppStampTxtObj.AddComponent<RectTransform>();
        kppStampTxtRt.anchorMin = Vector2.zero;
        kppStampTxtRt.anchorMax = Vector2.one;
        kppStampTxtRt.offsetMin = Vector2.zero;
        kppStampTxtRt.offsetMax = Vector2.zero;

        TextMeshProUGUI kppStampTxt = kppStampTxtObj.AddComponent<TextMeshProUGUI>();
        kppStampTxt.text = "* КОНТРОЛЬ КПП *\nРАЗРЕШЕНО";
        kppStampTxt.fontSize = 11;
        kppStampTxt.fontStyle = FontStyles.Bold;
        kppStampTxt.alignment = TextAlignmentOptions.Center;
        kppStampTxt.color = new Color(0.12f, 0.22f, 0.7f, 0.8f);
        kppStampTxt.lineSpacing = -8;

        // ================= КНОПКА ЗАКРЫТИЯ (ПРИНЯТЬ К СВЕДЕНИЮ - БЕЗ СЛОМАННЫХ СИМВОЛОВ) =================

        // 10. Кнопка "Принять к сведению" (прямой потомок для совместимости с GameManager)
        GameObject btnObj = new GameObject("CloseNewspaperBtn");
        btnObj.transform.SetParent(panelObj.transform, false);
        RectTransform btnRt = btnObj.AddComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0);
        btnRt.anchorMax = new Vector2(0.5f, 0);
        btnRt.anchoredPosition = new Vector2(0, 52);
        btnRt.sizeDelta = new Vector2(400, 56);
        
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.15f, 0.15f, 0.16f, 1f); // Угольно-черный стальной блок
        
        Outline btnOutline = btnObj.AddComponent<Outline>();
        btnOutline.effectColor = new Color(0.35f, 0.35f, 0.35f, 0.9f);
        btnOutline.effectDistance = new Vector2(2.5f, -2.5f);
        
        btnObj.AddComponent<Button>();

        GameObject btnTxtObj = new GameObject("Text");
        btnTxtObj.transform.SetParent(btnObj.transform, false);
        RectTransform btnTxtRt = btnTxtObj.AddComponent<RectTransform>();
        btnTxtRt.anchorMin = Vector2.zero;
        btnTxtRt.anchorMax = Vector2.one;
        btnTxtRt.offsetMin = Vector2.zero;
        btnTxtRt.offsetMax = Vector2.zero;
        
        TextMeshProUGUI btnTxt = btnTxtObj.AddComponent<TextMeshProUGUI>();
        btnTxt.text = "*** ПРИНЯТЬ К СВЕДЕНИЮ ***"; // Заменена unicode-звезда на ASCII звездочки
        btnTxt.fontSize = 18;
        btnTxt.fontStyle = FontStyles.Bold;
        btnTxt.alignment = TextAlignmentOptions.Center;
        btnTxt.color = new Color(0.92f, 0.9f, 0.84f, 1f);
        btnTxt.characterSpacing = 1.5f;

        // Скрываем газету по умолчанию
        panelObj.SetActive(false);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[Antigravity] Размеры шрифтов газеты еще раз значительно увеличены, убраны сломанные глифы!");
    }
}
