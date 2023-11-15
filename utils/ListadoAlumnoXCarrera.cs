namespace ProyectoJueves.Utils;
public class VistaCarrera{
  public int CarreraID { get; set; }
  public string CarreraNombre { get; set; }
  public List<VistaAlumno> ListaAlumnos { get; set; }
}
public class VistaAlumno{
  public int AlumnoID { get; set; }
  public string AlumnoNombre { get; set; }
}