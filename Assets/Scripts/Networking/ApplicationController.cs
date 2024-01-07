using System.Threading.Tasks;
using UnityEngine;

//Handles creating headless server, client, or host
public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
            LaunchDedicatedServer();
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated){
                //Go to main menu
                Debug.Log("Client authenticated going to main Menu");
                clientSingleton.GameManager.GoToMainMenu();
            }
            else
            {
                Debug.Log("Client failed to authenticate");
            }
                
        }
            
    }

    private void LaunchDedicatedServer()
    {
        throw new System.NotImplementedException();
    }
}
