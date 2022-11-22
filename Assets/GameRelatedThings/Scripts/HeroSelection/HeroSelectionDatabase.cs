using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HeroSelectionDatabase : MonoBehaviour
{
    [System.Serializable]
    public class DatabaseHero
    {
        public Sprite Image;
        public int Id;
        public string Name;
        public bool isOwned;
        public bool isUnlocked;
    }

    public static List<DatabaseHero> DatabaseHeroesList = new List<DatabaseHero>();
    public void AddHeroesToList(JObject json)
    {
        foreach (var x in json)
        {
            string name = x.Key;
            JToken value = x.Value;

            InitHeroes((int)value["ID"], (int)value["ID"], (string)value["Name"], (bool)value["HeroSelection"]["isOwned"], (bool)value["HeroSelection"]["isUnlocked"]);
        }
    }
    public void InitHeroes(int image, int id, string name, bool isOwned, bool isUnlocked)
    {
        DatabaseHero newHero = new DatabaseHero();
        newHero.isOwned = isOwned;
        newHero.Id = id;
        newHero.Name = name;
        newHero.isUnlocked = isUnlocked;
        DatabaseHeroesList.Add(newHero);
    }

}
