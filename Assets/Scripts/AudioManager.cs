using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public enum SoundsType{Ambient = 0, Effect = 1, Conversation = 2};

public class AudioManager : MonoBehaviour {


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
		sources = new List<GameObject>();
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
	/// <summary>
	/// DEPRECATED
	/// </summary>
	/// <param name="sound">Sound.</param>
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
	/// <summary>
	/// DEPRECATED
	/// </summary>
	/// <param name="clipname">Clipname.</param>
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

	public void StopAmbientMusic()
	{
		for (int i=0; i<sources.Count; ++i) {
			AudioSource source = sources[i].GetComponent<AudioSource>();

			if(source.loop)
			{
				source.Stop();
			}
		}
	}

	public void Play(AudioClip audioClip, SoundsType soundtype, float delay)
	{
		GameObject go = new GameObject("GameSound");
		go.transform.SetParent(gameObject.transform, false);

		AudioSource source = go.AddComponent<AudioSource>();
		source.playOnAwake = false;
		
		source.clip = audioClip;

		switch (soundtype) {
			case SoundsType.Ambient:
				source.outputAudioMixerGroup = music;
				source.loop = true;
				StopAmbientMusic();
				break;
			case SoundsType.Conversation:
			case SoundsType.Effect:
				source.outputAudioMixerGroup = sfx;
				break;
			default:
				break;
		}

		source.PlayDelayed(delay);
		sources.Add(go);
		
	}
	
}