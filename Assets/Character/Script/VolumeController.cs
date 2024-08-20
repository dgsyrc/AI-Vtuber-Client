
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI text;
    private void Start()
    {
        Awake();
    }
    private void Awake()
    {
        slider.value = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        VolumeControl();
    }
    private void VolumeControl()
    {
        audioSource.volume = slider.value;
        text.text = ((int)(slider.value * 100)).ToString();
    }
}
