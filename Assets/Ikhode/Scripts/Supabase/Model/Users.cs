using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System; // Add this line for DateTime

[Table("Users")] // Assuming your table is named 'Users'
public class Users : BaseModel
{
    [PrimaryKey("id", true)]
    public string ID { get; set; }  // Keep as long for bigint
    
    [Column("cash")]
    public decimal Cash { get; set; }
}
