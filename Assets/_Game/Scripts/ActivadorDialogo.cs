using UnityEngine;
using TMPro;
using System.Collections;

public class ActivadorDialogo : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoTMP;

    [Header("Configuración")]
    [TextArea(3, 10)]
    public string[] frases;
    public float velocidadEscritura = 0.05f;

    private int indice;
    private bool escribiendo;
    private bool jugadorCerca; // Para saber si Silente está al lado

    void Start()
    {
        panelDialogo.SetActive(false);
    }

    void Update()
    {
        // 1. SI ESTÁ CERCA Y PRESIONA 'E'
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {

            // Si el panel está oculto, empezamos el diálogo
            if (!panelDialogo.activeSelf)
            {
                IniciarDialogo();
            }
            // Si el panel ya está abierto...
            else
            {
                // Si terminó de escribir, pasa a la siguiente frase
                if (textoTMP.text == frases[indice])
                {
                    SiguienteFrase();
                }
                // Si aún está escribiendo, completa la frase de golpe
                else
                {
                    StopAllCoroutines();
                    textoTMP.text = frases[indice];
                }
            }
        }
    }

    public void IniciarDialogo()
    {
        indice = 0;
        panelDialogo.SetActive(true);
        StartCoroutine(EfectoEscribir());
    }

    IEnumerator EfectoEscribir()
    {
        textoTMP.text = "";
        foreach (char letra in frases[indice].ToCharArray())
        {
            textoTMP.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    void SiguienteFrase()
    {
        if (indice < frases.Length - 1)
        {
            indice++;
            StartCoroutine(EfectoEscribir());
        }
        else
        {
            CerrarDialogo();
        }
    }

    void CerrarDialogo()
    {
        panelDialogo.SetActive(false);
        indice = 0;
    }

    // DETECCIÓN DE PROXIMIDAD
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Silente está cerca del NPC. Presiona E para hablar.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            CerrarDialogo(); // Se cierra si te alejas
        }
    }
}