using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Collections;
using com.echo.EchoOne.Web;

namespace com.echo.EchoOne.WebService
{
    /// <summary>
    /// EchoOneWS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class EchoOneWS : System.Web.Services.WebService
    {
        private EchoOneDataContext cont;

        public EchoOneWS()
        {
            cont = new EchoOneDataContext();
        }

        /// <summary>
        /// 取得角色列表
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public ArrayList GetRoles()
        {
            ArrayList list = new ArrayList();
            var roles = from c in cont.Role
                          select new { c.RoleId, c.RoleName };

            foreach (var item in roles)
            {
                list.Add(item.RoleName);
            }
            return list;
        }

        /// <summary>
        /// 取得某角色的所有用户
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [WebMethod]
        public ArrayList GetUsersByRole(string roleName)
        {
            ArrayList list = new ArrayList();
            
            var userInRole = from c in cont.UsersInRole
                             where c.Role.RoleName == roleName
                             select new { c.User};
            foreach (var u in userInRole)
            {
                list.Add(u.User.UserName);
            }
            return list;
        }

        /// <summary>
        /// 取得部门列表
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public ArrayList GetDepts()
        {
            ArrayList list = new ArrayList();
            var depts = from c in cont.Dept
                        select c;

            foreach (var d in depts)
            {
                list.Add(d.DeptName);
            }
            return list;
        }

        /// <summary>
        /// 取得部门用户列表
        /// </summary>
        /// <param name="deptName"></param>
        /// <returns></returns>
        [WebMethod]
        public ArrayList GetUserByDept(string deptName)
        {
            ArrayList list = new ArrayList();
            var users = from c in cont.User
                             where c.UserInDept.Dept.DeptName == deptName
                             select c;
            foreach (var u in users)
            {
                list.Add(u.UserName);
            }
            return list;
        }

        //验证用户
        [WebMethod]
        public Boolean ValidUser(string userName, string password)
        {
            var user = cont.User.Single(x => x.UserName == userName);
            return user.Membership.Password == password;
        }

        //取得用户对象
        [WebMethod]
        public User GetUserByName(string userName)
        {
            var user = cont.User.Single(x => x.UserName == userName);
            return user;
        }

        //取得功能分类
        [WebMethod]
        public List<Function> GetFunctionByUserId(Guid userId)
        {
            var roles = from f in cont.UsersInRole
                     where f.UserId == userId
                     select new { f.Role };

            List<Function> l = new List<Function>();

            foreach (var item in roles)
	        {
                var func = from f in cont.FuncInRole
                           where f.RoleId == item.Role.RoleId
                           orderby f.Function.FuncType1.Id
                           select new { f.Function };
                foreach (var f in func)
                {
                    l.Add(f.Function);
                }
	        }
            return l;
        }
    }
}
