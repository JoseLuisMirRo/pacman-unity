using UnityEngine;
using UnityEngine.InputSystem; // el proyecto usa el nuevo Input System

// Mueve al jugador una casilla a la vez con WASD o las flechas
// y recoge los puntos por los que pasa.
public class Jugador : MonoBehaviour
{
    // Segundos que espera entre un paso y el siguiente si mantienes la tecla.
    const float TiempoPorPaso = 0.15f;

    // Casilla actual y la casilla donde estaba antes del último paso
    // (la anterior sirve para detectar choques "cruzados" con un perseguidor).
    public Vector2Int Celda { get; private set; }
    public Vector2Int CeldaAnterior { get; private set; }

    Laberinto laberinto;
    EstadoJuego estado;
    float temporizador;

    public void Iniciar(Laberinto laberinto, EstadoJuego estado)
    {
        this.laberinto = laberinto;
        this.estado = estado;
        VolverAlInicio();
    }

    // Coloca al jugador en su casilla inicial (al empezar o al perder una vida).
    public void VolverAlInicio()
    {
        Celda = laberinto.InicioJugador;
        CeldaAnterior = Celda;
        transform.position = laberinto.AMundo(Celda);
        temporizador = 0f;
    }

    void Update()
    {
        // Si el juego terminó (ganaste o perdiste), el jugador ya no se mueve.
        if (!estado.Jugando)
        {
            return;
        }

        // Espera a que pase el tiempo del paso anterior.
        temporizador -= Time.deltaTime;
        if (temporizador > 0f)
        {
            return;
        }

        Vector2Int direccion = LeerDireccion();
        if (direccion == Vector2Int.zero)
        {
            return; // no hay tecla presionada
        }

        Vector2Int destino = Celda + direccion;
        if (laberinto.EsPared(destino))
        {
            return; // no se puede atravesar una pared
        }

        // Da el paso.
        CeldaAnterior = Celda;
        Celda = destino;
        transform.position = laberinto.AMundo(Celda);
        temporizador = TiempoPorPaso;

        // Si había un punto, se suma al puntaje.
        if (laberinto.ComerPunto(Celda))
        {
            estado.PuntoRecogido();
        }

        estado.RevisarChoques();
    }

    // Devuelve la dirección de la tecla presionada o (0,0) si no hay ninguna.
    // Recuerda: en la cuadrícula la "y" crece hacia abajo, por eso arriba es (0, -1).
    Vector2Int LeerDireccion()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null)
        {
            return Vector2Int.zero; // no hay teclado conectado
        }

        if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) return new Vector2Int(0, -1);
        if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) return new Vector2Int(0, 1);
        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) return new Vector2Int(-1, 0);
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) return new Vector2Int(1, 0);

        return Vector2Int.zero;
    }
}
