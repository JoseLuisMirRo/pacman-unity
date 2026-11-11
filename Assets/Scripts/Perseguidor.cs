using System.Collections.Generic;
using UnityEngine;

// Un perseguidor que avanza por el laberinto una casilla a la vez.
// Hay dos comportamientos:
//   - PersigueAlJugador = true : elige el camino que más lo acerca al jugador.
//   - PersigueAlJugador = false: elige un camino al azar.
// En ambos casos evita dar media vuelta, salvo en un callejón sin salida.
public class Perseguidor : MonoBehaviour
{
    // Segundos entre cada paso (más lento que el jugador para que sea justo).
    const float TiempoPorPaso = 0.3f;

    // Las cuatro direcciones posibles: arriba, abajo, izquierda, derecha.
    static readonly Vector2Int[] Direcciones =
    {
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
    };

    public Vector2Int Celda { get; private set; }
    public Vector2Int CeldaAnterior { get; private set; }

    bool persigueAlJugador;
    Vector2Int inicio;
    Vector2Int direccionActual;
    float temporizador;

    Laberinto laberinto;
    Jugador jugador;
    EstadoJuego estado;

    public void Iniciar(Laberinto laberinto, Jugador jugador, EstadoJuego estado, Vector2Int inicio, bool persigueAlJugador)
    {
        this.laberinto = laberinto;
        this.jugador = jugador;
        this.estado = estado;
        this.inicio = inicio;
        this.persigueAlJugador = persigueAlJugador;
        VolverAlInicio();
    }

    // Regresa a su casilla inicial (al empezar o cuando el jugador pierde una vida).
    public void VolverAlInicio()
    {
        Celda = inicio;
        CeldaAnterior = inicio;
        direccionActual = Vector2Int.zero;
        transform.position = laberinto.AMundo(Celda);
        temporizador = TiempoPorPaso;
    }

    void Update()
    {
        if (!estado.Jugando)
        {
            return;
        }

        temporizador -= Time.deltaTime;
        if (temporizador > 0f)
        {
            return;
        }
        temporizador = TiempoPorPaso;

        // Elige hacia dónde ir y da un paso.
        direccionActual = ElegirDireccion();
        CeldaAnterior = Celda;
        Celda += direccionActual;
        transform.position = laberinto.AMundo(Celda);

        estado.RevisarChoques();
    }

    Vector2Int ElegirDireccion()
    {
        // 1) Junta las direcciones posibles: sin pared y sin dar media vuelta.
        List<Vector2Int> opciones = new List<Vector2Int>();
        foreach (Vector2Int direccion in Direcciones)
        {
            bool esMediaVuelta = direccion == -direccionActual;
            if (!laberinto.EsPared(Celda + direccion) && !esMediaVuelta)
            {
                opciones.Add(direccion);
            }
        }

        // 2) Si no hay opciones es un callejón sin salida: la única salida es regresar.
        if (opciones.Count == 0)
        {
            return -direccionActual;
        }

        // 3a) Perseguidor al azar: cualquier opción vale.
        if (!persigueAlJugador)
        {
            return opciones[Random.Range(0, opciones.Count)];
        }

        // 3b) Perseguidor que caza: la opción que deja menos distancia al jugador.
        //     Se usa la distancia "Manhattan" (pasos en horizontal + pasos en vertical).
        Vector2Int mejor = opciones[0];
        int menorDistancia = int.MaxValue;
        foreach (Vector2Int direccion in opciones)
        {
            Vector2Int siguiente = Celda + direccion;
            int distancia = Mathf.Abs(siguiente.x - jugador.Celda.x) + Mathf.Abs(siguiente.y - jugador.Celda.y);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                mejor = direccion;
            }
        }
        return mejor;
    }
}
