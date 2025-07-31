using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVCandlestickChart : MonoBehaviour
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
        Debug.Log("Procurando ficheiro em: Resources/" + fullPath);
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

        // Adiciona título
        var title = chart.EnsureChartComponent<Title>();
        title.text = "Candlestick Chart";

        // Legenda ajustada para a direita, abaixo do título
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical;
        legend.location.align = Location.Align.TopRight;
        legend.location.top = 30;
        legend.location.right = 5;

        // Usa a série existente (serie0) no prefab
        if (chart.series.Count > 0)
        {
            chart.series[0].ClearData();

            // Cria a estrutura com base nas linhas de dados (ignora o cabeçalho)
            for (int i = 1; i < lines.Length; i++)
            {
                chart.series[0].AddData(new List<double> { 0, 0, 0, 0, 0 });
            }
        }
        else
        {
            Debug.LogError("Serie0 não encontrada no prefab.");
            return;
        }

        CultureInfo culture = CultureInfo.InvariantCulture;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length == 5)
            {
                Debug.Log($"Linha {i}: Valores brutos - {string.Join(";", values)}");

                if (double.TryParse(values[0], NumberStyles.Any, culture, out double x) &&
                    double.TryParse(values[1], NumberStyles.Any, culture, out double open) &&
                    double.TryParse(values[2], NumberStyles.Any, culture, out double close) &&
                    double.TryParse(values[3], NumberStyles.Any, culture, out double low) &&
                    double.TryParse(values[4], NumberStyles.Any, culture, out double high))
                {
                    int index = i - 1;
                    if (index >= 0 && index < chart.series[0].dataCount)
                    {
                        List<double> data = new List<double> { x, open, close, low, high };
                        chart.series[0].UpdateData(index, data);
                        Debug.Log($"Linha {i}: Índice {index} → X: {x}, Open: {open}, Close: {close}, Low: {low}, High: {high}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Falha ao parsear linha {i}: {line}");
                }
            }
            else
            {
                Debug.LogWarning($"Linha {i} não contém 5 colunas: {line}");
            }
        }

        // Verificação final
        Serie serieFinal = chart.series[0];
        for (int i = 0; i < serieFinal.dataCount; i++)
        {
            double x = serieFinal.GetData(i, 0);
            double open = serieFinal.GetData(i, 1);
            double close = serieFinal.GetData(i, 2);
            double low = serieFinal.GetData(i, 3);
            double high = serieFinal.GetData(i, 4);
            Debug.Log($"Índice {i}: X: {x}, Open: {open}, Close: {close}, Low: {low}, High: {high}");
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        chart.RefreshChart();
    }
}
