using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

[InitializeOnLoad]
public class AutoZipBackup
{
    static AutoZipBackup()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Create ZIP Backup")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (EditorPrefs.GetBool("AutoZipBackup_v1", false)) return;
        EditorPrefs.SetBool("AutoZipBackup_v1", true);

        string projectPath = Path.GetDirectoryName(Application.dataPath);
        string zipPath = Path.Combine(projectPath, "ParallaxGame_Backup.zip");

        if (File.Exists(zipPath)) File.Delete(zipPath);

        UnityEngine.Debug.Log("[Antigravity] Создаю ZIP архив проекта...");

        string batPath = Path.Combine(projectPath, "create_backup.bat");
        
        string script = "@echo off\n" +
                        "echo =======================================\n" +
                        "echo ANTIGRAVITY: Создаем ZIP архив игры...\n" +
                        "echo Пожалуйста, подождите, это займет около минуты.\n" +
                        "echo =======================================\n" +
                        "cd /d \"" + projectPath + "\"\n" +
                        "powershell -Command \"Compress-Archive -Path 'Assets', 'ProjectSettings', 'Packages' -DestinationPath 'ParallaxGame_Backup.zip' -Force\"\n" +
                        "echo.\n" +
                        "echo Готово! Файл ParallaxGame_Backup.zip создан!\n" +
                        "pause\n";

        File.WriteAllText(batPath, script);

        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = batPath;
        psi.WorkingDirectory = projectPath;
        psi.UseShellExecute = true;

        try
        {
            Process process = Process.Start(psi);
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                if (File.Exists(batPath)) File.Delete(batPath);
                UnityEngine.Debug.Log("<color=green>[Antigravity] Архив ParallaxGame_Backup.zip успешно создан!</color>");
            };
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("[Antigravity] Ошибка при создании бэкапа: " + ex.Message);
        }
    }
}
