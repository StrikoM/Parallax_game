using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

[InitializeOnLoad]
public class AutoGitBackup
{
    static AutoGitBackup()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Parallax/Save to GitHub")]
    static void RunOnce()
    {
        if (Application.isPlaying) return;
        if (EditorPrefs.GetBool("AutoGitBackup_v1", false)) return;
        EditorPrefs.SetBool("AutoGitBackup_v1", true);

        string projectPath = Path.GetDirectoryName(Application.dataPath);
        UnityEngine.Debug.Log("[Antigravity] Запускаю сохранение на GitHub...");

        // Создаем bat-файл с командами, чтобы он открылся в отдельном окне
        string batPath = Path.Combine(projectPath, "auto_github_push.bat");
        
        string script = "@echo off\n" +
                        "echo =======================================\n" +
                        "echo ANTIGRAVITY: Сохраняем проект на GitHub\n" +
                        "echo =======================================\n" +
                        "cd /d \"" + projectPath + "\"\n" +
                        "git init\n" +
                        "git add .\n" +
                        "git commit -m \"Автоматический бэкап от Antigravity\"\n" +
                        "git branch -M main\n" +
                        "git remote add origin https://github.com/StrikoM/Porallax_Games.git\n" +
                        "git push -u origin main\n" +
                        "echo.\n" +
                        "echo Сохранение завершено! Можете закрыть это окно.\n" +
                        "pause\n";

        File.WriteAllText(batPath, script);

        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = batPath;
        psi.WorkingDirectory = projectPath;
        psi.UseShellExecute = true; // Откроет видимое окно терминала

        try
        {
            Process process = Process.Start(psi);
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                if (File.Exists(batPath)) File.Delete(batPath);
                UnityEngine.Debug.Log("<color=green>[Antigravity] Сохранение на GitHub отправлено!</color>");
            };
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("[Antigravity] Ошибка при запуске Git: " + ex.Message);
        }
    }
}
