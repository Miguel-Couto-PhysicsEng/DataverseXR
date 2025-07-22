using TMPro;
using UnityEngine;

public class ColliderRayLine : MonoBehaviour
{
    public GameObject tooltipPrefab;

    public void ShowTooltip(Transform point)
    {
        foreach (Transform child in point)
        {
            if (child.name.Contains("Tooltip"))
            {
                Destroy(child.gameObject);
            }
        }

        if (tooltipPrefab != null)
        {
            GameObject tooltip = Instantiate(tooltipPrefab, point.position + Vector3.up * 0.1f, Quaternion.identity, point);
            tooltip.name = "Tooltip";

            Vector3 pos = point.position;
            string coordText = $"X: {pos.x:F2}\nY: {pos.y:F2}\nZ: {pos.z:F2}";

            var textTMP = tooltip.GetComponentInChildren<TextMeshPro>();
            if (textTMP != null)
            {
                textTMP.text = coordText;
                textTMP.color = Color.black;
            }
            else
            {
                Debug.LogWarning("TooltipPrefab não tem TextMeshPro!");
            }
        }
    }
}


