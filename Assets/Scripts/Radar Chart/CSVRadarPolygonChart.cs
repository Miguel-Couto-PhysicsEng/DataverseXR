using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using XCharts.Runtime;
using static XCharts.Runtime.RadarCoord;

public class CSVRadarPolygonChart : MonoBehaviour
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

        RadarChart chart = chartGO.GetComponent<RadarChart>();
        if (chart == null)
        {
            Debug.LogError("Prefab não tem RadarChart.");
            return;
        }

        // Adiciona título
        var title = chart.EnsureChartComponent<Title>();
        title.text = "Radar Polygon Chart";

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
            Debug.LogError("serie0 não encontrada no RadarChart.");
            return;
        }

        serie.ClearData();

        string[] headers = lines[0].Trim().Split(';');
        int numPlayers = headers.Length - 1; // Player 1, Player 2

        // Criar uma série para cada jogador (coluna)
        for (int j = 1; j <= numPlayers; j++)
        {
            List<double> data = new();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split(';');
                if (values.Length != headers.Length) continue;

                if (double.TryParse(values[j], NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                {
                    data.Add(val);
                }
            }
            serie.AddData(data, headers[j]); // Usa o nome do jogador como nome da série
        }

        // Substituir os indicadores pelos nomes das categorias
        var radarCoord = chart.EnsureChartComponent<RadarCoord>();
        radarCoord.indicatorList.Clear();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(';');
            if (values.Length > 0)
            {
                radarCoord.indicatorList.Add(new Indicator { name = values[0], max = 100 }); // Usa a primeira coluna como indicador
            }
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        chart.RefreshChart();
    }
}