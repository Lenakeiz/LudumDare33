using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {


	public enum SoundsType{Ambient = 0, Effect = 1, Conversation = 2};
	
	[System.Serializable]
	public class CustomSound
	{
		public string name;
		public SoundsType type;
		public AudioClip clip;
	};

	public List<CustomSound> clips;
	
	public List<GameObject> sources;

	public AudioMixerGroup music;
	public AudioMixerGroup sfx;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<sources.Count; ++i) {
			if(sources[i].GetComponent<AudioSource>().isPlaying==false)
			{
				GameObject.Destroy(sources[i]);
				sources.RemoveAt(i);
				i--;
				
			}
		}
	}
	
	void SpawnAudioObject(CustomSound sound)
	{
		GameObject g = new GameObject ();
		g.name = "AudioClip" + name + sources.Count;
		g.AddComponent<AudioSource> ();
		AudioSource source = g.GetComponent<AudioSource> ();
		source.clip = sound.clip;
		if(sound.type == SoundsType.Ambient)
		{
			source.outputAudioMixerGroup = music;
		}
		if (sound.type == SoundsType.Effect || sound.type == SoundsType.Conversation) {
			source.outputAudioMixerGroup = sfx;
		}
		source.Play ();
		sources.Add (g);
	}
	
	public AudioSource Play(string clipname)
	{
		for (int i=0; i<clips.Count; ++i) {
			if (clipname == clips [i].name) {
				SpawnAudioObject(clips[i]);
				return sources[sources.Count-1].GetComponent<AudioSource>();
			}
		}
		return null;
	}
	
}