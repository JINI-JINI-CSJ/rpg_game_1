using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class DebugLogToUIText : MonoBehaviour
{
    [Header("Target UI Text")]
    public Text logText;

    [Header("Options")]
    public int maxLineCount = 200;

    private StringBuilder logBuilder = new StringBuilder();

    void OnEnable()
    {
        logText.text = "";
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string prefix = "";

        switch (type)
        {
            case LogType.Warning:
                prefix = "[WARN] ";
                break;
            case LogType.Error:
            case LogType.Exception:
                prefix = "[ERROR] ";
                break;
        }

        logBuilder.AppendLine(prefix + logString);

        TrimLines();
        logText.text = logBuilder.ToString();
    }

    void TrimLines()
    {
        string[] lines = logBuilder.ToString().Split('\n');
        if (lines.Length <= maxLineCount) return;

        logBuilder.Clear();
        for (int i = lines.Length - maxLineCount; i < lines.Length; i++)
        {
            logBuilder.AppendLine(lines[i]);
        }
    }
}
