using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SetTextFromSlider : MonoBehaviour {
    public Slider ChosenSlider;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateText()
    {
        if (ChosenSlider.GetComponent<Slider>() != null) {
            GetComponent<Text>().text = ChosenSlider.GetComponent<Slider>().value.ToString();
        } else
        {
            Debug.LogError("No Slider defined for SetTextFromSlider");
        }

    }
       
}
