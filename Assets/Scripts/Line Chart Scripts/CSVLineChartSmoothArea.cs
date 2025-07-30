using UnityEngine;
using XCharts.Runtime;
using System.Globalization;

public class CSVLineChartSmoothArea : MonoBehaviour
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

        LineChart chart = chartGO.GetComponent<LineChart>();
        if (chart == null)
        {
            Debug.LogError("O prefab não tem um componente LineChart.");
            return;
        }

        chart.ClearData();
        chart.EnsureChartComponent<Title>().text = "Smooth Area Chart";

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

        // Preencher séries necessárias
        legend.ClearData();  // limpa entradas antigas da legenda


        for (int s = 0; s < neededSeries; s++)
        {
            Serie serie = chart.series[s];
            serie.show = true;
            serie.lineType = LineType.Smooth;
            serie.areaStyle.show = true;
            serie.itemStyle.opacity = 1f;
            serie.lineStyle.width = 2f;
            serie.areaStyle.opacity = 0.6f;
            serie.data.Clear();

            // 🔁 Remover nome antigo da legenda (se existir)
            legend.RemoveData("serie" + s);

            // ✅ Adiciona nome correto com base no CSV
            legend.AddData(headers[s + 1]);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Trim().Split(';');
                if (values.Length <= s + 1) continue;

                if (float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float xVal) &&
                    float.TryParse(values[s + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out float yVal))
                {
                    serie.AddXYData(xVal, yVal);
                }
            }
        }


        // Remover séries extra (do fim para o início)
        for (int s = totalSeries - 1; s >= neededSeries; s--)
        {
            chart.series.RemoveAt(s);
        }


        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Value;
        xAxis.show = true;
        xAxis.axisLine.show = true;
        xAxis.axisTick.show = true;
        xAxis.axisLabel.show = true;
    }
}
