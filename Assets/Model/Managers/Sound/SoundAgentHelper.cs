using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Myth
{
    /// <summary>
    /// 默认声音代理辅助器
    /// </summary>
    public class SoundAgentHelper : MonoBehaviour
    {
        private Transform m_CachedTransform = null;
        private AudioSource m_AudioSource = null;
        private EntityBase m_BindingEntity = null;
        private float m_VolumeWhenPause = 0f;
        public Action ResetSoundAgent;

        /// <summary>
        /// 获取当前是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return m_AudioSource.isPlaying;
            }
        }

        /// <summary>
        /// 获取声音长度
        /// </summary>
        public float Length
        {
            get
            {
                return m_AudioSource.clip != null ? m_AudioSource.clip.length : 0f;
            }
        }

        /// <summary>
        /// 获取或设置播放位置
        /// </summary>
        public float Time
        {
            get
            {
                return m_AudioSource.time;
            }
            set
            {
                m_AudioSource.time = value;
            }
        }

        /// <summary>
        /// 获取或设置是否静音
        /// </summary>
        public bool Mute
        {
            get
            {
                return m_AudioSource.mute;
            }
            set
            {
                m_AudioSource.mute = value;
            }
        }

        /// <summary>
        /// 获取或设置是否循环播放
        /// </summary>
        public bool Loop
        {
            get
            {
                return m_AudioSource.loop;
            }
            set
            {
                m_AudioSource.loop = value;
            }
        }

        /// <summary>
        /// 获取或设置声音优先级
        /// </summary>
        public int Priority
        {
            get
            {
                return 128 - m_AudioSource.priority;
            }
            set
            {
                m_AudioSource.priority = 128 - value;
            }
        }

        /// <summary>
        /// 获取或设置音量大小
        /// </summary>
        public float Volume
        {
            get
            {
                return m_AudioSource.volume;
            }
            set
            {
                m_AudioSource.volume = value;
            }
        }

        /// <summary>
        /// 获取或设置声音音调
        /// </summary>
        public float Pitch
        {
            get
            {
                return m_AudioSource.pitch;
            }
            set
            {
                m_AudioSource.pitch = value;
            }
        }

        /// <summary>
        /// 获取或设置声音立体声声相
        /// </summary>
        public float PanStereo
        {
            get
            {
                return m_AudioSource.panStereo;
            }
            set
            {
                m_AudioSource.panStereo = value;
            }
        }

        /// <summary>
        /// 获取或设置声音空间混合量
        /// </summary>
        public float SpatialBlend
        {
            get
            {
                return m_AudioSource.spatialBlend;
            }
            set
            {
                m_AudioSource.spatialBlend = value;
            }
        }

        /// <summary>
        /// 获取或设置声音最大距离
        /// </summary>
        public float MaxDistance
        {
            get
            {
                return m_AudioSource.maxDistance;
            }

            set
            {
                m_AudioSource.maxDistance = value;
            }
        }

        /// <summary>
        /// 获取或设置声音多普勒等级
        /// </summary>
        public float DopplerLevel
        {
            get
            {
                return m_AudioSource.dopplerLevel;
            }
            set
            {
                m_AudioSource.dopplerLevel = value;
            }
        }

        /// <summary>
        /// 获取或设置声音代理辅助器所在的混音组
        /// </summary>
        public AudioMixerGroup AudioMixerGroup
        {
            get
            {
                return m_AudioSource.outputAudioMixerGroup;
            }
            set
            {
                m_AudioSource.outputAudioMixerGroup = value;
            }
        }


        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位</param>
        public void Play(float fadeInSeconds)
        {
            StopAllCoroutines();

            m_AudioSource.Play();
            if (fadeInSeconds > 0f)
            {
                float volume = m_AudioSource.volume;
                m_AudioSource.volume = 0f;
                StartCoroutine(FadeToVolume(m_AudioSource, volume, fadeInSeconds));
            }
        }

        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void Stop(float fadeOutSeconds)
        {
            StopAllCoroutines();

            if (fadeOutSeconds > 0f && gameObject.activeInHierarchy)
            {
                StartCoroutine(StopCo(fadeOutSeconds));
            }
            else
            {
                m_AudioSource.Stop();
            }
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void Pause(float fadeOutSeconds)
        {
            StopAllCoroutines();

            m_VolumeWhenPause = m_AudioSource.volume;
            if (fadeOutSeconds > 0f && gameObject.activeInHierarchy)
            {
                StartCoroutine(PauseCo(fadeOutSeconds));
            }
            else
            {
                m_AudioSource.Pause();
            }
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位</param>
        public void Resume(float fadeInSeconds)
        {
            StopAllCoroutines();

            m_AudioSource.UnPause();
            if (fadeInSeconds > 0f)
            {
                StartCoroutine(FadeToVolume(m_AudioSource, m_VolumeWhenPause, fadeInSeconds));
            }
            else
            {
                m_AudioSource.volume = m_VolumeWhenPause;
            }
        }

        /// <summary>
        /// 重置声音代理辅助器
        /// </summary>
        public void Reset()
        {
            m_CachedTransform.localPosition = Vector3.zero;
            m_AudioSource.clip = null;
            m_BindingEntity = null;
            m_VolumeWhenPause = 0f;
        }

        /// <summary>
        /// 设置声音资源
        /// </summary>
        /// <param name="soundAsset">声音资源</param>
        /// <returns>是否设置声音资源成功</returns>
        public bool SetSoundAsset(UnityEngine.Object soundAsset)
        {
            AudioClip audioClip = soundAsset as AudioClip;
            if (audioClip == null)
            {
                return false;
            }

            m_AudioSource.clip = audioClip;
            return true;
        }

        /// <summary>
        /// 设置声音绑定的实体
        /// </summary>
        /// <param name="bindingEntity">声音绑定的实体</param>
        public void SetBindingEntity(EntityBase bindingEntity)
        {
            m_BindingEntity = bindingEntity;
            if (m_BindingEntity != null)
            {
                UpdateAgentPosition();
                return;
            }

            if (ResetSoundAgent != null)
            {
                ResetSoundAgent();
            }
        }

        /// <summary>
        /// 设置声音所在的世界坐标
        /// </summary>
        /// <param name="worldPosition">声音所在的世界坐标</param>
        public void SetWorldPosition(Vector3 worldPosition)
        {
            m_CachedTransform.position = worldPosition;
        }

        private void Awake()
        {
            m_CachedTransform = transform;
            m_AudioSource = gameObject.GetOrAddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.rolloffMode = AudioRolloffMode.Custom;
        }

        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsPlaying && m_AudioSource.clip != null && ResetSoundAgent != null)
            {
                ResetSoundAgent();
                return;
            }

            if (m_BindingEntity != null)
            {
                UpdateAgentPosition();
            }
        }

        private void UpdateAgentPosition()
        {
            if (m_BindingEntity.IsAvailable)
            {
                m_CachedTransform.position = m_BindingEntity.SelfTransform.position;
                return;
            }

            if (ResetSoundAgent != null)
            {
                ResetSoundAgent();
            }
        }

        private IEnumerator StopCo(float fadeOutSeconds)
        {
            yield return FadeToVolume(m_AudioSource, 0f, fadeOutSeconds);
            m_AudioSource.Stop();
        }

        private IEnumerator PauseCo(float fadeOutSeconds)
        {
            yield return FadeToVolume(m_AudioSource, 0f, fadeOutSeconds);
            m_AudioSource.Pause();
        }

        private IEnumerator FadeToVolume(AudioSource audioSource, float volume, float duration)
        {
            float time = 0f;
            float originalVolume = audioSource.volume;
            while (time < duration)
            {
                time += UnityEngine.Time.deltaTime;
                audioSource.volume = Mathf.Lerp(originalVolume, volume, time / duration);
                yield return new WaitForEndOfFrame();
            }

            audioSource.volume = volume;
        }
    }
}
