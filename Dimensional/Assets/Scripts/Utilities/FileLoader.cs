using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utilities
{
    public static class FileLoader
    {
        public static IEnumerator LoadText(string fileName, string folderName, System.Action<string> onLoaded)
        {
            var path = System.IO.Path.Combine(Application.streamingAssetsPath, folderName, fileName);

            using var request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load file at {path}: {request.error}");
                onLoaded?.Invoke(null);
            }
            else
            {
                onLoaded?.Invoke(request.downloadHandler.text);
            }
        }
    }
}
