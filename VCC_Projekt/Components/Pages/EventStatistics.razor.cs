using ChartJs.Blazor;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.Common.Handlers;
using ChartJs.Blazor.Common.Time;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Util;
using MySqlConnector;

namespace VCC_Projekt.Components.Pages
{
    public partial class EventStatistics
    {
        private List<Event> Events = new();
        private Event SelectedEvent;
        private Event? _selectedEventStatistiks;
        private BarConfig _barConfig;
        private PieConfig _pieConfig;
        private LineConfig _lineConfig;
        private List<RanglisteResult> Rangliste;

        private bool barChartExpanded = true;
        private bool pieChartExpanded = true;
        private bool lineChartExpanded = true;
        private bool rankingExpanded = true;

        // Initialisierung: Events laden
        protected override async Task OnInitializedAsync()
        {
            Events = await dbContext.Events.ToListAsync();
            _barConfig = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = false,
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis>
                        {
                            new CategoryAxis
                            {
                                Ticks = new CategoryTicks
                                {
                                    FontSize = 12,
                                    FontColor = "#000",
                                }
                            }
                        }
                    },
                    Tooltips = new Tooltips
                    {
                        Enabled = true,
                        Mode = InteractionMode.Index,
                        Intersect = false
                    }
                }
            };
            // Konfiguration für das Tortendiagramm
            _pieConfig = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = false,
                    },
                    Tooltips = new Tooltips
                    {
                        Enabled = true,
                        Mode = InteractionMode.Index,
                        Intersect = false
                    }
                }
            };

            _lineConfig = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = false,
                    },
                    Tooltips = new Tooltips
                    {
                        Enabled = true,
                        Mode = InteractionMode.Index,
                        Intersect = false
                    }
                }
            };
        }

        // Event-Handler für die Auswahl eines Events
        private async Task OnEventSelected(Event? eventId)
        {
            try
            {
                SelectedEvent = eventId;
                if (SelectedEvent != null)
                {
                    _selectedEventStatistiks = await dbContext.Events
                                                            .Include(e => e.Levels)
                                                                .ThenInclude(l => l.Absolviert)
                                                            .Select(e => new Event
                                                            {
                                                                EventID = e.EventID,
                                                                Bezeichnung = e.Bezeichnung,
                                                                Levels = e.Levels.Select(l => new Level
                                                                {
                                                                    LevelID = l.LevelID,
                                                                    Levelnr = l.Levelnr,
                                                                    Absolviert = l.Absolviert
                                                                }).ToList()
                                                            })
                                                            .FirstOrDefaultAsync(e => e.EventID == SelectedEvent.EventID);

                    Rangliste = await dbContext.Set<RanglisteResult>()
                                               .FromSqlRaw("CALL ShowRangliste(@eventId)", new MySqlParameter("@eventId", SelectedEvent.EventID))
                                               .ToListAsync();

                    UpdateCharts(); // Diagramme aktualisieren
                }
                else
                {
                    _selectedEventStatistiks = null; // Kein Event ausgewählt
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting event: {ex.Message}");
                // Optional: Zeige eine Fehlermeldung im UI
            }
        }

        // Diagramme aktualisieren
        private void UpdateCharts()
        {
            if (_selectedEventStatistiks == null || _selectedEventStatistiks.Levels == null) return;
            _barConfig.Data.Datasets.Clear();
            _barConfig.Data.Labels.Clear();
            _pieConfig.Data.Datasets.Clear();
            _pieConfig.Data.Labels.Clear();
            _lineConfig.Data.Datasets.Clear();
            _lineConfig.Data.Labels.Clear();


            // Labels für die Diagramme
            var labels = _selectedEventStatistiks.Levels.Select(l => $"Level {l.Levelnr}").ToList();

            // Konfiguration für das Balkendiagramm
            for (int i = 0; i < labels.Count; i++)
            {
                _barConfig.Data.Labels.Add(labels[i]);
            }
            var barDataset = new BarDataset<int>
            {
                Label = "Completed Levels", // Set the dataset label
                BackgroundColor = new[]
                {
            ColorUtil.ColorHexString(255, 99, 132),  // Rot
            ColorUtil.ColorHexString(54, 162, 235),  // Blau
            ColorUtil.ColorHexString(255, 206, 86),  // Gelb
            ColorUtil.ColorHexString(75, 192, 192),  // Türkis
            ColorUtil.ColorHexString(153, 102, 255)  // Lila
        }
            };
            foreach (var data in _selectedEventStatistiks.Levels.Select(l => (int)l.Absolviert.Count(a => a.BenoetigteZeit != null)))
            {
                barDataset.Add(data);
            }
            _barConfig.Data.Datasets.Add(barDataset);

            
            for (int i = 0; i < labels.Count; i++)
            {
                _pieConfig.Data.Labels.Add(labels[i]);
            }
            var pieDataset = new PieDataset<int>
            {
                BackgroundColor = new[]
                {
            ColorUtil.ColorHexString(255, 99, 132),  // Rot
            ColorUtil.ColorHexString(54, 162, 235),  // Blau
            ColorUtil.ColorHexString(255, 206, 86),  // Gelb
            ColorUtil.ColorHexString(75, 192, 192),  // Türkis
            ColorUtil.ColorHexString(153, 102, 255)  // Lila
        }
            };
            foreach (var data in _selectedEventStatistiks.Levels.Select(l => (int)(l.Absolviert?.Sum(a => a.Fehlversuche) ?? 0)))
            {
                pieDataset.Add(data);
            }
            _pieConfig.Data.Datasets.Add(pieDataset);

            // Konfiguration für das Liniendiagramm
            
            _lineConfig.Data.Labels.Clear();
            for (int i = 0; i < labels.Count; i++)
            {
                _lineConfig.Data.Labels.Add(labels[i]);
            }
            _lineConfig.Data.Datasets.Clear();
            var lineDataset = new LineDataset<double>
            {
                Label = "Zeit", // Set the dataset label
                BackgroundColor = ColorUtil.ColorHexString(75, 192, 192),
                BorderColor = ColorUtil.ColorHexString(75, 192, 192)
            };
            foreach (var data in _selectedEventStatistiks.Levels.Select(l => Math.Round(l.Absolviert?.Sum(a => a.BenoetigteZeit?.TotalMinutes ?? 0) ?? 0, 2)))
            {
                lineDataset.Add(data);
            }
            _lineConfig.Data.Datasets.Add(lineDataset);
            StateHasChanged();
        }
        // Chart-Toggle-Handler
        private void ToggleChart(string chartType)
        {
            switch (chartType)
            {
                case "bar":
                    barChartExpanded = !barChartExpanded;
                    break;
                case "pie":
                    pieChartExpanded = !pieChartExpanded;
                    break;
                case "line":
                    lineChartExpanded = !lineChartExpanded;
                    break;
                case "ranking":
                    rankingExpanded = !rankingExpanded;
                    break;
            }
        }

    }
}