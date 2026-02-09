using UnityEngine;
using TMPro;
using System.Collections;

public class ManagerDialogos : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject panelDialogo; // Tu objeto con la imagen de la caja
    public TextMeshProUGUI textoDialogo;
    public float velocidadEscritura = 0.05f;

    [Header("Referencia al Jugador")]
    public MonoBehaviour scriptMovimientoJugador; // Arrastra a Silente aquí

    private string[] frases;
    private int indice;
    private bool escribiendo;

    public void IniciarDialogo(string[] lineasNuevas)
    {
        panelDialogo.SetActive(true);
        frases = lineasNuevas;
        indice = 0;

        // Bloqueamos el movimiento de Silente
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = false;

        StartCoroutine(EscribirLinea());
    }

    IEnumerator EscribirLinea()
    {
        escribiendo = true;
        textoDialogo.text = "";
        foreach (char letra in frases[indice].ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
        escribiendo = false;
    }

    public void SiguienteFrase()
    {
        if (escribiendo) return;

        if (indice < frases.Length - 1)
        {
            indice++;
            StartCoroutine(EscribirLinea());
        }
        else
        {
            CerrarDialogo();
        }
    }

    void CerrarDialogo()
    {
        panelDialogo.SetActive(false);

        // Devolvemos el control a Silente
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = true;

        Debug.Log("Diálogo finalizado.");
    }

    void Update()
    {
        // Solo detecta la E para avanzar si el panel está encendido
        if (panelDialogo.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            SiguienteFrase();
        }
    }
}