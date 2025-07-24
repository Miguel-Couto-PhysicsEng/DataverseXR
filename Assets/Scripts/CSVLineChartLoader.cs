using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVLineChartLoader : MonoBehaviour
{
    public GameObject lineChartPrefab;  // Prefab do gráfico de linhas (drag & drop do Resources/Prefabs)

    public void GenerateChart(string csvFileName, string style, Transform spawnPoint)
    {
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName + ".csv");

        if (!File.Exists(path))
        {
            Debug.LogError("[CSVLineChartLoader] Ficheiro CSV não encontrado: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        List<float> xValues = new List<float>();
        List<float> yValues = new List<float>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length >= 2 &&
                float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y))
            {
                xValues.Add(x);
                yValues.Add(y);
            }
        }

        // Verificação de existência do prefab
        if (lineChartPrefab == null)
        {
            Debug.LogError("[CSVLineChartLoader] Prefab do gráfico não atribuído!");
            return;
        }

        // Instancia o gráfico na posição indicada
        GameObject chart = Instantiate(lineChartPrefab, spawnPoint.position, spawnPoint.rotation);
        chart.name = $"Gráfico linhas {csvFileName}";

        // Aqui poderás configurar o gráfico com base nos dados carregados e estilo (futuro)
        // Exemplo: enviar os dados ao gráfico através de um script ChartBuilder
    }
}

