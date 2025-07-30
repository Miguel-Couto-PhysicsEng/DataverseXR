using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVBarChartBasicBar : MonoBehaviour
{
    public GameObject chartPrefab;
    public string csvName;
    public Vector3 spawnWorldScale = new Vector3(0.03f, 0.03f, 0.03f);

    public void LoadCSVAndCreateChart(string csvName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>("CSVFiles/" + csvName);
        if (csvFile == null)
        {
            Debug.LogError("Ficheiro CSV n�o encontrado: " + csvName);
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV vazio ou mal formatado.");
            return;
        }

        GameObject chartGO = Instantiate(chartPrefab);

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas n�o encontrado na cena.");
            Destroy(chartGO);
            return;
        }

        chartGO.transform.SetParent(canvas.transform, false);

        Transform menu = GameObject.Find("Menu - Generate Chart Button")?.transform;
        chartGO.transform.position = menu != null
            ? new Vector3(menu.position.x + 4.5f, menu.position.y, menu.position.z)
            : new Vector3(12f, 0f, 1f);

        chartGO.transform.localScale = spawnWorldScale;

        BaseChart chart = chartGO.GetComponent<BaseChart>();
        if (chart == null)
        {
            Debug.LogError("O prefab n�o tem um componente BaseChart.");
            return;
        }

        chart.ClearData();
        chart.EnsureChartComponent<Title>().text = "Basic Bar Chart";

        // Ativar legenda
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical;
        legend.itemWidth = 5;
        legend.itemHeight = 5;
        legend.location.align = Location.Align.TopRight;
        legend.location.right = 5;
        legend.location.top = 5;

        string[] headers = lines[0].Trim().Split(';');
        int columnCount = headers.Length;

        // Criar s�ries para cada coluna Y
        for (int s = 1; s < columnCount; s++)
        {
            var serie = chart.AddSerie<Bar>(headers[s]);
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < 2) continue;

            if (!float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float xVal))
                continue;

            for (int s = 1; s < Mathf.Min(values.Length, columnCount); s++)
            {
                if (float.TryParse(values[s], NumberStyles.Any, CultureInfo.InvariantCulture, out float yVal))
                {
                    chart.AddData(s - 1, xVal, yVal);
                }
            }
        }

        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.show = true;
        xAxis.type = Axis.AxisType.Value;
        xAxis.axisLine.show = true;
        xAxis.axisTick.show = true;
        xAxis.axisLabel.show = true;

        // CORRIGIDO: Mostrar os valores no eixo Y
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.show = true;
        yAxis.type = Axis.AxisType.Value;
        yAxis.axisLine.show = true;
        yAxis.axisTick.show = true;
        yAxis.axisLabel.show = true;

    }
}

