namespace NHSv2.Appointments.Application.Repositories;

public interface IEventStoreCheckpointRepository
{
    /// <summary>
    /// Get the last checkpoint for a stream.
    /// </summary>
    /// <param name="streamName"></param>
    /// <returns></returns>
    Task<long> GetCheckpoint(string streamName);

    /// <summary>
    /// Increment the checkpoint for a stream.
    /// </summary>
    /// <param name="streamName"></param>
    /// <returns></returns>
    Task IncrementCheckpoint(string streamName);
}