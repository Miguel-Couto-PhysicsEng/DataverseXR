using UnityEngine;
using XCharts.Runtime;
using System.Globalization;

public class CSVLineChartStackArea : MonoBehaviour
{
    public GameObject chartPrefab;
    public string csvName;
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
        if (lines.Length < 2) return;

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
        chart.EnsureChartComponent<Title>().text = "Stack Area Chart";

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
        int neededSeries = columnCount - 1;

        if (chart.series.Count < neededSeries)
        {
            Debug.LogError($"O gráfico precisa de pelo menos {neededSeries} séries no prefab, mas tem {chart.series.Count}.");
            return;
        }

        int totalSeries = chart.series.Count;

        float lastX = float.MinValue; // Rastrear o último valor de x

        // Preencher e configurar séries necessárias
        for (int s = 0; s < neededSeries; s++)
        {
            Serie serie = chart.series[s];
            serie.show = true;
            serie.lineType = LineType.Smooth;
            serie.stack = "total"; // Essencial para empilhamento
            serie.areaStyle.show = true;
            serie.itemStyle.opacity = 1f;
            serie.lineStyle.width = 2f;
            serie.areaStyle.opacity = 0.6f;
            serie.data.Clear();

            // Atualizar nome da legenda
            legend.RemoveData("serie" + s);
            legend.AddData(headers[s + 1]);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Trim().Split(';');
                if (values.Length <= s + 1) continue;

                if (float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float xVal) &&
                    float.TryParse(values[s + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out float yVal))
                {
                    Debug.Log($"Adicionando dado: x={xVal}, y={yVal}, série={s}"); // Depuração
                    serie.AddXYData(xVal, yVal);
                    lastX = xVal; // Atualiza o último valor de x
                }
            }
        }

        // Remover séries extra
        for (int s = totalSeries - 1; s >= neededSeries; s--)
        {
            chart.series.RemoveAt(s);
        }

        // Eixo X (limitado ao último valor de x)
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Value;
        xAxis.show = true;
        xAxis.axisLine.show = true;
        xAxis.axisTick.show = true;
        xAxis.axisLabel.show = true;
        xAxis.boundaryGap = false; // Impede espaço extra
        xAxis.max = lastX; // Define o máximo como o último valor de x
        xAxis.min = 0f; // Define o mínimo como 0 para consistência
        // Removi splitNumber para evitar interferência no autoajuste

        // Eixo Y
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.show = true;
        yAxis.type = Axis.AxisType.Value;
        yAxis.axisLine.show = true;
        yAxis.axisTick.show = true;
        yAxis.axisLabel.show = true;


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        // Forçar atualização do gráfico
        chart.RefreshChart();
    }
}