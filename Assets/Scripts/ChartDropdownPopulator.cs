using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ChartDropdownPopulator : MonoBehaviour
{
    public GameObject csvDropdownGO;
    public GameObject chartTypeDropdownGO;
    public GameObject chartStyleDropdownGO;

    private TMP_Dropdown csvDropdown;
    private TMP_Dropdown chartTypeDropdown;
    private TMP_Dropdown chartStyleDropdown;

    // ✅ Nomes curtos e consistentes com ChartMenuManager.cs
    private readonly Dictionary<string, List<string>> styleOptions = new Dictionary<string, List<string>>()
    {
        { "Line", new List<string> {
            "Smooth", "Step", "Dashed",
            "Area", "Basic", "Log",
            "Smooth Area", "Stack Area", "Stack", "Time"
        }},
        { "Bar", new List<string> { "Basic", "Stacked" } },
        { "Scatter", new List<string> { "Basic" } }
    };

    void Start()
    {
        csvDropdown = csvDropdownGO.GetComponentInChildren<TMP_Dropdown>();
        chartTypeDropdown = chartTypeDropdownGO.GetComponentInChildren<TMP_Dropdown>();
        chartStyleDropdown = chartStyleDropdownGO.GetComponentInChildren<TMP_Dropdown>();

        PopulateCSVFiles();
        PopulateChartTypes();
        chartTypeDropdown.onValueChanged.AddListener(UpdateChartStyles);
        UpdateChartStyles(0);
    }

    void PopulateCSVFiles()
    {
        csvDropdown.ClearOptions();
        List<string> fileNames = Resources.LoadAll<TextAsset>("CSVFiles")
                                    .Select(f => f.name)
                                    .ToList();

        if (fileNames.Count == 0)
            fileNames.Add("dados1"); // fallback

        csvDropdown.AddOptions(fileNames);
    }

    void PopulateChartTypes()
    {
        chartTypeDropdown.ClearOptions();
        chartTypeDropdown.AddOptions(styleOptions.Keys.ToList());
    }

    void UpdateChartStyles(int index)
    {
        chartStyleDropdown.ClearOptions();

        string selectedChartType = chartTypeDropdown.options[chartTypeDropdown.value].text;

        if (styleOptions.ContainsKey(selectedChartType))
            chartStyleDropdown.AddOptions(styleOptions[selectedChartType]);
        else
            chartStyleDropdown.AddOptions(new List<string> { "Default" });
    }
}
