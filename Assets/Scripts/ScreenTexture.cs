using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ScreenTexture : MonoBehaviour
{
    public Renderer screenRenderer;
    public Renderer rectScreenRenderer;
    public TMP_Dropdown textureDropdown;
    public string defaultTextureName;

    private FileInfo[] textureFiles;

    private void Start()
    {
        var textureDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Screen Textures");
        if (!Directory.Exists(textureDirectoryPath))
        {
            Debug.LogWarning("Screen texture directory does not exist!");
            return;
        }

        var dir = new DirectoryInfo(textureDirectoryPath);
        textureFiles = dir.GetFiles("*.jpg").Union(dir.GetFiles("*.png")).ToArray();

        textureDropdown.ClearOptions();
        var textureFileNames = textureFiles.Select(textureFile => textureFile.Name).ToList();
        textureDropdown.AddOptions(textureFileNames);

        textureDropdown.onValueChanged.AddListener(value =>
        {
            LoadScreenTexture(textureFiles[value].FullName);
        });

        var defaultTextureIndex = textureFileNames.IndexOf(defaultTextureName);
        var textureIndex = defaultTextureIndex >= 0 ? defaultTextureIndex : 0;
        LoadScreenTexture(textureFiles[textureIndex].FullName);
        textureDropdown.SetValueWithoutNotify(textureIndex);
    }

    private void LoadScreenTexture(string path)
    {
        var fileUri = new Uri(path);
        StartCoroutine(GetAndApplyTexture(fileUri.ToString()));
    }

    private IEnumerator GetAndApplyTexture(string uri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            myTexture.anisoLevel = 16;
            myTexture.filterMode = FilterMode.Trilinear;
            screenRenderer.material.mainTexture = myTexture;
            rectScreenRenderer.materials[1].mainTexture = myTexture;
        }
    }
}
