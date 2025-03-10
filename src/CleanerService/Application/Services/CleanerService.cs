using Infrastructure;
using MimeKit;
using Monitoring;
using Serilog;
using SharedModels;

namespace CleanerService.Application.Services;

public class CleanerService(CleanedMessagePublisher messagePublisher) {

    public async Task<IEnumerable<CleanedFileDTO>> CleanFilesAsync(string path = "../../data") {
        
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService.CleanFilesAsync");
        string[] allFiles = Directory.GetFiles(path, "", SearchOption.AllDirectories);
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
        var pathParts = Path.GetDirectoryName(path)!.Split(Path.DirectorySeparatorChar);
        var dataIndex = Array.IndexOf(pathParts, "data");
        var parentFolder = string.Join("_", pathParts.Skip(dataIndex + 1));
        var cleanedFilename = $"{parentFolder}_{filename[..^1]}.txt";
        Log.Logger.Information("Cleaning file {Filename}", cleanedFilename);
        return new CleanedFileDTO { Filename = cleanedFilename, Content = cleanedContent };
    }

    public async Task PublishCleanedFilesAsync(IEnumerable<CleanedFileDTO> cleanedFiles) {
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService.PublishCleanedFilesAsync");
        foreach (var cleanedFile in cleanedFiles) {
            Log.Logger.Information("Publishing cleaned file {Filename}", cleanedFile.Filename);
            await messagePublisher!.PublishCleanedMessage(cleanedFile);
        }
    }
}