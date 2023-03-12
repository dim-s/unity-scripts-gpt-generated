using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// A utility class for loading files from the StreamingAssets directory in Unity.
    /// </summary>
    public static class StreamingAssetsLoader
    {
        /// <summary>
        /// Loads the contents of a text file from the StreamingAssets directory.
        /// </summary>
        /// <param name="filePath">The path of the file to load, relative to the StreamingAssets directory.</param>
        /// <returns>The contents of the file as a string, or null if the file could not be found.</returns>
        public static string LoadString(string filePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);
            string fileData = null;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // On Android and iOS, files in the StreamingAssets directory are compressed in the app package.
                // Unity doesn't allow direct access to these files, so we need to use UnityWebRequest to load them.
                var www = new UnityEngine.Networking.UnityWebRequest(fullPath);
                www.method = UnityEngine.Networking.UnityWebRequest.kHttpVerbGET;
                www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
                www.url = fullPath;
                www.SendWebRequest();

                while (!www.isDone) { } // Wait for the request to complete.

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"Failed to load file {filePath}: {www.error}");
                    return null;
                }

                fileData = www.downloadHandler.text;
            }
            else
            {
                // On other platforms, we can use a StreamReader to load the file data.
                using var reader = new StreamReader(fullPath, Encoding.UTF8);
                fileData = reader.ReadToEnd();
            }

            return fileData;
        }

        /// <summary>
        /// Loads the contents of a binary file from the StreamingAssets directory.
        /// </summary>
        /// <param name="filePath">The path of the file to load, relative to the StreamingAssets directory.</param>
        /// <returns>The contents of the file as a byte array, or null if the file could not be found.</returns>
        public static byte[] LoadBytes(string filePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // On Android and iOS, files in the StreamingAssets directory are compressed in the app package.
                // Unity doesn't allow direct access to these files, so we need to use UnityWebRequest to load them.
                var www = new UnityEngine.Networking.UnityWebRequest(fullPath);

                #if UNITY_2020_1_OR_NEWER
                www.SendWebRequest();
                #else
                www.Send();
                #endif

                while (!www.isDone) { } // Wait for the request to complete.

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"Failed to load file {filePath}: {www.error}");
                    return null;
                }

                return www.downloadHandler.data;
            }

            // On other platforms, we can use File.ReadAllBytes to load the file as a byte array.
            return File.ReadAllBytes(fullPath);
        }

        /// <summary>
        /// Lists the files in a folder in the StreamingAssets directory, recursively including subfolders.
        /// </summary>
        /// <param name="folderPath">The path of the folder to list, relative to the StreamingAssets directory.</param>
        /// <param name="searchPattern">The search pattern to use to filter the files. Defaults to "*.*".</param>
        /// <returns>A list of strings containing the relative paths of the files in the folder and its subfolders, or null if the folder could not be found.</returns>
        public static List<string> ListFilesInFolderRecursive(string folderPath, string searchPattern = "*.*")
        {
            List<string> filePaths = new List<string>();

            string fullPath = Path.Combine(Application.streamingAssetsPath, folderPath);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // On Android and iOS, files in the StreamingAssets directory are compressed in the app package.
                // Unity doesn't allow direct access to these files, so we need to use UnityWebRequest to load them.
                var www = new UnityEngine.Networking.UnityWebRequest(fullPath);
                www.method = UnityEngine.Networking.UnityWebRequest.kHttpVerbGET;
                www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
                www.url = fullPath;
                www.SendWebRequest();

                while (!www.isDone) { } // Wait for the request to complete.

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"Failed to list files in folder {folderPath}: {www.error}");
                    return null;
                }

                string filesString = www.downloadHandler.text;
                string[] subfolderNames = filesString.Split(new string[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string subfolderName in subfolderNames)
                {
                    List<string> subfolderFilePaths = ListFilesInFolderRecursive(Path.Combine(folderPath, subfolderName), searchPattern);
                    filePaths.AddRange(subfolderFilePaths);
                }
            }
            else
            {
                // On other platforms, we can use Directory.GetFiles to list the files and Directory.GetDirectories to list the subfolders.
                foreach (string filePath in Directory.GetFiles(fullPath, searchPattern))
                {
                    string relativePath = Path.Combine(folderPath, Path.GetFileName(filePath));
                    filePaths.Add(relativePath);
                }
                
                foreach (string subFolderPath in Directory.GetDirectories(fullPath))
                {
                    List<string> subfolderFilePaths = ListFilesInFolderRecursive(Path.Combine(folderPath, Path.GetFileName(subFolderPath)), searchPattern);
                    filePaths.AddRange(subfolderFilePaths);
                }
            }

            return filePaths;
        }
    }
}
