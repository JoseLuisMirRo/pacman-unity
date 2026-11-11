# Come-puntos

Juego 2D sencillo de laberinto hecho en Unity. Controlas un círculo verde que recorre un laberinto
recogiendo puntos mientras tres perseguidores (cuadros de colores) intentan atraparte.

Todos los gráficos se generan por código: el proyecto no usa imágenes, sonidos ni otros archivos externos.

## Cómo se juega

- Recorre el laberinto casilla por casilla y recoge **todos los puntos** para ganar (cada punto vale 10).
- Hay **3 perseguidores**:
  - **Rojo**: siempre intenta acercarse a ti.
  - **Naranja** y **morado**: se mueven al azar.
- Empiezas con **3 vidas**. Si un perseguidor te toca pierdes una y todos regresan a su lugar inicial.
- Si te quedas sin vidas, se acaba la partida.

### Controles

| Tecla | Acción |
|---|---|
| `W` / `↑` | Mover arriba |
| `S` / `↓` | Mover abajo |
| `A` / `←` | Mover a la izquierda |
| `D` / `→` | Mover a la derecha |
| `R` | Reiniciar la partida |

Mantén presionada una tecla para seguir avanzando en esa dirección.

## Versión de Unity

**Unity 6000.6.0f1** (tomada de `ProjectSettings/ProjectVersion.txt`).

El proyecto se creó con la plantilla **Universal 2D** y usa el **nuevo Input System**
(`com.unity.inputsystem`), que es el que la plantilla trae activado por defecto.

## Cómo abrir el proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/JoseLuisMirRo/pacman-unity.git
   ```
2. Abre **Unity Hub** → pestaña **Projects** → **Add** → **Add project from disk**.
3. Selecciona la carpeta `pacman-unity` (la que contiene `Assets`, `Packages` y `ProjectSettings`).
4. Si Unity Hub pide instalar la versión 6000.6.0f1, instálala (o elige una versión 6000.x compatible).
5. La primera vez Unity tarda unos minutos en importar el proyecto.
6. Abre la escena `Assets/Scenes/SampleScene.unity` y presiona **Play**.

No hace falta configurar nada en la escena: el juego se crea solo al dar Play.

## Cómo está organizado el código

Todo el código está en `Assets/Scripts/`. Cada archivo tiene una sola responsabilidad:

| Archivo | Qué hace |
|---|---|
| `Arranque.cs` | Punto de entrada. Usa `[RuntimeInitializeOnLoadMethod]` para crear todo al dar Play: laberinto, jugador, perseguidores, estado de la partida y cámara ortográfica. También reinicia el juego. |
| `Laberinto.cs` | Contiene el mapa como texto (`#` pared, `.` punto, `J` jugador, `1-3` perseguidores), dibuja paredes y puntos y dice qué casillas son pared. |
| `Jugador.cs` | Lee el teclado y mueve al jugador una casilla a la vez; recoge los puntos. |
| `Perseguidor.cs` | Mueve a un perseguidor por el laberinto: uno busca al jugador y los otros eligen caminos al azar. |
| `EstadoJuego.cs` | Reglas de la partida: puntaje, vidas, ganar, perder, tecla R y el texto en pantalla (con `OnGUI`). |
| `GeneradorSprites.cs` | Crea por código los sprites de cuadro y círculo con `Texture2D` y `Sprite.Create`. |

Para cambiar el laberinto basta con editar el arreglo `Mapa` en `Laberinto.cs`.
