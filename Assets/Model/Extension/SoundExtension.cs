using Myth;

namespace Myth
{
    public static class SoundExtension
    {
        private const float FadeVolumeDuration = 1f;
        private static int? s_MusicSerialId = null;

        public static int? PlayMusic(this SoundComponent soundComponent, int musicId, object userData = null)
        {
            soundComponent.StopMusic();

            //IDataTable<DRMusic> dtMusic = GameEntry.DataTable.GetDataTable<DRMusic>();
            //DRMusic drMusic = dtMusic.GetDataRow(musicId);
            //if (drMusic == null)
            //{
            //    Log.Warning("Can not load music '{0}' from data table.", musicId.ToString());
            //    return null;
            //}

            //读表 todo
            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = 64,
                Loop = true,
                VolumeInSoundGroup = 1f,
                FadeInSeconds = FadeVolumeDuration,
                SpatialBlend = 0f,
            };

            s_MusicSerialId = soundComponent.PlaySound("声音名字", "Music", Constant.AssetPriority.MusicAsset, playSoundParams, null, userData);
            return s_MusicSerialId;
        }

        public static void StopMusic(this SoundComponent soundComponent)
        {
            if (!s_MusicSerialId.HasValue)
            {
                return;
            }

            soundComponent.StopSound(s_MusicSerialId.Value, FadeVolumeDuration);
            s_MusicSerialId = null;
        }

        public static int? PlaySound(this SoundComponent soundComponent, int soundId, Entity bindingEntity = null, object userData = null)
        {
            //IDataTable<DRSound> dtSound = GameEntry.DataTable.GetDataTable<DRSound>();
            //DRSound drSound = dtSound.GetDataRow(soundId);
            //if (drSound == null)
            //{
            //    Log.Warning("Can not load sound '{0}' from data table.", soundId.ToString());
            //    return null;
            //}

            //PlaySoundParams playSoundParams = new PlaySoundParams
            //{
            //    Priority = drSound.Priority,
            //    Loop = drSound.Loop,
            //    VolumeInSoundGroup = drSound.Volume,
            //    SpatialBlend = drSound.SpatialBlend,
            //};

            //读表 todo
            PlaySoundParams playSoundParams = new PlaySoundParams();
            return soundComponent.PlaySound("声音名字", "Sound", Constant.AssetPriority.SoundAsset, playSoundParams, bindingEntity != null ? bindingEntity : null, userData);
        }

        public static int? PlayUISound(this SoundComponent soundComponent, int uiSoundId, object userData = null)
        {
            //IDataTable<DRUISound> dtUISound = GameEntry.DataTable.GetDataTable<DRUISound>();
            //DRUISound drUISound = dtUISound.GetDataRow(uiSoundId);
            //if (drUISound == null)
            //{
            //    Log.Warning("Can not load UI sound '{0}' from data table.", uiSoundId.ToString());
            //    return null;
            //}

            //PlaySoundParams playSoundParams = new PlaySoundParams
            //{
            //    Priority = drUISound.Priority,
            //    Loop = false,
            //    VolumeInSoundGroup = drUISound.Volume,
            //    SpatialBlend = 0f,
            //};
            //读表 todo
            PlaySoundParams playSoundParams = new PlaySoundParams();
            return soundComponent.PlaySound("声音名字", "UISound", Constant.AssetPriority.UISoundAsset, playSoundParams, userData);
        }

        public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return true;
            }

            SoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return true;
            }

            return soundGroup.Mute;
        }

        public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            SoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Mute = mute;

            GameEntry.Setting.SetBool(TextUtil.Format(Constant.Setting.SoundGroupMuted, soundGroupName), mute);
            GameEntry.Setting.Save();
        }

        public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return 0f;
            }

            SoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return 0f;
            }

            return soundGroup.Volume;
        }

        public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            SoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Volume = volume;

            GameEntry.Setting.SetFloat(TextUtil.Format(Constant.Setting.SoundGroupVolume, soundGroupName), volume);
            GameEntry.Setting.Save();
        }
    }
}
