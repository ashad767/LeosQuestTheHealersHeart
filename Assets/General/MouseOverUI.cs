using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MouseOverUI : MonoBehaviour
{
    private Text txt;

    private void Awake()
    {
        txt = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        txt.color = Color.white;
    }

    public void OnMouseEnter()
    {
        txt.color = new Color(0.7372549f, 0.9411765f, 0.7725491f, 1);
    }

    public void OnMouseExit()
    {
        txt.color = Color.white;
    }
}
