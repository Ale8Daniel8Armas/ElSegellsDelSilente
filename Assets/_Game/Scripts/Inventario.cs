using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventario : MonoBehaviour
{
    [Header("Configuración de UI")]
    public List<Image> slotsUI; //
    public Sprite slotVacio;    //

    [Header("Base de Datos Interna")]
    // Esta lista guardará los nombres de los objetos en el mismo orden que los slots
    public List<string> itemsNombres = new List<string>();

    void Start()
    {
        // Inicializamos la lista de nombres con espacios vacíos
        for (int i = 0; i < slotsUI.Count; i++)
        {
            itemsNombres.Add("");
        }
    }

    public bool AgregarItem(Sprite iconoNuevo, string nombreItem)
    {
        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (slotsUI[i].sprite == slotVacio || slotsUI[i].sprite == null)
            {
                slotsUI[i].sprite = iconoNuevo;
                slotsUI[i].color = Color.white; //

                // Guardamos el nombre en la misma posición que el slot
                itemsNombres[i] = nombreItem;

                Debug.Log("Item guardado: " + nombreItem);
                return true;
            }
        }
        Debug.Log("Inventario lleno");
        return false;
    }

    public void UsarItem(int indice)
    {
        if (indice < slotsUI.Count && slotsUI[indice].sprite != null)
        {
            // PROTECCIÓN PARA EL JEFE FINAL:
            // Si el nombre es "RunaAncestral", el botón no hace nada.
            if (itemsNombres[indice] == "RunaAncestral")
            {
                Debug.Log("No puedes usar la Runa ahora; es para el Jefe Final.");
                return;
            }

            Debug.Log("Usando objeto: " + itemsNombres[indice]);

            // Lógica de limpieza (para pociones)
            slotsUI[indice].sprite = null;
            itemsNombres[indice] = ""; // Limpiamos el nombre también

            Color c = slotsUI[indice].color;
            c.a = 0f; //
            slotsUI[indice].color = c;
        }
    }

    // Esta función la usará el Jefe Final para verificar si tienes la runa
    public bool TieneObjeto(string nombreBuscado)
    {
        return itemsNombres.Contains(nombreBuscado);
    }
}