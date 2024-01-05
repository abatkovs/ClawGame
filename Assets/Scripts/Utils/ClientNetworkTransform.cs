using Unity.Netcode.Components;

namespace Utils
{
    public class ClientNetworkTransform : NetworkTransform
    {
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }
        
        protected override void Update()
        {
            CanCommitToTransform = IsOwner;
            base.Update();
            //TODO: might cause some issues stuttering/etc
        }
        
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
