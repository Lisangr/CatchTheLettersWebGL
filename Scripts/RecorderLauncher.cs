#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif
using UnityEngine;
using System.Collections;
using UnityEditor;

public class RecorderLauncher : MonoBehaviour
{
#if UNITY_EDITOR
    private static RecorderLauncher _instance; // Статический экземпляр для синглтона
    public static RecorderLauncher Instance
    {
        get
        {
            // Если экземпляр синглтона ещё не существует, ищем его в сцене
            if (_instance == null)
            {
                _instance = FindObjectOfType<RecorderLauncher>();

                // Если синглтон не найден в сцене, создаём новый объект
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(RecorderLauncher).Name);
                    _instance = singletonObject.AddComponent<RecorderLauncher>();
                    DontDestroyOnLoad(singletonObject); // Убираем уничтожение объекта при смене сцен
                }
            }

            return _instance;
        }
    }

    private RecorderController _controller;

    [Header("Recorder Settings (Editor‑only)")]
    [Tooltip("Частота кадров записи")]
    public int frameRate = 30;
    [Tooltip("Путь внутри Assets без расширения, куда сохранится .mp4")]
    public string outputPath = "Assets/VideoCaptures/game_capture";
    [Tooltip("Длительность записи в секундах")]
    public float recordDuration = 29f;  // фиксируем время записи на 29 секунд

    void Awake()
    {
        // Если экземпляр синглтона уже существует и это не текущий объект, уничтожаем этот объект
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // Уничтожаем лишний экземпляр
            return;
        }

        // Если экземпляр синглтона текущий, не уничтожаем объект
        _instance = this;
        DontDestroyOnLoad(this); // Убираем уничтожение объекта при смене сцены

        // Настроим Recorder для Game View
        var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        settings.SetRecordModeToManual();

        // Создаём настройки для видеозаписи
        var movie = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        movie.name = "GamePlayRecorder";
        movie.Enabled = true;
        movie.OutputFile = outputPath;
        movie.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = Screen.width,
            OutputHeight = Screen.height
        };
        movie.FrameRate = frameRate;  // Устанавливаем frameRate
        movie.VideoBitRateMode = VideoBitrateMode.High;

        // Добавляем настройки записи в контроллер
        settings.AddRecorderSettings(movie);

        // Создаём и запускаем контроллер
        _controller = new RecorderController(settings);
        _controller.PrepareRecording();
        _controller.StartRecording();
        Debug.Log($"[Recorder] Started recording to '{outputPath}' for {recordDuration} seconds");

        // Через recordDuration секунд остановим запись
        StartCoroutine(StopRecordingAfterDelay(recordDuration));
    }

    private IEnumerator StopRecordingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Останавливаем запись через заданное время
        if (_controller != null && _controller.IsRecording())
        {
            _controller.StopRecording();
            Debug.Log($"[Recorder] Stopped recording after {delay} seconds");
        }
    }

    void OnApplicationQuit()
    {
        // На всякий случай, если приложение закроется раньше
        if (_controller != null && _controller.IsRecording())
        {
            _controller.StopRecording();
            Debug.Log("[Recorder] Stopped recording on application quit");
        }
    }
#endif
}
