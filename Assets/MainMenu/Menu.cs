using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
	public GameObject settings;
	public GameObject start;

	public void startGame() {
		if (settings == !settings.activeSelf) {
			start.SetActive (!start.activeSelf);
		} else {
			settings.SetActive (!settings.activeSelf);
			start.SetActive (!start.activeSelf);
		}
	}

	public void Settings(){
		if (start == !start.activeSelf) {
			settings.SetActive (!settings.activeSelf);
		} else {
			start.SetActive (!start.activeSelf);
			settings.SetActive (!settings.activeSelf);
		}

	}

	public void Exit(){

		Application.Quit () ;
	}
}

