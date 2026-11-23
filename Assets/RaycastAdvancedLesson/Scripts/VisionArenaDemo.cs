using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Обгортка для сценаріїв 2 і 3: вмикає EnemyVision на ворогові та ClickToMove на гравці,
    /// поки активна ця демка, і повертає гравця на старт при вході.
    /// </summary>
    public class VisionArenaDemo : MonoBehaviour, IRaycastDemo
    {
        public EnemyVision enemyVision;
        public ClickToMove clickToMove;

        private Vector3 playerStartPos;
        private Quaternion playerStartRot;
        private bool startCached;

        public string Title => "Сценарії 2–3: AI-видимість + рух по кліку";
        public string Hint => "ПКМ по підлозі — гравець (синій) іде в точку. Ворог (сірий, з чорним «візором») бачить гравця в радіусі 15 м, якщо між ними немає стін (шар Obstacle). Зелена лінія — бачить, червона — заважає стіна.";
        public string Status =>
            (enemyVision != null ? enemyVision.Reason : "—") +
            (clickToMove != null && clickToMove.IsMoving ? "\nГравець рухається до цілі" : "");

        void OnEnable()
        {
            if (clickToMove != null)
            {
                if (!startCached)
                {
                    playerStartPos = clickToMove.transform.position;
                    playerStartRot = clickToMove.transform.rotation;
                    startCached = true;
                }
                clickToMove.transform.SetPositionAndRotation(playerStartPos, playerStartRot);
                clickToMove.enabled = true;
            }
            if (enemyVision != null) enemyVision.enabled = true;
        }

        void OnDisable()
        {
            if (clickToMove != null) clickToMove.enabled = false;
            if (enemyVision != null) enemyVision.enabled = false;
        }
    }
}
