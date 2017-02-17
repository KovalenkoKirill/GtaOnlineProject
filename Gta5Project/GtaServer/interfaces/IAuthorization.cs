using DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer.interfaces
{
    internal interface IAuthorization
    {
        /// <summary>
        /// Проверка логина пароля
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="pass">Пароль</param>
        /// <returns></returns>
        ServerPlayer Autorize(string login, string pass);
    }
}
