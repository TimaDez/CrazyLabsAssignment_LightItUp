using System.Collections.Generic;
using UnityEngine;
using LightItUp.Singletons;
using LightItUp.Sound;

namespace LightItUp.Data
{
	public class ArpeggioPlayer : SingletonLoad<ArpeggioPlayer>
	{

		[System.Serializable]
		public struct Chord
		{
			public string name;
			public List<string> notes;
		}

		public bool useChords = true;
		[SerializeField]
		public List<Chord> chords = new List<Chord>();
		int chordIndex;
		int prevChordIndex;
		int noteCounter;
		int arpegCounter = 0;
		int dir = -1;
		public List<string> notes = new List<string>();

		[ContextMenu("Test")]
		public void PlayKey()
		{
			if(useChords)
			{
				float musicMaxLength = SoundManager.Instance.musicSource.clip.length;
				float currentTimeMusic = SoundManager.Instance.musicSource.time;
				chordIndex = (int)(currentTimeMusic / (musicMaxLength / chords.Count));

				if (noteCounter >= chords[chordIndex].notes.Count)
				{
					noteCounter = 0;
				}
				SoundManager.PlaySound(chords[chordIndex].notes[noteCounter]);
				//if(chordIndex == prevChordIndex)
				noteCounter++;
			}
			else
			{
				if(arpegCounter >= notes.Count)
				{
					arpegCounter = 0;
				}


				

				SoundManager.PlaySound(notes[arpegCounter]);
				arpegCounter++;
			}
		
		}
	}
}
