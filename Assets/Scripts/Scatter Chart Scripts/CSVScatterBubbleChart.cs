using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVScatterBubbleChart : MonoBehaviour
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

        BaseChart chart = chartGO.GetComponent<BaseChart>(); // Usando BaseChart para flexibilidade
        if (chart == null)
        {
            Debug.LogError("Prefab não tem BaseChart.");
            return;
        }

        // Adiciona título
        var title = chart.EnsureChartComponent<Title>();
        title.text = "Scatter Bubble Chart";

        // Legenda ajustada para coluna à direita, abaixo do título
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical; // Garante que seja uma coluna
        legend.location.align = Location.Align.TopRight; // Alinha à direita
        legend.location.top = 30; // Move para baixo, evitando o título
        legend.location.right = 5; // Mantém à direita

        // Usa a série existente no prefab
        Serie serie = chart.series.Count > 0 ? chart.series[0] : null;
        if (serie == null)
        {
            Debug.LogError("serie0 não encontrada no chart.");
            return;
        }

        serie.ClearData();

        // Ler os dados do CSV (X, Y, Z) e substituir na série existente
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length != 3) continue; // Espera exatamente X, Y, Z

            if (double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                double.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y) &&
                double.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double z))
            {
                List<double> data = new List<double> { x, y, z }; // X, Y, Z
                serie.AddData(data); // Substitui os valores na série existente
            }
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        chart.RefreshChart();
    }
}