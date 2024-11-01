using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DraggedItem : MonoBehaviour
{
    [SerializeField]
    private Image BackgroundImage;
    [SerializeField]
    private Image ItemIconImage;
    [SerializeField]
    private TextMeshProUGUI CountText;

    [Space()]
    [SerializeField]
    private Color NormalColor;

    private Vector3 LastPosition;

    private void Start()
    {
        BackgroundImage.color = NormalColor;
    }

    public void UpdateState(ItemStack item)
    {
        var count = item.GetCount();
        ItemIconImage.sprite = item.GetItem().GetIcon();
        CountText.text = count != 0 ? count.ToString() : string.Empty;
    }

    private void Update()
    {
        if (LastPosition != Input.mousePosition)
        {
            transform.position = Input.mousePosition;
            LastPosition = Input.mousePosition;
        }
    }
}
