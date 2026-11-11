using System.Collections.Generic;
using UnityEngine;

// Guarda el mapa del laberinto, dibuja paredes y puntos,
// y responde preguntas como "¿esta casilla es pared?".
//
// Coordenadas de casilla: x = columna (0 a la izquierda),
// y = fila (0 ARRIBA, crece hacia abajo, igual que se lee el texto del mapa).
public class Laberinto : MonoBehaviour
{
    // Mapa del nivel. Cada carácter es una casilla:
    //   '#' = pared
    //   '.' = punto que se puede recoger
    //   'J' = casilla inicial del jugador (sin punto)
    //   '1', '2', '3' = casillas iniciales de los perseguidores (sin punto)
    static readonly string[] Mapa =
    {
        "###################",
        "#........#........#",
        "#.##.###.#.###.##.#",
        "#.................#",
        "#.##.#.#####.#.##.#",
        "#....#...#...#....#",
        "####.###.#.###.####",
        "#......#123#......#",
        "#.##.#.#####.#.##.#",
        "#........J........#",
        "#.##.###.#.###.##.#",
        "#....#.......#....#",
        "#.##.#.#####.#.##.#",
        "#.................#",
        "###################",
    };

    // Colores del laberinto.
    static readonly Color ColorPared = new Color(0.25f, 0.35f, 0.55f);
    static readonly Color ColorPunto = new Color(1f, 0.85f, 0.75f);

    public int Ancho { get; private set; }
    public int Alto { get; private set; }

    public Vector2Int InicioJugador { get; private set; }
    public List<Vector2Int> InicioPerseguidores { get; private set; } = new List<Vector2Int>();

    // true en las casillas que son pared.
    bool[,] paredes;

    // Puntos que aún no se han recogido (casilla -> objeto dibujado).
    readonly Dictionary<Vector2Int, GameObject> puntos = new Dictionary<Vector2Int, GameObject>();

    public int PuntosRestantes => puntos.Count;

    // Lee el mapa de texto y crea los objetos de paredes y puntos.
    public void Construir()
    {
        Alto = Mapa.Length;
        Ancho = Mapa[0].Length;
        paredes = new bool[Ancho, Alto];

        // Los perseguidores se guardan por número para que el '1' sea siempre el primero.
        SortedDictionary<char, Vector2Int> perseguidores = new SortedDictionary<char, Vector2Int>();

        for (int y = 0; y < Alto; y++)
        {
            for (int x = 0; x < Ancho; x++)
            {
                char c = Mapa[y][x];
                Vector2Int celda = new Vector2Int(x, y);

                if (c == '#')
                {
                    paredes[x, y] = true;
                    GeneradorSprites.CrearObjeto("Pared", GeneradorSprites.Cuadro(), ColorPared, 0, transform)
                        .transform.position = AMundo(celda);
                }
                else if (c == '.')
                {
                    GameObject punto = GeneradorSprites.CrearObjeto("Punto", GeneradorSprites.Circulo(), ColorPunto, 1, transform);
                    punto.transform.position = AMundo(celda);
                    punto.transform.localScale = Vector3.one * 0.25f; // los puntos son pequeños
                    puntos.Add(celda, punto);
                }
                else if (c == 'J')
                {
                    InicioJugador = celda;
                }
                else if (c >= '1' && c <= '9')
                {
                    perseguidores[c] = celda;
                }
            }
        }

        InicioPerseguidores.AddRange(perseguidores.Values);
    }

    // Una casilla fuera del mapa también cuenta como pared.
    public bool EsPared(Vector2Int celda)
    {
        if (celda.x < 0 || celda.y < 0 || celda.x >= Ancho || celda.y >= Alto)
        {
            return true;
        }
        return paredes[celda.x, celda.y];
    }

    // Si hay un punto en la casilla, lo quita y devuelve true.
    public bool ComerPunto(Vector2Int celda)
    {
        if (puntos.TryGetValue(celda, out GameObject punto))
        {
            Destroy(punto);
            puntos.Remove(celda);
            return true;
        }
        return false;
    }

    // Convierte una casilla en una posición del mundo.
    // El laberinto queda centrado en (0, 0) y la fila 0 queda arriba.
    public Vector3 AMundo(Vector2Int celda)
    {
        float x = celda.x - (Ancho - 1) / 2f;
        float y = (Alto - 1) / 2f - celda.y;
        return new Vector3(x, y, 0f);
    }
}
