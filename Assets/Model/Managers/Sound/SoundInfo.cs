﻿namespace Myth
{
    public sealed class SoundInfo
    {
        private readonly int m_SerialId;
        private readonly SoundGroup m_SoundGroup;
        private readonly PlaySoundParams m_PlaySoundParams;
        private readonly object m_UserData;

        public SoundInfo(int serialId, SoundGroup soundGroup, PlaySoundParams playSoundParams, object userData)
        {
            m_SerialId = serialId;
            m_SoundGroup = soundGroup;
            m_PlaySoundParams = playSoundParams;
            m_UserData = userData;
        }

        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        public SoundGroup SoundGroup
        {
            get
            {
                return m_SoundGroup;
            }
        }

        public PlaySoundParams PlaySoundParams
        {
            get
            {
                return m_PlaySoundParams;
            }
        }

        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }
    }
}

