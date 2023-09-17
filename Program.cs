namespace RecordStore2;

using cart;
using weapon;
using orders;

using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using static System.Console;

class Program
{
    static void Main(string[] args)
    { 
        Cart cart = new Cart();
        Arsenal arsenal = new Arsenal();
        List<Order> orders =  new List<Order>();
        TextInfo textinfo = new CultureInfo("en-GB", false).TextInfo;
       
        string showTable(string[] columns){
            
            var menuItemLengths = new Dictionary<string, int>(){
                {"id", -3},
                {"name",-13},
                {"stock",-5},
                {"ammo",-5},
                {"price",-7},
                {"type",-15},
                {"created",-7},
                {"amount",-3}
            };

            string format ="";
            
            for(int i = 0; i < columns.Length; i++){
                int length = menuItemLengths[columns[i]];
                string temp =  $"{i},{length}";
                format += "{"+temp+"} ";
            }
            
            string tableColumns = String.Format(format,columns.ToArray());
            string line = new string('-',tableColumns.Length);
            
            WriteLine(tableColumns);
            WriteLine(line);
            return format;
        }


        void ShowWeapons(string orderBy,string direction){
            arsenal.sortWeapons(orderBy,direction);
            string format = showTable(new string[] {"id", "name", "stock", "ammo","price","type","created"});
            foreach (Weapon weapon in arsenal.weapons)
            {   
                WriteLine(format,weapon.id.ToString(),textinfo.ToTitleCase(weapon.name),
                weapon.stock,weapon.ammo,weapon.price,weapon.type,weapon.created.ToString("yyyy"));
            }
        }

        string addToCart(string parameter)
        {   
            string cartResponse = "";
            int weaponId = int.Parse(parameter);
            int weaponIndex = arsenal.weapons.FindIndex(weapon => weapon.id == weaponId);
            Weapon selectedWeopon = arsenal.weapons.Find(weapon => weapon.id == weaponId);
            if (weaponIndex > -1 && selectedWeopon.stock > 0)
            {
                int cartIndex = cart.items.FindIndex(item => item.id == weaponId);
                string addCommand = $"{(cartIndex > -1 ? "increment cart":"fresh add")}";
                switch(addCommand){
                    case "increment cart":
                        cart.items[cartIndex].amount ++;
                        break;
                    case "fresh add":
                        cart.items.Add(new CartItem(){id=selectedWeopon.id,name=selectedWeopon.name,price=selectedWeopon.price,amount=1});
                        break;

                }
                selectedWeopon.stock --;
                cart.balance += selectedWeopon.price;
                cartResponse = cartIndex > -1 ? 
                    $"Weapon {textinfo.ToTitleCase(selectedWeopon.name)} amount updated in your cart": 
                    $"Successfully added {textinfo.ToTitleCase(selectedWeopon.name)} to your cart";
            }
            else
            {
                cartResponse = "Invalid weapon id number";
            }
            return cartResponse;
        }

        string removeFromCart(string parameter)
        {
            string cartResponse = "";
            int cartId = int.Parse(parameter);
            CartItem cartItem = cart.items.Find(item => item.id == cartId);
            bool existsInCart = cartItem != null;

            if (existsInCart){
                arsenal.weapons.Find(weapon => weapon.id == cartId).stock ++;
                string removeCommand = $"{(cartItem.amount == 1?"remove":"keep")} {(cart.items.Count() == 1? "items":"item")}";
                switch(removeCommand)
                {
                    case "remove items":
                        cart.items.Clear();
                        cart.balance = 0;
                        break;
                    case "remove item":
                        int cartIndex = cart.items.FindIndex(item => item.id == cartId);
                        cart.items.RemoveAt(cartIndex);
                        cart.balance -= cartItem.price;
                        break;
                    case "keep item":
                    case "keep items":
                        cartItem.amount --;
                        cart.balance -= cartItem.price;
                        break;
                }
                cartResponse = removeCommand.Split(" ")[0] == "remove" ? 
                    $"{textinfo.ToTitleCase(cartItem.name)} remove from your cart" : 
                    $"{textinfo.ToTitleCase(cartItem.name)} amount updated from your cart";
            }

            else 
            {
                cartResponse = "That weapon id doesn't exist in your cart";
            }
            return cartResponse;
        }

        void ShowCartItems(List<CartItem> items,string context,double balance){
            bool hasCartItems = items.Count > 0;
            if (hasCartItems)
            {
                string itemHeader = context == "cart" ? "Your cart items\n" : "Your order items\n";
                WriteLine(itemHeader);
                string format = showTable(new string[] {"id", "name", "price", "amount"});
                foreach(CartItem item in items)
                {
                    WriteLine(format,item.id,textinfo.ToTitleCase(item.name),item.price,item.amount);
                }
                string outPut = context == "cart" ? String.Format("\nYour balance is: \u20AC{0}", Math.Round(balance,2)) : String.Format("\nOrder total: \u20AC{0}", Math.Round(balance,2));
                WriteLine(outPut);
            }   
        }

        string checkOut(){
            string feedback = "";
            if (cart.items.Count() > 0)
            {
                Guid uuid = Guid.NewGuid();
                DateTime created = DateTime.Now;
                Order order = new Order(){total=cart.balance,checkOutDate=created,items=new List<CartItem>(cart.items),orderId=uuid};
                orders.Add(order);
                cart.items.Clear();
                cart.balance = 0;
                feedback = $"Order {order.orderId.ToString()} created at {order.checkOutDate}";
            }
            else{
                feedback ="No items exist in your cart buddy";
            }
            return feedback;
        }

        void showOrders(){
            bool hasOrders = orders.Count() > 0;
            if (hasOrders){
                foreach(Order order in orders){
                    WriteLine($"Order id:{order.orderId}\nCreated at:{order.checkOutDate}\n");
                    ShowCartItems(order.items,"orders",order.total); 
                }

            }
        }

        string sortBy = "stock";
        string direction = "down";
        string feedback = "";
        bool shopping = true;
        string shopOptions = 
            "options:\n"+
            "'add <weapon id>' - increment weapon amount in cart\n"+
            "'remove <weopon id>' - decrement weapon amount from cart\n"+
            "'sort <column name> up' - sort table of weapons by columns, up for ascending, down for descending\n"+
            "'search <weapon name>' - search table with weapon name containing characters\n"+
            "'checkout' - checkout order";        
        
        while (shopping)
        {   
            WriteLine("Here is our current stock:\n");
            ShowWeapons(sortBy,direction);
            Write("\n");
            ShowCartItems(cart.items,"cart",cart.balance);
            showOrders();
            Write(feedback);
            WriteLine(shopOptions);

            try
            {
                string[] shopResponse = ReadLine().Split(" ");
                string command = shopResponse[0];
                if (shopResponse.Length > 1){
                    string parameter = shopResponse[1];
                    switch(command)
                    {
                        case "add":
                            feedback = $"\n{addToCart(parameter)}\n\n" ;
                            break;
                        case "remove":
                            feedback = $"\n{removeFromCart(parameter)}\n\n" ;
                            break;
                        case "sort":
                            if (shopResponse.Length ==3){
                                sortBy = parameter;
                                direction = shopResponse[2];
                            }
                            break;
                        default:
                            feedback = "\nIncorrect input...\n\n";
                            break;
                    }

                }
                else if (command.Trim() =="checkout")
                {
                     feedback = $"\n{checkOut()}\n\n";
                }
            }
            catch (Exception e)
            {
                WriteLine(e);
                feedback = "\nIncorrect input..\n\n";
                
            }

            
        } 
    }
}
