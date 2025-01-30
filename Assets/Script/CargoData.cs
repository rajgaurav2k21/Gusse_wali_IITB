[System.Serializable]
public class CargoData
{
    public string groupName;  // The group name of the cargo
    public string itemName;   // The item name or number
    public float length;
    public float width;
    public float height;
    public float volume;

    public float weight;
    public int totalbox;       
 
    public override string ToString()
    {
        return $"Group: {groupName},Length:{length},Width:{width},Height:{height},Volume:{volume},Item: {itemName}, Total Shipments: {totalbox}, Weight: {weight}kg";
    }
}
