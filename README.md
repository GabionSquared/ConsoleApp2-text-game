# ConsoleApp2-text-game
Text-based C# dungeon crawler / rougelike with pokemon-esque combat

The parts of this worth noting is the hard data across the 4 .txt files that are read into some huge arrays.

These build the hostile entities (objects), weapons (structs interpreted by WeaponController) and map (a graph system with infomtion on what hostile or event to spawn).

The enemies technically are given a weapon with unique movesets rather than doing it themselves because of how EntityFramework is put together. You can find their AI on line 914 of Entities.cs.

Also at the bottom of Entities I overwrote Dicitonary<string,int> because not being able to use index locations annoyed me.

Sometimes I think about this diagram. It keeps me awake at night.
![System Diagram](https://images-ext-2.discordapp.net/external/dE2UuMJoDgcZuBi-PJPZjVDdIQ7KThwIB6uIs_tZh5E/%3Fwidth%3D1020%26height%3D270/https/media.discordapp.net/attachments/776124898488614914/818863123104006185/unknown.png)
