using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CampaignGenerator : EditorWindow
{
    [MenuItem("Parallax/Сгенерировать 7 Дней Игры (Кампанию)")]
    public static void GenerateCampaign()
    {
        string folderPath = "Assets/Campaign";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Campaign");
        }

        string[] visitorGuids = AssetDatabase.FindAssets("t:VisitorData", new[] { "Assets" });
        List<Sprite> normalSprites = new List<Sprite>();
        List<Sprite> monsterSprites = new List<Sprite>();

        foreach (string guid in visitorGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Игнорируем то, что мы уже сгенерировали, чтобы не было рекурсии
            if (path.Contains("Campaign")) continue;

            VisitorData vd = AssetDatabase.LoadAssetAtPath<VisitorData>(path);
            if (vd != null && vd.visitorSprite != null)
            {
                if (vd.isMonster)
                    monsterSprites.Add(vd.visitorSprite);
                else
                    normalSprites.Add(vd.visitorSprite);
            }
        }

        if (normalSprites.Count == 0 && monsterSprites.Count == 0)
        {
            EditorUtility.DisplayDialog("Ошибка", "В проекте не найдено ни одного файла VisitorData с картинками!", "OK");
            return;
        }

        if (normalSprites.Count == 0) normalSprites.AddRange(monsterSprites);
        if (monsterSprites.Count == 0) monsterSprites.AddRange(normalSprites);

        string[] names = { "Иван", "Алексей", "Петр", "Сергей", "Михаил", "Дмитрий", "Олег", "Анна", "Елена", "Ольга", "Мария", "Наталья" };
        string[] surnames = { "Иванов", "Смирнов", "Попов", "Соколов", "Лебедев", "Козлов", "Новиков", "Морозов", "Волков", "Алексеев" };
        string[] eyes = { "Карие", "Голубые", "Серые", "Зеленые" };

        int globalVisitorCount = 0;

        VisitorData CreateVisitor(bool makeMonster, bool useVisualAnomaly, string folder, int shiftIndex)
        {
            VisitorData vd = ScriptableObject.CreateInstance<VisitorData>();
            string firstName = names[Random.Range(0, names.Length)];
            string lastName = surnames[Random.Range(0, surnames.Length)];
            
            vd.dossierName = firstName + " " + lastName;
            vd.dossierId = Random.Range(10, 99) + "-" + (char)Random.Range('A', 'Z') + "-" + Random.Range(10, 99);
            vd.dossierEyes = eyes[Random.Range(0, eyes.Length)];

            vd.passportName = vd.dossierName;
            vd.passportId = vd.dossierId;
            vd.passportEyes = vd.dossierEyes;
            vd.isMonster = makeMonster;

            if (makeMonster)
            {
                if (useVisualAnomaly && monsterSprites.Count > 0)
                {
                    vd.visitorSprite = monsterSprites[Random.Range(0, monsterSprites.Count)];
                }
                else
                {
                    vd.visitorSprite = normalSprites[Random.Range(0, normalSprites.Count)];
                    
                    int errorType = Random.Range(0, 3);
                    if (errorType == 0) // ФИО
                    {
                        if (shiftIndex == 0) // Смена 1: полностью другая фамилия (Обучение)
                        {
                            vd.passportName = firstName + " " + surnames[Random.Range(0, surnames.Length)];
                        }
                        else if (shiftIndex < 3) // Смены 2-3: другое имя, но та же фамилия (Средний)
                        {
                            string diffFirst = names[Random.Range(0, names.Length)];
                            while (diffFirst == firstName) diffFirst = names[Random.Range(0, names.Length)];
                            vd.passportName = diffFirst + " " + lastName;
                        }
                        else // Смены 4-7: крайне незаметная опечатка в 1 букву в имени или фамилии (Сложный/Хардкор)
                        {
                            string typoName = firstName + " " + lastName;
                            char[] chars = typoName.ToCharArray();
                            int typoPos = Random.Range(0, chars.Length);
                            while (chars[typoPos] == ' ')
                            {
                                typoPos = Random.Range(0, chars.Length);
                            }
                            
                            char tc = chars[typoPos];
                            if (tc == 'о') chars[typoPos] = 'а';
                            else if (tc == 'а') chars[typoPos] = 'о';
                            else if (tc == 'и') chars[typoPos] = 'е';
                            else if (tc == 'е') chars[typoPos] = 'и';
                            else if (tc == 'в') chars[typoPos] = 'ф';
                            else if (tc >= 'а' && tc <= 'я') chars[typoPos] = (char)(tc + 1);
                            else if (tc >= 'А' && tc <= 'Я') chars[typoPos] = (char)(tc + 1);
                            
                            vd.passportName = new string(chars);
                        }
                    }
                    else if (errorType == 1) // ID
                    {
                        if (shiftIndex == 0) // Смена 1: полностью случайный другой ID (Обучение)
                        {
                            vd.passportId = Random.Range(10, 99) + "-" + (char)Random.Range('A', 'Z') + "-" + Random.Range(10, 99);
                        }
                        else if (shiftIndex < 3) // Смены 2-3: первые две цифры другие (Средний)
                        {
                            string originalId = vd.dossierId;
                            int diffPrefix = Random.Range(10, 99);
                            vd.passportId = diffPrefix + originalId.Substring(2);
                        }
                        else // Смены 4-7: только 1 цифра или буква изменена (Сложный/Хардкор)
                        {
                            char[] originalChars = vd.dossierId.ToCharArray();
                            int changeIndex = Random.Range(0, originalChars.Length);
                            while (originalChars[changeIndex] == '-')
                            {
                                changeIndex = Random.Range(0, originalChars.Length);
                            }
                            
                            char c = originalChars[changeIndex];
                            if (c >= '0' && c <= '9')
                            {
                                char diff = (char)('0' + ((c - '0' + Random.Range(1, 9)) % 10));
                                originalChars[changeIndex] = diff;
                            }
                            else if (c >= 'A' && c <= 'Z')
                            {
                                char diff = (char)('A' + ((c - 'A' + Random.Range(1, 25)) % 26));
                                originalChars[changeIndex] = diff;
                            }
                            vd.passportId = new string(originalChars);
                        }
                    }
                    else if (errorType == 2) // Глаза
                    {
                        if (shiftIndex < 3) // Смены 1-3: полностью другой цвет глаз (Обучение/Средний)
                        {
                            vd.passportEyes = eyes[Random.Range(0, eyes.Length)];
                            if (vd.passportEyes == vd.dossierEyes) vd.passportEyes = "Черные";
                        }
                        else // Смены 4-7: опечатка в написании цвета (Сложный/Хардкор)
                        {
                            string origEyes = vd.dossierEyes;
                            if (origEyes == "Карие") vd.passportEyes = "Карея";
                            else if (origEyes == "Голубые") vd.passportEyes = "Галубые";
                            else if (origEyes == "Серые") vd.passportEyes = "Сирые";
                            else if (origEyes == "Зеленые") vd.passportEyes = "Зеленыи";
                            else vd.passportEyes = "Черные";
                        }
                    }
                }
            }
            else
            {
                vd.visitorSprite = normalSprites[Random.Range(0, normalSprites.Count)];
            }

            vd.dossierSprite = vd.visitorSprite; // Добавляем копирование спрайта для досье и паспорта!

            globalVisitorCount++;
            AssetDatabase.CreateAsset(vd, $"{folder}/GeneratedVisitor_{globalVisitorCount}.asset");
            return vd;
        }

        ShiftData[] allShifts = new ShiftData[7];

        int[] dayCounts = { 3, 4, 4, 5, 5, 6, 7 };
        int[] dayMonsters = { 1, 1, 2, 2, 3, 3, 4 };
        string[] directives = {
            "Вводный день. Сверяйте ID паспорта.",
            "Внимание на лица! Появились визуальные двойники.",
            "Проверяйте цвет глаз. Шпионы часто ошибаются.",
            "Будьте бдительны. Угроза средняя.",
            "Угроза высокая. Внимательно читайте имена.",
            "Монстры научились подделывать ID. Ищите другие ошибки.",
            "ФИНАЛЬНАЯ СМЕНА. Город рассчитывает на вас."
        };

        for (int i = 0; i < 7; i++)
        {
            ShiftData sd = ScriptableObject.CreateInstance<ShiftData>();
            sd.shiftName = "Смена " + (i + 1);
            sd.directiveText = directives[i];
            
            int total = dayCounts[i];
            int monsters = dayMonsters[i];
            sd.shiftVisitors = new VisitorData[total];

            List<bool> isMonsterList = new List<bool>();
            for (int m = 0; m < monsters; m++) isMonsterList.Add(true);
            for (int n = 0; n < (total - monsters); n++) isMonsterList.Add(false);
            
            for (int k = 0; k < isMonsterList.Count; k++) {
                bool temp = isMonsterList[k];
                int randomIndex = Random.Range(k, isMonsterList.Count);
                isMonsterList[k] = isMonsterList[randomIndex];
                isMonsterList[randomIndex] = temp;
            }

            for (int v = 0; v < total; v++)
            {
                bool isM = isMonsterList[v];
                bool isVisual = (i >= 1) && (Random.value > 0.5f);
                sd.shiftVisitors[v] = CreateVisitor(isM, isVisual, folderPath, i);
            }

            AssetDatabase.CreateAsset(sd, $"{folderPath}/Shift_{i+1}.asset");
            allShifts[i] = sd;
        }

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.shiftsDatabase = allShifts;
            EditorUtility.SetDirty(gm);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        EditorUtility.DisplayDialog("Успех!", "Сгенерировано 7 дней и " + globalVisitorCount + " уникальных посетителей!\nОни автоматически загружены в GameManager.", "ОК");
    }
}
