using System.Reflection;
using System.Globalization;

namespace weapon;

class Weapon{
    public int id {get;set;}
    public string name {get;set;}
    public int stock {get;set;}
    public int ammo {get;set;}
    public double price {get;set;}
    public string type {get;set;}
    public DateTime created  {get;set;}
}

class Arsenal{
    public List<Weapon> weapons = new List<Weapon>();
    /* {new Weapon(){id=101,name="ak-47",stock=5,ammo=30,price=220.9,type="assault rifle",created=new DateTime(1947,3,27)},
    new Weapon(){id=102,name="berretta m9",stock=10,ammo=15,price=120.99,type="pistol",created=new DateTime(1992,8,15)},
    new Weapon(){id=103,name="m14",stock=10,ammo=20,price=551.23,type="assault rifle",created=new DateTime(1954,5,29)},
    new Weapon(){id=104,name="mossberg 500",stock=95,ammo=6,price=182.52,type="shotgun",created=new DateTime(1961,12,13)},
    new Weapon(){id=105,name="uzi",stock=6,ammo=32,price=162.05,type="submachine gun",created=new DateTime(1954,11,21)}}; */

    public void sortWeapons(string orderBy,string direction){
            int prev = direction == "down" ? 1: -1;
            int next = direction == "down" ? -1: 1;
            switch(orderBy)
            {
                case "id":
                    this.weapons.Sort((a, b) => a.id > b.id ? prev : next);
                    break;
                case "name":
                    if (direction == "down")
                    {
                        this.weapons.Sort((a, b) => b.name.CompareTo(a.name) );
                    }
                    else 
                    {
                        this.weapons.Sort((a, b) => a.name.CompareTo(b.name) );
                    }
                    break;
                case "stock":
                    this.weapons.Sort((a, b) => a.stock > b.stock ? prev : next);
                    break;
                case "ammo":
                    this.weapons.Sort((a, b) => a.ammo > b.ammo ? prev : next);
                    break;
                case "price":
                    this.weapons.Sort((a, b) => a.price > b.price ? prev : next);
                    break;
                case "type":
                    if (direction == "down")
                    {
                        this.weapons.Sort((a, b) => b.type.CompareTo(a.type) );
                    }
                    else 
                    {
                        this.weapons.Sort((a, b) => a.type.CompareTo(b.type) );
                    }
                    break;
                case "created":
                    if (direction == "down")
                    {
                        this.weapons.Sort((a, b) => b.created.CompareTo(a.created) );
                    }
                    else 
                    {
                        this.weapons.Sort((a, b) => a.created.CompareTo(b.created) );
                    }
                    break;
            }


    }

    public void initCsv(){
        Weapon weapon = new Weapon();
        string[] classProps = new string[weapon.GetType().GetProperties().Count()];
        for (int i=0;i<classProps.Length;i++)
        {
            classProps[i] = weapon.GetType().GetProperties()[i].Name;
        }

        string[] csvLines = File.ReadAllLines("weapons.csv");
        if (string.Join(",",classProps) == csvLines[0])
        {
            foreach(string line in csvLines.Skip(1))
            {
                string[] data = line.Split(",");
                Weapon csvWeapon = new Weapon(){id=int.Parse(data[0]),name=data[1],
                    stock=int.Parse(data[2]),ammo=int.Parse(data[3]),price=double.Parse(data[4]),
                    type=data[5],created=DateTime.Parse(data[6],CultureInfo.GetCultureInfo("en-GB"))};
                
                this.weapons.Add(csvWeapon);
            }
        }

    }

}