namespace MASA.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Service to persist data via keys in the application
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Create the data in the data store if it doesn't exist, or update the reference if it does.
        /// </summary>
        /// <param name="key">The key to associate the data with.</param>
        /// <param name="o">The object to save.</param>
        /// <returns></returns>
        Task CreateOrUpdateLocalData(String key, Object o);

        /// <summary>
        /// Get the data associated with the key, or return the defaultValue if nothing exists.
        /// </summary>
        /// <typeparam name="T">The expected type of the data to retrieve.</typeparam>
        /// <param name="key">The key associated with the data.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The data associated with the key or the defaultValue.</returns>
        Task<T> GetLocalDataOrDefault<T>(String key, T defaultValue);

        /// <summary>
        /// Delete the data associated with the key.
        /// </summary>
        /// <param name="key">The key associated with the data.</param>
        /// <returns></returns>
        Task DeleteData(String key);
    }
}
