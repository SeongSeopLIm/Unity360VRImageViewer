using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Camera playerCamera;
    public GameObject[] activeBinds;

    public virtual void Enable()
    {
        if(playerCamera != null)
            playerCamera.gameObject.SetActive(true);
        activeBinds.ToList().ForEach((iter) => iter.SetActive(true));
    }

    public virtual void Disable()
    {
        if(playerCamera != null)
            playerCamera.gameObject.SetActive(false);
        activeBinds.ToList().ForEach((iter) => iter.SetActive(false));
    }

}
