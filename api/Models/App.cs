namespace SenCloud.Api.Models;

public enum AppStatus
{
    Pending,    // saved, not deployed yet
    Deploying,  // Kubernetes objects being created
    Running,    // live at its URL
    Failed      // something went wrong
}

public class App
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Used in the Kubernetes object names and in the URL
    public required string Name { get; set; }

    // The Docker image to run
    public required string Image { get; set; }

    // The port the container listens on (nginx = 80)
    public int Port { get; set; } = 80;

    public AppStatus Status { get; set; } = AppStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}