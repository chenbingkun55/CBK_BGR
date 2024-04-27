using System;
using System.Collections.Generic;
using CBK.Framework.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CBK.Framework.Update
{
    /// <summary>
    /// 帧更新服务实现
    /// </summary>
    internal sealed partial class UpdateServiceImpl : IInitialize, IDisposable, IUpdateService
    {
        private Driver _driver;

        public void Initialize()
        {
            _driver = new GameObject("Update Driver").AddComponent<Driver>();
            _driver.OnUpdated += OnUpdated;
            _driver.OnLateUpdated += OnLateUpdated;
            _driver.OnFixedUpdated += OnFixedUpdated;
            _driver.OnUpdated += OnExecuteDelayCallActions;
        }

        public void Dispose()
        {
            if (_driver == null)
                return;
            
            _driver.OnUpdated -= OnUpdated;
            _driver.OnLateUpdated -= OnLateUpdated;
            _driver.OnFixedUpdated -= OnFixedUpdated;
            _driver.OnUpdated -= OnExecuteDelayCallActions;
            Object.Destroy(_driver.gameObject);
        }

        public void RegisterUpdate(IUpdate update)
        {
            if (_updates.Contains(update))
                return;
            
            _updates.Add(update);
        }

        public void UnRegisterUpdate(IUpdate update)
        {
            if (!_updates.Remove(update))
                return;

            var index = _runningUpdates.IndexOf(update);
            if (index <= _runningUpdateIndex)
                return;

            _runningUpdates.RemoveAt(index);
        }

        public void RegisterLateUpdate(ILateUpdate update)
        {
            if (_lateUpdates.Contains(update))
                return;
            
            _lateUpdates.Add(update);
        }

        public void UnRegisterLateUpdate(ILateUpdate update)
        {
            if (!_lateUpdates.Remove(update))
                return;

            var index = _runningLateUpdates.IndexOf(update);
            if (index <= _runningLateUpdateIndex)
                return;

            _runningLateUpdates.RemoveAt(index);
        }

        public void RegisterFixedUpdate(IFixedUpdate update)
        {
            if (_fixedUpdates.Contains(update))
                return;
            
            _fixedUpdates.Add(update);
        }

        public void UnRegisterFixedUpdate(IFixedUpdate update)
        {
            if (!_fixedUpdates.Remove(update))
                return;

            var index = _runningFixedUpdates.IndexOf(update);
            if (index <= _runningFixedUpdateIndex)
                return;

            _runningFixedUpdates.RemoveAt(index);
        }

        public void RegisterDelayCall(Action action)
        {
            _delayCallActions.Remove(action);
            _delayCallActions.Add(action);
        }

        public void UnRegisterDelayCall(Action action)
        {
            _delayCallActions.Remove(action);
        }

        private void OnUpdated(float elapseSeconds, float realElapseSeconds)
        {
            _runningUpdateIndex = 0;
            _runningUpdates.Clear();
            
            if (_updates.Count == 0)
                return;
            
            _runningUpdates.AddRange(_updates);

            while (_runningUpdateIndex < _runningUpdates.Count)
            {
                try
                {
                    _runningUpdates[_runningUpdateIndex].OnUpdate(elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }

                ++_runningUpdateIndex;
            }
        }

        private void OnLateUpdated(float elapseSeconds, float realElapseSeconds)
        {
            _runningLateUpdateIndex = 0;
            _runningLateUpdates.Clear();
            
            if (_lateUpdates.Count == 0)
                return;
            
            _runningLateUpdates.AddRange(_lateUpdates);

            while (_runningLateUpdateIndex < _runningLateUpdates.Count)
            {
                try
                {
                    _runningLateUpdates[_runningLateUpdateIndex].OnLateUpdate(elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }

                ++_runningLateUpdateIndex;
            }
        }

        private void OnFixedUpdated(float elapseSeconds, float realElapseSeconds)
        {
            _runningFixedUpdateIndex = 0;
            _runningFixedUpdates.Clear();
            
            if (_fixedUpdates.Count == 0)
                return;
            
            _runningFixedUpdates.AddRange(_fixedUpdates);

            while (_runningFixedUpdateIndex < _runningFixedUpdates.Count)
            {
                try
                {
                    _runningFixedUpdates[_runningFixedUpdateIndex].OnFixedUpdate(elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }

                ++_runningFixedUpdateIndex;
            }
        }

        private void OnExecuteDelayCallActions(float _, float __)
        {
            if (_delayCallActions.Count == 0)
                return;
            
            _runningCallActions.Clear();
            _runningCallActions.AddRange(_delayCallActions);
            
            foreach (var action in _delayCallActions)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
        }

        private readonly List<IUpdate> _updates = new();
        private readonly List<ILateUpdate> _lateUpdates = new();
        private readonly List<IFixedUpdate> _fixedUpdates = new();

        private readonly List<IUpdate> _runningUpdates = new();
        private readonly List<ILateUpdate> _runningLateUpdates = new();
        private readonly List<IFixedUpdate> _runningFixedUpdates = new();

        private readonly List<Action> _delayCallActions = new();
        private readonly List<Action> _runningCallActions = new();

        private int _runningUpdateIndex;
        private int _runningLateUpdateIndex;
        private int _runningFixedUpdateIndex;
    }
}