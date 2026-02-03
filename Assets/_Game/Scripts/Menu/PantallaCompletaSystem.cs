using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System.Collections.Generic;

public class PantallaCompletaSystem : MonoBehaviour
{

    public Toggle toggle;

    public TMP_Dropdown resolucionDropDown;
    Resolution[] resoluciones;

	void Start()
    {
        if (Screen.fullScreen)
        {
            toggle.isOn = true;
		}
        else
        {
           toggle.isOn = false;
		}

        RevisarResolucion();

	}

   
    void Update()
    {

    }

    public void ActiivarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
	}

    public void RevisarResolucion()
    {
        resoluciones = Screen.resolutions;
        resolucionDropDown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActua = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + "x" + resoluciones[i].height;
            opciones.Add(opcion);

            if (Screen.fullScreen && resoluciones[i].width == Screen.currentResolution.width && resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActua = i;
            }
        }
        resolucionDropDown.AddOptions(opciones);
        resolucionDropDown.value = resolucionActua;
        resolucionDropDown.RefreshShownValue();

        resolucionDropDown.value = PlayerPrefs.GetInt("numeroResolucion", 0);
	}

    public void CambiarResolucion(int indiceResolucion)
    {
        PlayerPrefs.SetInt("numeroResolucion", resolucionDropDown.value);

		Resolution resolucion = resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
	}
}
