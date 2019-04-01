using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Myth;

namespace Myth
{
    public static class UIExtension
    {
        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = alpha;
        }

        public static IEnumerator SmoothValue(this Slider slider, float value, float duration)
        {
            float time = 0f;
            float originalValue = slider.value;
            while (time < duration)
            {
                time += Time.deltaTime;
                slider.value = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }

            slider.value = value;
        }

        public static bool HasUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null)
        {
            return uiComponent.HasUIForm((int)uiFormId, uiGroupName);
        }

        public static bool HasUIForm(this UIComponent uiComponent, int uiFormId, string uiGroupName = null)
        {
            //IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            //DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            //if (drUIForm == null)
            //{
            //    return false;
            //}

            //string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
            //读表 todo
            string assetName = string.Empty;
            if (string.IsNullOrEmpty(uiGroupName))
            {
                return uiComponent.HasUIForm(assetName);
            }

            UIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
            if (uiGroup == null)
            {
                return false;
            }

            return uiGroup.HasUIForm(assetName);
        }

        public static UIForm GetUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null)
        {
            return uiComponent.GetUIForm((int)uiFormId, uiGroupName);
        }

        public static UIForm GetUIForm(this UIComponent uiComponent, int uiFormId, string uiGroupName = null)
        {
            //IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            //DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            //if (drUIForm == null)
            //{
            //    return null;
            //}

            //string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);

            //读表 todo
            string assetName = string.Empty;
            UIForm uiForm = null;
            if (string.IsNullOrEmpty(uiGroupName))
            {
                uiForm = (UIForm)uiComponent.GetUIForm(assetName);
                if (uiForm == null)
                {
                    return null;
                }

                return uiForm;
            }

            UIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
            if (uiGroup == null)
            {
                return null;
            }

            uiForm = (UIForm)uiGroup.GetUIForm(assetName);
            if (uiForm == null)
            {
                return null;
            }

            return uiForm;
        }

        public static int? OpenUIForm(this UIComponent uiComponent, UIFormId uiFormId, object userData = null)
        {
            return uiComponent.OpenUIForm((int)uiFormId, userData);
        }

        public static int? OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
        {
            //IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            //DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            //if (drUIForm == null)
            //{
            //    Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
            //    return null;
            //}

            //string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);

            //读表 todo
            string assetName = string.Empty;
            //if (!drUIForm.AllowMultiInstance)//允许多个实例 todo
            //{
            //    if (uiComponent.IsLoadingUIForm(assetName))
            //    {
            //        return null;
            //    }

            //    if (uiComponent.HasUIForm(assetName))
            //    {
            //        return null;
            //    }
            //}
            bool PauseCoveredUIForm = false;//是否暂停 todo
            
            return uiComponent.OpenUIForm(assetName,"组的名字", Constant.AssetPriority.UIFormAsset, PauseCoveredUIForm, userData);
        }
    }
}
