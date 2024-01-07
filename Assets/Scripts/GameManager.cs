using System.Collections.Generic;
using UnityEngine;

//Local gameManager instance for server
public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    [field: SerializeField] public string RoomName { get; private set; }
    [field: SerializeField] public List<Character> Characters { get; private set; } = new List<Character>();
    [field: SerializeField] public LevelSpawnPoints LevelSpawnPoints { get; private set; }    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetRoomName(string roomName)
    {
        RoomName = roomName;
    }
    
    public void AddCharacter(Character character)
    {
        Characters.Add(character);
    }
}