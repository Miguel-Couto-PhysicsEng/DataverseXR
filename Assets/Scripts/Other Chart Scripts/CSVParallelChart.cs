using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVParallelChart : MonoBehaviour
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

        chart.EnsureChartComponent<Title>().text = "Parallel Chart";

        Serie serie = chart.series.Count > 0 ? chart.series[0] : null;
        if (serie == null)
        {
            Debug.LogError("Serie0 não encontrada.");
            return;
        }

        serie.ClearData();

        CultureInfo culture = CultureInfo.InvariantCulture;

        for (int i = 1; i < lines.Length; i++) // Ignora cabeçalho
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < 4)
            {
                Debug.LogWarning($"Linha {i} tem menos de 4 colunas: {line}");
                continue;
            }

            List<double> data = new List<double>();
            bool parseOk = true;

            for (int j = 0; j < 4; j++) // Apenas 4 primeiras colunas
            {
                if (double.TryParse(values[j], NumberStyles.Any, culture, out double val))
                    data.Add(val);
                else
                {
                    Debug.LogWarning($"Erro ao converter valor na linha {i}, coluna {j + 1}: {values[j]}");
                    parseOk = false;
                    break;
                }
            }

            if (parseOk)
            {
                string name = values.Length > 4 ? values[4].Trim() : $"Data{i - 1}";
                serie.AddData(data, name); // Adiciona os valores, com ou sem label
                Debug.Log($"Linha {i}: {string.Join(", ", data)}");
            }
        }

        for (int i = 0; i < 4; i++)
        {
            var axis = chart.GetChartComponent<ParallelAxis>(i);
            if (axis != null)
            {
                axis.minMaxType = Axis.AxisMinMaxType.Custom;
                axis.min = 0;
                axis.max = 100;
            }
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);


        chart.RefreshChart();
    }
}
