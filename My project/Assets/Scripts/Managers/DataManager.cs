using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    // Referencia a todo el guardado
    public Data data;
    // Nombre que tendrá elfichero de guardado
    [SerializeField] private string fileName = "data.dat";
    // Combinaión de la ruta + nombre del arcgivo
    private string dataPath;


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        dataPath = Application.persistentDataPath + "/" + fileName;
    }

    /// <summary>
    /// Guarda la información de data de forma persistente
    /// </summary>
    public void Save()
    {
        // Objeto que se utiliza para serializar y deserializar
        BinaryFormatter bf = new BinaryFormatter();
        // Crea o sobreescribe el fichero con los datos en binario
        FileStream file = File.Create(dataPath);
        // Serializamos el contenido de nuestro objeto de datos volcado al archivo
        bf.Serialize(file, data);
        // Cerramos el stream una vez terminado el proceso
        file.Close();
    }

    public void Load()
    {
        // Si NO existe el archivo, no hacemos nada
        if (!File.Exists(dataPath)) return;
        // Objeto para serializar y deserializar datos
        BinaryFormatter bf = new BinaryFormatter();
        // Apertura del fichero para su lectura
        FileStream file = File.Open(dataPath, FileMode.Open);
        // Deserializamos el fichero utilizando la estruuctura de la  clase con un 
        // casteo implícito
        data = (Data)bf.Deserialize(file);
        // Una vez terminado
        file.Close();
    }

    /// <summary>
    /// Borra el fichero de guardado
    /// </summary>
    [ContextMenu("Delete Data")]
    public void DeleteSavedFiles()
    {
        // Intenta...
        try
        {
            // Borrar el archivo físicamente
            File.Delete(dataPath);
        }
        // Si no lo consigue...
        catch (System.Exception)
        {
            // Entramos aquí
            Debug.Log("No existe el archivo");
        }
    }
}

/// <summary>
/// Clase auxiliae que contendrá toda la info que queremos guardar 
/// </summary>
[System.Serializable]
public class Data
{
    public int currentEnemies;
}
