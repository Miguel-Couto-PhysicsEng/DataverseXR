using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class CSVBarChartZebraColumn : MonoBehaviour
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
        chart.EnsureChartComponent<Title>().text = "Zebra Column Chart";

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

        // Adiciona séries empilhadas com padrões visuais
        for (int s = 0; s < dataSeriesCount; s++)
        {
            var serie = chart.AddSerie<Bar>(headers[s + 1]);
            serie.stack = "total"; // Empilhamento para valores brutos
            serie.itemStyle.opacity = 0.8f;

            // Define padrões visuais para cada série (6 estilos que se repetem)
            switch (s % 6)
            {
                case 0: // Zebra (listras horizontais simuladas)
                    serie.itemStyle.color = new Color(0.9f, 0.9f, 0.9f, 0.8f); // Cinza claro translúcido
                    serie.itemStyle.borderColor = new Color(0.2f, 0.2f, 0.2f, 0.9f); // Cinza escuro
                    serie.itemStyle.borderWidth = 2.5f;
                    Debug.Log($"Série {headers[s + 1]} configurada com padrão zebra (listras)");
                    break;
                case 1: // Bolinhas (pontos distribuídos)
                    serie.itemStyle.color = new Color(1f, 0.5f, 0.5f, 0.6f); // Rosa claro translúcido
                    serie.itemStyle.borderColor = new Color(1f, 0f, 0f, 0.8f); // Vermelho
                    serie.itemStyle.borderWidth = 1f;
                    Debug.Log($"Série {headers[s + 1]} configurada com padrão bolinhas");
                    break;
                case 2: // Estrelinhas (estrelas simuladas)
                    serie.itemStyle.color = new Color(0.5f, 0.5f, 1f, 0.7f); // Azul claro translúcido
                    serie.itemStyle.borderColor = new Color(1f, 1f, 0f, 0.9f); // Amarelo
                    serie.itemStyle.borderWidth = 1.8f;
                    Debug.Log($"Série {headers[s + 1]} configurada com padrão estrelinhas");
                    break;
                case 3: // Xadrez (checkerboard simulado)
                    serie.itemStyle.color = new Color(0.7f, 0.7f, 0.7f, 0.6f); // Cinza médio translúcido
                    serie.itemStyle.borderColor = new Color(0f, 0f, 0f, 0.9f); // Preto
                    serie.itemStyle.borderWidth = 1.2f;
                    Debug.Log($"Série {headers[s + 1]} configurada com padrão xadrez");
                    break;
                case 4: // Ondas (linhas onduladas simuladas)
                    serie.itemStyle.color = new Color(0f, 0.5f, 0.5f, 0.6f); // Verde-água translúcido
                    serie.itemStyle.borderColor = new Color(0f, 1f, 1f, 0.8f); // Ciano
                    serie.itemStyle.borderWidth = 1.5f;
                    Debug.Log($"Série {headers[s + 1]} configurada com padrão ondas");
                    break;
                case 5: // Diamantes (losangos simulados)
                    serie.itemStyle.color = new Color(1f, 0.8f, 0f, 0.7f); // Amarelo-alaranjado translúcido
                    serie.itemStyle.borderColor = new Color(1f, 0.5f, 0f, 0.9f); // Laranja
                    serie.itemStyle.borderWidth = 1.7f;
                    Debug.Log($"Série {headers[s + 1]} configurada com padrão diamantes");
                    break;
            }
        }

        // Lista para rastrear valores únicos de x
        HashSet<float> uniqueXValues = new HashSet<float>();

        // Lê os dados e empilha os valores brutos
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < columnCount) continue;

            if (!float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float xVal)) continue;

            uniqueXValues.Add(xVal); // Adiciona xVal aos valores únicos

            for (int s = 0; s < dataSeriesCount; s++)
            {
                if (float.TryParse(values[s + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out float yVal))
                {
                    Debug.Log($"x={xVal}, série={headers[s + 1]}, valor={yVal}"); // Depuração
                    chart.series[s].AddXYData(xVal, yVal);
                }
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
        xAxis.boundaryGap = true; // Ativa gap automático

        // Configurações do eixo Y (valores brutos)
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;
        yAxis.axisLabel.show = true;
        yAxis.axisLine.show = true;
        yAxis.axisTick.show = true;
        // Deixado para autoajuste aos valores brutos


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        // Forçar atualização do gráfico
        chart.RefreshChart();
    }
}