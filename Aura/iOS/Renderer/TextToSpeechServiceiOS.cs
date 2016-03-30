using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using Aura.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (TextToSpeechServiceiOS))]
namespace Aura.iOS
{
	public class TextToSpeechServiceiOS : ITextToSpeechService
	{
		const string DEFAULT_LOCALE = "en-US";
		/// <summary>
		/// The speak.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="language">The language.</param>
		public void Speak (string text, string language = DEFAULT_LOCALE)
		{
			var speechSynthesizer = new AVSpeechSynthesizer();

			var voice = AVSpeechSynthesisVoice.FromLanguage (language) ?? AVSpeechSynthesisVoice.FromLanguage (DEFAULT_LOCALE);
		
			var speechUtterance = new AVSpeechUtterance(text)
				                      {
					                      Rate = AVSpeechUtterance.MaximumSpeechRate / 16,
										  Voice = AVSpeechSynthesisVoice.FromLanguage (language),
					                      Volume = 0.5f,
					                      PitchMultiplier = 1.0f
				                      };

			speechSynthesizer.SpeakUtterance(speechUtterance);
		}

		/// <summary>
		/// Get installed languages.
		/// </summary>
		/// <returns>The installed language names.</returns>
		public IEnumerable<string> GetInstalledLanguages()
		{
			return AVSpeechSynthesisVoice.GetSpeechVoices().Select(a => a.Language).Distinct();
		}
	}
}