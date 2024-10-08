using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;

[Table("purchases")] // Assuming your table is named 'purchases'
public class Purchases : BaseModel // Inherit from BaseModel
{
    [PrimaryKey("id", false)] // Use "id" as the primary key
    public int Id { get; set; } // Unique identifier for each purchase record

    [Column("player_uid")]
    public string PlayerUID { get; set; } // Reference to the player's UID from authentication

    [Column("vehicle_id")]
    public int VehicleID { get; set; } // ID of the purchased vehicle

    [Column("purchase_date")]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow; // Date of purchase with default to current timestamp
}
