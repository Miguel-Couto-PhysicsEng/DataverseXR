using UnityEngine;
using TMPro;

public class ChartMenuManager : MonoBehaviour
{
    public GameObject csvDropdownGO;
    public GameObject chartTypeDropdownGO;
    public GameObject chartStyleDropdownGO;

    public GameObject stepChartSpawner;
    public GameObject dashedChartSpawner;

    private TMP_Dropdown csvDropdown;
    private TMP_Dropdown chartTypeDropdown;
    private TMP_Dropdown chartStyleDropdown;

    void Start()
    {
        // Obtém os componentes dos GameObjects com segurança
        csvDropdown = csvDropdownGO.GetComponentInChildren<TMP_Dropdown>();
        chartTypeDropdown = chartTypeDropdownGO.GetComponentInChildren<TMP_Dropdown>();
        chartStyleDropdown = chartStyleDropdownGO.GetComponentInChildren<TMP_Dropdown>();
    }

    public void GenerateChart()
    {
        string csvName = csvDropdown.options[csvDropdown.value].text;
        string chartType = chartTypeDropdown.options[chartTypeDropdown.value].text;
        string chartStyle = chartStyleDropdown.options[chartStyleDropdown.value].text;

        Debug.Log($"CSV: {csvName}, Tipo: {chartType}, Estilo: {chartStyle}");

        if (chartType == "Line")
        {
            if (chartStyle == "Step")
            {
                stepChartSpawner.GetComponent<CSVLineChartStep>().LoadCSVAndCreateChart(csvName);
            }
            else if (chartStyle == "Dashed")
            {
                dashedChartSpawner.GetComponent<CSVLineChartDashed>().LoadCSVAndCreateChart(csvName);
            }
            else
            {
                Debug.LogWarning("Estilo de linha não suportado.");
            }
        }
        else
        {
            Debug.LogWarning("Tipo de gráfico não suportado.");
        }
    }
}
