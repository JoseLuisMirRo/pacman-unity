using System.Collections.Generic;
using UnityEngine;

// Punto de entrada del juego.
// No hace falta poner nada en la escena: al dar Play, Unity llama
// automáticamente a AlIniciar() gracias a [RuntimeInitializeOnLoadMethod]
// y desde ahí se crea todo por código.
public static class Arranque
{
    // Colores propios de los personajes.
    static readonly Color ColorJugador = new Color(0.3f, 0.9f, 0.4f); // círculo verde
    static readonly Color[] ColoresPerseguidores =
    {
        new Color(0.95f, 0.25f, 0.3f), // rojo: el que persigue al jugador
        new Color(1f, 0.6f, 0.1f),     // naranja: se mueve al azar
        new Color(0.7f, 0.4f, 1f),     // morado: se mueve al azar
    };

    // Objeto raíz que contiene todo el juego; al reiniciar se destruye y se vuelve a crear.
    static GameObject raiz;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AlIniciar()
    {
        CrearJuego();
    }

    // Borra la partida actual y crea una nueva (tecla R).
    public static void Reiniciar()
    {
        if (raiz != null)
        {
            Object.Destroy(raiz);
        }
        CrearJuego();
    }

    static void CrearJuego()
    {
        raiz = new GameObject("Juego");

        // 1) Laberinto: paredes y puntos.
        Laberinto laberinto = raiz.AddComponent<Laberinto>();
        laberinto.Construir();

        // 2) Estado de la partida (puntaje, vidas, interfaz).
        EstadoJuego estado = raiz.AddComponent<EstadoJuego>();

        // 3) Jugador: un círculo de color propio.
        GameObject objetoJugador = GeneradorSprites.CrearObjeto("Jugador", GeneradorSprites.Circulo(), ColorJugador, 2, raiz.transform);
        objetoJugador.transform.localScale = Vector3.one * 0.8f;
        Jugador jugador = objetoJugador.AddComponent<Jugador>();
        jugador.Iniciar(laberinto, estado);

        // 4) Perseguidores: cuadros de colores. El primero persigue, los demás van al azar.
        List<Perseguidor> perseguidores = new List<Perseguidor>();
        for (int i = 0; i < laberinto.InicioPerseguidores.Count; i++)
        {
            Color color = ColoresPerseguidores[i % ColoresPerseguidores.Length];
            GameObject objeto = GeneradorSprites.CrearObjeto("Perseguidor " + (i + 1), GeneradorSprites.Cuadro(), color, 3, raiz.transform);
            objeto.transform.localScale = Vector3.one * 0.8f;

            Perseguidor perseguidor = objeto.AddComponent<Perseguidor>();
            bool persigue = i == 0;
            perseguidor.Iniciar(laberinto, jugador, estado, laberinto.InicioPerseguidores[i], persigue);
            perseguidores.Add(perseguidor);
        }

        estado.Iniciar(laberinto, jugador, perseguidores);

        // 5) Cámara ortográfica que muestra todo el laberinto.
        ConfigurarCamara(laberinto);
    }

    static void ConfigurarCamara(Laberinto laberinto)
    {
        // La escena de la plantilla ya trae una "Main Camera"; si no existiera, se crea una.
        Camera camara = Camera.main;
        if (camara == null)
        {
            GameObject objetoCamara = new GameObject("Main Camera");
            objetoCamara.tag = "MainCamera";
            camara = objetoCamara.AddComponent<Camera>();
        }

        camara.orthographic = true;
        camara.transform.position = new Vector3(0f, 0.5f, -10f); // un poco arriba para dejar espacio al marcador
        camara.clearFlags = CameraClearFlags.SolidColor;
        camara.backgroundColor = Color.black;

        // orthographicSize es la mitad de lo que se ve en vertical.
        // Se elige el tamaño para que quepa el laberinto a lo alto y a lo ancho.
        float mitadAlto = laberinto.Alto / 2f + 1.5f;
        float mitadAncho = (laberinto.Ancho / 2f + 0.5f) / camara.aspect;
        camara.orthographicSize = Mathf.Max(mitadAlto, mitadAncho);
    }
}
