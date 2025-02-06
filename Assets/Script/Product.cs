public class Product
{
    public string productName;
    public float length;
    public float breadth;
    public float height;
    public int quantity;
    public float weight;

    public Product(string name, float length, float breadth, float height, int quantity, float weight)
    {
        this.productName = name;
        this.length = length;
        this.breadth = breadth;
        this.height = height;
        this.quantity = quantity;
        this.weight = weight;
    }
}
