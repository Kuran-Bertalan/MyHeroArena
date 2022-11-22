using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelection : MonoBehaviour
{
    public static HeroSelection Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [System.Serializable] public class Hero
    {
        public Sprite Image;
        public int Id;
        public string Name;
        public bool isOwned;
        public bool isUnlocked;
    }


    [System.Serializable]
    public class SerializableList<T>
    {
        public List<Hero> SelectedHeroesList;
    }
    [SerializeField] private SerializableList<string> SelectedHeroes;

    [SerializeField] public Transform HeroScrollView;
    [SerializeField] public GameObject ItemTemplate;
    [SerializeField] public Button SaveSelectionBtn;

    public List<Hero> HeroesList;
    

    GameObject g;
    Toggle selectBtn;

    void Start()
    {
        var DatabaseHeroes = HeroSelectionDatabase.DatabaseHeroesList;
        Debug.Log(DatabaseHeroes);
        for (int i = 0; i < DatabaseHeroes.Count; i++)
        {
            int j = i;
            g = Instantiate(ItemTemplate, HeroScrollView);
            g.transform.GetChild(0).GetComponent<Image>().sprite = HeroesList[i].Image;
            HeroesList[i].isOwned = DatabaseHeroes[i].isOwned;
            HeroesList[i].isUnlocked = DatabaseHeroes[i].isUnlocked;
            selectBtn = g.transform.GetChild(1).GetComponent<Toggle>();
            if (DatabaseHeroes[i].isUnlocked && DatabaseHeroes[i].isOwned)
            {
                g.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "SELECT";
            }
            else
            {
                DisableSelectButton();
            }
            selectBtn.onValueChanged.AddListener((i) => OnSelectBtnClicked(i, HeroesList[j].Id));
        }
    }


    void OnSelectBtnClicked(bool value, int id)
    {
        if(SelectedHeroes.SelectedHeroesList.Count == 3)
        {
            if(value == false)
            {
                SelectedHeroes.SelectedHeroesList.Remove(HeroesList[id]);
                HeroScrollView.GetChild(id).GetChild(1).GetChild(0).GetComponent<Text>().text = "SELECT";
            }

            Debug.Log(SelectedHeroes.SelectedHeroesList.Count);
            return;
        }
        if (value)
        {
            SelectedHeroes.SelectedHeroesList.Add(HeroesList[id]);
            HeroScrollView.GetChild(id).GetChild(1).GetChild(0).GetComponent<Text>().text = "SELECTED";
        }
        else
        {
            SelectedHeroes.SelectedHeroesList.Remove(HeroesList[id]);
            HeroScrollView.GetChild(id).GetChild(1).GetChild(0).GetComponent<Text>().text = "SELECT";
        }
        
        Debug.Log(SelectedHeroes.SelectedHeroesList.Count);
    }

    void DisableSelectButton()
    {
        selectBtn.interactable = false;
        g.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "NOT OWNED";
    }

    public void OnSaveSelection()
    {
        Debug.Log("Save");
        string selectedHeroes = JsonUtility.ToJson(SelectedHeroes); 
        Debug.Log(selectedHeroes);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SelectedHeroes.json", selectedHeroes);
    }

    void Update()
    {
        if(SelectedHeroes.SelectedHeroesList.Count == 3)
        {
            SaveSelectionBtn.interactable = true;
        }
        else
        {
            SaveSelectionBtn.interactable = false;
        }
    }
}


