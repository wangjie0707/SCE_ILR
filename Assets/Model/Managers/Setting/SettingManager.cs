//===================================================
//
//===================================================
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 设置管理器
    /// </summary>
    public class SettingManager : ManagerBase, IDisposable
    {

        public SettingManager()
        {

        }


        public override void Dispose()
        {

        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns>是否保存配置成功</returns>
        public bool Save()
        {
            PlayerPrefs.Save();
            return true;
        }

        /// <summary>
        /// 检查是否存在指定配置项
        /// </summary>
        /// <param name="settingName">要检查配置项的名称</param>
        /// <returns>指定的配置项是否存在</returns>
        public bool HasSetting(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.HasKey(settingName);
        }

        /// <summary>
        /// 移除指定配置项
        /// </summary>
        /// <param name="settingName">要移除配置项的名称</param>
        public void RemoveSetting(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            PlayerPrefs.DeleteKey(settingName);
        }

        /// <summary>
        /// 清空所有配置项
        /// </summary>
        public void RemoveAllSettings()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// 从指定配置项中读取布尔值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <returns>读取的布尔值</returns>
        public bool GetBool(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetInt(settingName) != 0;
        }

        /// <summary>
        /// 从指定配置项中读取布尔值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的布尔值</returns>
        public bool GetBool(string settingName, bool defaultValue)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetInt(settingName, defaultValue ? 1 : 0) != 0;
        }

        /// <summary>
        /// 向指定配置项写入布尔值
        /// </summary>
        /// <param name="settingName">要写入配置项的名称</param>
        /// <param name="value">要写入的布尔值</param>
        public void SetBool(string settingName, bool value)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            PlayerPrefs.SetInt(settingName, value ? 1 : 0);
        }

        /// <summary>
        /// 从指定配置项中读取整数值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <returns>读取的整数值</returns>
        public int GetInt(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetInt(settingName);
        }

        /// <summary>
        /// 从指定配置项中读取整数值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的整数值</returns>
        public int GetInt(string settingName, int defaultValue)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetInt(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定配置项写入整数值
        /// </summary>
        /// <param name="settingName">要写入配置项的名称</param>
        /// <param name="value">要写入的整数值</param>
        public void SetInt(string settingName, int value)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            PlayerPrefs.SetInt(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <returns>读取的浮点数值</returns>
        public float GetFloat(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetFloat(settingName);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的浮点数值</returns>
        public float GetFloat(string settingName, float defaultValue)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetFloat(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定配置项写入浮点数值
        /// </summary>
        /// <param name="settingName">要写入配置项的名称</param>
        /// <param name="value">要写入的浮点数值</param>
        public void SetFloat(string settingName, float value)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            PlayerPrefs.SetFloat(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <returns>读取的字符串值</returns>
        public string GetString(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetString(settingName);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值
        /// </summary>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的字符串值</returns>
        public string GetString(string settingName, string defaultValue)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return PlayerPrefs.GetString(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定配置项写入字符串值
        /// </summary>
        /// <param name="settingName">要写入配置项的名称</param>
        /// <param name="value">要写入的字符串值</param>
        public void SetString(string settingName, string value)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            PlayerPrefs.SetString(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取对象
        /// </summary>
        /// <typeparam name="T">要读取对象的类型</typeparam>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <returns>读取的对象</returns>
        public T GetObject<T>(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            return JsonMapper.ToObject<T>(PlayerPrefs.GetString(settingName));
        }

        /// <summary>
        /// 从指定配置项中读取对象
        /// </summary>
        /// <typeparam name="T">要读取对象的类型</typeparam>
        /// <param name="settingName">要获取配置项的名称</param>
        /// <param name="defaultObj">当指定的配置项不存在时，返回此默认对象</param>
        /// <returns>读取的对象</returns>
        public T GetObject<T>(string settingName, T defaultObj)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            string json = PlayerPrefs.GetString(settingName, null);
            if (json == null)
            {
                return defaultObj;
            }

            return JsonMapper.ToObject<T>(json);
        }

        /// <summary>
        /// 向指定配置项写入对象
        /// </summary>
        /// <typeparam name="T">要写入对象的类型</typeparam>
        /// <param name="settingName">要写入配置项的名称</param>
        /// <param name="obj">要写入的对象</param>
        public void SetObject<T>(string settingName, T obj)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            PlayerPrefs.SetString(settingName, JsonMapper.ToJson(obj));
        }

        /// <summary>
        /// 向指定配置项写入对象
        /// </summary>
        /// <param name="settingName">要写入配置项的名称</param>
        /// <param name="obj">要写入的对象</param>
        public void SetObject(string settingName, object obj)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new Exception("Setting name is invalid.");
            }
            PlayerPrefs.SetString(settingName, JsonMapper.ToJson(obj));
        }
    }
}
