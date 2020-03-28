using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Teo
{
    
    [AddComponentMenu("")]
    public class NetworkManagerEngarde : NetworkManager
    {
        public Transform player1Spawn;
        public Transform player2Spawn;
        // Start is called before the first frame update
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Transform start = numPlayers == 0 ? player1Spawn : player2Spawn;
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            // spawn ball if two players
            if (numPlayers == 2)
            {
                //ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
                //Mirror.NetworkServer.Spawn(ball);
            }
        }
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy ball
            //if (ball != null)
            //    NetworkServer.Destroy(ball);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}
