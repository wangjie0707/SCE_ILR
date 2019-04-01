using System;
using System.Collections.Generic;
using UnityEngine;
using Myth;

namespace Myth
{
    /// <summary>
    /// 启动流程
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("ProcedureLaunch OnEnter");

            // 画质配置：设置即将使用的画质选项
            InitQualitySettings();

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();

            //初始化语言
            InitLanguage();

            //访问帐号服务器
            string url = GameEntry.Http.RealWebAccountUrl + "/api/init";

            Dictionary<string, object> dic = GameEntry.Pool.SpawnClassObject<Dictionary<string, object>>();
            dic.Clear();
            dic["ChannelId"] = 0;
            dic["InnerVersion"] = 1001;

            GameEntry.Http.SendData(url,OnWebAccountInit,true,dic);
        }
        
        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate( deltaTime,  unscaledDeltaTime);
        }

        public override void OnLeave()
        {
            base.OnLeave();
            Debug.Log("ProcedureLaunch OnLeave");
        }

        public override void OnDestory()
        {
            base.OnDestory();
        }

        /// <summary>
        /// 帐号服务器访问回调
        /// </summary>
        /// <param name="args"></param>
        private void OnWebAccountInit(HttpCallBackArgs args)
        {
            Debug.Log("HasError" + args.HasError);
            Debug.Log("Value" + args.Value);
        }
        
        
        /// <summary>
        /// 初始化画面
        /// </summary>
        private void InitQualitySettings()
        {
            QualityLevelType defaultQuality = QualityLevelType.Fantastic;
            int qualityLevel = GameEntry.Setting.GetInt(Constant.Setting.QualityLevel, (int)defaultQuality);
            QualitySettings.SetQualityLevel(qualityLevel, true);

            Log.Info("Init quality settings complete.");
        }

        /// <summary>
        /// 初始化声音
        /// </summary>
        private void InitSoundSettings()
        {
            GameEntry.Sound.Mute("Music", GameEntry.Setting.GetBool(Constant.Setting.MusicMuted, false));
            GameEntry.Sound.SetVolume("Music", GameEntry.Setting.GetFloat(Constant.Setting.MusicVolume, 0.3f));
            GameEntry.Sound.Mute("Sound", GameEntry.Setting.GetBool(Constant.Setting.SoundMuted, false));
            GameEntry.Sound.SetVolume("Sound", GameEntry.Setting.GetFloat(Constant.Setting.SoundVolume, 1f));
            GameEntry.Sound.Mute("UISound", GameEntry.Setting.GetBool(Constant.Setting.UISoundMuted, false));
            GameEntry.Sound.SetVolume("UISound", GameEntry.Setting.GetFloat(Constant.Setting.UISoundVolume, 1f));

            Log.Info("Init sound settings complete.");
        }

        /// <summary>
        /// 初始化语言
        /// </summary>
        private void InitLanguage()
        {
            string languageString = GameEntry.Setting.GetString(Constant.Setting.Language);
            if (!string.IsNullOrEmpty(languageString))
            {
                try
                {
                    GameEntry.Localization.CurrLanguage = (Language)Enum.Parse(typeof(Language), languageString);
                }
                catch
                {
                    GameEntry.Localization.CurrLanguage = Language.Chinese;
                }
            }
            else
            {
                GameEntry.Localization.CurrLanguage = Language.Chinese;
                GameEntry.Setting.SetString(Constant.Setting.Language, GameEntry.Localization.CurrLanguage.ToString());
                GameEntry.Setting.Save();
            }

            Log.Info("Init language settings complete, current language is '{0}'.", GameEntry.Localization.CurrLanguage.ToString());
        }
    }
}