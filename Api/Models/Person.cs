using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Person
{
  public int Id { get; set; }
  public int PersonTypeId { get; set; }
  public required string Name { get; set; }
  public required string LastName { get; set; }
  public required string Phone { get; set; }
  public required string Email { get; set; }
  public required string Address { get; set; }
  public DateTime RegisterDate { get; set; }
  public bool Active { get; set; }

  [ForeignKey(nameof(PersonTypeId))]
  public PersonType? PersonType { get; set; }
}
