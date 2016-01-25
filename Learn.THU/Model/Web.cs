using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Learn.THU.Model
{
    /// <summary>
    /// 网络组件，用于访问网络学堂获取信息
    /// </summary>
    class Web
    {
        CookieContainer cc;

        /// <summary>
        /// 登录旧版网络学堂，获取Cookie
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <param name="userPwd">密码</param>
        /// <returns>登录结果</returns>
        public LoginResult LoginOld(string userId, string userPwd)
        {

            return LoginResult.Success;
        }

        /// <summary>
        /// 登录新版网络学堂，获取Cookie
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <param name="userPwd">密码</param>
        /// <returns>登录结果</returns>
        public LoginResult LoginNew(string userId, string userPwd)
        {

            return LoginResult.Success;
        }

        public enum LoginResult
        {
            Success, Failed, Error
        }
    }
}
