using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBox.Logging;
using AudioBox.Tasks;
using AudioBox.Audio;
using UnityEngine;
using AudioBox.UI;
using Zenject;

namespace AudioBox.ASF
{
	public enum ASFPlayerState
	{
		Stop,
		Play,
	}

	public partial class ASFPlayer
	{
		[ContextMenu("Test serialize")]
		public void Serialize() { }

		public void Deserialize(string _JSON)
		{
			Debug.LogError(JsonUtility.ToJson(m_Tracks[0], true));
		}
	}

	public partial class ASFPlayer : UIEntity
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

		public event Action OnFinish;

		ASFPlayerState State { get; set; } = ASFPlayerState.Stop;

		public double Length => m_MusicMin + m_MusicMax + m_AudioSource.clip.length;

		float MinTime => m_Duration * (m_Ratio - 1);

		float MaxTime => m_Duration * m_Ratio;

		[SerializeField] double      m_Time;
		[SerializeField] float       m_Ratio;
		[SerializeField] float       m_Duration;
		[SerializeField] double      m_MusicMin;
		[SerializeField] double      m_MusicMax;
		[SerializeField] AudioSource m_AudioSource;

		[Inject] ASFTapTrack.Factory    m_TapTrackFactory;
		[Inject] ASFDoubleTrack.Factory m_DoubleTrackFactory;

		readonly List<ASFTrack> m_Tracks = new List<ASFTrack>();

		CancellationTokenSource m_TokenSource;

		protected override void Awake()
		{
			base.Awake();
			
			m_Tracks.Clear();
			
			ASFTapTrack tapTrack = m_TapTrackFactory.Create();
			for (int i = 0; i < 240; i++)
			{
				if (i % 5 != 0)
					tapTrack.AddClip(new ASFTapClip(m_MusicMin + i, i % 4));
			}
			m_Tracks.Add(tapTrack);
			
			ASFDoubleTrack doubleTrack = m_DoubleTrackFactory.Create();
			for (int i = 0; i < 240; i++)
			{
				if (i % 5 == 0)
					doubleTrack.AddClip(new ASFDoubleClip(m_MusicMin + i));
			}
			m_Tracks.Add(doubleTrack);
		}

		public void PlayTest()
		{
			Play();
		}

		public async void RewindTest()
		{
			Stop();
			
			double source = Time;
			double target = Time - 1;
			
			await UnityTask.Phase(_Phase => Time = ASFMath.Lerp(source, target, _Phase), 2);
			
			Play();
		}

		public void StopTest()
		{
			Stop();
		}

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

		public void Sample()
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
	}
}