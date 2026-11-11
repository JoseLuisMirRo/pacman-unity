using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Lleva las reglas de la partida: puntaje, vidas, ganar, perder y reiniciar.
// También dibuja la interfaz (puntaje, vidas y mensajes) con OnGUI.
public class EstadoJuego : MonoBehaviour
{
    const int VidasIniciales = 3;
    const int PuntosPorPunto = 10;

    // Jugando = se puede mover; Ganaste y Perdiste detienen la partida.
    enum Fase { Jugando, Ganaste, Perdiste }

    Fase fase = Fase.Jugando;
    int puntaje;
    int vidas = VidasIniciales;

    Laberinto laberinto;
    Jugador jugador;
    List<Perseguidor> perseguidores;

    // Estilos de texto para la interfaz (se crean una sola vez).
    GUIStyle estiloMarcador;
    GUIStyle estiloMensaje;

    public bool Jugando => fase == Fase.Jugando;

    public void Iniciar(Laberinto laberinto, Jugador jugador, List<Perseguidor> perseguidores)
    {
        this.laberinto = laberinto;
        this.jugador = jugador;
        this.perseguidores = perseguidores;
    }

    void Update()
    {
        // La tecla R reinicia en cualquier momento.
        Keyboard teclado = Keyboard.current;
        if (teclado != null && teclado.rKey.wasPressedThisFrame)
        {
            Arranque.Reiniciar();
        }
    }

    // Lo llama el jugador cada vez que recoge un punto.
    public void PuntoRecogido()
    {
        puntaje += PuntosPorPunto;

        if (laberinto.PuntosRestantes == 0)
        {
            fase = Fase.Ganaste;
        }
    }

    // Revisa si algún perseguidor tocó al jugador.
    // Se llama después de cada paso del jugador o de un perseguidor.
    public void RevisarChoques()
    {
        if (!Jugando)
        {
            return;
        }

        foreach (Perseguidor perseguidor in perseguidores)
        {
            // Choque normal: están en la misma casilla.
            bool mismaCasilla = perseguidor.Celda == jugador.Celda;

            // Choque cruzado: se intercambiaron de casilla en el mismo momento
            // (sin esto se podrían "atravesar" sin tocarse).
            bool seCruzaron = perseguidor.Celda == jugador.CeldaAnterior
                              && perseguidor.CeldaAnterior == jugador.Celda;

            if (mismaCasilla || seCruzaron)
            {
                PerderVida();
                return;
            }
        }
    }

    void PerderVida()
    {
        vidas--;

        if (vidas <= 0)
        {
            fase = Fase.Perdiste;
            return;
        }

        // Todavía quedan vidas: todos regresan a su lugar inicial.
        jugador.VolverAlInicio();
        foreach (Perseguidor perseguidor in perseguidores)
        {
            perseguidor.VolverAlInicio();
        }
    }

    // Dibuja la interfaz. Unity llama a OnGUI automáticamente cada cuadro.
    void OnGUI()
    {
        if (estiloMarcador == null)
        {
            estiloMarcador = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            estiloMensaje = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        }

        // El tamaño de letra depende del alto de la ventana.
        estiloMarcador.fontSize = Mathf.Max(14, Screen.height / 28);
        estiloMensaje.fontSize = Mathf.Max(18, Screen.height / 16);

        GUI.Label(new Rect(15, 10, Screen.width, 60), "Puntaje: " + puntaje + "     Vidas: " + vidas, estiloMarcador);

        string mensaje = null;
        if (fase == Fase.Ganaste) mensaje = "¡Ganaste!\nPresiona R para reiniciar";
        if (fase == Fase.Perdiste) mensaje = "Fin del juego\nPresiona R para reiniciar";

        if (mensaje != null)
        {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), mensaje, estiloMensaje);
        }
    }
}
