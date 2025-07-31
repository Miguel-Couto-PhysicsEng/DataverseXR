using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVRingChart : MonoBehaviour
{
    public GameObject chartPrefab;
    public Vector3 spawnWorldScale = new Vector3(0.03f, 0.03f, 0.03f);

    public void LoadCSVAndCreateChart(string csvName)
    {
        if (string.IsNullOrEmpty(csvName))
        {
            Debug.LogWarning("Nome de ficheiro CSV não fornecido.");
            return;
        }

        string fullPath = "CSVFiles/" + csvName;
        TextAsset csvFile = Resources.Load<TextAsset>(fullPath);
        if (csvFile == null)
        {
            Debug.LogError("Ficheiro CSV não encontrado: " + fullPath);
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

        BaseChart chart = chartGO.GetComponent<BaseChart>();
        if (chart == null)
        {
            Debug.LogError("Prefab não tem BaseChart.");
            return;
        }

        // Título e legenda
        chart.EnsureChartComponent<Title>().text = "Ring Chart";

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical;
        legend.location.align = Location.Align.TopRight;
        legend.location.top = 30;
        legend.location.right = 5;

        if (chart.series.Count == 0)
        {
            Debug.LogError("Serie0 não encontrada no prefab.");
            return;
        }

        var serie = chart.series[0];
        serie.ClearData();

        CultureInfo culture = CultureInfo.InvariantCulture;

        // Começa da linha 1, ignorando cabeçalho
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < 3)
            {
                Debug.LogWarning($"Linha {i} não tem 3 colunas: {line}");
                continue;
            }

            if (double.TryParse(values[0], NumberStyles.Any, culture, out double valor) &&
                double.TryParse(values[1], NumberStyles.Any, culture, out double max))
            {
                string label = values[2].Trim();
                serie.AddData(new List<double> { valor, max }, label);
                Debug.Log($"Linha {i}: Valor = {valor}, Max = {max}, Label = {label}");
            }
            else
            {
                Debug.LogWarning($"Erro a ler números na linha {i}: {line}");
            }
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        chart.RefreshChart();
    }
}
