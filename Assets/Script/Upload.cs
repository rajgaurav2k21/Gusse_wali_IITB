using System;
using System.IO;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    public string filePath = @"C:\Users\tript\Downloads\example.csv"; // Update this to your actual CSV file path

    void Start()
    {
        ReadCSV();
    }

    void ReadCSV()
    {
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV file not found at: {filePath}");
            Console.WriteLine($"CSV file not found at: {filePath}");
            return;
        }

        try
        {
            // Open and read the file
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Print the entire line
                    Debug.Log($"Line: {line}"); // Unity Console
                    Console.WriteLine($"Line: {line}"); // External Console

                    // Split the line by commas and print each value
                    string[] values = line.Split(',');
                    foreach (string value in values)
                    {
                        Debug.Log($"Value: {value}");
                        Console.WriteLine($"Value: {value}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading CSV file: {ex.Message}");
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }
    }
}
