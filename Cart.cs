namespace cart;

class Cart{
    public List<CartItem> items = new List<CartItem>(); 
    public double balance;
}

class CartItem
{
    public int id;
    public string name;
    public double price;
    public int amount;
}