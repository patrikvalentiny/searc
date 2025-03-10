using MimeKit;
using Monitoring;
using Serilog;
using SharedModels;

namespace CleanerService.Application.Services;

public class CleanerService() {

    public async Task<IEnumerable<CleanedFileDTO>> CleanFilesAsync() {
        
        using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("CleanerService.CleanFilesAsync");
        string[] allFiles = Directory.GetFiles("../../data", "", SearchOption.AllDirectories);
        Log.Logger.Information("Cleaning {FileCount} files", allFiles.Length);
        var paths = allFiles.Select(Path.GetFullPath).Take(10);
        var tasks = paths.Select(CleanFileAsync);
        return await Task.WhenAll(tasks);
    }

    private async Task<CleanedFileDTO> CleanFileAsync(string path) {
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService.CleanFileAsync");
        var message = await MimeMessage.LoadAsync(path);
        var cleanedContent = message.TextBody;
        var filename = Path.GetFileName(path);
        var parentFolder = Path.GetDirectoryName(path)!;
        var cleanedPath = Path.Combine(parentFolder, "cleaned", filename);
        Log.Logger.Information("Cleaning file {Filename}", Path.GetFullPath(cleanedPath));
        
        // Log.Logger.Debug("Content: {Content}", cleanedContent);
        return new CleanedFileDTO { Filename = Path.GetFileName(path), Content = cleanedContent };
    }
}