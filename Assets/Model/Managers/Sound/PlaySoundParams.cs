﻿namespace Myth
{
    /// <summary>
    /// 播放声音参数
    /// </summary>
    public sealed class PlaySoundParams
    {
        private float m_Time;
        private bool m_MuteInSoundGroup;
        private bool m_Loop;
        private int m_Priority;
        private float m_VolumeInSoundGroup;
        private float m_FadeInSeconds;
        private float m_Pitch;
        private float m_PanStereo;
        private float m_SpatialBlend;
        private float m_MaxDistance;
        private float m_DopplerLevel;

        /// <summary>
        /// 初始化播放声音参数的新实例
        /// </summary>
        public PlaySoundParams()
        {
            m_Time = Constant.Sound.DefaultTime;
            m_MuteInSoundGroup = Constant.Sound.DefaultMute;
            m_Loop = Constant.Sound.DefaultLoop;
            m_Priority = Constant.Sound.DefaultPriority;
            m_VolumeInSoundGroup = Constant.Sound.DefaultVolume;
            m_FadeInSeconds = Constant.Sound.DefaultFadeInSeconds;
            m_Pitch = Constant.Sound.DefaultPitch;
            m_PanStereo = Constant.Sound.DefaultPanStereo;
            m_SpatialBlend = Constant.Sound.DefaultSpatialBlend;
            m_MaxDistance = Constant.Sound.DefaultMaxDistance;
            m_DopplerLevel = Constant.Sound.DefaultDopplerLevel;
        }

        /// <summary>
        /// 获取或设置播放位置
        /// </summary>
        public float Time
        {
            get
            {
                return m_Time;
            }
            set
            {
                m_Time = value;
            }
        }

        /// <summary>
        /// 获取或设置在声音组内是否静音
        /// </summary>
        public bool MuteInSoundGroup
        {
            get
            {
                return m_MuteInSoundGroup;
            }
            set
            {
                m_MuteInSoundGroup = value;
            }
        }

        /// <summary>
        /// 获取或设置是否循环播放
        /// </summary>
        public bool Loop
        {
            get
            {
                return m_Loop;
            }
            set
            {
                m_Loop = value;
            }
        }

        /// <summary>
        /// 获取或设置声音优先级
        /// </summary>
        public int Priority
        {
            get
            {
                return m_Priority;
            }
            set
            {
                m_Priority = value;
            }
        }

        /// <summary>
        /// 获取或设置在声音组内音量大小
        /// </summary>
        public float VolumeInSoundGroup
        {
            get
            {
                return m_VolumeInSoundGroup;
            }
            set
            {
                m_VolumeInSoundGroup = value;
            }
        }

        /// <summary>
        /// 获取或设置声音淡入时间，以秒为单位
        /// </summary>
        public float FadeInSeconds
        {
            get
            {
                return m_FadeInSeconds;
            }
            set
            {
                m_FadeInSeconds = value;
            }
        }

        /// <summary>
        /// 获取或设置声音音调
        /// </summary>
        public float Pitch
        {
            get
            {
                return m_Pitch;
            }
            set
            {
                m_Pitch = value;
            }
        }

        /// <summary>
        /// 获取或设置声音立体声声相
        /// </summary>
        public float PanStereo
        {
            get
            {
                return m_PanStereo;
            }
            set
            {
                m_PanStereo = value;
            }
        }

        /// <summary>
        /// 获取或设置声音空间混合量
        /// </summary>
        public float SpatialBlend
        {
            get
            {
                return m_SpatialBlend;
            }
            set
            {
                m_SpatialBlend = value;
            }
        }

        /// <summary>
        /// 获取或设置声音最大距离
        /// </summary>
        public float MaxDistance
        {
            get
            {
                return m_MaxDistance;
            }
            set
            {
                m_MaxDistance = value;
            }
        }

        /// <summary>
        /// 获取或设置声音多普勒等级
        /// </summary>
        public float DopplerLevel
        {
            get
            {
                return m_DopplerLevel;
            }
            set
            {
                m_DopplerLevel = value;
            }
        }
    }
}
