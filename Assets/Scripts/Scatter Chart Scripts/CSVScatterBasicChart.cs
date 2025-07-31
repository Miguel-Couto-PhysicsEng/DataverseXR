using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVScatterBasicChart : MonoBehaviour
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
        title.text = "Scatter Basic Chart";

        // Legenda ajustada para coluna à direita, abaixo do título
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical; // Garante que seja uma coluna
        legend.location.align = Location.Align.TopRight; // Alinha à direita
        legend.location.top = 30; // Move para baixo, evitando o título
        legend.location.right = 5; // Mantém à direita

        // Usa as séries existentes no prefab e remove as extras
        string[] headers = lines[0].Trim().Split(';');
        int numSeries = headers.Length - 1; // Número de séries (excluindo a coluna X)

        // Remove séries extras, se existirem
        while (chart.series.Count > numSeries)
        {
            chart.RemoveSerie(chart.series.Count - 1);
        }

        // Limpa e prepara as séries usadas
        for (int j = 0; j < numSeries; j++)
        {
            if (chart.series.Count > j)
            {
                chart.series[j].ClearData();
            }
            else
            {
                Debug.LogWarning($"Série {j} não encontrada no prefab, mas não deveria ocorrer após remoção.");
                break;
            }
        }

        // Ler os dados do CSV (X comum, múltiplos Y) e substituir nas séries existentes
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length != headers.Length) continue; // Espera o mesmo número de colunas que o cabeçalho

            if (double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
            {
                for (int j = 1; j < values.Length; j++)
                {
                    if (double.TryParse(values[j], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                    {
                        Serie serie = chart.series[j - 1];
                        if (serie != null)
                        {
                            List<double> data = new List<double> { x, y }; // X, Y
                            serie.AddData(data); // Substitui os valores na série correspondente
                        }
                    }
                }
            }
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        chart.RefreshChart();
    }
}