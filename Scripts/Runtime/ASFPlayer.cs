using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBox.Logging;
using AudioBox.Tasks;
using AudioBox.Audio;
using UnityEngine;
using AudioBox.UI;

namespace AudioBox.ASF
{
	public enum ASFPlayerState
	{
		Stop,
		Play,
	}

	public abstract class ASFPlayer : UIEntity
	{
		const float MUSIC_OFFSET = 1;

		public double Time
		{
			get => m_Time;
			set
			{
				if (Math.Abs(m_Time - value) < float.Epsilon * 2)
					return;
				
				m_Time = value;
				
				Sample();
			}
		}

		public double Length => m_MusicMin + m_MusicMax + m_AudioSource.clip.length;

		public float Duration
		{
			get => m_Duration;
			protected set => m_Duration = value;
		}

		public float Ratio
		{
			get => m_Ratio;
			set => m_Ratio = value;
		}

		public AudioClip AudioClip
		{
			get => m_AudioSource.clip;
			set => m_AudioSource.clip = value;
		}

		public event Action OnFinish;

		ASFPlayerState State { get; set; } = ASFPlayerState.Stop;

		float MinTime => m_Duration * (m_Ratio - 1);

		float MaxTime => m_Duration * m_Ratio;

		[SerializeField] double      m_Time;
		[SerializeField] float       m_Ratio;
		[SerializeField] float       m_Duration;
		[SerializeField] double      m_MusicMin;
		[SerializeField] double      m_MusicMax;
		[SerializeField] AudioSource m_AudioSource;

		readonly List<ASFTrack> m_Tracks = new List<ASFTrack>();

		CancellationTokenSource m_TokenSource;

		void LateUpdate()
		{
			if (State == ASFPlayerState.Play)
				Time += UnityEngine.Time.deltaTime;
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			
			Sample();
		}
		#endif

		public async void Play()
		{
			if (State == ASFPlayerState.Play)
				return;
			
			AudioClip audioClip = m_AudioSource.clip;
			
			if (audioClip == null)
				return;
			
			m_TokenSource?.Cancel();
			m_TokenSource?.Dispose();
			
			m_TokenSource = new CancellationTokenSource();
			
			CancellationToken token = m_TokenSource.Token;
			
			double offset  = Math.Max(0, m_MusicMin - Time);
			double samples = ASFMath.RemapClamped(Time - m_MusicMin, 0, (double)audioClip.samples / audioClip.frequency, 0, audioClip.samples);
			
			m_AudioSource.Stop();
			
			m_AudioSource.timeSamples = (int)samples;
			
			m_AudioSource.PlayScheduled(AudioSettings.dspTime + offset + MUSIC_OFFSET - AudioManager.Latency);
			
			double error = 0;
			
			try
			{
				error = UnityEngine.Time.realtimeSinceStartup;
				
				await UnityTask.Delay(MUSIC_OFFSET, token);
				
				error = UnityEngine.Time.realtimeSinceStartup - error - MUSIC_OFFSET;
			}
			catch (TaskCanceledException)
			{
				Log.Info(this, "Play cancelled.");
			}
			catch (Exception exception)
			{
				Log.Exception(this, exception);
			}
			
			if (token.IsCancellationRequested)
				return;
			
			Time -= error;
			
			State = ASFPlayerState.Play;
			
			Sample();
			
			m_TokenSource?.Dispose();
			m_TokenSource = null;
		}

		public void Stop()
		{
			if (State == ASFPlayerState.Stop)
				return;
			
			State = ASFPlayerState.Stop;
			
			AudioClip audioClip = m_AudioSource.clip;
			
			if (audioClip == null)
				return;
			
			m_TokenSource?.Cancel();
			m_TokenSource?.Dispose();
			
			double samples = ASFMath.RemapClamped(Time - m_MusicMin, 0, (double)audioClip.samples / audioClip.frequency, 0, audioClip.samples);
			
			m_AudioSource.Stop();
			
			m_AudioSource.timeSamples = (int)samples;
			
			Sample();
		}

		public virtual void Sample()
		{
			double minTime = Time + MinTime;
			double maxTime = Time + MaxTime;
			
			foreach (ASFTrack track in m_Tracks)
				track.Sample(Time, minTime, maxTime);
			
			if (Time < Length)
				return;
			
			OnFinish?.Invoke();
			
			Stop();
		}

		public IDictionary<string, object> Serialize()
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			
			foreach (ASFTrack track in m_Tracks)
			{
				if (track == null)
				{
					Log.Error(this, "Serialization failed. Track is null.");
					continue;
				}
				
				string name = track.GetType().Name;
				
				data[name] = track.Serialize();
			}
			
			return data;
		}

		public void Deserialize(object _Data)
		{
			IDictionary<string, object> data = _Data as IDictionary<string, object>;
			
			if (data == null)
				return;
			
			foreach (var entry in data)
			{
				Type type = Type.GetType($"AudioBox.ASF.{entry.Key}");
				
				if (type == null)
					continue;
				
				ASFTrack track = m_Tracks.FirstOrDefault(_Track => _Track.GetType() == type);
				
				if (track == null)
				{
					Log.Error(this, "Deserialization failed. Track '{0}' not found.", type);
					continue;
				}
				
				track.Deserialize(entry.Value as IList);
			}
		}

		protected void AddTrack(ASFTrack _Track)
		{
			m_Tracks.Add(_Track);
		}

		protected void RemoveTrack(ASFTrack _Track)
		{
			m_Tracks.Remove(_Track);
		}

		public T GetTrack<T>() where T : ASFTrack
		{
			return m_Tracks.OfType<T>().FirstOrDefault();
		}
	}
}