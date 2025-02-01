                                                        __   __              _____
                                                        |  \/  | __ _ _______| __ )  ___   ___  __ _ ___ 
                                                        | |\/| |/ _` |_  / _ \ __ \ / _ \ / _ \| `_`_ _ \
                                                        | |  | | (_| |/ /  __/ |_) | (_) | (_) | | | | | |
                                                        | |  | |\__,_/___\___|____/ \___/ \___/|_| |_| |_|

Sinopsis:
El juego trata sobre un grupo de cietíficos atrapados en la termonuclear Antonio Guiteras que se haya apunto de explotar. El objetivo del juego es lograr salir de la central, 
pero para lograrlo necesitarán primero adquirir la llave de la salida de emergencia.

Cómo ejcutar el proyecto:
-clonar el proyecto en su dispositivo 
-abrir la carperta "MazeBoom"
-abrir la carpeta "bin" 
-abrir a carpeta "debug"
-arir la carpeta "net8.0"
-abrir el ejecutable "b"


Cómo jugar:
al comenzar el juego tendrás la opción de elegir cuantos jugadores desean jugar la partida (mínimo:2, máximo:4). Tras lo cual cada jugador ingresará su nombre y deberá escoger un personaje
(una vez un jugador haya escogido un personaje éste ya no estará disponible para otros jugadores). Una vez hecho todo esto cada jugador necesita llegar a la salida de la central pero
primero es necesario que obtengan una llave que estará en algún lugar del mapa (hay tantas llaves como jugadores), durante todo el camino los jugadores se encontrarán con distintas trampas
que dificultarán el objetivo. Ganará la partida el primer jugador en alcanzar la salida con una llave.
 

cada personaje a elegir tiene una habilidad única:
-fisíco: este personaje puede teletranportarse a si mismo a una posición alatoria en un radio de 7 casillas. 7 turnos de enfriamiento
-químico: este pesonaje es capaz de envenenar a los jugadores que estén en su misma posición enviandolos al inicio. 9 turnos de enfriamiento
-director: este personaje roba la llave al jugador que se encuentre en la misma posición que él. 8 turnos de enfriamiento
-ingeniero: este personaje sabotea las tuberías de refrigeración en cualquier casilla adyacente a él (solo si tiene al menos 2 casillas adyacentes asi como 
tampoco es posile colocarla en la casilla inicial). 10 turnos de enfriamiento
-médico: este personaje, al activar la habilidad otiene durante 3 turnos inmunidad a la siguiente trampa despues de lo cual el efecto desaparece. 15 turnos de enfriamiento
-montruo radioactivo: este personaje se aparece detrás de un jugador seleccionado para asustarlo. 12 turnos de enfriamiento


tanto las trampas como las llaves estan representadas con cuadrados(■) de distintos colores: 
-verde(llave) objeto necesario para desbloquear la salida de emergencia y escapar
-rojo (escombros) escombros caeran sobre tí inmobilisandote durante tres turnos
-morado(incendio) un incendio repentino te ha abrazado hasta morir y reapareceras en el inicio
-azul(tuberias rotas) el agua liberada de unas tuberias de refrigeracion rotas te han arrastrado y retrocedes 5 turnos


Diagrama de flujo:
Inicio -> Menú Principal -> (Seleccionar Opción) -> Iniciar Juego ->
1. Generar Laberinto
2. Crear y Seleccionar Personajes
3. Asignar Llaves y Posiciones Iniciales ->
Inicio del Juego (Bucle de Turnos) ->
    Turno del Jugador ->
    1. Mostrar Estado
    2. Manejar Habilidades y Penalizaciones
    3. Mostrar Opciones de Movimiento/Habilidad ->
    Movimiento del Jugador -> Encontrar Trampas/Llaves ->
    [Si Gana] -> Fin del Juego: Mostrar Menú de Fin de Juego
    [Si No Gana] -> Siguiente Jugador ->
[Repetir] Hasta que un jugador gane o todos abandonen ->
Fin del Juego.
