 // Personas {
 //        int Id PK
 //        int TipoPersonaId FK
 //        string Nombre
 //        string Apellido
 //        string Telefono
 //        string Email
 //        string Direccion
 //        datetime FechaRegistro
 //        bool Activo
 //    }
 
public class Person {
  public int Id { get; set; }
  public int PersonType { get; set; }
  public required string Name { get; set; }
  public required string LastName { get; set; }
  public required string Phone { get; set; }
  public required string Email { get; set; }
  public required string Address { get; set; }
  public DateTime RegisterDate { get; set; }
  public bool Active { get; set; }
} 
