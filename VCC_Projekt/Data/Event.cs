namespace VCC_Projekt.Data;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Add profile data for application users by adding properties to the ApplicationUser class
using System;
using System.Collections.Generic;

// Entität für die Tabelle _event
public class Event
{
    public Event() { }
    public Event(string bezeichnung, DateTime beginn, int dauer, int strafminutenProFehlversuch)
    {
        Bezeichnung = bezeichnung;
        Beginn = beginn;
        Dauer = dauer;
        StrafminutenProFehlversuch = strafminutenProFehlversuch;
    }

    public int EventID { get; set; }

    public string Bezeichnung { get; set; }

    public DateTime Beginn { get; set; }

    public int Dauer { get; set; }

    public int StrafminutenProFehlversuch { get; set; }

    // Navigation zu Gruppen
    public virtual ICollection<Gruppe>? Gruppen { get; set; }

    // Navigation zu Levels
    public virtual ICollection<Level>? Levels { get; set; }
}

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("vcc_event");
        builder.HasKey(e => e.EventID);
    }
}