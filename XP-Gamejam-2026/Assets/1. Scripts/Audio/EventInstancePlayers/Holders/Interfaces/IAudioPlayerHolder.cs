using XPGJ2026.Audio.Interfaces;

namespace XPGJ2026.Audio.EventInstancePlayers.Holders.Interfaces
{
	public interface IAudioPlayerHolder
	{
		public IAudioEventInstancePlayer AudioPlayer { get; }

		public void SetAudioPlayer(IAudioEventInstancePlayer audioPlayer);
	}
}