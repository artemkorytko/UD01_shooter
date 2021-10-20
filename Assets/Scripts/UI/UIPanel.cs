using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    [SerializeField] protected EUIPanelType _type = EUIPanelType.Unknown;

    public EUIPanelType Type => _type;

    protected UIManager _manager = null;

    public virtual void Initialize(UIManager manager)
    {
        _manager = manager;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnHide();
    }

    protected abstract void OnShow();
    protected abstract void OnHide();
}
