using UnityEngine;
using TMPro;

public class ChartMenuManager : MonoBehaviour
{
    public GameObject csvDropdownGO;
    public GameObject chartTypeDropdownGO;
    public GameObject chartStyleDropdownGO;

    // Todos os Spawners para os estilos do tipo Line
    public GameObject stepChartSpawner;
    public GameObject dashedChartSpawner;
    public GameObject smoothChartSpawner;
    public GameObject areaChartSpawner;
    public GameObject basicChartSpawner;
    public GameObject logChartSpawner;
    public GameObject smoothAreaChartSpawner;
    public GameObject stackAreaChartSpawner;
    public GameObject stackChartSpawner;
    public GameObject timeChartSpawner;

    private TMP_Dropdown csvDropdown;
    private TMP_Dropdown chartTypeDropdown;
    private TMP_Dropdown chartStyleDropdown;

    void Start()
    {
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
            switch (chartStyle)
            {
                case "Step":
                    stepChartSpawner.GetComponent<CSVLineChartStep>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Dashed":
                    dashedChartSpawner.GetComponent<CSVLineChartDashed>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Smooth":
                    smoothChartSpawner.GetComponent<CSVLineChartSmooth>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Area":
                    areaChartSpawner.GetComponent<CSVLineChartArea>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Basic":
                    basicChartSpawner.GetComponent<CSVLineChartBasic>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Log":
                    logChartSpawner.GetComponent<CSVLineChartLog>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Smooth Area":
                    smoothAreaChartSpawner.GetComponent<CSVLineChartSmoothArea>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Stack Area":
                    stackAreaChartSpawner.GetComponent<CSVLineChartStackArea>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Stack":
                    stackChartSpawner.GetComponent<CSVLineChartStack>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Time":
                    timeChartSpawner.GetComponent<CSVLineChartTime>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de linha não suportado.");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Tipo de gráfico não suportado.");
        }
    }
}
