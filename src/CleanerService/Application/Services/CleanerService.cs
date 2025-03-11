using Infrastructure;
using MimeKit;
using Monitoring;
using Serilog;
using SharedModels;

namespace CleanerService.Application.Services;

public class CleanerService(CleanedMessagePublisher messagePublisher)
{

    public async Task<IEnumerable<CleanedFileDTO>> CleanFilesAsync(string path = "../../data")
    {
         var paths = new List<string>();
        using (MonitoringService.ActivitySource.StartActivity("LoadPaths"))
        {
            string[] allFiles = Directory.GetFiles(path, "", SearchOption.AllDirectories);
            Log.Logger.Information("Cleaning {FileCount} files", allFiles.Length);
            paths = allFiles.Select(Path.GetFullPath).Take(10).ToList();
        }
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanFilesAsync");
        var tasks = paths.Select(CleanFileAsync);
        return await Task.WhenAll(tasks);
    }

    private async Task<CleanedFileDTO> CleanFileAsync(string path)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanFileAsync");
        var message = await MimeMessage.LoadAsync(path);
        var cleanedContent = message.TextBody;
        var filename = Path.GetFileName(path);
        var pathParts = Path.GetDirectoryName(path)!.Split(Path.DirectorySeparatorChar);
        var dataIndex = Array.IndexOf(pathParts, "data");
        var parentFolder = string.Join("_", pathParts.Skip(dataIndex + 1));
        var cleanedFilename = $"{parentFolder}_{filename[..^1]}.txt";
        Log.Logger.Information("Cleaning file {Filename}", cleanedFilename);
        var cleanedFIleDTO = new CleanedFileDTO { Filename = cleanedFilename, Content = cleanedContent };
        await messagePublisher!.PublishCleanedMessage(cleanedFIleDTO);
        return cleanedFIleDTO;
    }

    public async Task PublishCleanedFilesAsync(IEnumerable<CleanedFileDTO> cleanedFiles)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("PublishCleanedFilesAsync");
        await Parallel.ForEachAsync(cleanedFiles, async (cleanedFile, cancellationToken) =>
        {
            Log.Logger.Information("Publishing cleaned file {Filename}", cleanedFile.Filename);
            await messagePublisher!.PublishCleanedMessage(cleanedFile);
        });
    }
}