using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PanelDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject controlObject;
    public bool startState = false;

    void Start()
    {
        controlObject.SetActive(startState);
    }
    public void changeState()
    {
        startState = !startState;
        controlObject.SetActive(startState);
    }
}
