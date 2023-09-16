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
            int possibleWeaponId = int.Parse(parameter);
            int possibleWeaponIndex = arsenal.weapons.FindIndex(weapon => weapon.id == possibleWeaponId);
            Weapon selectedWeopon = arsenal.weapons.Find(weapon => weapon.id == possibleWeaponId);
            if (possibleWeaponIndex > -1 && selectedWeopon.stock > 0)
            {
                int existsInCart = cart.items.FindIndex(item => item.id == possibleWeaponId);
                if (existsInCart > -1)
                {
                    cart.items[existsInCart].amount ++;
                    cart.balance += cart.items[existsInCart].price;
                }
                else {
                    
                    cart.items.Add(new CartItem(){id=selectedWeopon.id,name=selectedWeopon.name,price=selectedWeopon.price,amount=1});
                    cart.balance += selectedWeopon.price;
                }
                cartResponse = String.Format(existsInCart > -1 ? "Weapon {0} amount updated in your cart": "Successfully added {0} to your cart",selectedWeopon.name);
                selectedWeopon.stock --;
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
            int possibleCartId = int.Parse(parameter);
            int possibleCartIndex = cart.items.FindIndex(item => item.id == possibleCartId);
            CartItem cartItem = cart.items.Find(item => item.id == possibleCartId);
            if (possibleCartIndex > -1)
            {
                cartResponse = String.Format(cartItem.amount == 1?"Weapon {0} removed from your cart":"Weapon {0} cart amount updated",cartItem.name);
                arsenal.weapons.Find(weapon => weapon.id == possibleCartId).stock ++; 
               
                if (cart.items.Count() == 1 && cartItem.amount == 1)
                {
                    cart.items.Clear();
                    cart.balance = 0;
                }
                else if (cart.items.Count() != 1 && cartItem.amount == 1){
                    cart.balance -= cartItem.price;
                    cart.items.RemoveAt(possibleCartIndex);
                }

                else 
                {
                    cartItem.amount --;
                    cart.balance -= cartItem.price;
                } 
            
            }

            else
            {
                cartResponse = "Invalid weapon id number";
            }
            

            return cartResponse;
        }

        void ShowCartItems(List<CartItem> items,string context,double balance){
            bool hasCartItems = items.Count > 0;
            if (hasCartItems)
            {
                string format = showTable(new string[] {"id", "name", "price", "amount"});
                foreach(CartItem item in items)
                {
                    WriteLine(format,item.id,item.name,item.price,item.amount);
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
            WriteLine("Your orders:\n");
            if (hasOrders){
               
                foreach(Order order in orders){
                    WriteLine($"Order id:{order.orderId}\nCreated at:{order.checkOutDate}\nItems:\n");
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
                        case "checkout":
                           
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
            catch (Exception)
            {
               feedback = "\nIncorrect input..\n\n";
                
            }

            
        } 
    }
}
