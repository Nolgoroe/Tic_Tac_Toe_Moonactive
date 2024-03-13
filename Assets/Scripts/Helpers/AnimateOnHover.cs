using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimateOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string parameterName;
    private Animator anim;

    void Start()
    {
        if(!TryGetComponent<Animator>(out anim))
            Debug.LogError("Can't find animator on object");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool(parameterName, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool(parameterName, false);
    }

}
