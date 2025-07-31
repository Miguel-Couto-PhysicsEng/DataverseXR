using TMPro;
using UnityEngine;
using System.Globalization;

public class CSVTableViewer : MonoBehaviour
{
    public GameObject rowPrefab;
    public GameObject cellPrefab;
    public Transform tableContent;
    public TMP_Dropdown csvDropdown;
    public GameObject scrollViewPanel; // 👉 o GameObject que contém o painel cinzento (ScrollView)

    void Start()
    {
        // Garante que começa invisível
        if (scrollViewPanel != null)
            scrollViewPanel.SetActive(false);
    }

    public void ShowCSVTable()
    {
        // Limpar tabela anterior
        foreach (Transform child in tableContent)
        {
            Destroy(child.gameObject);
        }

        // Mostrar o painel (caso esteja oculto)
        if (scrollViewPanel != null)
            scrollViewPanel.SetActive(true);

        // Obter nome do ficheiro CSV (sem extensão)
        string fileName = csvDropdown.options[csvDropdown.value].text;

        // Carregar CSV da pasta Resources/CSVFiles
        TextAsset csvFile = Resources.Load<TextAsset>("CSVFiles/" + fileName);

        if (csvFile == null)
        {
            Debug.LogError("Ficheiro CSV não encontrado em Resources: " + fileName);
            return;
        }

        Debug.Log("Conteúdo do ficheiro CSV:\n" + csvFile.text);

        // Ler linhas e gerar tabela
        string[] lines = csvFile.text.Split('\n');

        foreach (string line in lines)
        {
            string[] values = line.Trim().Split(';');

            GameObject row = Instantiate(rowPrefab, tableContent);

            foreach (string value in values)
            {
                string trimmedValue = value.Trim();
                string displayValue = trimmedValue;

                // Tentar converter em número para formatar
                if (double.TryParse(trimmedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
                {
                    if (num == Mathf.Floor((float)num))
                        displayValue = num.ToString("F0", CultureInfo.InvariantCulture);
                    else
                        displayValue = num.ToString("F2", CultureInfo.InvariantCulture);
                }

                GameObject cell = Instantiate(cellPrefab, row.transform);
                cell.GetComponentInChildren<TMP_Text>().text = displayValue;
            }
        }
    }

    public void HideCSVTable()
    {
        // Apagar conteúdo da tabela
        foreach (Transform child in tableContent)
        {
            Destroy(child.gameObject);
        }

        // Ocultar o painel
        if (scrollViewPanel != null)
            scrollViewPanel.SetActive(false);
    }
}
