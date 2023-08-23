using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    string dataPath = null;

    public static SaveSystem current;

    public Dictionary<string, object> state;

    private void Awake()
    {
        if(current == null)
        {
            current = this;
            state = LoadFile();
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        dataPath = Application.persistentDataPath + "/save32.txt";
    }

    [ContextMenu("Save")]
    private void Save()
    {
        //var state = LoadFile();
        //CaptureState(state);
        SaveFile(state);
    }

    [ContextMenu("Load")]
    private void Load()
    {
        state = LoadFile();
        RestoreState();
    }

    private void SaveFile(object state)
    {
        FileStream stream = new FileStream(dataPath, FileMode.Create);


        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, state);

        stream.Close();

    }
    private Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(dataPath))
        {
            return new Dictionary<string, object>();
        }
        else
        {
            FileStream stream = new FileStream(dataPath, FileMode.Open);

            var formatter = new BinaryFormatter();


            Dictionary<string, object> data = formatter.Deserialize(stream) as Dictionary<string, object>;

            stream.Close();

            return data;
        }
    }

    public void CaptureState()
    {
        foreach (var saveable in FindObjectsOfType<SaveComponent>())
        {
            state[saveable.Id] = saveable.CaptureState();
        }
    }

    public void RestoreState()
    {
        foreach (var saveable in FindObjectsOfType<SaveComponent>())
        {
            if (state.TryGetValue(saveable.Id, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);
        RestoreState();
    }

    private void OnDisable()
    {
        if(current == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

public interface ISaveable
{
    object CaptureState();
    void RestoreState(object state);
}
