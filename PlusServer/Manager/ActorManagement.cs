using System;
using System.Linq;
using System.Collections.Concurrent;
using Messages;
using PlusGame.Config;
using PlusServerCommon.Base;
using Proto;

namespace PlusServer.Manager
{
    public static class ActorManagement
    {
        /// <summary>
        /// 第一个Key存类型，第二个key为唯一标识
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, PID>> _globalPids =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, PID>>();

        private static PID _locationPid;

        public static int GetPidCount()
        {
            int count = 0;
            foreach (var item in _globalPids.Values)
            {
                count += item.Values.Count();
            }
            return count;
        }

        public static string GetPidTypeNames()
        {
            return string.Join(",", _globalPids.Keys);
        }

        public static void SetLocationPid(PID locationPid)
        {
            _locationPid = locationPid;
        }

        public static PID GetLocationPid()
        {
            return _locationPid;
        }

        #region 公共方法
        public static bool TryRemovePid<T>(params object[] keys)
        {
            string name = GetTypeName<T>();
            string key = GetKey<T>(keys);
            return TryRemovePid(name, key);
        }

        public static bool TryRemovePid(string actorName)
        {
            string name = string.Empty;
            string key = string.Empty;
            if (TryGetActorKeys(actorName, out name, out key))
            {
                return TryRemovePid(name, key);
            }
            return false;
        }

        public static bool TryRemovePid(string name, string key)
        {
            PID pid;
            ConcurrentDictionary<string, PID> tDictionary;
            if (_globalPids.TryGetValue(name, out tDictionary))
            {
                return tDictionary.TryRemove(key, out pid);
            }
            return false;
        }

        public static bool TryAddPid<T>(PID pid, params object[] keys)
        {
            string name = GetTypeName<T>();
            string key = GetKey<T>(keys);
            return TryAddPid(pid, name, key);
        }

        public static bool TryAddPid(PID pid, string actorName)
        {
            string name = string.Empty;
            string key = string.Empty;
            if (TryGetActorKeys(actorName, out name, out key))
            {
                return TryAddPid(pid, name, key);
            }
            return false;
        }

        public static bool TryAddPid(PID pid, string name, string key)
        {
            ConcurrentDictionary<string, PID> tDictionary;
            if (_globalPids.TryGetValue(name, out tDictionary))
            {
                tDictionary.AddOrUpdate(key, pid, (m, n) => pid);
                return true;
            }
            else
            {
                tDictionary = new ConcurrentDictionary<string, PID>();
                if (tDictionary.TryAdd(key, pid) && _globalPids.TryAdd(name, tDictionary))
                {
                    return true;
                }
            }
            return false;
        }

        public static PID TryGetPid<T>(params object[] keys)
        {
            string name = GetTypeName<T>();
            string key = GetKey<T>(keys);
            return TryGetPid(name, key);
        }

        public static PID TryGetPid(string actorName)
        {
            string name = string.Empty;
            string key = string.Empty;
            if (TryGetActorKeys(actorName, out name, out key))
            {
                return TryGetPid(name, key);
            }
            return null;
        }

        public static PID TryGetPid(string name, string key)
        {
            PID pid = null;
            ConcurrentDictionary<string, PID> tDictionary;
            if (_globalPids.TryGetValue(name, out tDictionary) && tDictionary.TryGetValue(key, out pid))
            {
                return pid;
            }
            return null;
        }

        private static string GetTypeName<T>()
        {
            return typeof(T).Name;
        }

        private static string GetKey<T>(params object[] keys)
        {
            string value = string.Empty;
            if (typeof(IPersonalActor).IsAssignableFrom(typeof(T)) && keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    if (value.Length > 0)
                    {
                        value += "_";
                    }
                    value += key;
                }
            }
            else if (typeof(IShareActor).IsAssignableFrom(typeof(T)) && keys.Length == 2)
            {
                value = string.Format("{0}_{1}", keys[0], keys[1]);
            }
            else if (typeof(IShareActor).IsAssignableFrom(typeof(T)) && keys.Length == 0)
            {
                value = string.Format("{0}_{1}", GameEnvironment.AppConfig.GameId, GameEnvironment.AppConfig.ServerId);
            }

            return value;
        }

        public static string GetActorName<T>(params object[] keys)
        {
            return string.Format("{0}_{1}", GetTypeName<T>(), GetKey<T>(keys));
        }

        public static string GetActorName(string name, string key)
        {
            if (string.IsNullOrEmpty(key))
                return name;
            return string.Format("{0}_{1}", name, key);
        }

        public static bool TryGetActorKeys(string actorName, out string name, out string key)
        {
            name = string.Empty;
            key = string.Empty;
            string[] strs = actorName.Split('_');
            if (strs.Length > 0)
            {
                name = strs[0];
                key = actorName.Substring(strs[0].Length + 1);
                return true;
            }
            return false;
        }

        #endregion

        #region 与位置服务器交互

        public static PID GetOrLoadPid<T>(params object[] keys)
        {
            string name = GetTypeName<T>();
            string key = GetKey<T>(keys);
            return GetOrLoadPid(name, key);
        }

        public static PID GetOrLoadPid(string actorName)
        {
            string name;
            string key;
            if (TryGetActorKeys(actorName, out name, out key))
            {
                GetOrLoadPid(name, key);
            }
            return null;
        }

        public static PID GetOrLoadPid(string name, string key)
        {
            ConcurrentDictionary<string, PID> tDictionary;
            PID pid = null;
            if (_globalPids.TryGetValue(name, out tDictionary) && tDictionary.TryGetValue(name, out pid))
            {
                return pid;
            }

            if (pid == null)
            {
                var response = _locationPid.RequestAsync<ResponsePid>(new RequestPid() { Name = GetActorName(name, key) });
                pid = response.Result.Sender;
                if (pid != null)
                {
                    TryAddPid(pid, name, key);
                    return pid;
                }
            }
            return null;
        }

        public static void RegisterPid<T>(PID pid, params object[] keys)
        {
            string name = GetTypeName<T>();
            string key = GetKey<T>(keys);

            if (TryAddPid(pid, name, key))
            {
                _locationPid.RequestAsync<Done>(new RegisterPid() { Name = GetActorName(name, key), Sender = pid }).Wait();
            }
        }

        public static void UnRegisterPid<T>(params object[] keys)
        {
            string name = GetTypeName<T>();
            string key = GetKey<T>(keys);
            PID pid;
            ConcurrentDictionary<string, PID> tDictionary;
            if (_globalPids.TryGetValue(name, out tDictionary) && tDictionary.TryRemove(key, out pid))
            {
                _locationPid.RequestAsync<Done>(new UnRegisterPid() { Name = GetActorName(name, key) }).Wait();
            }
        }

        #endregion
    }
}
