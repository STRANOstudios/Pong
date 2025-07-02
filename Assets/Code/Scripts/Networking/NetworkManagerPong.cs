using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace AndreaFrigerio.Network
{
    public class NetworkManagerPong : NetworkManager
    {
        [Header("Spawn Points")]
        [SerializeField] private Vector3 leftRacketSpawn;
        [SerializeField] private Vector3 rightRacketSpawn;

        [SerializeField] private string mainMenuScene = "MainMenu";

        private GameObject ballInstance;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Vector3 spawnPoint = numPlayers == 0 ? leftRacketSpawn : rightRacketSpawn;

            GameObject player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);

            if (numPlayers == 2)
            {
                SpawnBall();
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            if (ballInstance != null)
            {
                NetworkServer.Destroy(ballInstance);
                ballInstance = null;
            }

            // Torna al menu se un client si disconnette (server side)
            ReturnToMenu();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

            // Torna al menu se il client perde connessione
            ReturnToMenu();
        }

        private void SpawnBall()
        {
            GameObject ballPrefab = spawnPrefabs.Find(p => p.name == "Ball");

            if (ballPrefab != null)
            {
                ballInstance = Instantiate(ballPrefab);
                NetworkServer.Spawn(ballInstance);
            }
            else
            {
                Debug.LogError("Ball prefab not found in spawnPrefabs list.");
            }
        }

        private void ReturnToMenu()
        {
            // Ferma tutto e torna alla scena iniziale
            if (NetworkServer.active) StopHost();
            else if (NetworkClient.isConnected) StopClient();

            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
