namespace AndreaFrigerio.Core.Runtime.Gameplay
{
    /// <summary>
    /// Contract exposed by the <see cref="PongGameManager"/> so that any
    /// server-side component (e.g. <see cref="GoalDetector"/>) can add a
    /// point without knowing the concrete implementation.
    /// </summary>
    public interface IScoreService
    {
        /// <summary>
        /// Adds one point to the specified <paramref name="scorer"/>.
        /// </summary>
        /// <param name="scorer">
        /// Side that earns the point (<see cref="PlayerSide.Left"/> or 
        /// <see cref="PlayerSide.Right"/>).
        /// </param>
        void ServerAddScore(PlayerSide scorer);
    }

    /// <summary>
    /// Logical side of the field.
    /// </summary>
    public enum PlayerSide : byte { Left = 0, Right = 1 }
}
