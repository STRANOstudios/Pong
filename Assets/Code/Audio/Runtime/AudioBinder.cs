namespace AndreaFrigerio.Audio.Runtime
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Audio.Runtime.Library;
    using AndreaFrigerio.Core.Runtime.Gameplay;
    using AndreaFrigerio.Core.Runtime.Locator;
    
    /// <summary>
    /// Binds audio events to the <see cref="AudioManager"/>.
    /// </summary>
    [HideMonoScript]
    [AddComponentMenu("Andrea Frigerio/Audio/Audio Binder")]
    public class AudioBinder : MonoBehaviour
    {
        private AudioManager m_audioManager;

        private void Start() => this.m_audioManager = ServiceLocator.Get<AudioManager>();

        private void OnEnable()
        {
            BallController.OnPaddleBounce += SoundPaddle;
            BallController.OnWallBounce += SoundWall;
            PongGameManager.OnGoal += SoundGoal;
        }

        private void OnDisable()
        {
            BallController.OnPaddleBounce -= SoundPaddle;
            BallController.OnWallBounce -= SoundWall;
            PongGameManager.OnGoal -= SoundGoal;
        }

        private void PlaySound(string clipName) => this.m_audioManager.Play(clipName);

        private void SoundPaddle() => PlaySound("Paddle");
        private void SoundWall() => PlaySound("Wall");
        private void SoundGoal() => PlaySound("Score");
    }
}