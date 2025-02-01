using Spectre.Console;

partial class Maze
{
    // Clase Juego que contiene la lógica del juego
    class Juego
    {
        private static readonly Random rnd = new Random(); // Generador de números aleatorios
        public List<Personaje> Jugadores { get; set; }
        public Casilla[,] Laberinto { get; set; }
        private (int x, int y) Meta { get; set; }

        public Juego(List<Personaje> jugadores, Casilla[,] laberinto)
        {
            Jugadores = jugadores;
            Laberinto = laberinto;
            Meta = (laberinto.GetLength(0) - 1, laberinto.GetLength(1) - 2); // Definir la meta en la casilla (32, 31)
        }

        // Función para iniciar el juego
        public void Iniciar()
        {
            bool juegoEnCurso = true;

            while (juegoEnCurso)
            {
                foreach (var jugador in Jugadores.ToList()) // Usar ToList() para evitar errores al modificar la lista
                {
                    // Mostrar estado del jugador
                    MostrarEstadoJugador(jugador);

                    // Gestionar enfriamiento de la habilidad
                    if (!jugador.HabilidadDisponible && jugador.TiempoEnfriamiento > 0)
                    {
                        jugador.TiempoEnfriamiento--;
                        if (jugador.TiempoEnfriamiento == 0)
                        {
                            jugador.HabilidadDisponible = true;
                            AnsiConsole.Write(new Markup($"[green]¡La habilidad de {jugador.Nombre} está disponible nuevamente![/]\n"));
                        }
                    }

                    // Verificar si el jugador está en un estado de "Turnos Sin Jugar"
                    if (jugador.TurnosSinJugar > 0)
                    {
                        AnsiConsole.Write(new Markup($"[yellow]{jugador.Nombre} te has quedado atorado {jugador.TurnosSinJugar}[/]\n"));
                        jugador.TurnosSinJugar--;
                        continue; // Saltar el turno del jugador
                    }

                    // Mostrar opciones y mover al jugador
                    MostrarOpcionesJugador(jugador);
                    PrintMaze(Laberinto, Jugadores);

                    // Verificar si el jugador ha alcanzado la meta
                    if (jugador.Posicion == Meta)
                    {
                        if (jugador.TieneLlave)
                        {
                            AnsiConsole.Write(new Markup($"[green]¡{jugador.Nombre} ha ganado el juego![/]\n"));
                            juegoEnCurso = false;
                            break;
                        }
                        else
                        {
                            AnsiConsole.Write(new Markup("[red]No puedes salir. Te falta una llave para salir.[/]\n"));
                        }
                    }
                    // Reducir el contador de inmunidad al final del turno del jugador
            if (jugador.TurnosDeInmunidad > 0)
            {
                jugador.TurnosDeInmunidad--;
                if (jugador.TurnosDeInmunidad == 0)
                {
                    AnsiConsole.Write(new Markup($"[yellow]{jugador.Nombre}, tu inmunidad ha terminado. Ahora eres vulnerable a las trampas.[/]\n"));
                }
            }
                }

                // Si el juego ha terminado, mostrar el menú de opciones
                if (!juegoEnCurso)
                {
                    MostrarMenuFinDeJuego();
                }
            }
        }

        // Función para mostrar el menú de fin de juego
        void MostrarMenuFinDeJuego()
        {
            Console.WriteLine("Elige una opción:");
            Console.WriteLine("[1] Jugar una nueva partida");
            Console.WriteLine("[2] Salir");

            EntradaValida();
        }

        // Función para mostrar el estado actual del jugador
        void MostrarEstadoJugador(Personaje jugador)
        {
            AnsiConsole.Write(new Markup($"\nJugador {jugador.Nombre} - Habilidad: {jugador.Habilidad} (Enfriamiento: {jugador.TiempoEnfriamiento})\n"));
            AnsiConsole.Write(new Markup($"Posición: ({jugador.Posicion.x}, {jugador.Posicion.y})\n"));
            if (jugador.TieneLlave)
            {
                AnsiConsole.Write(new Markup("[yellow]Llave: Sí[/]\n"));
            }
            else
            {
                AnsiConsole.Write(new Markup("[yellow]Llave: No[/]\n"));
            }
        }

        // Función para mostrar las opciones disponibles para el jugador
void MostrarOpcionesJugador(Personaje jugador)
{
    Console.WriteLine("Opciones:");
    Console.WriteLine("[1] Mover hacia arriba");
    Console.WriteLine("[2] Mover hacia abajo");
    Console.WriteLine("[3] Mover hacia la izquierda");
    Console.WriteLine("[4] Mover hacia la derecha");
    Console.WriteLine("[5] Activar habilidad");
    Console.WriteLine("[Esc] Abandonar partida");

    while (true)
    {
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.D1:
                if (Mover(jugador, -1, 0)) return;
                break;
            case ConsoleKey.D2:
                if (Mover(jugador, 1, 0)) return;
                break;
            case ConsoleKey.D3:
                if (Mover(jugador, 0, -1)) return;
                break;
            case ConsoleKey.D4:
                if (Mover(jugador, 0, 1)) return;
                break;
            case ConsoleKey.D5:
                ActivarHabilidad(jugador);
                return;
            case ConsoleKey.Escape:
                AbandonarPartida(jugador);
                return;
            default:
                Console.WriteLine("Opción no válida. Intenta de nuevo.");
                break;
        }
    }
}
        // Método para manejar la opción de abandonar partida
        void AbandonarPartida(Personaje jugador)
        {
            AnsiConsole.Write(new Markup($"[red]{jugador.Nombre} ha abandonado la partida.[/]\n"));
            Jugadores.Remove(jugador);

            if (Jugadores.Count == 1)
            {
                Console.WriteLine($"Lo siento, {Jugadores[0].Nombre}, te has quedado sin oponente.");
                MostrarMenuFinDeJuego();
            }
            else if (Jugadores.Count >= 2)
            {
                var jugadoresRestantes = new List<Personaje>(Jugadores); // Crear una copia de la lista de jugadores
                foreach (var j in jugadoresRestantes)
                {
                    Console.WriteLine($"{j.Nombre}, ¿deseas continuar jugando? (S/N)");
                    var respuesta = Console.ReadLine()?.ToUpper();
                    if (respuesta == "N")
                    {
                        AnsiConsole.Write(new Markup($"[red]{j.Nombre} ha decidido abandonar la partida.[/]\n"));
                        Jugadores.Remove(j);
                    }
                }

                if (Jugadores.Count == 1)
                {
                    Console.WriteLine($"Lo siento, {Jugadores[0].Nombre}, te has quedado sin oponente.");
                    MostrarMenuFinDeJuego();
                }
                else if (Jugadores.Count >= 2)
                {
                    Console.WriteLine("El juego continúa con los jugadores restantes.");
                }
                else
                {
                    MostrarMenuFinDeJuego();
                }
            }
        }

        // Método para activar la habilidad del jugador
        void ActivarHabilidad(Personaje jugador)
        {
            if (!jugador.HabilidadDisponible)
            {
                AnsiConsole.Write(new Markup("[yellow]La habilidad aún está en enfriamiento.[/]\n"));
                Thread.Sleep(2000);
                return;
            }

            AnsiConsole.Write(new Markup($"[green]{jugador.Nombre} activó su habilidad: {jugador.Habilidad}[/]\n"));

            switch (jugador.Habilidad)
            {
                case "Teletransporte":
                    Teletransporte(jugador);
                    break;
                case "Envenenar":
                    Envenenar(jugador);
                    break;
                case "Robar la llave":
                    RobarLlave(jugador);
                    break;
                case "Sabotaje":
                    Sabotaje(jugador);
                    break;
                case "Curación":
                    Curacion(jugador);
                    break;
                case "Boo":
                    Boo(jugador);
                    break;
                default:
                    AnsiConsole.Write(new Markup("[red]Habilidad desconocida![/]\n"));
                    break;
            }

            jugador.HabilidadDisponible = false;
        }
        // Método para la habilidad Sabotaje del ingeniero
void Sabotaje(Personaje jugador)
{
    int dx = 0, dy = 0;

    while (true)
    {
        Console.WriteLine("Elige la dirección para colocar la trampa de tuberías rotas:");
        Console.WriteLine("[1] Arriba");
        Console.WriteLine("[2] Abajo");
        Console.WriteLine("[3] Izquierda");
        Console.WriteLine("[4] Derecha");

        bool opcionValida = false;

        while (!opcionValida)
        {
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    dx = -1;
                    dy = 0;
                    opcionValida = true;
                    break;
                case ConsoleKey.D2:
                    dx = 1;
                    dy = 0;
                    opcionValida = true;
                    break;
                case ConsoleKey.D3:
                    dx = 0;
                    dy = -1;
                    opcionValida = true;
                    break;
                case ConsoleKey.D4:
                    dx = 0;
                    dy = 1;
                    opcionValida = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida. Intenta de nuevo.");
                    break; // Permitir al jugador volver a escoger sin mostrar el menú nuevamente
            }
        }

        var nuevaPosicion = (x: jugador.Posicion.x + dx, y: jugador.Posicion.y + dy);

        // Verificar que haya al menos dos posiciones disponibles
        var direccionesPosibles = new List<(int dx, int dy)> { (-1, 0), (1, 0), (0, -1), (0, 1) };
        int posicionesDisponibles = 0;

        foreach (var (dxPosible, dyPosible) in direccionesPosibles)
        {
            var posicionPosible = (x: jugador.Posicion.x + dxPosible, y: jugador.Posicion.y + dyPosible);

            if (posicionPosible.x >= 0 && posicionPosible.x < Laberinto.GetLength(0) &&
                posicionPosible.y >= 0 && posicionPosible.y < Laberinto.GetLength(1) &&
                Laberinto[posicionPosible.x, posicionPosible.y] == Casilla.Free &&
                posicionPosible != (0, 1))
            {
                posicionesDisponibles++;
            }
        }

        if (posicionesDisponibles < 2)
        {
            AnsiConsole.Write(new Markup("[red]No hay suficientes posiciones disponibles para colocar la trampa.[/]\n"));
            return;
        }

        // Verificar que la nueva posición esté dentro del rango del laberinto, sea una casilla libre y no sea la casilla de inicio
        if (nuevaPosicion.x >= 0 && nuevaPosicion.x < Laberinto.GetLength(0) && 
            nuevaPosicion.y >= 0 && nuevaPosicion.y < Laberinto.GetLength(1) && 
            Laberinto[nuevaPosicion.x, nuevaPosicion.y] == Casilla.Free && 
            nuevaPosicion != (0, 1))
        {
            // Verificar si hay un jugador en la nueva posición
            var jugadorEnCasilla = Jugadores.FirstOrDefault(j => j.Posicion == nuevaPosicion);

            if (jugadorEnCasilla != null)
            {
                // Activar la trampa inmediatamente si hay un jugador en la nueva posición
                AnsiConsole.Write(new Markup($"[red]¡{jugadorEnCasilla.Nombre} ha caído en la trampa de tuberías rotas![/]\n"));
                Laberinto[nuevaPosicion.x, nuevaPosicion.y] = Casilla.BrokenPipes;
                Thread.Sleep(2000);

                // Aplicar efecto de la trampa al jugador afectado
                if (jugadorEnCasilla.PosicionesAnteriores.Count == 4)
                {
                    jugadorEnCasilla.Posicion = jugadorEnCasilla.PosicionesAnteriores.Dequeue();
                    AnsiConsole.Write(new Markup("[red]El agua te ha llevado 5 casillas hacia atrás por donde habías venido.[/]\n"));
                    Thread.Sleep(2000);
                }
                else
                {
                    // No hay suficientes turnos guardados, llevar al inicio
                    jugadorEnCasilla.Posicion = (0, 1);
                    AnsiConsole.Write(new Markup("[red]El agua te ha llevado hasta el inicio.[/]\n"));
                    ActivarTrampaInicio(jugadorEnCasilla);
                    Thread.Sleep(2000);
                }

                Laberinto[nuevaPosicion.x, nuevaPosicion.y] = Casilla.Free; // Desactivar trampa
            }
            else
            {
                // Colocar la trampa en la nueva posición si no hay jugador
                Laberinto[nuevaPosicion.x, nuevaPosicion.y] = Casilla.BrokenPipes;
                AnsiConsole.Write(new Markup("[green]¡Sabotaje exitoso! Has colocado una trampa de tuberías rotas.[/]\n"));
                Thread.Sleep(2000);
            }

            break; // Salir del bucle una vez que se ha colocado la trampa con éxito
        }
        else
        {
            Console.WriteLine("[red]No puedes colocar la trampa fuera del rango del laberinto, en una casilla que no sea libre o en la casilla de inicio.[/]");
        }
    }

    jugador.HabilidadDisponible = false;
    jugador.TiempoEnfriamiento = 4; // Ajustar el tiempo de enfriamiento según la habilidad
}

        // Método para manejar la activación inmediata de trampas en el inicio
        void ActivarTrampaInicio(Personaje jugador)
        {
            AnsiConsole.Write(new Markup($"[red]¡{jugador.Nombre} ha caído en la trampa de tuberías rotas en el inicio![/]\n"));
            Thread.Sleep(2000);

            if (jugador.PosicionesAnteriores.Count == 4)
            {
                jugador.Posicion = jugador.PosicionesAnteriores.Dequeue();
                AnsiConsole.Write(new Markup("[red]El agua te ha llevado 5 casillas hacia atrás por donde habías venido.[/]\n"));
                Thread.Sleep(2000);
            }
            else
            {
                // No hay suficientes turnos guardados, llevar al inicio
                jugador.Posicion = (0, 1);
                AnsiConsole.Write(new Markup("[red]El agua te ha llevado hasta el inicio.[/]\n"));
                Thread.Sleep(2000);
            }

            Laberinto[jugador.Posicion.x, jugador.Posicion.y] = Casilla.Free; // Desactivar trampa
        }

        // Método para la habilidad de teletransporte del físico
        void Teletransporte(Personaje jugador)
        {
            int filas = Laberinto.GetLength(0);
            int columnas = Laberinto.GetLength(1);
            var casillasLibresEnRadio = new List<(int x, int y)>();

            // Buscar casillas libres en un radio de 7 casillas
            for (int i = Math.Max(0, jugador.Posicion.x - 7); i <= Math.Min(filas - 1, jugador.Posicion.x + 7); i++)
            {
                for (int j = Math.Max(0, jugador.Posicion.y - 7); j <= Math.Min(columnas - 1, jugador.Posicion.y + 7); j++)
                {
                    // Solo considerar las casillas libres y que no sean la misma en la que está el jugador
                    if (Laberinto[i, j] == Casilla.Free && (i != jugador.Posicion.x || j != jugador.Posicion.y))
                    {
                        casillasLibresEnRadio.Add((i, j));
                    }
                }
            }

            if (casillasLibresEnRadio.Count > 0)
            {
                // Seleccionar una casilla libre aleatoria
                var nuevaPosicion = casillasLibresEnRadio[rnd.Next(casillasLibresEnRadio.Count)];
                jugador.PosicionesAnteriores.Enqueue(jugador.Posicion); // Guardar la posición anterior
                if (jugador.PosicionesAnteriores.Count > 4)
                {
                    jugador.PosicionesAnteriores.Dequeue(); // Limitar a las últimas 4 posiciones
                }
                jugador.Posicion = nuevaPosicion;
                AnsiConsole.Write(new Markup("[green]¡Teletransporte exitoso![/]\n"));
                Thread.Sleep(2000);
            }
            else
            {
                AnsiConsole.Write(new Markup("[red]No hay casillas libres disponibles para teletransportarse dentro del radio permitido.[/]\n"));
            }
            jugador.TiempoEnfriamiento = 5;
        }

        // Método para la habilidad Envenenar del químico
        void Envenenar(Personaje jugador)
        {
            var jugadoresEnMismaCasilla = Jugadores.Where(j => j != jugador && j.Posicion == jugador.Posicion).ToList();

            if (jugadoresEnMismaCasilla.Count == 0)
            {
                AnsiConsole.Write(new Markup("[red]No puedes envenenar a nadie. No hay otros jugadores en la misma casilla.[/]\n"));
                Thread.Sleep(2000);
            }
            else
            {
                foreach (var otroJugador in jugadoresEnMismaCasilla)
                {
                    AnsiConsole.Write(new Markup($"[red]{otroJugador.Nombre} ha sido envenenado y ha regresado a la casilla de inicio.[/]\n"));
                    Thread.Sleep(2000);
                    otroJugador.Posicion = (0, 1); // Regresar al inicio

                    // Verificar si hay una trampa de tuberías rotas en la posición inicial
                    if (Laberinto[0, 1] == Casilla.BrokenPipes)
                    {
                        ActivarTrampaInicio(otroJugador);
                    }
                }
            }
        }

        // Método para la habilidad Robar la llave del director
        void RobarLlave(Personaje jugador)
        {
            if (!jugador.HabilidadDisponible)
            {
                AnsiConsole.Write(new Markup("[yellow]La habilidad aún está en enfriamiento.[/]\n"));
                Thread.Sleep(2000);
                return;
            }

            bool llaveRobada = false;

            foreach (var otroJugador in Jugadores)
            {
                if (otroJugador != jugador && otroJugador.Posicion == jugador.Posicion && otroJugador.TieneLlave)
                {
                    AnsiConsole.Write(new Markup($"[red]{otroJugador.Nombre} ha sido robado. {jugador.Nombre} ahora tiene la llave.[/]\n"));
                    Thread.Sleep(2000);
                    otroJugador.TieneLlave = false;

                    if (!jugador.TieneLlave)
                    {
                        jugador.TieneLlave = true;
                    }
                    else
                    {
                        // Encontrar una casilla libre aleatoria para la nueva llave robada
                        List<(int x, int y)> casillasLibres = new List<(int x, int y)>();
                        for (int i = 0; i < Laberinto.GetLength(0); i++)
                        {
                            for (int j = 0; j < Laberinto.GetLength(1); j++)
                            {
                                if (Laberinto[i, j] == Casilla.Free)
                                {
                                    casillasLibres.Add((i, j));
                                }
                            }
                        }

                        if (casillasLibres.Count > 0)
                        {
                            var posicionLlaveNueva = casillasLibres[rnd.Next(casillasLibres.Count)];
                            Laberinto[posicionLlaveNueva.x, posicionLlaveNueva.y] = Casilla.Llave;
                            AnsiConsole.Write(new Markup($"[green]La nueva llave ha aparecido en una casilla libre del laberinto.[/]\n"));
                            Thread.Sleep(2000);
                        }
                    }

                    llaveRobada = true;
                    break;
                }
            }

            if (!llaveRobada)
            {
                AnsiConsole.Write(new Markup("[red]No hay ningún jugador con una llave en la misma casilla.[/]\n"));
                Thread.Sleep(2000);
            }

            jugador.HabilidadDisponible = false;
            jugador.TiempoEnfriamiento = 7; // Tiempo de enfriamiento de 7 turnos
        }
        // Método para la habilidad Boo del monstruo radioactivo
void Boo(Personaje jugador)
{
    while (true)
    {
        Console.WriteLine("Elige el jugador al que quieres aparecer:");
        for (int i = 0; i < Jugadores.Count; i++)
        {
            if (Jugadores[i] != jugador)
            {
                Console.WriteLine($"[{i + 1}] {Jugadores[i].Nombre}");
            }
        }

        bool opcionValida = false;

        while (!opcionValida)
        {
            if (int.TryParse(Console.ReadLine(), out int eleccion) && eleccion > 0 && eleccion <= Jugadores.Count && Jugadores[eleccion - 1] != jugador)
            {
                jugador.Posicion = Jugadores[eleccion - 1].Posicion;
                AnsiConsole.Write(new Markup($"[green]¡{jugador.Nombre} ha aparecido en la casilla de {Jugadores[eleccion - 1].Nombre}![/]\n"));
                Thread.Sleep(2000);
                opcionValida = true;
            }
            else
            {
                Console.WriteLine("Selección no válida. Intenta de nuevo.");
            }
        }

        if (opcionValida) break;
    }
}

// Método para la habilidad de curación del médico
void Curacion(Personaje jugador)
{
    if (jugador.HabilidadDisponible)
    {
        AnsiConsole.Write(new Markup("[green]El médico ha activado su habilidad de curación. Las trampas no le afectarán durante los próximos 3 turnos.[/]\n"));
        Thread.Sleep(2000);
        jugador.HabilidadDisponible = false;
        jugador.TiempoEnfriamiento = 15; // Tiempo de enfriamiento de 15 turnos
        jugador.TurnosDeInmunidad = 4; // Proporcionar inmunidad durante 3 turnos
    }
    else
    {
        AnsiConsole.Write(new Markup("[yellow]La habilidad de curación aún está en enfriamiento.[/]\n"));
        Thread.Sleep(2000);
    }
}

// Método para mover al jugador en la dirección indicada
bool Mover(Personaje jugador, int dx, int dy)
{
    int nuevoY = jugador.Posicion.x + dx; // x ahora representa la fila
    int nuevoX = jugador.Posicion.y + dy; // y ahora representa la columna
    int ancho = Laberinto.GetLength(1); // Ancho del laberinto
    int altura = Laberinto.GetLength(0); // Altura del laberinto

    // Verificar que la nueva posición esté dentro del rango
    if (nuevoX >= 0 && nuevoX < ancho && nuevoY >= 0 && nuevoY < altura)
    {
        Casilla casillaDestino = Laberinto[nuevoY, nuevoX]; // Obtener la casilla de destino

        switch (casillaDestino)
        {
            case Casilla.Free:
            case Casilla.Llave:
                if (casillaDestino == Casilla.Llave)
                {
                    if (!jugador.TieneLlave)
                    {
                        jugador.TieneLlave = true;
                        Laberinto[nuevoY, nuevoX] = Casilla.Free; // Cambiar la casilla de llave a libre
                        MostrarMensaje("[green]¡Has encontrado una llave![/]");
                    }
                    else
                    {
                        MostrarMensaje("[yellow]Ya tienes una llave, esta casilla se considera libre.[/]");
                    }
                }
                jugador.PosicionesAnteriores.Enqueue(jugador.Posicion); // Guardar la posición anterior
                if (jugador.PosicionesAnteriores.Count > 4)
                {
                    jugador.PosicionesAnteriores.Dequeue(); // Limitar a las últimas 4 posiciones
                }
                jugador.Posicion = (nuevoY, nuevoX);
                return true;

            case Casilla.BrokenPipes:
                // Verificar si el jugador tiene inmunidad
                if (jugador.TurnosDeInmunidad > 0)
                {
                    MostrarMensaje("[green]¡Has evitado la trampa gracias a tu inmunidad![/]");
                    jugador.TurnosDeInmunidad--; // Reducir el contador de turnos de inmunidad
                    jugador.Posicion = (nuevoY, nuevoX);

                    // Restaurar la trampa después de pasar por ella
                    RestaurarTrampa(nuevoY, nuevoX, casillaDestino);
                }
                else
                {
                    // Aplicar efecto de la trampa al jugador afectado
                    if (jugador.PosicionesAnteriores.Count == 4)
                    {
                        jugador.Posicion = jugador.PosicionesAnteriores.Dequeue();
                        MostrarMensaje("[red]El agua te ha llevado 5 casillas hacia atrás por donde habías venido.[/]");
                    }
                    else
                    {
                        // No hay suficientes turnos guardados, llevar al inicio
                        jugador.Posicion = (0, 1);
                        MostrarMensaje("[red]El agua te ha llevado hasta el inicio.[/]");

                        // Verificar si hay una trampa de tuberías rotas en la posición inicial
                        if (Laberinto[0, 1] == Casilla.BrokenPipes)
                        {
                            ActivarTrampaInicio(jugador);
                        }
                    }
                    Laberinto[nuevoY, nuevoX] = Casilla.Free; // Desactivar trampa
                }
                return true;

            case Casilla.Debris:
            case Casilla.Fire:
                // Verificar si el jugador tiene inmunidad
                if (jugador.TurnosDeInmunidad > 0)
                {
                    MostrarMensaje("[green]¡Has evitado la trampa gracias a tu inmunidad![/]");
                    jugador.TurnosDeInmunidad--; // Reducir el contador de turnos de inmunidad
                    jugador.Posicion = (nuevoY, nuevoX);

                    // Restaurar la trampa después de pasar por ella
                    RestaurarTrampa(nuevoY, nuevoX, casillaDestino);
                }
                else
                {
                    // Aplicar efecto de la trampa al jugador
                    switch (casillaDestino)
                    {
                        case Casilla.Debris:
                            jugador.Posicion = (nuevoY, nuevoX);
                            MostrarMensaje("[red]Se ha caído algo y salir de los escombros te tomará 3 turnos.[/]");
                            jugador.TurnosSinJugar = 3; // Aplicar penalización de 3 turnos
                            Laberinto[nuevoY, nuevoX] = Casilla.Free; // Desactivar trampa
                            break;
                        case Casilla.Fire:
                            jugador.Posicion = (nuevoY, nuevoX);
                            MostrarMensaje("[red]Se ha incendiado la habitación y te han llevado a un lugar seguro.[/]");
                            jugador.Posicion = (0, 1); // Volver a la posición inicial
                            Laberinto[nuevoY, nuevoX] = Casilla.Free; // Desactivar trampa

                            // Verificar si hay una trampa de tuberías rotas en la posición inicial
                            if (Laberinto[0, 1] == Casilla.BrokenPipes)
                            {
                                ActivarTrampaInicio(jugador);
                            }
                            break;
                    }
                }
                return true;

            default:
                MostrarMensaje($"[red]Movimiento no válido: ({nuevoY}, {nuevoX}) no es una casilla transitable[/]");
                return false;
        }
    }
    else
    {
        MostrarMensaje($"[red]Movimiento no válido: ({nuevoY}, {nuevoX}) está fuera del rango del laberinto[/]");
        return false;
    }
}
// Método para mostrar mensajes en la parte inferior de la consola
void MostrarMensaje(string mensaje)
{
    // Asumimos que el cursor ya está en la posición adecuada después del menú de opciones
    AnsiConsole.Write(new Markup(mensaje)); // Mostrar el mensaje
    Thread.Sleep(2000); // Pausa para que el mensaje sea visible
    Console.WriteLine(""); // Mover el cursor a la siguiente línea para futuros mensajes
}

// Método para restaurar la trampa en una casilla después de que el médico haya pasado
void RestaurarTrampa(int x, int y, Casilla tipoTrampa)
{
    // Esperar un turno antes de restaurar la trampa
    Task.Delay(1000).ContinueWith(_ =>
    {
        Laberinto[x, y] = tipoTrampa;
    });
}

// Método para imprimir el laberinto y las posiciones de los jugadores
public static void PrintMaze(Casilla[,] laberinto, List<Personaje> jugadores)
{
    int filas = laberinto.GetLength(0);
    int columnas = laberinto.GetLength(1);
    var posicionesJugadores = new Dictionary<(int, int), string>();

    // Almacenar las posiciones de los jugadores
    for (int i = 0; i < jugadores.Count; i++)
    {
        var jugador = jugadores[i];
        posicionesJugadores[jugador.Posicion] = (i + 1).ToString();
    }

    // Calcular el espacio en blanco necesario para centrar el laberinto
    int windowWidth = Console.WindowWidth;
    int windowHeight = Console.WindowHeight;
    int leftPadding = (windowWidth - columnas * 3) / 2;
    int topPadding = (windowHeight - filas) / 2;

    // Imprimir el laberinto centrado
    Console.Clear();
    for (int i = 0; i < topPadding; i++)
    {
        Console.WriteLine();
    }

    for (int i = 0; i < filas; i++)
    {
        for (int j = 0; j < leftPadding; j++)
        {
            Console.Write(" ");
        }

        for (int j = 0; j < columnas; j++)
        {
            if (posicionesJugadores.ContainsKey((i, j)))
            {
                AnsiConsole.Markup($"[bold green]{posicionesJugadores[(i,j)]}[/]");
            }
            else
            {
                switch (laberinto[i, j])
                {
                    case Casilla.Walls:
                        AnsiConsole.Markup("[yellow]■[/]"); // Paredes
                        break;
                    case Casilla.Free:
                        AnsiConsole.Markup("[black]■[/]"); // Camino
                        break;
                    case Casilla.BrokenPipes:
                        AnsiConsole.Markup("[blue]■[/]"); // Tuberías rotas
                        break;
                    case Casilla.Debris:
                        AnsiConsole.Markup("[red]■[/]"); // Escombros
                        break;
                    case Casilla.Fire:
                        AnsiConsole.Markup("[magenta]■[/]"); // Fuego
                        break;
                    case Casilla.Llave:
                        AnsiConsole.Markup("[green]■[/]"); // Llave
                        break;
                }
            }
            Console.Write(" ");
        }
        Console.WriteLine();
    }
}
    }
}