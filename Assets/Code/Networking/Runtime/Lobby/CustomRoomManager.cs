namespace AndreaFrigerio.Network.Runtime.Lobby
{
    using UnityEngine;
    using Mirror;

    /// <summary>
    /// A custom room manager.
    /// </summary>
    [AddComponentMenu("Andrea Frigerio/Networking/Room Manager")]
    public sealed class CustomRoomManager : NetworkRoomManager
    {
        #region Server

        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);

            if (NetworkServer.connections.Count == 2)
            {
                ServerChangeScene(GameplayScene);
            }
        }

        #endregion
    }
}