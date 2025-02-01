class Personaje
{
    public string Nombre { get; set; }
    public string Habilidad { get; set; }
    public string ExplicacionHabilidad { get; set; }
    public int TiempoEnfriamiento { get; set; }
    public int Velocidad { get; set; }
    public (int x, int y) Posicion { get; set; }
    public int TurnosSinJugar { get; set; } = 0;
    public int TurnosDeInmunidad { get; set; } = 0;
    public Queue<(int x, int y)> PosicionesAnteriores { get; set; } = new Queue<(int x, int y)>();
    public bool TieneLlave { get; set; } = false;
    public bool HabilidadDisponible { get; set; } = true;
    public bool HabilidadTuberiasActivada { get; set; } = false;
    public int MovimientosRestantes { get; set; } = 0;
}
