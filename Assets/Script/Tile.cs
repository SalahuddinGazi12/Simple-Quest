using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class Tile : MonoBehaviour
{
    public int number;
    public TextMeshProUGUI tiletext;
    public bool isRemoved = false;
    public void OnTileClicked()
    {
        GameManager.Instance.TileClicked(this);
    }
    public void Update()
    {
        tiletext.text = "" + number;
    }
}
