using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;

public class CSVLoader : MonoBehaviour
{
    public TextAsset csvFile;
    public BaseChart chart;

    void Start()
    {
        var lines = csvFile.text.Split('\n');
        List<string> labels = new();
        List<float> values = new();

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Trim();
            if (string.IsNullOrEmpty(row)) continue;
            var parts = row.Split(';');
            if (parts.Length < 2) continue;
            labels.Add(parts[0]);
            values.Add(float.Parse(parts[1]));
        }

        chart.ClearData();

        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        xAxis.data = labels;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;

        var serie = chart.AddSerie<Line>("Dados CSV");

        for (int i = 0; i < values.Count; i++)
        {
            serie.AddData(i, values[i]);
        }

        chart.RefreshChart();
    }
}
