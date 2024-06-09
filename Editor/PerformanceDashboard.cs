// Assets/PerformanceToolkit/Editor/PerformanceDashboard.cs
using UnityEditor;
using UnityEngine;
using ChartAndGraph;

public class PerformanceDashboard : EditorWindow
{
    private Vector2 scrollPosition;
    private string optimizationSuggestion = "";
    private DataCollector dataCollector;
    private string reportPath = "PerformanceReport.txt";

    [MenuItem("Window/Performance Dashboard")]
    public static void ShowWindow()
    {
        GetWindow<PerformanceDashboard>("Performance Dashboard");
    }

    void OnEnable()
    {
        RefreshData();
    }

    void OnGUI()
    {
        GUILayout.Label("Performance Dashboard", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Data"))
        {
            RefreshData();
        }

        if (GUILayout.Button("Save Report"))
        {
            SaveReport();
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        DisplayFrameRate();
        DisplayMemoryUsage();
        DisplayCPUUsage();
        DisplayGPUUsage();
        DisplayOptimizationSuggestions();
        GUILayout.EndScrollView();
    }

    private void RefreshData()
    {
        optimizationSuggestion = OptimizationSuggestions.GetSuggestion(
            1.0f / Time.deltaTime,
            Profiler.GetTotalReservedMemoryLong(),
            Profiler.GetTotalAllocatedMemoryLong(),
            Profiler.GetTotalUsedMemoryLong() / (float)SystemInfo.systemMemorySize,
            Profiler.GetGpuFrameTime());
    }

    private void DisplayFrameRate()
    {
        GUILayout.Label("Frame Rate", EditorStyles.largeLabel);
        float frameRate = 1.0f / Time.deltaTime;
        GUILayout.Label($"Current Frame Rate: {frameRate:F2} FPS");

        // Add a line chart for frame rate visualization
        LineChart lineChart = CreateLineChart();
        lineChart.DataSource.AddPointToCategory("Frame Rate", Time.realtimeSinceStartup, frameRate);
        lineChart.gameObject.SetActive(true);
    }

    private void DisplayMemoryUsage()
    {
        GUILayout.Label("Memory Usage", EditorStyles.largeLabel);
        long totalMemory = Profiler.GetTotalReservedMemoryLong();
        long usedMemory = Profiler.GetTotalAllocatedMemoryLong();
        GUILayout.Label($"Total Memory: {totalMemory / (1024 * 1024)} MB");
        GUILayout.Label($"Used Memory: {usedMemory / (1024 * 1024)} MB");

        // Add a bar chart for memory usage visualization
        BarChart barChart = CreateBarChart();
        barChart.DataSource.AddCategory("Memory Usage");
        barChart.DataSource.AddBarToCategory("Memory Usage", "Total Memory", totalMemory / (1024 * 1024));
        barChart.DataSource.AddBarToCategory("Memory Usage", "Used Memory", usedMemory / (1024 * 1024));
        barChart.gameObject.SetActive(true);
    }

    private void DisplayCPUUsage()
    {
        GUILayout.Label("CPU Usage", EditorStyles.largeLabel);
        float cpuUsage = Profiler.GetTotalUsedMemoryLong() / (float)SystemInfo.systemMemorySize;
        GUILayout.Label($"CPU Usage: {cpuUsage * 100:F2}%");

        // Add a pie chart for CPU usage visualization
        PieChart pieChart = CreatePieChart();
        pieChart.DataSource.AddCategory("CPU Usage", cpuUsage * 100);
        pieChart.gameObject.SetActive(true);
    }

    private void DisplayGPUUsage()
    {
        GUILayout.Label("GPU Usage", EditorStyles.largeLabel);
        float gpuUsage = Profiler.GetGpuFrameTime();
        GUILayout.Label($"GPU Frame Time: {gpuUsage:F2} ms");

        // Add a line chart for GPU frame time visualization
        LineChart lineChart = CreateLineChart();
        lineChart.DataSource.AddPointToCategory("GPU Frame Time", Time.realtimeSinceStartup, gpuUsage);
        lineChart.gameObject.SetActive(true);
    }

    private void DisplayOptimizationSuggestions()
    {
        GUILayout.Label("Optimization Suggestions", EditorStyles.largeLabel);
        GUILayout.Label($"Suggestion: {optimizationSuggestion}");
    }

    private void SaveReport()
    {
        using (StreamWriter writer = new StreamWriter(reportPath, false))
        {
            writer.WriteLine("Performance Report");
            writer.WriteLine("------------------");

            float frameRate = 1.0f / Time.deltaTime;
            long totalMemory = Profiler.GetTotalReservedMemoryLong();
            long usedMemory = Profiler.GetTotalAllocatedMemoryLong();
            float cpuUsage = Profiler.GetTotalUsedMemoryLong() / (float)SystemInfo.systemMemorySize;
            float gpuUsage = Profiler.GetGpuFrameTime();

            writer.WriteLine($"Frame Rate: {frameRate:F2} FPS");
            writer.WriteLine($"Total Memory: {totalMemory / (1024 * 1024)} MB");
            writer.WriteLine($"Used Memory: {usedMemory / (1024 * 1024)} MB");
            writer.WriteLine($"CPU Usage: {cpuUsage * 100:F2}%");
            writer.WriteLine($"GPU Frame Time: {gpuUsage:F2} ms");
            writer.WriteLine($"Optimization Suggestion: {optimizationSuggestion}");

            writer.WriteLine("End of Report");
        }

        EditorUtility.DisplayDialog("Report Saved", "Performance report saved successfully!", "OK");
    }

    private LineChart CreateLineChart()
    {
        GameObject chartObj = new GameObject("LineChart");
        LineChart lineChart = chartObj.AddComponent<LineChart>();
        lineChart.transform.SetParent(this.transform);
        return lineChart;
    }

    private BarChart CreateBarChart()
    {
        GameObject chartObj = new GameObject("BarChart");
        BarChart barChart = chartObj.AddComponent<BarChart>();
        barChart.transform.SetParent(this.transform);
        return barChart;
    }

    private PieChart CreatePieChart()
    {
        GameObject chartObj = new GameObject("PieChart");
        PieChart pieChart = chartObj.AddComponent<PieChart>();
        pieChart.transform.SetParent(this.transform);
        return pieChart;
    }
}
