using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVPieDonut : MonoBehaviour
{
    public GameObject chartPrefab;
    public Vector3 spawnWorldScale = new Vector3(0.03f, 0.03f, 0.03f);

    public void LoadCSVAndCreateChart(string csvName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>("CSVFiles/" + csvName);
        if (csvFile == null)
        {
            Debug.LogError("Ficheiro CSV não encontrado: " + csvName);
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
            Debug.LogError("Canvas não encontrado.");
            Destroy(chartGO);
            return;
        }

        chartGO.transform.SetParent(canvas.transform, false);

        Transform menu = GameObject.Find("Menu - Generate Chart Button")?.transform;
        chartGO.transform.position = menu != null
            ? new Vector3(menu.position.x + 4.5f, menu.position.y, menu.position.z)
            : new Vector3(12f, 0f, 1f);

        chartGO.transform.localScale = spawnWorldScale;

        PieChart chart = chartGO.GetComponent<PieChart>();
        if (chart == null)
        {
            Debug.LogError("Prefab não tem PieChart.");
            return;
        }

        var title = chart.EnsureChartComponent<Title>();
        title.text = "Donut Pie Chart";

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical;
        legend.itemWidth = 5;
        legend.itemHeight = 5;
        legend.location.align = Location.Align.TopRight;
        legend.location.right = 5;
        legend.location.top = 5;

        Serie serie = chart.series.Count > 0 ? chart.series[0] : null;
        if (serie == null)
        {
            Debug.LogError("serie0 não encontrada no PieChart.");
            return;
        }

        // Definir raio interior e exterior (ex: 0.3 a 0.6 → donut)
        serie.radius = new float[] { 0.15f, 0.3f };


        serie.ClearData();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < 2) continue;

            string label = values[0];
            if (float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float val))
            {
                serie.AddData(new List<double> { val }, label);
            }
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        chart.RefreshChart();
    }
}
