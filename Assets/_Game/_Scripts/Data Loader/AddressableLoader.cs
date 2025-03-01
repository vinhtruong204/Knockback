using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AddressableLoader<T> where T : Object
{
    public static void ReleaseHandle(AsyncOperationHandle<T> handle)
    {
        Addressables.Release(handle);
    }

    public static void ReleaseHandle(AsyncOperationHandle<IList<T>> handle)
    {
        Addressables.Release(handle);
    }

    public static async Task<(T asset, AsyncOperationHandle<T> handle)> LoadAssetAsync(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            Debug.LogError($"Address cannot be null or empty when loading asset of type {typeof(T).Name}");
            return (null, default);
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return (handle.Result, handle);
        }

        Debug.LogError($"Failed to load asset of type {typeof(T).Name} at address: {address}");
        Addressables.Release(handle); // Clean up on failure
        return (null, default);
    }

    public static async Task<(IList<T> assets, AsyncOperationHandle<IList<T>> handle)> LoadAssetsAsync(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            Debug.LogError($"Address cannot be null or empty when loading assets of type {typeof(T).Name}");
            return (null, default);
        }

        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return (handle.Result, handle);
        }

        Debug.LogError($"Failed to load assets of type {typeof(T).Name} at address: {address}");
        Addressables.Release(handle); // Clean up on failure
        return (new List<T>(), default); // Return empty list instead of null
    }
}