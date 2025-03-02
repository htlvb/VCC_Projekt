using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace VCC_Projekt.Controllers
{
    [Route("api/v1/files")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FileController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public FileController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{levelId}")]
        public async Task<IActionResult> DownloadFile(int levelId)
        {
            // Get the file from the database
            var file = await _dbContext.Levels
                .Where(l => l.LevelID == levelId)
                .Select(l => new { l.Angabe_PDF, l.Levelnr })
                .FirstOrDefaultAsync();
            if (file == null) return NotFound();

            return File(file.Angabe_PDF, "application/pdf",fileDownloadName: $"Level {file.Levelnr}.pdf");
        }

        [HttpGet("{levelId}/{aufgabeId}/input")]
        public async Task<IActionResult> DownloadInputFile(int levelId, int aufgabeId)
        {
            // Get the file from the database
            var file = await _dbContext.Aufgabe
                .Where(l => l.Level_LevelID == levelId && l.AufgabenID==aufgabeId)
                .Select(l => new { l.Input_TXT, l.Aufgabennr, l.Level.Levelnr })
                .FirstOrDefaultAsync();
            if (file == null) return NotFound();

            return File(file.Input_TXT, "plain/text", fileDownloadName: $"level{file.Aufgabennr}_{file.Levelnr}.txt");
        }

        [HttpGet("{levelId}/{aufgabeId}/ergebnis")]
        public async Task<IActionResult> DownloadErgebnisFile(int levelId, int aufgabeId)
        {
            var file = await _dbContext.Aufgabe
                .Where(l => l.Level_LevelID == levelId && l.AufgabenID == aufgabeId)
                .Select(l => new { l.Ergebnis_TXT, l.Aufgabennr, l.Level.Levelnr })
                .FirstOrDefaultAsync();
            if (file == null) return NotFound();

            return File(file.Ergebnis_TXT, "plain/text", fileDownloadName: $"level{file.Aufgabennr}_{file.Levelnr}.txt");
        }

        [HttpGet("{levelId}/input/zip")]
        public async Task<IActionResult> DownloadFiles(int levelId)
        {
            var files = await _dbContext.Aufgabe
                                        .Where(l => l.Level_LevelID == levelId)
                                        .Select(l => new { l.Input_TXT, l.Level.Levelnr, l.Aufgabennr })
                                        .OrderBy(a => a.Aufgabennr)
                                        .ToListAsync();

            if (files == null || files.Count == 0) return NotFound();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipArchiveEntry = archive.CreateEntry($"level{file.Levelnr}_{file.Aufgabennr}.txt");  // Hier den Dateinamen anpassen
                        using (var zipStream = zipArchiveEntry.Open())
                        {
                            // Hier die Datei-Inhalte in den Zip-Stream schreiben
                            var fileContent = file.Input_TXT; // Hier kannst du die tatsächlichen Daten einfügen
                            await zipStream.WriteAsync(file.Input_TXT, 0, file.Input_TXT.Length);
                        }
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", fileDownloadName: $"level{files.First().Levelnr}.zip");
            }
        }
    }
}
