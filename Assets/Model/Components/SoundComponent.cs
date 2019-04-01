using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Myth
{
    /// <summary>
    /// 场景组件
    /// </summary>
    public class SoundComponent : GameBaseComponent
    {
        private const int DefaultPriority = 0;

        private AudioListener m_AudioListener = null;

        [SerializeField]
        private bool m_EnablePlaySoundSuccessEvent = true;

        [SerializeField]
        private bool m_EnablePlaySoundFailureEvent = true;

        [SerializeField]
        private bool m_EnablePlaySoundUpdateEvent = false;

        [SerializeField]
        private bool m_EnablePlaySoundDependencyAssetEvent = false;

        [SerializeField]
        private AudioMixer m_AudioMixer = null;

        [SerializeField]
        private SoundGroupInfo[] m_SoundGroupInfos = null;

        /// <summary>
        /// 获取声音组数量。
        /// </summary>
        public int SoundGroupCount
        {
            get
            {
                return m_SoundManager.SoundGroupCount;
            }
        }

        /// <summary>
        /// 获取声音混响器。
        /// </summary>
        public AudioMixer AudioMixer
        {
            get
            {
                return m_AudioMixer;
            }
        }

        /// <summary>
        /// 声音管理器
        /// </summary>
        private SoundManager m_SoundManager;


        protected override void OnAwake()
        {
            base.OnAwake();
            m_SoundManager = new SoundManager();

            m_SoundManager.PlaySoundSuccess += OnPlaySoundSuccess;
            m_SoundManager.PlaySoundFailure += OnPlaySoundFailure;
            m_SoundManager.PlaySoundUpdate += OnPlaySoundUpdate;
            m_SoundManager.PlaySoundDependencyAsset += OnPlaySoundDependencyAsset;
            m_AudioListener = gameObject.GetOrAddComponent<AudioListener>();


#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
#endif
        }


        protected override void OnStart()
        {
            base.OnStart();
            for (int i = 0; i < m_SoundGroupInfos.Length; i++)
            {
                if (!AddSoundGroup(m_SoundGroupInfos[i].Name, m_SoundGroupInfos[i].AvoidBeingReplacedBySamePriority, m_SoundGroupInfos[i].Mute, m_SoundGroupInfos[i].Volume, m_SoundGroupInfos[i].AgentHelperCount))
                {
                    Log.Warning("Add sound group '{0}' failure.", m_SoundGroupInfos[i].Name);
                    continue;
                }
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_SoundManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 是否存在指定声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <returns>指定声音组是否存在</returns>
        public bool HasSoundGroup(string soundGroupName)
        {
            return m_SoundManager.HasSoundGroup(soundGroupName);
        }

        /// <summary>
        /// 获取指定声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <returns>要获取的声音组</returns>
        public SoundGroup GetSoundGroup(string soundGroupName)
        {
            return m_SoundManager.GetSoundGroup(soundGroupName);
        }

        /// <summary>
        /// 获取所有声音组
        /// </summary>
        /// <returns>所有声音组</returns>
        public SoundGroup[] GetAllSoundGroups()
        {
            return m_SoundManager.GetAllSoundGroups();
        }

        /// <summary>
        /// 获取所有声音组
        /// </summary>
        /// <param name="results">所有声音组</param>
        public void GetAllSoundGroups(List<SoundGroup> results)
        {
            m_SoundManager.GetAllSoundGroups(results);
        }

        /// <summary>
        /// 增加声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundAgentHelperCount">声音代理辅助器数量</param>
        /// <returns>是否增加声音组成功</returns>
        public bool AddSoundGroup(string soundGroupName, int soundAgentHelperCount)
        {
            return AddSoundGroup(soundGroupName, false, false, 1f, soundAgentHelperCount);
        }

        /// <summary>
        /// 增加声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundGroupAvoidBeingReplacedBySamePriority">声音组中的声音是否避免被同优先级声音替换</param>
        /// <param name="soundGroupMute">声音组是否静音</param>
        /// <param name="soundGroupVolume">声音组音量</param>
        /// <param name="soundAgentHelperCount">声音代理辅助器数量</param>
        /// <returns>是否增加声音组成功</returns>
        public bool AddSoundGroup(string soundGroupName, bool soundGroupAvoidBeingReplacedBySamePriority, bool soundGroupMute, float soundGroupVolume, int soundAgentHelperCount)
        {
            if (m_SoundManager.HasSoundGroup(soundGroupName))
            {
                return false;
            }

            SoundGroupHelper soundGroupHelper = new GameObject().AddComponent<SoundGroupHelper>();
            if (soundGroupHelper == null)
            {
                Log.Error("Can not create sound group helper.");
                return false;
            }

            soundGroupHelper.name = string.Format("Sound Group - {0}", soundGroupName);
            Transform transform = soundGroupHelper.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            if (m_AudioMixer != null)
            {
                AudioMixerGroup[] audioMixerGroups = m_AudioMixer.FindMatchingGroups(string.Format("Master/{0}", soundGroupName));
                if (audioMixerGroups.Length > 0)
                {
                    soundGroupHelper.AudioMixerGroup = audioMixerGroups[0];
                }
                else
                {
                    soundGroupHelper.AudioMixerGroup = m_AudioMixer.FindMatchingGroups("Master")[0];
                }
            }

            if (!m_SoundManager.AddSoundGroup(soundGroupName, soundGroupAvoidBeingReplacedBySamePriority, soundGroupMute, soundGroupVolume))
            {
                return false;
            }

            for (int i = 0; i < soundAgentHelperCount; i++)
            {
                if (!AddSoundAgentHelper(soundGroupName, soundGroupHelper, i))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取所有正在加载声音的序列编号
        /// </summary>
        /// <returns>所有正在加载声音的序列编号</returns>
        public int[] GetAllLoadingSoundSerialIds()
        {
            return m_SoundManager.GetAllLoadingSoundSerialIds();
        }

        /// <summary>
        /// 获取所有正在加载声音的序列编号
        /// </summary>
        /// <param name="results">所有正在加载声音的序列编号</param>
        public void GetAllLoadingSoundSerialIds(List<int> results)
        {
            m_SoundManager.GetAllLoadingSoundSerialIds(results);
        }

        /// <summary>
        /// 是否正在加载声音
        /// </summary>
        /// <param name="serialId">声音序列编号</param>
        /// <returns>是否正在加载声音</returns>
        public bool IsLoadingSound(int serialId)
        {
            return m_SoundManager.IsLoadingSound(serialId);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="assetBundleName">声音AssetBundle资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName)
        {
            return PlaySound(soundAssetName, soundGroupName, DefaultPriority, null, null, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="assetBundleName">声音AssetBundle资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="priority">加载声音资源的优先级</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, int priority)
        {
            return PlaySound(soundAssetName, soundGroupName, priority, null, null, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams)
        {
            return PlaySound(soundAssetName, soundGroupName, DefaultPriority, playSoundParams, null, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="assetBundleName">声音AssetBundle资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="bindingEntity">声音绑定的实体</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, EntityBase bindingEntity)
        {
            return PlaySound(soundAssetName, soundGroupName, DefaultPriority, null, bindingEntity, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="worldPosition">声音所在的世界坐标</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, Vector3 worldPosition)
        {
            return PlaySound(soundAssetName, soundGroupName, DefaultPriority, null, worldPosition, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, object userData)
        {
            return PlaySound(soundAssetName, soundGroupName, DefaultPriority, null, null, userData);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="priority">加载声音资源的优先级</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, int priority, PlaySoundParams playSoundParams)
        {
            return PlaySound(soundAssetName, soundGroupName, priority, playSoundParams, null, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="priority">加载声音资源的优先级</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, int priority, PlaySoundParams playSoundParams, object userData)
        {
            return PlaySound(soundAssetName, soundGroupName, priority, playSoundParams, null, userData);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="priority">加载声音资源的优先级</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <param name="bindingEntity">声音绑定的实体</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, int priority, PlaySoundParams playSoundParams, EntityBase bindingEntity)
        {
            return PlaySound(soundAssetName, soundGroupName, priority, playSoundParams, bindingEntity, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="priority">加载声音资源的优先级</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <param name="bindingEntity">声音绑定的实体</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, int priority, PlaySoundParams playSoundParams, EntityBase bindingEntity, object userData)
        {
            return m_SoundManager.PlaySound(soundAssetName, soundGroupName, priority, playSoundParams, new PlaySoundInfo(bindingEntity, Vector3.zero, userData));
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="priority">加载声音资源的优先级</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <param name="worldPosition">声音所在的世界坐标</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, int priority, PlaySoundParams playSoundParams, Vector3 worldPosition)
        {
            return PlaySound(soundAssetName, soundGroupName, priority, playSoundParams, worldPosition, null);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="soundAssetName">声音资源名称</param>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="priority">加载声音资源的优先级</param>
        /// <param name="playSoundParams">播放声音参数</param>
        /// <param name="worldPosition">声音所在的世界坐标</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>声音的序列编号</returns>
        public int PlaySound(string soundAssetName, string soundGroupName, int priority, PlaySoundParams playSoundParams, Vector3 worldPosition, object userData)
        {
            return m_SoundManager.PlaySound(soundAssetName, soundGroupName, priority, playSoundParams, new PlaySoundInfo(null, worldPosition, userData));
        }
        
        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="serialId">要停止播放声音的序列编号</param>
        /// <returns>是否停止播放声音成功</returns>
        public bool StopSound(int serialId)
        {
            return m_SoundManager.StopSound(serialId);
        }

        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="serialId">要停止播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        /// <returns>是否停止播放声音成功</returns>
        public bool StopSound(int serialId, float fadeOutSeconds)
        {
            return m_SoundManager.StopSound(serialId, fadeOutSeconds);
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        public void StopAllLoadedSounds()
        {
            m_SoundManager.StopAllLoadedSounds();
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void StopAllLoadedSounds(float fadeOutSeconds)
        {
            m_SoundManager.StopAllLoadedSounds(fadeOutSeconds);
        }

        /// <summary>
        /// 停止所有正在加载的声音
        /// </summary>
        public void StopAllLoadingSounds()
        {
            m_SoundManager.StopAllLoadingSounds();
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="serialId">要暂停播放声音的序列编号</param>
        public void PauseSound(int serialId)
        {
            m_SoundManager.PauseSound(serialId);
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="serialId">要暂停播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void PauseSound(int serialId, float fadeOutSeconds)
        {
            m_SoundManager.PauseSound(serialId, fadeOutSeconds);
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="serialId">要恢复播放声音的序列编号</param>
        public void ResumeSound(int serialId)
        {
            m_SoundManager.ResumeSound(serialId);
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="serialId">要恢复播放声音的序列编号</param>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位</param>
        public void ResumeSound(int serialId, float fadeInSeconds)
        {
            m_SoundManager.ResumeSound(serialId, fadeInSeconds);
        }

        /// <summary>
        /// 增加声音代理辅助器
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundGroupHelper">声音组辅助器</param>
        /// <param name="index">声音代理辅助器索引</param>
        /// <returns>是否增加声音代理辅助器成功</returns>
        private bool AddSoundAgentHelper(string soundGroupName, SoundGroupHelper soundGroupHelper, int index)
        {

            SoundAgentHelper soundAgentHelper = new GameObject().AddComponent<SoundAgentHelper>();
            if (soundAgentHelper == null)
            {
                Log.Error("Can not create sound agent helper.");
                return false;
            }

            soundAgentHelper.name = string.Format("Sound Agent Helper - {0} - {1}", soundGroupName, index.ToString());
            Transform transform = soundAgentHelper.transform;
            transform.SetParent(soundGroupHelper.transform);
            transform.localScale = Vector3.one;

            if (m_AudioMixer != null)
            {
                AudioMixerGroup[] audioMixerGroups = m_AudioMixer.FindMatchingGroups(string.Format("Master/{0}/{1}", soundGroupName, index.ToString()));
                if (audioMixerGroups.Length > 0)
                {
                    soundAgentHelper.AudioMixerGroup = audioMixerGroups[0];
                }
                else
                {
                    soundAgentHelper.AudioMixerGroup = soundGroupHelper.AudioMixerGroup;
                }
            }

            m_SoundManager.AddSoundAgentHelper(soundGroupName, soundAgentHelper);

            return true;
        }

        private void OnPlaySoundSuccess(int serialId, string soundAssetName, SoundAgent soundAgent, float duration, object userData)
        {
            PlaySoundInfo playSoundInfo = (PlaySoundInfo)userData;

            if (playSoundInfo != null)
            {
                SoundAgentHelper soundAgentHelper = soundAgent.Helper;
                if (playSoundInfo.BindingEntity != null)
                {
                    soundAgentHelper.SetBindingEntity(playSoundInfo.BindingEntity);
                }
                else
                {
                    soundAgentHelper.SetWorldPosition(playSoundInfo.WorldPosition);
                }
            }

            if (m_EnablePlaySoundSuccessEvent)
            {
                Debug.Log("声音播放成功事件");
            }
        }


        private void OnPlaySoundFailure(int serialId, string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams, PlaySoundErrorCode errorCode, string errorMessage, object userData)
        {
            string logMessage = string.Format("Play sound failure, asset name '{0}', sound group name '{1}', error code '{2}', error message '{3}'.", soundAssetName, soundGroupName, errorCode.ToString(), errorMessage);
            if (errorCode == PlaySoundErrorCode.IgnoredDueToLowPriority)
            {
                Debug.Log(logMessage);
            }
            else
            {
                Debug.LogWarning(logMessage);
            }

            if (m_EnablePlaySoundFailureEvent)
            {
                Debug.Log("声音播放失败事件");
            }
        }


        private void OnPlaySoundUpdate(int serialId, string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams, float progress, object userData)
        {
            if (m_EnablePlaySoundUpdateEvent)
            {
                Debug.Log("播放声音更新事件");
            }
        }

        private void OnPlaySoundDependencyAsset(int serialId, string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            if (m_EnablePlaySoundDependencyAssetEvent)
            {
                Debug.Log("播放声音时加载依赖资源事件");
            }
        }



        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            RefreshAudioListener();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            RefreshAudioListener();
        }

        private void RefreshAudioListener()
        {
            m_AudioListener.enabled = FindObjectsOfType<AudioListener>().Length <= 1;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_SoundManager.Dispose();
        }
    }
}
