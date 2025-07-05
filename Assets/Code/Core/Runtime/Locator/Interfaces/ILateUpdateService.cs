namespace AndreaFrigerio.Core.Runtime.Locator
{
    /// <summary>
    /// Interface for the late update service
    /// </summary>
    public interface ILateUpdateService 
    {
        /// <summary>
        /// Called every frame
        /// </summary>
        void OnLateUpdate(); 
    }
}
