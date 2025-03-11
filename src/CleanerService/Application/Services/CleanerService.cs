using Infrastructure;
using MimeKit;
using Monitoring;
using Serilog;
using SharedModels;

namespace CleanerService.Application.Services;

public class CleanerService(CleanedMessagePublisher messagePublisher)
{

    public async Task<IEnumerable<CleanedFileDTO>> CleanFilesAsync(string path = "../../data") {
        
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService.CleanFilesAsync");
        string[] allFiles = Directory.GetFiles(path, "", SearchOption.AllDirectories);
        Log.Logger.Information("Cleaning {FileCount} files", allFiles.Length);
        var paths = allFiles.Select(Path.GetFullPath).Take(10);
        var tasks = paths.Select(CleanFileAsync);

        var cleanedFiles = (await Task.WhenAll(tasks)).Where(f => f != null);
        return cleanedFiles;
    }

    private async Task<CleanedFileDTO> CleanFileAsync(string path) {
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService.CleanFileAsync");
        
        try {
            var message = await MimeMessage.LoadAsync(path);
            var cleanedContent = message.TextBody;
            var filename = Path.GetFileName(path);
            var pathParts = Path.GetDirectoryName(path)!.Split(Path.DirectorySeparatorChar);
            var dataIndex = Array.IndexOf(pathParts, "data");
            var parentFolder = string.Join("_", pathParts.Skip(dataIndex + 1));
            var cleanedFilename = $"{parentFolder}_{filename[..^1]}.txt";
            Log.Logger.Information("Cleaning file {Filename}", cleanedFilename);
            var cleanedFile = new CleanedFileDTO { Filename = cleanedFilename, Content = cleanedContent };
            if (!string.IsNullOrWhiteSpace(cleanedContent)) {
                await messagePublisher!.PublishCleanedMessage(cleanedFile);
            }
            return cleanedFile;
        } 
        catch (Exception ex) {
            Log.Logger.Error("Skipping file {Path} due to error: {ErrorMessage}", path, ex.Message);
            throw;
        }
    }

    public async Task PublishCleanedFilesAsync(IEnumerable<CleanedFileDTO> cleanedFiles) {
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService.PublishCleanedFilesAsync");
        await Parallel.ForEachAsync(cleanedFiles, async (cleanedFile, cancellationToken) => {
            Log.Logger.Information("Publishing cleaned file {Filename}", cleanedFile.Filename);
            await messagePublisher!.PublishCleanedMessage(cleanedFile);
        });
    }
}