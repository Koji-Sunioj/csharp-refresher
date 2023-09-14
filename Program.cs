namespace RecordStore2;

using cart;
using weapon;

using System.Text;
using System.Globalization;
using static System.Console;

class Program
{
    static void Main(string[] args)
    { 
        Cart cart = new Cart();
        Arsenal arsenal = new Arsenal();
        TextInfo textinfo = new CultureInfo("en-GB", false).TextInfo;
      

        string showTable(string[] columns){
            
            var menuItemLengths = new Dictionary<string, int>(){
                {"id", -3},
                {"name",-15},
                {"stock",-5},
                {"ammo",-5},
                {"price",-7},
                {"type",-20},
                {"created",-10},
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

        

        void ShowWeapons(){
            string format = showTable(new string[] {"id", "name", "stock", "ammo","price","type","created"});
            foreach (Weapon weapon in arsenal.weapons)
            {   
                WriteLine(format,weapon.id.ToString(),textinfo.ToTitleCase(weapon.name),
                weapon.stock,weapon.ammo,weapon.price,weapon.type,weapon.created.ToString("yyyy"));
            }
        }

        void ShowCart(){
            string format = showTable(new string[] {"id", "name", "price", "amount"});
            foreach(CartItem item in cart.items)
            {
                WriteLine(format,item.id,item.name,item.price,item.amount);
            }
            
        }

        bool visiting = true;
        bool shopping = false;
        string shopOptions = 
            "\nwhat would you like to do?\n1. buy a weapon (enter id)\n2. sort the table (enter the column name)\n3. search for a weapon\n4. go to main menu";
        string mainMenu = 
            "Welcome to the arsenal main menu. please select an option.\n\n1. view guns\n2. view cart\n3. leave";
        


        if(visiting)
        {
            Clear();
            WriteLine(mainMenu);
        }

        
        
        while (visiting)
        {
            string visitPrompt = ReadLine().Trim();
            switch (visitPrompt)
            {   
                case "1":
                    shopping = true;
                    Clear();
                    WriteLine("Here is our current stock:\n");
                    ShowWeapons();
                    WriteLine(shopOptions);
                    while (shopping)
                    {       
                        string shopResponse = ReadLine();
                        int possibleWeaponId = int.Parse(shopResponse);
                        int possibleWeaponIndex = arsenal.weapons.FindIndex(weapon => weapon.id == possibleWeaponId);
                        
                        if (possibleWeaponIndex > -1 && arsenal.weapons[possibleWeaponIndex].stock > 0)
                        {
                            int existsInCart = cart.items.FindIndex(item => item.id == possibleWeaponId);
                            if (existsInCart > -1)
                            {
                                cart.items[existsInCart].amount ++;
                                cart.balance += cart.items[existsInCart].price;
                            }
                            else {
                                Weapon selectedWeopon = arsenal.weapons.Find(weapon => weapon.id == possibleWeaponId);
                                cart.items.Add(new CartItem(){id=selectedWeopon.id,name=selectedWeopon.name,price=selectedWeopon.price,amount=1});
                                cart.balance += selectedWeopon.price;
                                WriteLine("Successfully added {0} to your cart",selectedWeopon.name);
                            }

                            arsenal.weapons[possibleWeaponIndex].stock --;
                            ShowCart();
                            WriteLine("\nyour balance is:\u20AC{0}",cart.balance);
                            
                        }

                        else {
                            switch(shopResponse)
                            {
                                case "4":
                                    shopping = false;
                                    Clear();
                                    WriteLine(mainMenu);
                                    break;
                                default:
                                    WriteLine("no option or weapon id with that input exists");
                                    break;
                            }
                        } 
                    } 
                    break;
                case "3":
                    visiting = false;
                    WriteLine("\nOkay, cya later buddy");
                    break;
                default:
                    WriteLine("\numm... didn't get that buddy");
                    break;

                
            }
        }
    }
}
