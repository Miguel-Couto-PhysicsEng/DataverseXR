using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class CSVBarChartPercentColumn : MonoBehaviour
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
            Debug.LogError("Canvas não encontrado na cena.");
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
            Debug.LogError("O prefab não tem um componente BaseChart.");
            return;
        }

        chart.ClearData();
        chart.EnsureChartComponent<Title>().text = "Percent Column Chart";

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
        int dataSeriesCount = columnCount - 1;

        // Adiciona séries empilhadas
        for (int s = 0; s < dataSeriesCount; s++)
        {
            var serie = chart.AddSerie<Bar>(headers[s + 1]);
            serie.stack = "total"; // Empilhamento para percentuais
            // Removido barWidth para adaptação automática
            // Opção manual: serie.barWidth = 15f; // Aumenta um pouco a largura (comente/descomente para testar)
        }

        // Lista para rastrear valores únicos de x
        HashSet<float> uniqueXValues = new HashSet<float>();

        // Lê os dados e normaliza para percentagens por linha
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < columnCount) continue;

            if (!float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float xVal)) continue;

            float[] yValues = new float[dataSeriesCount];
            float total = 0f;

            for (int s = 0; s < dataSeriesCount; s++)
            {
                if (float.TryParse(values[s + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out float yVal))
                {
                    yValues[s] = yVal;
                    total += yVal;
                }
                else
                {
                    yValues[s] = 0;
                }
            }

            if (total == 0f) continue;

            uniqueXValues.Add(xVal); // Adiciona xVal aos valores únicos

            for (int s = 0; s < dataSeriesCount; s++)
            {
                float percent = (yValues[s] / total) * 100f;
                Debug.Log($"x={xVal}, série={headers[s + 1]}, percent={percent}"); // Depuração
                chart.series[s].AddXYData(xVal, percent);
            }
        }

        // Configurações do eixo X (valores únicos com espaçamento inicial)
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Value;
        xAxis.axisLabel.show = true;
        xAxis.axisLine.show = true;
        xAxis.axisTick.show = true;
        xAxis.minMaxType = Axis.AxisMinMaxType.Custom; // Controle manual
        float minX = uniqueXValues.Count > 0 ? uniqueXValues.Min() : 0f;
        xAxis.min = minX > 0 ? minX - 1f : 0f; // Espaçamento inicial
        xAxis.max = uniqueXValues.Count > 0 ? uniqueXValues.Max() : 10f; // Limita ao maior x
        xAxis.boundaryGap = true; // Ativa gap automático (corrigido para bool)

        // Configurações do eixo Y (percentagem)
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;
        yAxis.axisLabel.show = true;
        yAxis.axisLine.show = true;
        yAxis.axisTick.show = true;
        yAxis.min = 0;
        yAxis.max = 100;
        yAxis.axisLabel.formatter = "{value}%";

        // Forçar atualização do gráfico
        chart.RefreshChart();
    }
}