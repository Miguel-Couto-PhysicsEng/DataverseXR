using UnityEngine;
using TMPro;
using System.Collections.Generic;
using XCharts.Runtime; // ⬅️ IMPORTANTE para aceder a Title

/// <summary>
/// Gere a lista de gráficos gerados dinamicamente e permite apagá-los via dropdown.
/// </summary>
public class ChartManager : MonoBehaviour
{
    [Header("Dropdown com a lista de gráficos gerados")]
    public TMP_Dropdown chartDropdown;

    // Lista dos gráficos instanciados
    private List<GameObject> chartInstances = new List<GameObject>();

    /// <summary>
    /// Adiciona um novo gráfico à lista e ao dropdown.
    /// </summary>
    public void AddChart(GameObject chartGO)
    {
        chartInstances.Add(chartGO);

        // Tenta obter o título do gráfico
        string titleText = "Gráfico sem título";
        var chart = chartGO.GetComponent<BaseChart>();
        if (chart != null)
        {
            var title = chart.GetChartComponent<Title>();
            if (title != null && !string.IsNullOrEmpty(title.text))
            {
                titleText = title.text;
            }
        }

        // Adiciona ao dropdown com o nome real
        chartDropdown.options.Add(new TMP_Dropdown.OptionData(titleText));
        chartDropdown.value = chartInstances.Count - 1;
        chartDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Elimina o gráfico selecionado no dropdown.
    /// </summary>
    public void DeleteSelectedChart()
    {
        int index = chartDropdown.value;
        if (index >= 0 && index < chartInstances.Count)
        {
            GameObject chartToDelete = chartInstances[index];
            if (chartToDelete != null)
                Destroy(chartToDelete);

            chartInstances.RemoveAt(index);
            chartDropdown.options.RemoveAt(index);

            chartDropdown.value = Mathf.Max(0, chartDropdown.options.Count - 1);
            chartDropdown.RefreshShownValue();
        }
    }
}
