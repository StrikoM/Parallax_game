using UnityEngine;
using UnityEditor;
using System.IO;

public class VisitorBuilder : EditorWindow
{
    [MenuItem("Parallax/Создать новых персонажей из спрайтов")]
    public static void CreateNewVisitors()
    {
        string folderPath = "Assets/Data/NewCharacters";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        // Описываем расширенные пресеты персонажей, точно соответствующие именам ваших .png файлов
        var presets = new[] {
            // --- Базовые спрайты игрока ---
            new {
                spriteName = "бабушкамонстр-removebg-preview",
                assetName = "ZinaidaMonster",
                dossierName = "ЗИНАИДА ПЕТРОВНА",
                dossierId = "44-88-21",
                dossierEyes = "КРАСНЫЕ",
                passportName = "ЗИНАИДА ПЕТРОВНА",
                passportId = "44-88-21",
                passportEyes = "ГОЛУБЫЕ", // ОШИБКА! В базе глаза красные (монстр)
                isMonster = true,
                isImpatient = true,
                isMimic = false,
                welcomeSpeech = "Ой, сыночек, совсем замерзла стоять у ворот бункера... Впусти старую бабушку чайку попить...",
                responseEyes = "Да это от давления глаза припухли и покраснели, сынок... Врачи говорят, возрастное это...",
                responseName = "Имя то мое, Зина я, Зинаида Петровна!",
                responseDate = "Срок годности? Да годен еще паспорт, сынок, до конца смены точно..."
            },
            new {
                spriteName = "бабушканорм-removebg-preview",
                assetName = "ZinaidaNormal",
                dossierName = "ЗИНАИДА ПЕТРОВНА",
                dossierId = "44-88-21",
                dossierEyes = "ГОЛУБЫЕ",
                passportName = "ЗИНАИДА ПЕТРОВНА",
                passportId = "44-88-21",
                passportEyes = "ГОЛУБЫЕ",
                isMonster = false,
                isImpatient = false,
                isMimic = false,
                welcomeSpeech = "Добрый вечер, сынок. Я принесла отчеты из архивного сектора бункера.",
                responseEyes = "Глаза голубые, как в молодости, сыночек.",
                responseName = "Все верно, Зинаида Петровна.",
                responseDate = "Паспорт в порядке, сыночек, проверяй тщательно."
            },
            new {
                spriteName = "дедпотный",
                assetName = "SweatyDed",
                dossierName = "СЕМЕН СЕМЕНЫЧ",
                dossierId = "77-12-34",
                dossierEyes = "КАРИЕ",
                passportName = "СЕМЕН СЕМЕНЫЧ",
                passportId = "77-12-99", // ОШИБКА! Не совпадает ID (человек нервничает)
                passportEyes = "КАРИЕ",
                isMonster = false,
                isImpatient = false,
                isMimic = false,
                welcomeSpeech = "Фух... З-здравствуйте. Едва успел добежать. За мной кто-то шел по туннелю... Впустите быстрее!",
                responseEyes = "Обычные карие глаза, дочка торопила, ничего не вижу без очков...",
                responseName = "Ой, да это дочка документы заполняла, ошиблась в циферке, наверное... Прошу вас, впустите!",
                responseDate = "Да годен он, годен! Посмотрите на печать!"
            },
            new {
                spriteName = "дедулыбается-removebg-preview",
                assetName = "SmilingDed",
                dossierName = "НИКОЛАЙ ВАСИЛЬЕВИЧ",
                dossierId = "12-34-56",
                dossierEyes = "ЖЕЛТЫЕ",
                passportName = "НИКОЛАЙ ВАСИЛЬЕВИЧ",
                passportId = "12-34-56",
                passportEyes = "СЕРЫЕ", // ОШИБКА! В досье глаза желтые
                isMonster = true,
                isImpatient = false,
                isMimic = true, // МИМИК!
                welcomeSpeech = "Приветствую... Друг... Очень рад... Видеть... Открой... Дверь... Рад видеть...",
                responseEyes = "Глаза... Серые... Серое небо... Желтое солнце... Все хорошо... Открой...",
                responseName = "Николай... Васильевич... Мое имя... Твое имя... Все хорошо...",
                responseDate = "Паспорт... Годен... Вечно годен... Впусти..."
            },
            new {
                spriteName = "деввушкамонтсрбезглаз-removebg-preview",
                assetName = "EyelessGirlMonster",
                dossierName = "АЛИСА КУПРИНА",
                dossierId = "66-66-66",
                dossierEyes = "ПУСТЫЕ",
                passportName = "АЛИСА КУПРИНА",
                passportId = "66-66-66",
                passportEyes = "СИНЫЕ", // ОШИБКА!
                isMonster = true,
                isImpatient = true,
                isMimic = false,
                welcomeSpeech = "Помогите мне... Мне так темно... В бункера погас свет... Впустите меня...",
                responseEyes = "Мои глаза? Я оставила их в глубоких шахтах бункера... Помоги мне их найти...",
                responseName = "Да... Меня зовут Алиса... Открой...",
                responseDate = "Срок паспорта не важен... Там, во тьме, времени нет..."
            },
            new {
                spriteName = "скелет-removebg-preview",
                assetName = "SkeletonMonster",
                dossierName = "СЕРГЕЙ СКЕЛЕТОВ",
                dossierId = "13-13-13",
                dossierEyes = "ПУСТЫЕ",
                passportName = "СЕРГЕЙ СКЕЛЕТОВ",
                passportId = "13-13-13",
                passportEyes = "КАРИЕ", // ОШИБКА!
                isMonster = true,
                isImpatient = false,
                isMimic = false,
                welcomeSpeech = "Добрый вечер, товарищ дежурный. Я просто шел мимо в архив. Я обычный человек, честно.",
                responseEyes = "Глаза? Просто глубокие глазницы, семейное это у нас...",
                responseName = "Да, Сергей Скелетов. Родственники тоже Скелетовы.",
                responseDate = "Паспорт настоящий, даже кожаный переплет... Ой, кожи у меня нет, переплет бумажный."
            },
            new {
                spriteName = "женшинасцанапинами-removebg-preview",
                assetName = "ScratchedWomanMonster",
                dossierName = "ЕКАТЕРИНА ВОЛКОВА",
                dossierId = "99-33-11",
                dossierEyes = "ЗЕЛЕНЫЕ",
                passportName = "ЕКАТЕРИНА ВОЛКОВА",
                passportId = "99-33-00", // ОШИБКА!
                passportEyes = "ЗЕЛЕНЫЕ",
                isMonster = true,
                isImpatient = true,
                isMimic = false,
                welcomeSpeech = "Быстрее! Закройте гермозатвор! Оно прорвалось в вентиляцию и гналось за мной! Впустите меня!",
                responseEyes = "Да зеленые у меня глаза! Посмотрите в упор!",
                responseName = "Имя Екатерина, Волкова! Что вы копаетесь, мы сейчас все погибнем!",
                responseDate = "Срок действия в порядке! Штамп стоит!"
            },
            new {
                spriteName = "наталья_рыба-removebg-preview",
                assetName = "NataliaFishMonster",
                dossierName = "НАТАЛЬЯ РЫБИНА",
                dossierId = "33-55-77",
                dossierEyes = "РЫБЬИ",
                passportName = "НАТАЛЬЯ РЫБИНА",
                passportId = "33-55-77",
                passportEyes = "КАРИЕ", // ОШИБКА!
                isMonster = true,
                isImpatient = false,
                isMimic = true,
                welcomeSpeech = "Буль... Здравствуйте. Я из гидропонного сектора бункера... Принесла свежий улов...",
                responseEyes = "Глаза большие, чтобы лучше видеть во влажных туннелях... Буль...",
                responseName = "Рыбина Наталья, все верно. Работаю с водорослями...",
                responseDate = "Вода размыла печать, но паспорт действителен..."
            },

            // --- Новые изобретенные персонажи (С заделом на будущее рисование спрайтов!) ---
            new {
                spriteName = "ученый",
                assetName = "GlitchedScientist",
                dossierName = "ЛЕОНИД СЕДОВ",
                dossierId = "88-99-00",
                dossierEyes = "СЕРЫЕ",
                passportName = "ВИКТОР МАРКОВ", // ОШИБКА! В панике взял чужой паспорт!
                passportId = "88-99-00",
                passportEyes = "СЕРЫЕ",
                isMonster = false,
                isImpatient = false,
                isMimic = false,
                welcomeSpeech = "Я проводил испытания в Секторе 4... Произошел выброс... Все мои воспоминания... стерлись. Помню только формулу...",
                responseEyes = "Серые глаза... От пыли и вспышки всё плывет перед взором...",
                responseName = "Кто я? Я Седов... Леонид... Или Марков? О боже, я взял паспорт напарника в спешке!",
                responseDate = "Паспорт... На нем печать Сектора 4, он действителен!"
            },
            new {
                spriteName = "рабочий_расплавленный",
                assetName = "MeltedWorkerMonster",
                dossierName = "ГРИГОРИЙ КЛИМОВ",
                dossierId = "55-44-33",
                dossierEyes = "РАСПЛАВЛЕННЫЕ",
                passportName = "ГРИГОРИЙ КЛИМОВ",
                passportId = "55-44-33",
                passportEyes = "ГОЛУБЫЕ", // ОШИБКА! В базе глаза расплавлены
                isMonster = true,
                isImpatient = true,
                isMimic = false,
                welcomeSpeech = "Обычная авария в цеху... Немного пролил кислоты... Ничего страшного... Впусти... Кожа заживет...",
                responseEyes = "Глаза... Голубые... Просто ожог роговицы... Открой же дверь дежурный...",
                responseName = "Климов Григорий, да... Это я... Лицо просто горит...",
                responseDate = "Паспорт в кармане робы лежал, немного обгорел, но годен..."
            },
            new {
                spriteName = "солдат_будущего",
                assetName = "ChronoSoldierAnomaly",
                dossierName = "СЕРЖАНТ ХРОНОВ",
                dossierId = "99-99-99",
                dossierEyes = "СИНЫЕ",
                passportName = "СЕРЖАНТ ХРОНОВ",
                passportId = "99-99-99",
                passportEyes = "СИНЫЕ",
                isMonster = false,
                isImpatient = false,
                isMimic = false,
                // ОШИБКА В ГОДУ ДЕЙСТВИЯ (2089 вместо 1989)
                welcomeSpeech = "Я сержант охраны из 2089 года! Произошел прорыв временного ядра! Впустите, пока реактор не рванул у вас!",
                responseEyes = "Синие глаза, военная модификация сетчатки из будущего!",
                responseName = "Сержант Хронов, жетон на броне совпадает с базой!",
                responseDate = "Мой паспорт выдан в 2080 году! У вас сейчас 1989-й? Черт, временной сдвиг оказался сильнее..."
            },
            new {
                spriteName = "девочка",
                assetName = "SilentChildAnomaly",
                dossierName = "ОБЪЕКТ 7",
                dossierId = "00-00-01",
                dossierEyes = "ПУСТЫЕ",
                passportName = "ОБЪЕКТ 7",
                passportId = "00-00-00", // ОШИБКА!
                passportEyes = "ПУСТЫЕ",
                isMonster = true,
                isImpatient = false,
                isMimic = true,
                welcomeSpeech = "(Шипение радиоприемника) ... ВНИМАНИЕ ... ОБЪЕКТ ... ЖЕЛАЕТ ... ВОЙТИ ... ОТКРОЙТЕ ... ДВЕРЬ ...",
                responseEyes = "(Помехи радио) ... ЗРИТЕЛЬНЫЙ ОРГАН ОТСУТСТВУЕТ ... ОТВЕТ ... ОТРИЦАТЕЛЬНЫЙ ...",
                responseName = "(Помехи радио) ... ИДЕНТИФИКАЦИЯ ОБЪЕКТА ... СБОЙ СИСТЕМЫ ...",
                responseDate = "(Помехи радио) ... ВРЕМЯ ИСТЕКЛО ... ВПУСТИТЕ ..."
            },
            new {
                spriteName = "лунатик",
                assetName = "SleepwalkerCitizen",
                dossierName = "АНДРЕЙ СОНИН",
                dossierId = "22-33-44",
                dossierEyes = "КАРИЕ",
                passportName = "АНДРЕЙ СОНИН",
                passportId = "22-33-44",
                passportEyes = "СЕРЫЕ", // ОШИБКА!
                isMonster = false,
                isImpatient = false,
                isMimic = false,
                welcomeSpeech = "Поезд метро уже уходит... Мама просила закрыть гермозатвор... Где мои тапочки? Впустите в спальный сектор...",
                responseEyes = "Глаза закрыты... Я сплю... Серые, наверное... Или карие... Отстаньте...",
                responseName = "Андрей Сонин... Я сплю... Пропустите к кровати...",
                responseDate = "Паспорт под подушкой лежал... Да, он действителен..."
            },
            new {
                spriteName = "диктор",
                assetName = "SmilingPropagandistMimic",
                dossierName = "ГРИГОРИЙ ВЕЩАТЕЛЬ",
                dossierId = "55-55-55",
                dossierEyes = "ЖЕЛТЫЕ",
                passportName = "ГРИГОРИЙ ВЕЩАТЕЛЬ",
                passportId = "55-55-99", // ОШИБКА!
                passportEyes = "ЖЕЛТЫЕ",
                isMonster = true,
                isImpatient = false,
                isMimic = true,
                welcomeSpeech = "Добрый вечер, товарищи! Партия заботится о нас! Все системы работают штатно! Бункер — наш общий дом!",
                responseEyes = "Глаза сияют счастьем и преданностью Бункеру! Желтый свет — свет стабильности!",
                responseName = "Каждый гражданин знает голос Григория Вещателя! Партия одобряет!",
                responseDate = "Наши документы действуют вечно во благо Великой Родины!"
            },
            new {
                spriteName = "слепой_часовой",
                assetName = "BlindGuardCitizen",
                dossierName = "СТАРШИНА СЛЕПЦОВ",
                dossierId = "11-22-33",
                dossierEyes = "СТАЛЬНЫЕ",
                passportName = "СТАРШИНА СЛЕПЦОВ",
                passportId = "11-22-33",
                passportEyes = "СЕРЫЕ", // ОШИБКА! В паспорте серые (старые), в базе стальные импланты
                isMonster = false,
                isImpatient = false,
                isMimic = false,
                welcomeSpeech = "Я отдал зрение родине, сынок. Дрон ведет меня в казармы архивного сектора. Впусти старика.",
                responseEyes = "Глаза стальные, импланты модели 'Взор-3'. В паспорте еще старые серые написаны...",
                responseName = "Слепцов моя фамилия, старшина Слепцов. Запиши в журнал.",
                responseDate = "Действителен паспорт, сынок, печать штаба дивизии стоит."
            }
        };

        int createdCount = 0;
        foreach (var p in presets)
        {
            // Ищем спрайт в Assets/Visitors
            string spritePath = $"Assets/Visitors/{p.spriteName}.png";
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (s == null)
            {
                // Резервный поиск
                string[] guids = AssetDatabase.FindAssets($"{p.spriteName} t:Sprite");
                if (guids.Length > 0)
                {
                    spritePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    s = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                }
            }

            if (s != null)
            {
                VisitorData vd = ScriptableObject.CreateInstance<VisitorData>();
                vd.visitorSprite = s;
                vd.dossierSprite = s; 
                vd.dossierName = p.dossierName;
                vd.dossierId = p.dossierId;
                vd.dossierEyes = p.dossierEyes;
                vd.dossierExpDate = "1989-12-31";

                vd.passportName = p.passportName;
                vd.passportId = p.passportId;
                vd.passportEyes = p.passportEyes;
                vd.passportExpDate = "1989-12-31";

                vd.isMonster = p.isMonster;
                vd.isImpatient = p.isImpatient;
                vd.isMimic = p.isMimic;

                vd.welcomeSpeech = p.welcomeSpeech;
                vd.responseName = p.responseName;
                vd.responseEyes = p.responseEyes;
                vd.responseDate = p.responseDate;

                AssetDatabase.CreateAsset(vd, $"{folderPath}/{p.assetName}.asset");
                createdCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Персонажи созданы", $"Успешно создано {createdCount} новых уникальных персонажей на основе ваших спрайтов в папке Assets/Data/NewCharacters!\n\nТеперь запустите генератор кампании (Parallax -> Сгенерировать 7 Дней Игры), чтобы они попали в игру!", "Отлично");
    }
}
