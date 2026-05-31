using UnityEngine;

namespace XPGJ2026
{
    public class MusicChangeTrigger : MonoBehaviour
    {

        [Header("Area")]
        [SerializeField] private MusicArea area;

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.tag.Equals("Player"))
			{

				Debug.Log("Change");
				AudioManager.Instance.SetMusicArea(area);
			}
		}
	}
}
