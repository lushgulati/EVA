using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnabler : MonoBehaviour
{
    public GameObject Pistol;
    private void Start()
    {
        GameEvents.current.onEndGrapple += OnEndingGrapple;
    }

    private void OnEndingGrapple()
    {
        Pistol.SetActive(true);
    }
}
