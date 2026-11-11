using UnityEngine;

// Crea por código los sprites del juego (no se usan imágenes externas).
// Solo hay dos formas: un cuadro y un círculo, ambos blancos.
// El color final de cada objeto se pone con SpriteRenderer.color.
public static class GeneradorSprites
{
    // Se guardan para no crear la misma textura cada vez que se piden.
    static Sprite cuadro;
    static Sprite circulo;

    // Devuelve un sprite cuadrado blanco que mide 1x1 unidades del mundo.
    public static Sprite Cuadro()
    {
        if (cuadro == null)
        {
            const int tam = 16;
            Texture2D textura = new Texture2D(tam, tam);
            textura.filterMode = FilterMode.Point; // bordes nítidos

            // Todos los píxeles en blanco.
            Color[] pixeles = new Color[tam * tam];
            for (int i = 0; i < pixeles.Length; i++)
            {
                pixeles[i] = Color.white;
            }
            textura.SetPixels(pixeles);
            textura.Apply();

            // pixelsPerUnit = tam  ->  el sprite mide exactamente 1 unidad.
            cuadro = Sprite.Create(textura, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), tam);
        }
        return cuadro;
    }

    // Devuelve un sprite circular blanco de 1 unidad de diámetro.
    public static Sprite Circulo()
    {
        if (circulo == null)
        {
            const int tam = 64;
            Texture2D textura = new Texture2D(tam, tam);
            textura.filterMode = FilterMode.Bilinear; // bordes suaves

            float radio = tam / 2f;
            Vector2 centro = new Vector2(radio, radio);

            // Cada píxel dentro del radio es blanco; fuera es transparente.
            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    float distancia = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), centro);
                    textura.SetPixel(x, y, distancia <= radio ? Color.white : Color.clear);
                }
            }
            textura.Apply();

            circulo = Sprite.Create(textura, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), tam);
        }
        return circulo;
    }

    // Atajo: crea un GameObject con un SpriteRenderer ya configurado.
    public static GameObject CrearObjeto(string nombre, Sprite sprite, Color color, int orden, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre, false);

        SpriteRenderer render = objeto.AddComponent<SpriteRenderer>();
        render.sprite = sprite;
        render.color = color;
        render.sortingOrder = orden; // los números mayores se dibujan encima

        return objeto;
    }
}
