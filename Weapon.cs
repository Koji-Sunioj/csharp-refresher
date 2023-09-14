namespace weapon;

class Weapon{
    public int id;
    public string name;
    public int stock;
    public int ammo;
    public double price;
    public string type;
    public DateTime created;
}

class Arsenal{
    public List<Weapon> weapons = new()
    {new Weapon(){id=101,name="ak-47",stock=5,ammo=30,price=220.9,type="assault rifle",created=new DateTime(1947,3,27)},
    new Weapon(){id=102,name="berretta m9",stock=10,ammo=15,price=120.99,type="pistol",created=new DateTime(1992,8,15)},
    new Weapon(){id=103,name="m14",stock=10,ammo=20,price=551.23,type="assault rifle",created=new DateTime(1954,5,29)},
    new Weapon(){id=104,name="mossberg 500",stock=95,ammo=6,price=182.52,type="shotgun",created=new DateTime(1961,12,13)},
    new Weapon(){id=105,name="uzi",stock=6,ammo=32,price=162.05,type="submachine gun",created=new DateTime(1954,11,21)}};
}