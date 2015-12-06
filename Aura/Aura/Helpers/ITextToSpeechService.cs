
namespace Aura
{
	using System.Collections.Generic;

	/// <summary>
	/// The Text To Speech Service interface.
	/// </summary>
	public interface ITextToSpeechService
	{
		void Speak(string text, string language = "en-US");

		IEnumerable<string> GetInstalledLanguages();
	}
}
