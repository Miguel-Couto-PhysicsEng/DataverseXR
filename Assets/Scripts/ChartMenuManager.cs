using System.Linq;
using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class ChartMenuManager : MonoBehaviour
{
    public GameObject csvDropdownGO;
    public GameObject chartTypeDropdownGO;
    public GameObject chartStyleDropdownGO;

    // Line chart spawners
    public GameObject stepChartSpawner;
    public GameObject dashedChartSpawner;
    public GameObject smoothChartSpawner;
    public GameObject basicChartSpawner;
    public GameObject logChartSpawner;
    public GameObject smoothAreaChartSpawner;

    // Bar chart spawners
    public GameObject basicBarChartSpawner;
    public GameObject percentColumnChartSpawner;
    public GameObject stackedColumnChartSpawner;
    public GameObject zebraColumnChartSpawner;

    // Pie chart spawners
    public GameObject basicPieChartSpawner;
    public GameObject donutPieChartSpawner;
    public GameObject radiusRoseChartSpawner;

    // Radar chart spawners
    public GameObject radarCircleChartSpawner;
    public GameObject radarPolygonChartSpawner;

    // Scatter chart spawners
    public GameObject scatterBasicChartSpawner;
    public GameObject scatterBubbleChartSpawner;

    // Polar chart spawners
    public GameObject polarLineChartSpawner;

    // Candlestick chart spawner
    public GameObject candlestickChartSpawner;

    // Ring chart spawner
    public GameObject ringChartSpawner;

    // Parallel chart spawner
    public GameObject parallelChartSpawner;



    private TMP_Dropdown csvDropdown;
    private TMP_Dropdown chartTypeDropdown;
    private TMP_Dropdown chartStyleDropdown;

    void Start()
    {
        csvDropdown = csvDropdownGO.GetComponentInChildren<TMP_Dropdown>();
        chartTypeDropdown = chartTypeDropdownGO.GetComponentInChildren<TMP_Dropdown>();
        chartStyleDropdown = chartStyleDropdownGO.GetComponentInChildren<TMP_Dropdown>();

        // Depuração para verificar as opções disponíveis
        Debug.Log("Opções de Estilo no Dropdown: " + string.Join(", ", chartStyleDropdown.options.Select(opt => opt.text)));
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
                case "Basic":
                    basicChartSpawner.GetComponent<CSVLineChartBasic>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Log":
                    logChartSpawner.GetComponent<CSVLineChartLog>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Smooth Area":
                    smoothAreaChartSpawner.GetComponent<CSVLineChartSmoothArea>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de linha não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Bar")
        {
            switch (chartStyle)
            {
                case "Basic Bar":
                    basicBarChartSpawner.GetComponent<CSVBarChartBasicBar>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Percent Column":
                    percentColumnChartSpawner.GetComponent<CSVBarChartPercentColumn>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Stacked Column":
                    stackedColumnChartSpawner.GetComponent<CSVBarChartStackedColumn>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Zebra Column":
                    zebraColumnChartSpawner.GetComponent<CSVBarChartZebraColumn>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de barras não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Pie")
        {
            switch (chartStyle)
            {
                case "Basic":
                    basicPieChartSpawner.GetComponent<CSVPieBasicWithLabel>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Rose":
                    radiusRoseChartSpawner.GetComponent<CSVPieRadiusRoseChart>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Donut":
                    donutPieChartSpawner.GetComponent<CSVPieDonut>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de pie não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Radar")
        {
            switch (chartStyle)
            {
                case "Circle Radar":
                    radarCircleChartSpawner.GetComponent<CSVRadarCircleChart>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Polygon Radar":
                    radarPolygonChartSpawner.GetComponent<CSVRadarPolygonChart>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de Radar não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Scatter")
        {
            switch (chartStyle)
            {
                case "Basic":
                    scatterBasicChartSpawner.GetComponent<CSVScatterBasicChart>().LoadCSVAndCreateChart(csvName);
                    break;
                case "Bubble":
                    scatterBubbleChartSpawner.GetComponent<CSVScatterBubbleChart>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de scatter não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Polar")
        {
            switch (chartStyle)
            {
                case "Polar Line":
                    polarLineChartSpawner.GetComponent<CSVPolarLineChart>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de Polar não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Candlestick")
        {
            switch (chartStyle)
            {
                case "Candlestick":
                    candlestickChartSpawner.GetComponent<CSVCandlestickChart>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de Candlestick não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Ring")
        {
            switch (chartStyle)
            {
                case "Ring":
                    ringChartSpawner.GetComponent<CSVRingChart>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de Ring não suportado: " + chartStyle);
                    break;
            }
        }
        else if (chartType == "Parallel")
        {
            switch (chartStyle)
            {
                case "Parallel":
                    parallelChartSpawner.GetComponent<CSVParallelChart>().LoadCSVAndCreateChart(csvName);
                    break;
                default:
                    Debug.LogWarning("Estilo de Parallel não suportado: " + chartStyle);
                    break;
            }
        }

        else
        {
            Debug.LogWarning("Tipo de gráfico não suportado: " + chartType);
        }
    }
}