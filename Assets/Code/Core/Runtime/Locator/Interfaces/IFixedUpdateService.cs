namespace AndreaFrigerio.Core.Runtime.Locator
{
    /// <summary>
    /// Interface for the fixed update service
    /// </summary>
    public interface IFixedUpdateService
    {
        /// <summary>
        /// Called every fixed update
        /// </summary>
        void OnFixedUpdate();
    }
}
