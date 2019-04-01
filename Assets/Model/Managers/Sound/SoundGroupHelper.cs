﻿using UnityEngine;
using UnityEngine.Audio;

namespace Myth
{
    /// <summary>
    /// 声音组辅助器
    /// </summary>
    public class SoundGroupHelper : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerGroup m_AudioMixerGroup = null;

        /// <summary>
        /// 获取或设置声音组辅助器所在的混音组
        /// </summary>
        public AudioMixerGroup AudioMixerGroup
        {
            get
            {
                return m_AudioMixerGroup;
            }
            set
            {
                m_AudioMixerGroup = value;
            }
        }
    }
}
