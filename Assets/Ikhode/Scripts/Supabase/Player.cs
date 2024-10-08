using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

[Table("players")] // Assuming your table is named 'players'
public class Player : BaseModel // Inherit from BaseModel
{
    [PrimaryKey("uid", false)]
    public string UID { get; set; } // Using string for UUID

    [Column("username")]
    public string Username { get; set; }

    [Column("cash")]
    public decimal Cash { get; set; } // Use decimal for cash
}
