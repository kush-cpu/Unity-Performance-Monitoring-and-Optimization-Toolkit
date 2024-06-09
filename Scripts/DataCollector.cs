// Assets/PerformanceToolkit/Scripts/DataCollector.cs
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

public class DataCollector : MonoBehaviour
{
    private StreamWriter dataWriter;
    private string filePath = "PerformanceData.csv";

    void Start()
    {
        if (!File.Exists(filePath))
        {
            dataWriter = new StreamWriter(filePath, false);
            dataWriter.WriteLine("FrameRate,TotalMemory,UsedMemory,CPUUsage,GPUFrameTime,OptimizationSuggestion");
        }
        else
        {
            dataWriter = new StreamWriter(filePath, true);
        }
    }

    void Update()
    {
        float frameRate = 1.0f / Time.deltaTime;
        long totalMemory = Profiler.GetTotalReservedMemoryLong();
        long usedMemory = Profiler.GetTotalAllocatedMemoryLong();
        float cpuUsage = Profiler.GetTotalUsedMemoryLong() / (float)SystemInfo.systemMemorySize;
        float gpuUsage = Profiler.GetGpuFrameTime();
        
        // Placeholder for actual optimization suggestion
        string optimizationSuggestion = "Optimize game objects and scripts";

        dataWriter.WriteLine($"{frameRate},{totalMemory},{usedMemory},{cpuUsage},{gpuUsage},{optimizationSuggestion}");
    }

    void OnDestroy()
    {
        dataWriter.Close();
    }
}
