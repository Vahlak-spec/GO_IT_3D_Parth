using UnityEngine;
using static AmbientTrigger;

public class AmbientTrigger : MonoBehaviour
{
    [SerializeField] private AmbientType ambientType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (ambientType)
            {
                case AmbientType.Dungeon:
                    FindObjectOfType<AmbientManager>().TransitionToDungeon();
                    break;
                case AmbientType.Forest:
                    FindObjectOfType<AmbientManager>().TransitionToForest();
                    break;
            }
        }
    }
}
public enum AmbientType
{
    Forest,
    Dungeon
}