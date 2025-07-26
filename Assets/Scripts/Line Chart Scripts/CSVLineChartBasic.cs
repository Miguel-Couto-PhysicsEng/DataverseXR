using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVLineChartBasic : MonoBehaviour
{
    public GameObject chartPrefab;
    public string csvName;

    // Posição e escala desejada
    public Vector3 spawnPosition = new Vector3(12f, 0f, 1f);
    public Vector3 spawnScale = new Vector3(0.03f, 0.03f, 0.03f);

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

        // Criar o gráfico
        GameObject chartGO = Instantiate(chartPrefab);

        
        // Colocar dentro do Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas não encontrado na cena.");
            Destroy(chartGO);
            return;
        }

        chartGO.transform.SetParent(canvas.transform, false);
        chartGO.transform.localScale = spawnScale;
        chartGO.transform.localPosition = spawnPosition; // <- respeita (12, 0, 1)



        // Adicionar dados ao gráfico
        BaseChart chart = chartGO.GetComponent<BaseChart>();
        if (chart == null)
        {
            Debug.LogError("O prefab não tem um componente BaseChart.");
            return;
        }

        // Limpar dados anteriores
        chart.ClearData();

        chart.EnsureChartComponent<Title>().text = "LineChart";
        chart.AddSerie<Line>();

        // Lê dados do CSV (assume separador ';')
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < 2) continue;

            if (float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float xVal) &&
                float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float yVal))
            {
                chart.AddData(0, xVal, yVal); // Serie 0

                var xAxis = chart.EnsureChartComponent<XAxis>();
                xAxis.show = true;
                xAxis.type = Axis.AxisType.Value;
                xAxis.splitNumber = 5;
                xAxis.axisLine.show = true;
                xAxis.axisTick.show = true;
                xAxis.axisLabel.show = true;
            }
            else
            {
                Debug.LogWarning($"Linha mal formatada no CSV: {line}");
            }
        }
    }
}



