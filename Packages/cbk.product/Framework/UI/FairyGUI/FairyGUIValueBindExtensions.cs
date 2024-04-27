using System;
using FairyGUI;
using CBK.Framework.Collections;
using CBK.Framework.Core;
using CBK.Framework.Utility;
using UnityEngine;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// 值绑定扩展
    /// </summary>
    public static class FairyGUIValueBindExtensions
    {
        #region [Text]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的text属性上
        /// </summary>
        public static IDisposable BindText(this GObject gObject, ObservableVariable<string> variable)
        {
            void OnValueChanged(string _, string value)
            {
                gObject.text = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的text属性上
        /// </summary>
        public static IDisposable BindText<T>(this GObject gObject, ObservableVariable<T> variable)
        {
            var isValueType = typeof(T).IsValueType;

            void OnValueChanged(T _, T value)
            {
                gObject.text = value.ToString();
            }

            void OnValueChangedRef(T _, T value)
            {
                gObject.text = value?.ToString() ?? string.Empty;
            }

            return variable.SubscribeAsDisposable(isValueType ? OnValueChanged : OnValueChangedRef, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的字符串格式化绑定到UI的text属性上
        /// </summary>
        public static IDisposable BindText<T>(this GObject gObject, ObservableVariable<T> variable, string format)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.text = string.Format(format, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的text属性上
        /// </summary>
        public static IDisposable BindText<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, string> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.text = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的text属性上
        /// </summary>
        public static IDisposable BindText<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, string> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.text = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }
        
        /// <summary>
        /// 为GTextField绑定一个变量 将变量的值以指定的变量名绑定到GTextField的文本模板上
        /// </summary>
        public static IDisposable BindTextVar(this GTextField gTextField, ObservableVariable<string> variable, string varName)
        {
            void OnValueChanged(string _, string value)
            {
                gTextField.SetVar(varName, value).FlushVars();
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为GTextField绑定一个变量 将变量的值以指定的变量名绑定到GTextField的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GTextField gTextField, ObservableVariable<T> variable, string varName)
        {
            var isValueType = typeof(T).IsValueType;

            void OnValueChanged(T _, T value)
            {
                gTextField.SetVar(varName, value.ToString()).FlushVars();
            }

            void OnValueChangedRef(T _, T value)
            {
                gTextField.SetVar(varName, value?.ToString() ?? string.Empty).FlushVars();
            }

            return isValueType ? variable.SubscribeAsDisposable(OnValueChanged, true) : variable.SubscribeAsDisposable(OnValueChangedRef, true);
        }

        /// <summary>
        /// 为GTextField绑定一个变量 将变量的值以指定的变量名绑定到GTextField的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GTextField gTextField, ObservableVariable<T> variable, string varName, Func<T, string> func)
        {
            void OnValueChanged(T _, T value)
            {
                gTextField.SetVar(varName, func(value)).FlushVars();
            }
            
            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }
        
        /// <summary>
        /// 为GTextField绑定一个变量 将变量的值以指定的变量名绑定到GTextField的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GTextField gTextField, ObservableVariable<T> variable, string varName, Func<T, T, string> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gTextField.SetVar(varName, func(pre, value)).FlushVars();
            }
            
            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为GButton绑定一个变量 将变量的值以指定的变量名绑定到GButton的文本模板上
        /// </summary>
        public static IDisposable BindTextVar(this GButton gButton, ObservableVariable<string> variable, string varName)
        {
            return gButton.GetTextField()?.BindTextVar(variable, varName);
        }
        
        /// <summary>
        /// 为GButton绑定一个变量 将变量的值以指定的变量名绑定到GButton的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GButton gButton, ObservableVariable<T> variable, string varName)
        {
            return gButton.GetTextField()?.BindTextVar(variable, varName);
        }
        
        /// <summary>
        /// 为GButton绑定一个变量 将变量的值以指定的变量名绑定到GButton的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GButton gButton, ObservableVariable<T> variable, string varName, Func<T, string> func)
        {
            return gButton.GetTextField()?.BindTextVar(variable, varName, func);
        }
        
        /// <summary>
        /// 为GButton绑定一个变量 将变量的值以指定的变量名绑定到GButton的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GButton gButton, ObservableVariable<T> variable, string varName, Func<T, T, string> func)
        {
            return gButton.GetTextField()?.BindTextVar(variable, varName, func);
        }

        /// <summary>
        /// 为GLabel绑定一个变量 将变量的值以指定的变量名绑定到GLabel的文本模板上
        /// </summary>
        public static IDisposable BindTextVar(this GLabel gLabel, ObservableVariable<string> variable, string varName)
        {
            return gLabel.GetTextField()?.BindTextVar(variable, varName);
        }
        
        /// <summary>
        /// 为GLabel绑定一个变量 将变量的值以指定的变量名绑定到GLabel的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GLabel gLabel, ObservableVariable<T> variable, string varName)
        {
            return gLabel.GetTextField()?.BindTextVar(variable, varName);
        }
        
        /// <summary>
        /// 为GLabel绑定一个变量 将变量的值以指定的变量名绑定到GLabel的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GLabel gLabel, ObservableVariable<T> variable, string varName, Func<T, string> func)
        {
            return gLabel.GetTextField()?.BindTextVar(variable, varName, func);
        }
        
        /// <summary>
        /// 为GLabel绑定一个变量 将变量的值以指定的变量名绑定到GLabel的文本模板上
        /// </summary>
        public static IDisposable BindTextVar<T>(this GLabel gLabel, ObservableVariable<T> variable, string varName, Func<T, T, string> func)
        {
            return gLabel.GetTextField()?.BindTextVar(variable, varName, func);
        }

        #endregion

        #region [Icon]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的icon属性上
        /// </summary>
        public static IDisposable BindIcon(this GObject gObject, ObservableVariable<string> variable)
        {
            void OnValueChanged(string _, string value)
            {
                gObject.icon = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的icon属性上
        /// </summary>
        public static IDisposable BindIcon<T>(this GObject gObject, ObservableVariable<T> variable)
        {
            var isValueType = typeof(T).IsValueType;

            void OnValueChanged(T _, T value)
            {
                gObject.icon = value.ToString();
            }

            void OnValueChangedRef(T _, T value)
            {
                gObject.icon = value?.ToString() ?? string.Empty;
            }

            return variable.SubscribeAsDisposable(isValueType ? OnValueChanged : OnValueChangedRef, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的字符串格式化绑定到UI的icon属性上
        /// </summary>
        public static IDisposable BindIcon<T>(this GObject gObject, ObservableVariable<T> variable, string format)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.icon = string.Format(format, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的icon属性上
        /// </summary>
        public static IDisposable BindIcon<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, string> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.icon = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的icon属性上
        /// </summary>
        public static IDisposable BindIcon<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, string> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.icon = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Visible]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的visible属性上
        /// </summary>
        public static IDisposable BindVisible(this GObject gObject, ObservableVariable<bool> variable, bool isReverse = false)
        {
            void OnValueChanged(bool _, bool value)
            {
                gObject.visible = value;
            }

            void OnValueChangedReverse(bool _, bool value)
            {
                gObject.visible = !value;
            }

            return variable.SubscribeAsDisposable(isReverse ? OnValueChangedReverse : OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的visible属性上
        /// </summary>
        public static IDisposable BindVisible<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, bool> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.visible = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的visible属性上
        /// </summary>
        public static IDisposable BindVisible<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, bool> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.visible = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Touchable]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的touchable属性上
        /// </summary>
        public static IDisposable BindTouchable(this GObject gObject, ObservableVariable<bool> variable, bool isReverse = false)
        {
            void OnValueChanged(bool _, bool value)
            {
                gObject.touchable = value;
            }

            void OnValueChangedReverse(bool _, bool value)
            {
                gObject.touchable = !value;
            }

            return variable.SubscribeAsDisposable(isReverse ? OnValueChangedReverse : OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的touchable属性上
        /// </summary>
        public static IDisposable BindTouchable<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, bool> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.touchable = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的touchable属性上
        /// </summary>
        public static IDisposable BindTouchable<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, bool> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.touchable = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Grayed]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的grayed属性上
        /// </summary>
        public static IDisposable BindGrayed(this GObject gObject, ObservableVariable<bool> variable, bool isReverse = false)
        {
            void OnValueChanged(bool _, bool value)
            {
                gObject.grayed = value;
            }

            void OnValueChangedReverse(bool _, bool value)
            {
                gObject.grayed = !value;
            }

            return variable.SubscribeAsDisposable(isReverse ? OnValueChangedReverse : OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的grayed属性上
        /// </summary>
        public static IDisposable BindGrayed<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, bool> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.grayed = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的grayed属性上
        /// </summary>
        public static IDisposable BindGrayed<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, bool> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.grayed = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Alpha]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的alpha属性上
        /// </summary>
        public static IDisposable BindAlpha(this GObject gObject, ObservableVariable<float> variable, bool isReverse = false)
        {
            void OnValueChanged(float _, float value)
            {
                gObject.alpha = value;
            }

            void OnValueChangedReverse(float _, float value)
            {
                gObject.alpha = 1 - value;
            }

            return variable.SubscribeAsDisposable(isReverse ? OnValueChangedReverse : OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的alpha属性上
        /// </summary>
        public static IDisposable BindAlpha<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.alpha = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值以特定的规则绑定到UI的alpha属性上
        /// </summary>
        public static IDisposable BindAlpha<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.alpha = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Pos X]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的x属性上
        /// </summary>
        public static IDisposable BindX(this GObject gObject, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gObject.x = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的x属性上
        /// </summary>
        public static IDisposable BindX<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.x = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的x属性上
        /// </summary>
        public static IDisposable BindX<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.x = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Pos Y]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的y属性上
        /// </summary>
        public static IDisposable BindY(this GObject gObject, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gObject.y = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的y属性上
        /// </summary>
        public static IDisposable BindY<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.y = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的y属性上
        /// </summary>
        public static IDisposable BindY<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.y = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [XY]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的xy属性上
        /// </summary>
        public static IDisposable BindXY(this GObject gObject, ObservableVariable<Vector2> variable)
        {
            void OnValueChanged(Vector2 _, Vector2 value)
            {
                gObject.xy = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的xy属性上
        /// </summary>
        public static IDisposable BindXY<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, Vector2> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.xy = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的xy属性上
        /// </summary>
        public static IDisposable BindXY<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, Vector2> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.xy = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Width]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的width属性上
        /// </summary>
        public static IDisposable BindWidth(this GObject gObject, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gObject.width = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的width属性上
        /// </summary>
        public static IDisposable BindWidth<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.width = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的width属性上
        /// </summary>
        public static IDisposable BindWidth<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.width = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Height]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的height属性上
        /// </summary>
        public static IDisposable BindHeight(this GObject gObject, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gObject.height = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的height属性上
        /// </summary>
        public static IDisposable BindHeight<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.height = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的height属性上
        /// </summary>
        public static IDisposable BindHeight<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.height = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Size]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的size属性上
        /// </summary>
        public static IDisposable BindSize(this GObject gObject, ObservableVariable<Vector2> variable)
        {
            void OnValueChanged(Vector2 _, Vector2 value)
            {
                gObject.size = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的size属性上
        /// </summary>
        public static IDisposable BindSize<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, Vector2> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.size = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的size属性上
        /// </summary>
        public static IDisposable BindSize<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, Vector2> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.size = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Rotation]

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的rotation属性上(角度制)
        /// </summary>
        public static IDisposable BindRotation(this GObject gObject, ObservableVariable<float> variable, bool isReverse = false)
        {
            void OnValueChanged(float _, float value)
            {
                gObject.rotation = value;
            }

            void OnValueChangedReverse(float _, float value)
            {
                gObject.rotation = 360f - value;
            }

            return variable.SubscribeAsDisposable(isReverse ? OnValueChangedReverse : OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的rotation属性上(角度制)
        /// </summary>
        public static IDisposable BindRotation<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gObject.rotation = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为UI绑定一个变量 将变量的值绑定到UI的rotation属性上(角度制)
        /// </summary>
        public static IDisposable BindRotation<T>(this GObject gObject, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gObject.rotation = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [GProgressBar]

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值绑定到ProgressBar的value属性上
        /// </summary>
        public static IDisposable BindProgressValue(this GProgressBar gProgressBar, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gProgressBar.value = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值绑定到ProgressBar的value属性上
        /// </summary>
        public static IDisposable BindProgressValue(this GProgressBar gProgressBar, ObservableVariable<double> variable)
        {
            void OnValueChanged(double _, double value)
            {
                gProgressBar.value = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的value属性上
        /// </summary>
        public static IDisposable BindProgressValue<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, double> func)
        {
            void OnValueChanged(T _, T value)
            {
                gProgressBar.value = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的value属性上
        /// </summary>
        public static IDisposable BindProgressValue<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, T, double> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gProgressBar.value = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的value属性上 并且在变化时进行缓动
        /// </summary>
        public static IDisposable BindProgressTweenValue(this GProgressBar gProgressBar, ObservableVariable<float> variable, float duration)
        {
            void OnValueChanged(float _, float value)
            {
                gProgressBar.TweenValue(value, duration);
            }

            gProgressBar.value = variable.Value;

            return variable.SubscribeAsDisposable(OnValueChanged);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的value属性上 并且在变化时进行缓动
        /// </summary>
        public static IDisposable BindProgressTweenValue(this GProgressBar gProgressBar, ObservableVariable<double> variable, float duration)
        {
            void OnValueChanged(double _, double value)
            {
                gProgressBar.TweenValue(value, duration);
            }

            gProgressBar.value = variable.Value;

            return variable.SubscribeAsDisposable(OnValueChanged);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的value属性上 并且在变化时进行缓动
        /// </summary>
        public static IDisposable BindProgressTweenValue<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, double> func, float duration)
        {
            void OnValueChanged(T _, T value)
            {
                gProgressBar.TweenValue(func(value), duration);
            }

            gProgressBar.value = func(variable.Value);

            return variable.SubscribeAsDisposable(OnValueChanged);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的value属性上 并且在变化时进行缓动
        /// </summary>
        public static IDisposable BindProgressTweenValue<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, T, double> func, float duration)
        {
            void OnValueChanged(T pre, T value)
            {
                gProgressBar.TweenValue(func(pre, value), duration);
            }

            gProgressBar.value = func(variable.Value, variable.Value);

            return variable.SubscribeAsDisposable(OnValueChanged);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值绑定到ProgressBar的min属性上
        /// </summary>
        public static IDisposable BindProgressMin(this GProgressBar gProgressBar, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gProgressBar.min = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值绑定到ProgressBar的min属性上
        /// </summary>
        public static IDisposable BindProgressMin(this GProgressBar gProgressBar, ObservableVariable<double> variable)
        {
            void OnValueChanged(double _, double value)
            {
                gProgressBar.min = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的min属性上
        /// </summary>
        public static IDisposable BindProgressMin<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, double> func)
        {
            void OnValueChanged(T _, T value)
            {
                gProgressBar.min = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的min属性上
        /// </summary>
        public static IDisposable BindProgressMin<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, T, double> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gProgressBar.min = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值绑定到ProgressBar的max属性上
        /// </summary>
        public static IDisposable BindProgressMax(this GProgressBar gProgressBar, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gProgressBar.max = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值绑定到ProgressBar的max属性上
        /// </summary>
        public static IDisposable BindProgressMax(this GProgressBar gProgressBar, ObservableVariable<double> variable)
        {
            void OnValueChanged(double _, double value)
            {
                gProgressBar.max = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的max属性上
        /// </summary>
        public static IDisposable BindProgressMax<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, double> func)
        {
            void OnValueChanged(T _, T value)
            {
                gProgressBar.max = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为ProgressBar绑定一个变量 将变量的值以特定的规则绑定到ProgressBar的max属性上
        /// </summary>
        public static IDisposable BindProgressMax<T>(this GProgressBar gProgressBar, ObservableVariable<T> variable, Func<T, T, double> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gProgressBar.max = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Slider]

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值绑定到Slider的value属性上
        /// </summary>
        public static IDisposable BindSliderValue(this GSlider gSlider, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gSlider.value = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值以特定的规则绑定到Slider的value属性上
        /// </summary>
        public static IDisposable BindSliderValue<T>(this GSlider gSlider, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gSlider.value = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值以特定的规则绑定到Slider的value属性上
        /// </summary>
        public static IDisposable BindSliderValue<T>(this GSlider gSlider, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gSlider.value = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值绑定到Slider的min属性上
        /// </summary>
        public static IDisposable BindSliderMin(this GSlider gSlider, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gSlider.min = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值以特定的规则绑定到Slider的min属性上
        /// </summary>
        public static IDisposable BindSliderMin<T>(this GSlider gSlider, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gSlider.min = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值以特定的规则绑定到Slider的min属性上
        /// </summary>
        public static IDisposable BindSliderMin<T>(this GSlider gSlider, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gSlider.min = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值绑定到Slider的max属性上
        /// </summary>
        public static IDisposable BindSliderMax(this GSlider gSlider, ObservableVariable<float> variable)
        {
            void OnValueChanged(float _, float value)
            {
                gSlider.max = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值以特定的规则绑定到Slider的max属性上
        /// </summary>
        public static IDisposable BindSliderMax<T>(this GSlider gSlider, ObservableVariable<T> variable, Func<T, float> func)
        {
            void OnValueChanged(T _, T value)
            {
                gSlider.max = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Slider绑定一个变量 将变量的值以特定的规则绑定到Slider的max属性上
        /// </summary>
        public static IDisposable BindSliderMax<T>(this GSlider gSlider, ObservableVariable<T> variable, Func<T, T, float> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gSlider.max = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [GButton]

        /// <summary>
        /// 为Button绑定一个变量 将变量的值绑定到Button的selected属性上
        /// </summary>
        public static IDisposable BindButtonSelected(this GButton gButton, ObservableVariable<bool> variable)
        {
            void OnValueChanged(bool _, bool value)
            {
                gButton.selected = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Button绑定一个变量 将变量的值绑定到Button的selected属性上
        /// </summary>
        public static IDisposable BindButtonSelected<T>(this GButton gButton, ObservableVariable<T> variable, Func<T, bool> func)
        {
            void OnValueChanged(T _, T value)
            {
                gButton.selected = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Button绑定一个变量 将变量的值绑定到Button的selected属性上
        /// </summary>
        public static IDisposable BindButtonSelected<T>(this GButton gButton, ObservableVariable<T> variable, Func<T, T, bool> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gButton.selected = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [Controller]

        /// <summary>
        /// 为Controller绑定一个变量 将变量的值绑定到Controller的selectedIndex属性上
        /// </summary>
        public static IDisposable BindControllerSelectedIndex(this Controller controller, ObservableVariable<int> variable)
        {
            void OnValueChanged(int _, int value)
            {
                controller.selectedIndex = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Controller绑定一个变量 将变量的值绑定到Controller的selectedIndex属性上
        /// </summary>
        public static IDisposable BindControllerSelectedIndex<T>(this Controller controller, ObservableVariable<T> variable, Func<T, int> func)
        {
            void OnValueChanged(T _, T value)
            {
                controller.selectedIndex = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Controller绑定一个变量 将变量的值绑定到Controller的selectedIndex属性上
        /// </summary>
        public static IDisposable BindControllerSelectedIndex<T>(this Controller controller, ObservableVariable<T> variable, Func<T, T, int> func)
        {
            void OnValueChanged(T pre, T value)
            {
                controller.selectedIndex = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Controller绑定一个变量 将变量的值绑定到Controller的selectedPage属性上
        /// </summary>
        public static IDisposable BindControllerSelectedPage(this Controller controller, ObservableVariable<string> variable)
        {
            void OnValueChanged(string _, string value)
            {
                controller.selectedPage = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Controller绑定一个变量 将变量的值绑定到Controller的selectedPage属性上
        /// </summary>
        public static IDisposable BindControllerSelectedPage<T>(this Controller controller, ObservableVariable<T> variable, Func<T, string> func)
        {
            void OnValueChanged(T _, T value)
            {
                controller.selectedPage = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        /// <summary>
        /// 为Controller绑定一个变量 将变量的值绑定到Controller的selectedPage属性上
        /// </summary>
        public static IDisposable BindControllerSelectedPage<T>(this Controller controller, ObservableVariable<T> variable, Func<T, T, string> func)
        {
            void OnValueChanged(T pre, T value)
            {
                controller.selectedPage = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [ComboBox]

        /// <summary>
        /// 为ComboBox绑定一个变量 将变量的值绑定到ComboBox的selectedIndex属性上
        /// </summary>
        public static IDisposable BindComboBoxSelectedIndex(this GComboBox gComboBox, ObservableVariable<int> variable)
        {
            void OnValueChanged(int _, int value)
            {
                gComboBox.selectedIndex = value;
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }
        
        /// <summary>
        /// 为ComboBox绑定一个变量 将变量的值绑定到ComboBox的selectedIndex属性上
        /// </summary>
        public static IDisposable BindComboBoxSelectedIndex<T>(this GComboBox gComboBox, ObservableVariable<T> variable, Func<T, int> func)
        {
            void OnValueChanged(T _, T value)
            {
                gComboBox.selectedIndex = func(value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }
        
        /// <summary>
        /// 为ComboBox绑定一个变量 将变量的值绑定到ComboBox的selectedIndex属性上
        /// </summary>
        public static IDisposable BindComboBoxSelectedIndex<T>(this GComboBox gComboBox, ObservableVariable<T> variable, Func<T, T, int> func)
        {
            void OnValueChanged(T pre, T value)
            {
                gComboBox.selectedIndex = func(pre, value);
            }

            return variable.SubscribeAsDisposable(OnValueChanged, true);
        }

        #endregion

        #region [GList]

        /// <summary>
        /// 为GList绑定一个ObservableList 将ObservableList的元素绑定到GList的数据源上
        /// </summary>
        public static IDisposable BindListElements<T>(this GList gList, ObservableList<T> observableList, Action<T, GObject> itemRenderer)
        {
            observableList.OnChanged += OnListChanged;
            gList.itemRenderer = OnItemRenderer;
            gList.numItems = observableList.Count;

            return ((Action)DisposeAction).AsDisposable();

            void DisposeAction()
            {
                observableList.OnChanged -= OnListChanged;
                gList.itemRenderer -= OnItemRenderer;
            }

            // 列表元素绘制回调
            void OnItemRenderer(int index, GObject obj)
            {
                if (index < 0 || index >= observableList.Count)
                    return;
                
                itemRenderer(observableList[index], obj);
            }

            // 列表更新回调
            void OnListChanged()
            {
                gList.numItems = observableList.Count;
            }
        }

        #endregion
    }
}