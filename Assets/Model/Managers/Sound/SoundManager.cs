using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 声音管理器
    /// </summary>
    public class SoundManager : ManagerBase, IDisposable
    {
        private readonly Dictionary<string, SoundGroup> m_SoundGroups;
        private readonly List<int> m_SoundsBeingLoaded;
        private readonly HashSet<int> m_SoundsToReleaseOnLoad;
        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;
        private int m_Serial;

        /// <summary>
        /// 播放声音成功事件  声音的序列编号  声音资源名称  用于播放的声音代理 加载持续时间 用户自定义数据
        /// </summary>
        public Action<int, string, SoundAgent, float, object> PlaySoundSuccess;

        /// <summary>
        /// 播放声音失败事件  声音的序列编号 声音资源名称 声音组名称 播放声音参数 错误码 错误信息 用户自定义数据
        /// </summary>
        public Action<int, string, string, PlaySoundParams, PlaySoundErrorCode, string, object> PlaySoundFailure;

        /// <summary>
        /// 播放声音更新事件 声音的序列编号 声音资源名称 声音组名称 播放声音参数 加载声音进度 用户自定义数据
        /// </summary>
        public Action<int, string, string, PlaySoundParams, float, object> PlaySoundUpdate;

        /// <summary>
        /// 播放声音时加载依赖资源事件 声音的序列编号 声音资源名称 声音组名称 播放声音参数 被加载的依赖资源名称 当前已加载依赖资源数量 总共加载依赖资源数量 用户自定义数据
        /// </summary>
        public Action<int, string, string, PlaySoundParams, string, int, int, object> PlaySoundDependencyAsset;


        /// <summary>
        /// 初始化声音管理器的新实例
        /// </summary>
        public SoundManager()
        {
            m_SoundGroups = new Dictionary<string, SoundGroup>();
            m_SoundsBeingLoaded = new List<int>();
            m_SoundsToReleaseOnLoad = new HashSet<int>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadSoundSuccessCallback, LoadSoundFailureCallback, LoadSoundUpdateCallback, LoadSoundDependencyAssetCallback);
            m_Serial = 0;
        }



        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            var soundGroup = m_SoundGroups.GetEnumerator();
            while (soundGroup.MoveNext())
            {
                soundGroup.Current.Value.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        #region 加载完毕回调
        private void LoadSoundSuccessCallback(string soundAssetName, UnityEngine.Object soundAsset, float duration, object userData)
        {
            SoundInfo soundInfo = (SoundInfo)userData;
            if (soundInfo == null)
            {
                throw new Exception("Play sound info is invalid.");
            }

            m_SoundsBeingLoaded.Remove(soundInfo.SerialId);
            if (m_SoundsToReleaseOnLoad.Contains(soundInfo.SerialId))
            {
                Log.Info("Release sound '{0}' on loading success.", soundInfo.SerialId.ToString());
                m_SoundsToReleaseOnLoad.Remove(soundInfo.SerialId);
                GameEntry.Resource.UnloadAsset(soundAsset);
                return;
            }

            PlaySoundErrorCode? errorCode = null;
            SoundAgent soundAgent = soundInfo.SoundGroup.PlaySound(soundInfo.SerialId, soundAsset, soundInfo.PlaySoundParams, out errorCode);
            if (soundAgent != null)
            {
                if (PlaySoundSuccess != null)
                {
                    PlaySoundSuccess(soundInfo.SerialId, soundAssetName, soundAgent, duration, soundInfo.UserData);
                }
            }
            else
            {
                m_SoundsToReleaseOnLoad.Remove(soundInfo.SerialId);
                GameEntry.Resource.UnloadAsset(soundAsset);
                string errorMessage = string.Format("Sound group '{0}' play sound '{1}' failure.", soundInfo.SoundGroup.Name, soundAssetName);


                if (PlaySoundFailure != null)
                {
                    PlaySoundFailure(soundInfo.SerialId, soundAssetName, soundInfo.SoundGroup.Name, soundInfo.PlaySoundParams, errorCode.Value, errorMessage, soundInfo.UserData);
                    return;
                }

                throw new Exception(errorMessage);
            }
        }

        private void LoadSoundFailureCallback(string soundAssetName, string errorMessage, object userData)
        {
            SoundInfo soundInfo = (SoundInfo)userData;
            if (soundInfo == null)
            {
                throw new Exception("Play sound info is invalid.");
            }

            m_SoundsBeingLoaded.Remove(soundInfo.SerialId);
            m_SoundsToReleaseOnLoad.Remove(soundInfo.SerialId);
            string appendErrorMessage = string.Format("Load sound failure, asset name '{0}', error message '{1}'.", soundAssetName, errorMessage);

            if (PlaySoundFailure != null)
            {
                PlaySoundFailure(soundInfo.SerialId, soundAssetName, soundInfo.SoundGroup.Name, soundInfo.PlaySoundParams, PlaySoundErrorCode.LoadAssetFailure, appendErrorMessage, soundInfo.UserData);
                return;
            }

            throw new Exception(appendErrorMessage);
        }

        private void LoadSoundUpdateCallback(string soundAssetName, float progress, object userData)
        {
            SoundInfo soundInfo = (SoundInfo)userData;
            if (soundInfo == null)
            {
                throw new Exception("Play sound info is invalid.");
            }

            if (PlaySoundUpdate != null)
            {
                PlaySoundUpdate(soundInfo.SerialId, soundAssetName, soundInfo.SoundGroup.Name, soundInfo.PlaySoundParams, progress, soundInfo.UserData);
            }
        }

        private void LoadSoundDependencyAssetCallback(string soundAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            SoundInfo soundInfo = (SoundInfo)userData;
            if (soundInfo == null)
            {
                throw new Exception("Play sound info is invalid.");
            }

            if (PlaySoundDependencyAsset != null)
            {
                PlaySoundDependencyAsset(soundInfo.SerialId, soundAssetName, soundInfo.SoundGroup.Name, soundInfo.PlaySoundParams, dependencyAssetName, loadedCount, totalCount, soundInfo.UserData);
            }
        }
        #endregion

        /// <summary>
        /// 获取声音组数量。
        /// </summary>
        public int SoundGroupCount
        {
            get
            {
                return m_SoundGroups.Count;
            }
        }

        /// <summary>
        /// 是否存在指定声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <returns>指定声音组是否存在</returns>
        public bool HasSoundGroup(string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                throw new Exception("Sound group name is invalid.");
            }

            return m_SoundGroups.ContainsKey(soundGroupName);
        }

        /// <summary>
        /// 获取指定声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <returns>要获取的声音组</returns>
        public SoundGroup GetSoundGroup(string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                throw new Exception("Sound group name is invalid.");
            }

            SoundGroup soundGroup = null;
            if (m_SoundGroups.TryGetValue(soundGroupName, out soundGroup))
            {
                return soundGroup;
            }

            return null;
        }

        /// <summary>
        /// 获取所有声音组
        /// </summary>
        /// <returns>所有声音组</returns>
        public SoundGroup[] GetAllSoundGroups()
        {
            int index = 0;
            SoundGroup[] results = new SoundGroup[m_SoundGroups.Count];
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                results[index++] = soundGroup.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有声音组
        /// </summary>
        /// <param name="results">所有声音组</param>
        public void GetAllSoundGroups(List<SoundGroup> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                results.Add(soundGroup.Value);
            }
        }

        /// <summary>
        /// 增加声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundGroupHelper">声音组辅助器</param>
        /// <returns>是否增加声音组成功</returns>
        public bool AddSoundGroup(string soundGroupName)
        {
            return AddSoundGroup(soundGroupName, false, Constant.Sound.DefaultMute, Constant.Sound.DefaultVolume);
        }

        /// <summary>
        /// 增加声音组
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundGroupAvoidBeingReplacedBySamePriority">声音组中的声音是否避免被同优先级声音替换</param>
        /// <param name="soundGroupMute">声音组是否静音</param>
        /// <param name="soundGroupVolume">声音组音量</param>
        /// <param name="soundGroupHelper">声音组辅助器</param>
        /// <returns>是否增加声音组成功</returns>
        public bool AddSoundGroup(string soundGroupName, bool soundGroupAvoidBeingReplacedBySamePriority, bool soundGroupMute, float soundGroupVolume)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                throw new Exception("Sound group name is invalid.");
            }

            if (HasSoundGroup(soundGroupName))
            {
                return false;
            }

            SoundGroup soundGroup = new SoundGroup(soundGroupName)
            {
                AvoidBeingReplacedBySamePriority = soundGroupAvoidBeingReplacedBySamePriority,
                Mute = soundGroupMute,
                Volume = soundGroupVolume
            };

            m_SoundGroups.Add(soundGroupName, soundGroup);

            return true;
        }

        /// <summary>
        /// 增加声音代理辅助器
        /// </summary>
        /// <param name="soundGroupName">声音组名称</param>
        /// <param name="soundAgentHelper">要增加的声音代理辅助器</param>
        public void AddSoundAgentHelper(string soundGroupName, SoundAgentHelper soundAgentHelper)
        {

            SoundGroup soundGroup = GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                throw new Exception(string.Format("Sound group '{0}' is not exist.", soundGroupName));
            }

            soundGroup.AddSoundAgentHelper(soundAgentHelper);
        }

        /// <summary>
        /// 获取所有正在加载声音的序列编号
        /// </summary>
        /// <returns>所有正在加载声音的序列编号</returns>
        public int[] GetAllLoadingSoundSerialIds()
        {
            return m_SoundsBeingLoaded.ToArray();
        }

        /// <summary>
        /// 获取所有正在加载声音的序列编号
        /// </summary>
        /// <param name="results">所有正在加载声音的序列编号</param>
        public void GetAllLoadingSoundSerialIds(List<int> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            results.AddRange(m_SoundsBeingLoaded);
        }

        /// <summary>
        /// 是否正在加载声音
        /// </summary>
        /// <param name="serialId">声音序列编号</param>
        /// <returns>是否正在加载声音</returns>
        public bool IsLoadingSound(int serialId)
        {
            return m_SoundsBeingLoaded.Contains(serialId);
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
        public int PlaySound(string soundAssetName,   string soundGroupName, int priority, PlaySoundParams playSoundParams, object userData)
        {

            if (playSoundParams == null)
            {
                playSoundParams = new PlaySoundParams();
            }

            int serialId = m_Serial++;
            PlaySoundErrorCode? errorCode = null;
            string errorMessage = null;
            SoundGroup soundGroup = GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                errorCode = PlaySoundErrorCode.SoundGroupNotExist;
                errorMessage = string.Format("Sound group '{0}' is not exist.", soundGroupName);
            }
            else if (soundGroup.SoundAgentCount <= 0)
            {
                errorCode = PlaySoundErrorCode.SoundGroupHasNoAgent;
                errorMessage = string.Format("Sound group '{0}' is have no sound agent.", soundGroupName);
            }

            if (errorCode.HasValue)
            {
                if (PlaySoundFailure != null)
                {
                    PlaySoundFailure(serialId, soundAssetName, soundGroupName, playSoundParams, errorCode.Value, errorMessage, userData);
                    return serialId;
                }

                throw new Exception(errorMessage);
            }

            m_SoundsBeingLoaded.Add(serialId);
            GameEntry.Resource.LoadAsset(soundAssetName,  typeof(AudioClip), priority, m_LoadAssetCallbacks, new SoundInfo(serialId, soundGroup, playSoundParams, userData));
            return serialId;
        }
        

        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="serialId">要停止播放声音的序列编号</param>
        /// <returns>是否停止播放声音成功</returns>
        public bool StopSound(int serialId)
        {
            return StopSound(serialId, Constant.Sound.DefaultFadeOutSeconds);
        }

        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="serialId">要停止播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        /// <returns>是否停止播放声音成功</returns>
        public bool StopSound(int serialId, float fadeOutSeconds)
        {
            if (IsLoadingSound(serialId))
            {
                m_SoundsToReleaseOnLoad.Add(serialId);
                return true;
            }

            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                if (soundGroup.Value.StopSound(serialId, fadeOutSeconds))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        public void StopAllLoadedSounds()
        {
            StopAllLoadedSounds(Constant.Sound.DefaultFadeOutSeconds);
        }

        /// <summary>
        /// 停止所有已加载的声音
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void StopAllLoadedSounds(float fadeOutSeconds)
        {
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                soundGroup.Value.StopAllLoadedSounds(fadeOutSeconds);
            }
        }

        /// <summary>
        /// 停止所有正在加载的声音
        /// </summary>
        public void StopAllLoadingSounds()
        {
            foreach (int serialId in m_SoundsBeingLoaded)
            {
                m_SoundsToReleaseOnLoad.Add(serialId);
            }
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="serialId">要暂停播放声音的序列编号</param>
        public void PauseSound(int serialId)
        {
            PauseSound(serialId, Constant.Sound.DefaultFadeOutSeconds);
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="serialId">要暂停播放声音的序列编号</param>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位</param>
        public void PauseSound(int serialId, float fadeOutSeconds)
        {
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                if (soundGroup.Value.PauseSound(serialId, fadeOutSeconds))
                {
                    return;
                }
            }

            throw new Exception(string.Format("Can not find sound '{0}'.", serialId.ToString()));
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="serialId">要恢复播放声音的序列编号</param>
        public void ResumeSound(int serialId)
        {
            ResumeSound(serialId, Constant.Sound.DefaultFadeInSeconds);
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="serialId">要恢复播放声音的序列编号</param>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位</param>
        public void ResumeSound(int serialId, float fadeInSeconds)
        {
            foreach (KeyValuePair<string, SoundGroup> soundGroup in m_SoundGroups)
            {
                if (soundGroup.Value.ResumeSound(serialId, fadeInSeconds))
                {
                    return;
                }
            }

            throw new Exception(string.Format("Can not find sound '{0}'.", serialId.ToString()));
        }



        public override void Dispose()
        {
            StopAllLoadedSounds();
            m_SoundGroups.Clear();
            m_SoundsBeingLoaded.Clear();
            m_SoundsToReleaseOnLoad.Clear();
        }
    }
}
