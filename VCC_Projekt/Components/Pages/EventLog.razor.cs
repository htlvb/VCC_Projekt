using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using Org.BouncyCastle.Asn1.X509;
using System.Net.Http;
using System.Net.Http.Json;
using static MudBlazor.CategoryTypes;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Net.NetworkInformation;
namespace VCC_Projekt.Components.Pages
{
    public partial class EventLog
    {

        private class EventLogViewModel
        {
            public int EventLogID { get; set; }
            public string Tabellenname { get; set; }
            public string Beschreibung { get; set; }
            public DateTime Zeit { get; set; }
            public string KategorieBeschreibung { get; set; }
        }

        private List<EventLogViewModel> eventLogViewModels = new();

        protected override void OnInitialized()
        {
            // Use a projection to get both the log and category information
            eventLogViewModels = dbContext.Set<Data.EventLog>()
                .Include(e => e.LogKat)
                .Select(e => new EventLogViewModel
                {
                    EventLogID = e.EventLogID,
                    Tabellenname = e.Tabellenname,
                    Beschreibung = e.Beschreibung,
                    Zeit = e.Zeit,
                    KategorieBeschreibung = e.LogKat != null ? e.LogKat.Beschreibung : "N/A"
                })
                .ToList();
        }
    }
}
