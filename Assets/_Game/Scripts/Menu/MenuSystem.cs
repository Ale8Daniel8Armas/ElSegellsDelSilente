using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{


    public GameObject PantallaDeCarga;
    public Slider Slider;
    public void Salir()
    {
        Debug.Log("Saliendo del juego");
		Application.Quit();
	}

    public void CargaNivel(int NumeroScena)
    {
        StartCoroutine(CargarAsync(NumeroScena));
	}
    IEnumerator CargarAsync(int NumeroDeEscena)
    {
		AsyncOperation Operacion = SceneManager.LoadSceneAsync(NumeroDeEscena);
        PantallaDeCarga.SetActive(true);

		while (!Operacion.isDone)
        {
            float Progreso = Mathf.Clamp01(Operacion.progress / 0.9f);
            Slider.value = Progreso;
			Debug.Log(Progreso);
            yield return null;
        }
		
	}
}
