using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private PlayerController jugador;
    private float maxima;

	void Start()
    {
        jugador = GameObject.Find("Player").GetComponent<PlayerController>();
        maxima = jugador.vidaMaxima;
    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarraVida.fillAmount = jugador.vidaMaxima / maxima;
    }
}
