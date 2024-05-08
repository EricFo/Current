using System.Linq;
using UnityEngine;
using ParticleShader;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class StencilInteraction : MonoBehaviour
{
    [SerializeField]
    private SpriteMaskInteraction interaction;
    public SpriteMaskInteraction Interaction{
        get
        {
            return interaction;
        }
        set
        {
            if (interaction != value)
            {
                interaction = value;
                SetInteraction();
            }
        }
    }
    
    [SerializeField]
    private bool UseIndependentMaterial;

    private List<Renderer> allRenderers;
    public List<Renderer> AllRenderers
    {
        get
        {
            if (allRenderers == null || allRenderers.Count == 0)
            {
                allRenderers = GetComponentsInChildren<Renderer>().ToList();
            }

            return allRenderers;
        }
    }

    private static int a = 3;
    public void Awake()
    {
        if (UseIndependentMaterial)
        {
            foreach (var renderer in AllRenderers)
            {
                var rendererObj = renderer.gameObject;
                var checker = rendererObj.GetComponent<CheckDestory>();
                if (checker == null)
                {
                    checker = renderer.gameObject.AddComponent<CheckDestory>();
                }
                checker.OnDestoryEvent = OnCheckDestoryEvent;
            }
        }
        SetInteraction();
    }


    private static readonly int CompareValueID = Shader.PropertyToID("_CompareValue");

    private void SetInteraction()
    {
        int compareValue = (int)CompareFunction.Always;
        switch (interaction)
        {
            case SpriteMaskInteraction.VisibleInsideMask:
                compareValue = (int)CompareFunction.NotEqual;
                break;
            case SpriteMaskInteraction.VisibleOutsideMask:
                compareValue = (int)CompareFunction.Equal;
                break;
        }
        
        for (int i = 0; i < AllRenderers.Count; i++)
        {
            if (AllRenderers[i] != null)
            {
                if (UseIndependentMaterial)
                {
                    AllRenderers[i].material.SetInt(CompareValueID, compareValue);
                }
                else
                {
                    AllRenderers[i].sharedMaterial.SetInt(CompareValueID, compareValue);  
                }
            }
        }
    }

    private void OnCheckDestoryEvent(Renderer renderer)
    {
        if (UseIndependentMaterial)
        {
            AllRenderers.Remove(renderer);
            Destroy(renderer.sharedMaterial);
        }
    }
}
