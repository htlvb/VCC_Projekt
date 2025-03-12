using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Axes;
using ChartJs.Blazor.Common.Axes.Ticks;
using ChartJs.Blazor.Common.Enums;
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
        private bool doughnutChartExpanded = true;
        private bool rankingExpanded = true;

        // Initialisierung: Events laden
        protected override async Task OnInitializedAsync()
        {
            Events = await dbContext.Events.ToListAsync();
        }

        // Event-Handler für die Auswahl eines Events
        private async Task OnEventSelected(Event? eventId)
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

        // Diagramme aktualisieren
        private void UpdateCharts()
        {
            if (_selectedEventStatistiks == null || _selectedEventStatistiks.Levels == null) return;

            // Labels für die Diagramme
            var labels = _selectedEventStatistiks.Levels.Select(l => $"Level {l.Levelnr}").ToList();

            // Konfiguration für das Balkendiagramm
            _barConfig = new BarConfig
            {
                Options = new BarOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Groups Completed Levels"
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
                            FontColor = "#000"
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
            for (int i = 0; i < labels.Count; i++)
            {
                _barConfig.Data.Labels.Add(labels[i]);
            }
            var barDataset = new BarDataset<double>
            {
                Label = "Completed Levels", // Set the dataset label
                BackgroundColor = new[] { ColorUtil.ColorHexString(255, 99, 132), ColorUtil.ColorHexString(54, 162, 235), ColorUtil.ColorHexString(255, 206, 86) }
            };
            foreach (var data in _selectedEventStatistiks.Levels.Select(l => (double)(l.Absolviert?.Count ?? 0)))
            {
                barDataset.Add(data);
            }
            _barConfig.Data.Datasets.Add(barDataset);

            // Konfiguration für das Tortendiagramm
            _pieConfig = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Fehlversuche pro Level"
                    },
                    Tooltips = new Tooltips
                    {
                        Enabled = true,
                        Mode = InteractionMode.Index,
                        Intersect = false
                    }
                }
            };
            for (int i = 0; i < labels.Count; i++)
            {
                _pieConfig.Data.Labels.Add(labels[i]);
            }
            var pieDataset = new PieDataset<double>
            {
                BackgroundColor = new[] { ColorUtil.ColorHexString(255, 99, 132), ColorUtil.ColorHexString(54, 162, 235), ColorUtil.ColorHexString(255, 206, 86) }
            };
            foreach (var data in _selectedEventStatistiks.Levels.Select(l => (double)(l.Absolviert?.Sum(a => a.Fehlversuche) ?? 0)))
            {
                pieDataset.Add(data);
            }
            _pieConfig.Data.Datasets.Add(pieDataset);

            // Konfiguration für das Liniendiagramm
            _lineConfig = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Verbrachte Zeit pro Level in min (ohne Strafminuten)"
                    },
                    Tooltips = new Tooltips
                    {
                        Enabled = true,
                        Mode = InteractionMode.Index,
                        Intersect = false
                    }
                }
            };
            for (int i = 0; i < labels.Count; i++)
            {
                _lineConfig.Data.Labels.Add(labels[i]);
            }
            var lineDataset = new LineDataset<double>
            {
                Label = "Zeit", // Set the dataset label
                BackgroundColor = ColorUtil.ColorHexString(75, 192, 192),
                BorderColor = ColorUtil.ColorHexString(75, 192, 192)
            };
            foreach (var data in _selectedEventStatistiks.Levels.Select(l => l.Absolviert?.Sum(a => a.BenoetigteZeit?.TotalMinutes ?? 0) ?? 0))
            {
                lineDataset.Add(data);
            }
            _lineConfig.Data.Datasets.Add(lineDataset);
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