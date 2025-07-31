using UnityEngine;
using XCharts.Runtime;
using System.Collections.Generic;
using System.Globalization;

public class CSVPolarLineChart : MonoBehaviour
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

        BaseChart chart = chartGO.GetComponent<BaseChart>();
        if (chart == null)
        {
            Debug.LogError("Prefab não tem BaseChart.");
            return;
        }

        // Adiciona título
        var title = chart.EnsureChartComponent<Title>();
        title.text = "Polar Line Chart";

        // Legenda ajustada para coluna à direita, abaixo do título
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical;
        legend.location.align = Location.Align.TopRight;
        legend.location.top = 30;
        legend.location.right = 5;

        // Usa a série existente (série0) no prefab, assumindo 361 pontos (0–360 graus)
        if (chart.series.Count > 0)
        {
            chart.series[0].ClearData();
            for (int i = 0; i <= 360; i++)
            {
                chart.series[0].AddData(new List<double> { 0, i }); // Inicializa com raio 0 e ângulo i
            }
        }
        else
        {
            Debug.LogError("Série0 não encontrada no prefab.");
            return;
        }

        // Cultura para separador decimal (usar InvariantCulture para ponto)
        CultureInfo culture = CultureInfo.InvariantCulture; // Força ponto como separador decimal

        // Ler os 361 pontos do CSV (Raio;Ângulo) e atualizar apenas o raio
        for (int i = 1; i <= 361; i++) // Processa todas as 361 linhas de dados (ignorando cabeçalho na linha 1)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(';');
            if (values.Length != 2) continue;

            Debug.Log($"Linha {i}: Valor bruto - {values[0]};{values[1]}"); // Depuração dos valores brutos
            if (double.TryParse(values[0], NumberStyles.Any, culture, out double radialValue) &&
                double.TryParse(values[1], NumberStyles.Any, culture, out double angleDegree))
            {
                Debug.Log($"Linha {i}: Valor lido do CSV - Raio: {radialValue}, Ângulo: {angleDegree}");
                int index = (int)Mathf.Round((float)angleDegree);
                if (index == 360) index = 0; // 360° mapeia para 0° (cíclico)

                if (index >= 0 && index < chart.series[0].dataCount)
                {
                    // Obtém o ângulo existente e cria uma nova lista com o raio atualizado
                    double currentAngle = chart.series[0].GetData(index, 1); // Ângulo existente
                    List<double> data = new List<double> { radialValue, currentAngle }; // [raio, ângulo]
                    chart.series[0].UpdateData(index, data);
                    Debug.Log($"Linha {i}: Mapeando ângulo {angleDegree} graus -> Índice {index} com valor radial {radialValue}");
                }
            }
            else
            {
                Debug.LogWarning($"Falha ao parsear linha {i}: {line}");
            }
        }

        // Verificação final
        Serie serieFinal = chart.series[0];
        for (int i = 0; i < serieFinal.dataCount; i++)
        {
            double radial = serieFinal.GetData(i, 0); // raio
            double angle = serieFinal.GetData(i, 1);  // ângulo
            Debug.Log($"Índice {i}: Valor radial {radial}, Ângulo {angle}");
        }


        // ✅ Adicionar à dropdown do ChartManager
        FindFirstObjectByType<ChartManager>()?.AddChart(chartGO);

        chart.RefreshChart();
    }
}