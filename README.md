# unity-scripts-gpt-generated
GPT generated scripts and utils for Unity


## StreamingAssetsLoader

The `StreamingAssetsLoader` utility class provides methods for loading files from the `StreamingAssets` directory in Unity, as well as listing the files in a folder in the directory.

The `StreamingAssets` folder in Unity is used to store files that should be included in the built game or application and are available at runtime. This folder can be used to store any kind of file, such as audio files, video files, images, text files, and more.

To use the `StreamingAssetsLoader` utility class in your Unity project, follow these steps:

1. Import the `StreamingAssetsLoader.cs` file into your project's Assets directory.

2. Use the `LoadString` method to load the contents of a text file from the `StreamingAssets` directory:

    ```csharp
    string fileContents = StreamingAssetsLoader.LoadString("MyFolder/MyTextFile.txt");
    ```

3. Use the `LoadBytes` method to load the contents of a binary file from the `StreamingAssets` directory:

    ```csharp
    byte[] fileContents = StreamingAssetsLoader.LoadBytes("MyFolder/MyBinaryFile.bin");
    ```

4. Use the `ListFilesInFolder` method to list the files in a folder in the `StreamingAssets` directory:

    ```csharp
    string[] filesInFolder = StreamingAssetsLoader.ListFilesInFolder("MyFolder");
    foreach (string fileName in filesInFolder)
    {
        Debug.Log($"Found file: {fileName}");
    }
    ```

5. Use the `ListFilesInFolderRecursive` method to list the files in a folder in the `StreamingAssets` directory and all its subfolders:

    ```csharp
    List<string> allFiles = StreamingAssetsLoader.ListFilesInFolderRecursive("MyFolder");
    foreach (string fileName in allFiles)
    {
        Debug.Log($"Found file: {fileName}");
    }
    ```

Note that on Android and iOS, files in the `StreamingAssets` directory are compressed in the app package, and Unity doesn't allow direct access to these files. As a result, the `LoadString`, `LoadBytes`, and `ListFilesInFolderRecursive` methods use `UnityWebRequest` to load file listings and file contents from the `StreamingAssets` directory on these platforms.
