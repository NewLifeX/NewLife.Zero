using System;
using System.Collections.Concurrent;
using NewLife;
using Zero.Core.WeiXin;
using Zero.Data.Projects;

namespace Zero.Data.Common
{
    /// <summary>机器人助手</summary>
    public static class RobotHelper
    {
        #region 根据应用创建机器人
        private static readonly ConcurrentDictionary<Int32, Robot> _robots = new();
        /// <summary>根据团队创建机器人</summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static Robot CreateRobot(ITeam team)
        {
            if (team.WebHook.IsNullOrEmpty()) return null;

            if (_robots.TryGetValue(team.ID, out var robot)) return robot;

            robot = new Robot { Url = team.WebHook };

            return _robots.GetOrAdd(team.ID, robot);
        }
        #endregion
    }
}