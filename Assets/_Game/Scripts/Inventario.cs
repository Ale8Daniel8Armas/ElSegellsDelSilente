using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventario : MonoBehaviour
{
    [Header("Configuración de UI")]
    // Lista donde arrastraste Slot1, Slot2, etc.
    public List<Image> slotsUI;

    // Déjalo en "None" en el Inspector para mayor precisión
    public Sprite slotVacio;

    /// <summary>
    /// Agrega un item al primer espacio disponible.
    /// </summary>
    public bool AgregarItem(Sprite iconoNuevo)
    {
        foreach (Image slot in slotsUI)
        {
            // Detecta si el slot no tiene imagen asignada
            if (slot.sprite == slotVacio || slot.sprite == null)
            {
                slot.sprite = iconoNuevo;

                // Hace que la imagen sea visible (Alpha al 100%)
                slot.color = Color.white;

                Debug.Log("Item guardado en inventario");
                return true;
            }
        }

        Debug.Log("Inventario lleno");
        return false;
    }

    /// <summary>
    /// Función para "sacar" o usar el objeto al hacer clic en el botón del slot.
    /// </summary>
    public void UsarItem(int indice)
    {
        // Verifica que el slot tenga algo adentro
        if (indice < slotsUI.Count && slotsUI[indice].sprite != null)
        {
            Debug.Log("Usando objeto del slot: " + indice);

            // 1. Aquí puedes añadir lógica de curación (ej: Player.vida += 20)

            // 2. Limpiamos el slot visualmente
            slotsUI[indice].sprite = null;

            // 3. Lo volvemos transparente para que no se vea el cuadro blanco
            Color c = slotsUI[indice].color;
            c.a = 0f;
            slotsUI[indice].color = c;

            Debug.Log("Objeto removido del inventario.");
        }
    }
}