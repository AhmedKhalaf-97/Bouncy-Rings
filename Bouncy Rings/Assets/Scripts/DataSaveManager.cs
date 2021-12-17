using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DataSaveManager
{
    static string saveLocation = Application.persistentDataPath + "/PlayerSave/";

    static void CreateSaveLocation()
    {
        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }
    }

    public static void DeleteAllData()
    {
        if (Directory.Exists(saveLocation))
        {
            Directory.Delete(saveLocation, true);
        }
    }

    public static void DeleteData(string name)
    {
        if (File.Exists(saveLocation + name))
        {
            File.Delete(saveLocation + name);
        }
    }

    public static void SaveFloat(string name, float data)
    {
        CreateSaveLocation();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(saveLocation + name, FileMode.OpenOrCreate);
        
        bf.Serialize(stream, data);
        stream.Close();
    }

    public static float LoadFloat(string name)
    {
        if (File.Exists(saveLocation + name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(saveLocation + name, FileMode.Open);

            float data = (float)bf.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            return new float();
        }
    }

    public static void SaveInt(string name, int data)
    {
        CreateSaveLocation();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(saveLocation + name, FileMode.OpenOrCreate);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static int LoadInt(string name)
    {
        if (File.Exists(saveLocation + name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(saveLocation + name, FileMode.Open);

            int data = (int)bf.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            return new int();
        }
    }

    public static void SaveString(string name, string data)
    {
        CreateSaveLocation();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(saveLocation + name, FileMode.OpenOrCreate);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static string LoadString(string name)
    {
        if (File.Exists(saveLocation + name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(saveLocation + name, FileMode.Open);

            string data = (string)bf.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveBoolean(string name, bool data)
    {
        CreateSaveLocation();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(saveLocation + name, FileMode.OpenOrCreate);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static bool LoadBoolean(string name)
    {
        if (File.Exists(saveLocation + name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(saveLocation + name, FileMode.Open);

            bool data = (bool)bf.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            return new bool();
        }
    }

    public static void SaveDateTime(string name, DateTime dateTime)
    {
        CreateSaveLocation();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(saveLocation + name, FileMode.OpenOrCreate);

        bf.Serialize(stream, dateTime);
        stream.Close();
    }

    public static DateTime LoadDateTime(string name)
    {
        if (File.Exists(saveLocation + name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(saveLocation + name, FileMode.Open);

            DateTime dateTime = (DateTime)bf.Deserialize(stream);
            stream.Close();
            return dateTime;
        }
        else
        {
            return new DateTime();
        }
    }

    public static bool IsDataExist(string name)
    {
        if(File.Exists(saveLocation + name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
