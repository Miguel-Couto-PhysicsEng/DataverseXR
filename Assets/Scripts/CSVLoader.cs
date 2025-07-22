using UnityEngine;
using XCharts.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CSVLoader : MonoBehaviour
{
    public TextAsset csvFile;
    public BaseChart chart;
    public Transform pontosPai; // "Gráfico linhas Dados 1"
    public GameObject pointPrefab; // Prefab da esfera

    void Start()
    {
        if (chart == null || csvFile == null || pontosPai == null)
        {
            Debug.LogError("CSVLoader: Verifique se chart, csvFile e pontosPai estão atribuídos!");
            return;
        }

        var lines = csvFile.text.Split('\n');
        List<string> labels = new();
        List<float> values = new();

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Trim();
            if (string.IsNullOrEmpty(row)) continue;
            var parts = row.Split(';');
            if (parts.Length < 2) continue;
            labels.Add(parts[0]);
            values.Add(float.Parse(parts[1]));
        }

        // Configura o gráfico
        chart.ClearData();
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        xAxis.data = labels;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;

        var serie = chart.AddSerie<Line>("Dados CSV");
        for (int i = 0; i < values.Count; i++)
        {
            serie.AddData(i, values[i]);
        }

        chart.RefreshChart();

        if (pointPrefab != null)
        {
            StartCoroutine(CreatePointsAfterChart(labels, values));
        }
    }

    IEnumerator CreatePointsAfterChart(List<string> labels, List<float> values)
    {
        yield return null; // Espera o gráfico renderizar

        // Transform do gráfico
        RectTransform rect = chart.GetComponent<RectTransform>();
        Vector3 chartCenter = rect.position;
        Vector3 chartSize = rect.rect.size;

        float chartWidth = chartSize.x * rect.lossyScale.x;
        float chartHeight = chartSize.y * rect.lossyScale.y;

        Vector3 bottomLeft = chartCenter - new Vector3(chartWidth / 2f, chartHeight / 2f, 0);

        for (int i = 0; i < values.Count; i++)
        {
            GameObject point = Instantiate(pointPrefab, pontosPai);
            point.name = $"Point_{i}";

            float xPos = bottomLeft.x + (i * (chartWidth / (values.Count - 1)));
            float yPos = bottomLeft.y + (values[i] / 100f) * chartHeight;
            float zPos = chartCenter.z - 0.01f; // ligeiramente à frente do gráfico

            point.transform.position = new Vector3(xPos, yPos, zPos);
            point.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            SphereCollider col = point.AddComponent<SphereCollider>();
            col.radius = 0.1f;
            col.isTrigger = true;

            var interactable = point.AddComponent<XRSimpleInteractable>();
            interactable.hoverEntered.AddListener((args) =>
            {
                Debug.Log($"Hover em {point.name} em {point.transform.position}");
                FindFirstObjectByType<ColliderRayLine>()?.ShowTooltip(point.transform);
            });

            Debug.Log($"✅ Criado ponto: {point.name} em {point.transform.position}");
        }
    }

}
