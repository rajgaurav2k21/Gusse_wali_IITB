using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataPersistence : MonoBehaviour
{
    public List<CargoData> cargoGroups = new List<CargoData>();

    // Save data to a JSON file
    public void SaveToFile()
    {
        string json = JsonUtility.ToJson(new CargoWrapper(cargoGroups), true);
        File.WriteAllText(Application.persistentDataPath + "/cargoData.json", json);
        Debug.Log("Data saved to: " + Application.persistentDataPath + "/cargoData.json");
    }

    // Load data from a JSON file
    public void LoadFromFile()
    {
        string filePath = Application.persistentDataPath + "/cargoData.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CargoWrapper wrapper = JsonUtility.FromJson<CargoWrapper>(json);
            cargoGroups = wrapper.cargoDataList;
            Debug.Log("Data loaded successfully!");
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }

    // Add a new cargo item
    public void AddCargo(string groupName,float length ,float width,float height,float volume,string itemName, int totalbox, float weight)
    {
        CargoData newCargo = new CargoData
        {
            groupName = groupName,
            itemName = itemName,
            totalbox=totalbox,
            weight = weight,
            length=length,
            width=width,
            height=height,
            volume=volume
        };
        cargoGroups.Add(newCargo);
    }

    // Wrapper class for JSON serialization
    [System.Serializable]
    private class CargoWrapper
    {
        public List<CargoData> cargoDataList;

        public CargoWrapper(List<CargoData> cargoGroups)
        {
            cargoDataList = cargoGroups;
        }
    }
}
