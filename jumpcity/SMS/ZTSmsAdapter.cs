using System;
using System.Linq;
using System.Collections.Generic;
using Jumpcity.Utility.Extend;

namespace Jumpcity.SMS
{
    /// <summary>
    /// 该类是对助通短信平台的实现
    /// </summary>
    public class ZTSmsAdapter : SmsAdapter
    {
        /// <summary>
        /// 获取助通定义的短信内容分割符
        /// </summary>
        public string Split
        {
            get{return "※";}
        }

        /// <summary>
        /// 获取或设置助通的产品ID
        /// </summary>
        public string ProductId
        {
            get;
            set;
        }

        /// <summary>
        /// 创建助通短信平台操作类的实例
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="productId">产品ID</param>
        public ZTSmsAdapter(string username, string password, string productId)
        {
            this.UserName = username;
            this.Password = password;
            this.ProductId = productId;
        }

        /// <summary>
        /// 批量发送指定的短信内容
        /// </summary>
        /// <param name="mobileNumbers">接收短信内容的手机号码列表</param>
        /// <param name="content">要发送的短信内容</param>
        /// <param name="delay">设置发送延迟的时间(分钟)，零代表立即发送</param>
        /// <returns>返回服务器响应的结果对象</returns>
        public Result Send(List<string> mobileNumbers, string content, int delay = 0)
        {
            Dictionary<string, string> others = new Dictionary<string, string> { { "productid", this.ProductId } };
            if (delay > 0)
                others.Add("dstime", DateTime.Now.AddMinutes(delay).ToString("yyyyMMddHHmmss"));

            string source = base.Send(mobileNumbers, content, others);
            return Result.Parse(source);
        }

        /// <summary>
        /// 用于封装助通短信平台服务器返回的结果集
        /// </summary>
        [Serializable]
        public class Result
        {
            /// <summary>
            /// 获取或设置返回的状态ID
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 获取或设置返回的状态ID所代表的含义描述文本
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 获取或设置原始的返回内容
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// 获取或设置返回的消息编号(如果有的话)
            /// </summary>
            public string MessageId { get; set; }
            
            /// <summary>
            /// 创建返回结果集的实例
            /// </summary>
            /// <param name="source">服务器响应的原始内容</param>
            public Result(string source)
            {
                this.FromSource(source);
            }

            /// <summary>
            /// 创建返回结果集的实例
            /// </summary>
            public Result() { }

            /// <summary>
            /// 根据服务器响应的原始内容构造结果集对象
            /// </summary>
            /// <param name="source">服务器响应的原始内容</param>
            private void FromSource(string source)
            {
                if (!General.IsNullable(source))
                {
                    if (source.IndexOf(",") != -1)
                    {
                        string[] s = source.Split(',');
                        this.Id = s[0].ToInt32();
                        this.MessageId = s[1];
                    }
                    else
                        this.Id = source.ToInt32();

                    Result r = Preset.Where(p => p.Id == this.Id).FirstOrDefault();
                    if (r != null)
                        this.Description = r.Description;

                    this.Source = source;
                }
            }

            /// <summary>
            /// 助通平台预置的结果集列表
            /// </summary>
            public static readonly List<Result> Preset = new List<Result> { 
                new Result{ Id = -1, Description = "用户名或者密码不正确" },
                new Result{ Id = 0, Description = "发送失败" },
                new Result{ Id = 1, Description = "发送成功" },
                new Result{ Id = 2, Description = "余额不足" },
                new Result{ Id = 3, Description = "扣费失败" },
                new Result{ Id = 5, Description = "短信定时成功" },
                new Result{ Id = 6, Description = "有效号码为空" },
                new Result{ Id = 7, Description = "短信内容为空" },
                new Result{ Id = 8, Description = "无签名" },
                new Result{ Id = 9, Description = "没有Url提交权限" },
                new Result{ Id = 10, Description = "发送号码过多" },
                new Result{ Id = 11, Description = "产品ID异常" },
                new Result{ Id = 12, Description = "参数异常" },
                new Result{ Id = 13, Description = "重复提交" },
                new Result{ Id = 14, Description = "禁止提交" },
                new Result{ Id = 15, Description = "Ip验证失败" },
                new Result{ Id = 19, Description = "短信内容过长" },
                new Result{ Id = 20, Description = "定时时间格式不正确" }
            };

            /// <summary>
            /// 将服务器响应的原始内容转换为结果集对象
            /// </summary>
            /// <param name="source">服务器响应的原始内容</param>
            /// <returns>返回转换后的结果集对象</returns>
            public static Result Parse(string source)
            {
                return new Result(source);
            }
        }
    }
}
