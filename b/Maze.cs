using Figgle;
using Spectre.Console;

partial class Maze
{
    private static readonly Random rnd = new Random();
    static void Main()
{
    Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
    MostrarMenuPrincipal();
}
static void MostrarHistoria()
{
    CenterText("En una planta de energía termonuclear, algo ha fallado y está a punto de explotar.");
    CenterText("Si quieres salvarte, tendrás que conseguir la llave de la salida.");
    CenterText("¿Podrás?");
}

static void MostrarMenuPrincipal()
{
    string asciiArt = FiggleFonts.Standard.Render("MazeBoom");
    CenterText(asciiArt);
    MostrarHistoria();

    // Mostrar opciones de menú
    CenterText("Elige una opción:");
    CenterText("[1] Comenzar");
    CenterText("[2] Salir");
    EntradaValida();
}    
static void EntradaValida()
{    while (true) // Bucle para pedir entrada hasta que sea válida
    {
        string input = Console.ReadLine()??string.Empty;
        
        if (int.TryParse(input, out int opcion))
        {
            if (opcion == 1)
            {
                IniciarJuego();
                break;
            }
            else if (opcion == 2)
            {
                SalirJuego();
                break;
            }
            else
            {
                AnsiConsole.Write(new Markup($"[red]Opción no válida. Escoja una de estas opciones.[/]\n"));
            }
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]Entrada no válida. Por favor, ingrese un número.[/]\n"));
        }
    }        
}

// Método para centrar texto en la consola
static void CenterText(string text)
{
    string[] lines = text.Split('\n');
    foreach (string line in lines)
    {
        int windowWidth = Console.WindowWidth;
        int length = line.Length;
        int leftPadding = (windowWidth - length) / 2;
        Console.WriteLine($"{new string(' ', leftPadding)}{line}");
    }
}

static void MostrarInstrucciones()
{
    CenterText("Instrucciones del Juego:");
    CenterText("- Evita las trampas y recoge las llaves para ganar.");
    CenterText("- El número de jugador que eres,es tu personaje");
    CenterText("- Cada personaje tiene habilidades únicas, úsalas sabiamente.");
    CenterText("- Encuentra una llave para poder salir.");
    Console.WriteLine("Leyenda de Colores:");
    AnsiConsole.Markup("[white]■[/] Paredes\n");
    AnsiConsole.Markup("[black]■[/] Camino\n");
    AnsiConsole.Markup("[blue]▲[/] Tuberías rotas\n");
    AnsiConsole.Markup("[red]▲[/] Escombros\n");
    AnsiConsole.Markup("[magenta]▲[/] Fuego\n");
    AnsiConsole.Markup("[green]▲[/] Llave\n");
}

    public static void IniciarJuego()
    {
        MostrarInstrucciones();
        int ancho = 15;
        int altura = 15;

        // Definir el laberinto aquí
        Casilla[,] laberinto = GetMaze(ancho, altura);

        var jugadores = CrearPersonajes();
        var personajesSeleccionados = SeleccionarPersonajes(jugadores);

        // Asignar llaves según la cantidad de jugadores después de crear el laberinto
        ConseguirLlaves(laberinto, personajesSeleccionados.Count, 14, 13);

        AsignarPosicionesIniciales(personajesSeleccionados, laberinto);

        Juego juego = new Juego(personajesSeleccionados, laberinto);
        juego.Iniciar();
    }

    public static void SalirJuego()
    {
        Console.WriteLine("Saliendo del juego...");
        Environment.Exit(0);
    }

    static Casilla[,] GetMaze(int ancho, int altura)
    {
        Casilla[,] laberinto = new Casilla[ancho, altura];

        for (int i = 0; i < ancho; i++)
        {
            for (int j = 0; j < altura; j++)
            {
                laberinto[i, j] = Casilla.Walls;
            }
        }

        CrearCaminos(laberinto, 1, 1);
        laberinto[0, 1] = Casilla.Free;
        laberinto[ancho - 1, altura - 2] = Casilla.Free;

        AgregarTrampas(laberinto, 3);

        return laberinto;
    }

    static void CrearCaminos(Casilla[,] laberinto, int x, int y)
    {
        var directions = new List<(int dx, int dy)>
        {
            (0, -2),
            (0, 2),
            (-2, 0),
            (2, 0)
        };
        directions.Sort((a, b) => rnd.Next().CompareTo(rnd.Next()));
        foreach (var (dx, dy) in directions)
        {
            int nx = x + dx;
            int ny = y + dy;
            if (nx > 0 && nx < laberinto.GetLength(1) && ny > 0 && ny < laberinto.GetLength(0) && laberinto[ny, nx] == Casilla.Walls)
            {
                laberinto[ny, nx] = Casilla.Free;
                laberinto[y + dy / 2, x + dx / 2] = Casilla.Free;
                CrearCaminos(laberinto, nx, ny);
            }
        }
    }

    static void AgregarTrampas(Casilla[,] laberinto, int cantidadTrampas)
    {
        int filas = laberinto.GetLength(0);
        int columnas = laberinto.GetLength(1);
        var posicionesTrampas = new List<(int x, int y)>();

        for (int i = 0; i < cantidadTrampas * 7; i++)
        {
            while (true)
            {
                int x = rnd.Next(1, columnas - 1);
                int y = rnd.Next(1, filas - 1);

                if (laberinto[y, x] == Casilla.Free && !EstaCercaDeOtraTrampa(posicionesTrampas, x, y))
                {
                    Casilla trampa = (i % 3) switch
                    {
                        0 => Casilla.BrokenPipes,
                        1 => Casilla.Debris,
                        2 => Casilla.Fire,
                        _ => Casilla.Free,
                    };

                    laberinto[y, x] = trampa;
                    posicionesTrampas.Add((x, y));
                    break;
                }
            }
        }
    }

    static bool EstaCercaDeOtraTrampa(List<(int x, int y)> posicionesTrampas, int x, int y)
    {
        foreach (var (tx, ty) in posicionesTrampas)
        {
            if (Math.Abs(tx - x) + Math.Abs(ty - y) < 3)
            {
                return true;
            }
        }
        return false;
    }

    static void ConseguirLlaves(Casilla[,] laberinto, int cantidadLlaves, int salidaX, int salidaY)
    {
        int filas = laberinto.GetLength(0);
        int columnas = laberinto.GetLength(1);
        var posicionesLlaves = new List<(int x, int y)>();

        for (int i = 0; i < cantidadLlaves; i++)
        {
            while (true)
            {
                int x = rnd.Next(1, columnas - 1);
                int y = rnd.Next(1, filas - 1);

                if (laberinto[y, x] == Casilla.Free && !EstaCercaDeOtraLlave(posicionesLlaves, x, y) && DistanciaManhattan(x, y, salidaX, salidaY) >= 15)
                {
                    laberinto[y, x] = Casilla.Llave;
                    posicionesLlaves.Add((x, y));
                    break;
                }
            }
        }
    }

    static bool EstaCercaDeOtraLlave(List<(int x, int y)> posicionesLlaves, int x, int y)
    {
        foreach (var (lx, ly) in posicionesLlaves)
        {
            if (Math.Abs(lx - x) + Math.Abs(ly - y) < 3)
            {
                return true;
            }
        }
        return false;
    }

    static int DistanciaManhattan(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    static List<Personaje> CrearPersonajes()
    {
        return new List<Personaje>
        {
            new Personaje { Nombre = "Físico", Habilidad = "Teletransporte", ExplicacionHabilidad = "Mueve al jugador a una casilla libre aleatoria en un radio de 7 casillas", TiempoEnfriamiento = 7, Velocidad = 1 },
            new Personaje { Nombre = "Químico", Habilidad = "Envenenar", ExplicacionHabilidad = "Envenena a los jugadores que están en la misma casilla donde el jugador activó la habilidad, enviándolo de vuelta al inicio", TiempoEnfriamiento = 9, Velocidad = 1 },
            new Personaje { Nombre = "Director", Habilidad = "Robar la llave", ExplicacionHabilidad = "Roba la llave de un jugador en la misma casilla,si él no tiene una llave cuando la roba se queda con ella,si ya tiene una llave cuando roba otra esta aparece en una casilla libre aleatoria  ", TiempoEnfriamiento = 8, Velocidad = 1 },
            new Personaje { Nombre = "Ingeniero", Habilidad = "Sabotaje", ExplicacionHabilidad = "Coloca una trampa de tuberias rotas en una de las casillas libres a su alrededor,tiene que tener al menos dos opciones donde ponerla y nunca se puede poner en el inicio", TiempoEnfriamiento = 10, Velocidad = 1 },
            new Personaje { Nombre = "Médico", Habilidad = "Curación", ExplicacionHabilidad = "Proporciona inmunidad a trampas durante 3 turnos,si el jugador activa la habilidad y pasa por una casilla con trampa en el próximo turno ya no hace efecto", TiempoEnfriamiento = 15, Velocidad = 1 },
            new Personaje { Nombre = "Monstruo Radioactivo", Habilidad = "Boo", ExplicacionHabilidad = "Permite aparecer en la casilla de cualquier otro jugador", TiempoEnfriamiento = 12, Velocidad = 1 },
        };
    }

static List<Personaje> SeleccionarPersonajes(List<Personaje> personajesDisponibles)
{
    var personajesSeleccionados = new List<Personaje>();
    int cantidadJugadores;

    // Solicitar la cantidad de jugadores
    while (true)
    {
        Console.WriteLine("Ingrese la cantidad de jugadores (mínimo 2, máximo 4):");

        if (int.TryParse(Console.ReadLine(), out cantidadJugadores) && cantidadJugadores >= 2 && cantidadJugadores <= 4)
        {
            break;
        }
        else
        {
            Console.WriteLine("Cantidad de jugadores no válida. Por favor, ingrese un número entre 2 y 4.");
        }
    }

    // Solicitar el nombre de cada jugador y la selección de personajes
    for (int i = 0; i < cantidadJugadores; i++)
    {
        Console.WriteLine($"Jugador {i + 1}, ingresa tu nombre:");
        string nombreJugador = Console.ReadLine() ?? string.Empty;

        Console.WriteLine($"{nombreJugador}, elige tu personaje:");

        // Mostrar opciones de personajes disponibles una vez
        for (int j = 0; j < personajesDisponibles.Count; j++)
        {
            var personaje = personajesDisponibles[j];
            Console.WriteLine($"[{j + 1}] {personaje.Nombre} - Habilidad: {personaje.Habilidad} (Explicación: {personaje.ExplicacionHabilidad}) (Enfriamiento: {personaje.TiempoEnfriamiento})");
        }

        while (true)
        {
            // Selección de personaje por el jugador
            Console.Write("Selecciona un personaje: ");
            if (int.TryParse(Console.ReadLine(), out int eleccionPersonaje) && eleccionPersonaje >= 1 && eleccionPersonaje <= personajesDisponibles.Count)
            {
                personajesDisponibles[eleccionPersonaje - 1].Nombre = nombreJugador;
                personajesSeleccionados.Add(personajesDisponibles[eleccionPersonaje - 1]);
                personajesDisponibles.RemoveAt(eleccionPersonaje - 1);
                break;
            }
            else
            {
                Console.WriteLine("Selección no válida. Por favor, elija un número de la lista de personajes.");
            }
        }
    }

    return personajesSeleccionados;
}

    static void AsignarPosicionesIniciales(List<Personaje> personajes, Casilla[,] laberinto)
    {
        foreach (var jugador in personajes)
        {
            jugador.Posicion = (0, 1); // Asignar posición inicial
        }

        Juego.PrintMaze(laberinto, personajes);
        foreach (var jugador in personajes)
        {
            Console.WriteLine($"Jugador {jugador.Nombre} está en la posición ({jugador.Posicion.x}, {jugador.Posicion.y})");
        }
    }
}