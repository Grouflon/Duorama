using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(BoxCollider))]
public class InteractiveItem : MonoBehaviour
{
    public string targetRoomId;

    public bool hovered {
        get => m_hovered;
        set
        {
            if (m_hovered == value)
                return;
            
            m_hovered = value;
            m_outline.enabled = m_hovered;
        }
    }

    void Awake()
    {
        m_outline = GetComponent<Outline>();
        m_outline.enabled = false;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    private bool m_hovered = false;
    private Outline m_outline;
}
