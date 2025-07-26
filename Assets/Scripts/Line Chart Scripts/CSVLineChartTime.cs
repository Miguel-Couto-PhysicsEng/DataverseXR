using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVLineChartTime : MonoBehaviour
{
    public GameObject chartPrefab;
    public Vector3 spawnPosition = new Vector3(1.5f, 1f, 2f);
    public Vector2 chartSize = new Vector2(600, 400);

    public void LoadCSVAndCreateChart(string csvName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>("CSVFiles/" + csvName);
        if (csvFile == null)
        {
            Debug.LogError("CSV não encontrado: " + csvName);
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        List<string> xLabels = new List<string>();
        List<float> yValues = new List<float>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(';');
            if (values.Length >= 2 &&
                float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float y))
            {
                xLabels.Add(values[0]);
                yValues.Add(y);
            }
        }

        GameObject canvas = GameObject.Find("Canvas");
        GameObject chartObj = Instantiate(chartPrefab, canvas.transform);
        chartObj.name = "LineChart_Time(Clone)";

        RectTransform rt = chartObj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localPosition = new Vector3(9f, 0f, 1f);
            rt.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }

        LineChart chart = chartObj.GetComponent<LineChart>();

        for (int i = 0; i < xLabels.Count; i++)
        {
            chart.AddXAxisData(xLabels[i]);
            chart.AddData(0, yValues[i]);
        }
    }
}
