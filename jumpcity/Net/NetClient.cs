using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using Jumpcity.Utility;
using Jumpcity.Utility.Extend;

namespace Jumpcity.Net
{
    /// <summary>
    /// 当请求发生错误时可供执行的委托
    /// </summary>
    /// <param name="ex">错误对象</param>
    public delegate void NetClientError(Exception ex);

    /// <summary>
    /// 该类使用代码实现特定HTTP的提交
    /// </summary>
    [Serializable]
    public class NetClient
    {
        private Encoding _encoding = Encoding.UTF8;
        private string _method = HttpMethod.POST;

        /// <summary>
        /// 获取或设置要请求的URI
        /// </summary>
        public string RequestUir { get; set; }
      
        /// <summary>
        /// 获取或设置POST请求时提交的内容文本
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// 获取请求提交失败时返回的错误信息
        /// </summary>
        public string Error { get; protected set; }

        /// <summary>
        /// 获取或设置请求的类型，默认为POST提交
        /// </summary>
        public string Method 
        {
            get { return _method; }
            set { _method = value; }
        }
        
        /// <summary>
        /// 获取或设置提交内容的编码类型，默认为UTF8
        /// </summary>
        public Encoding Encoding 
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary>
        /// 提交请求发生错误时的处理方法
        /// </summary>
        public event NetClientError OnError = null;

        /// <summary>
        /// 提交请求，并返回服务器响应的内容
        /// </summary>
        /// <param name="requestUri">要请求的URI</param>
        /// <param name="method">设置请求的类型</param>
        /// <param name="encoding">设置提交内容的编码类型</param>
        /// <param name="content">设置POST请求时提交的内容文本</param>
        /// <returns>返回服务器响应的内容</returns>
        public virtual string Submit(string requestUri, string method = HttpMethod.GET, Encoding encoding = null, string content = null)
        {
            StreamReader reader = null;
            Stream writter = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            string result = string.Empty;
            encoding = (encoding == null ? Encoding.UTF8 : encoding);

            try
            {
                request = (HttpWebRequest)WebRequest.Create(new Uri(requestUri));
                request.Method = General.ToNullable(method, HttpMethod.GET);
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;

                if (method == HttpMethod.POST)
                {
                    content = General.ToNullable(content, DateTime.Now.Ticks.ToString());
                    byte[] buffer = encoding.GetBytes(content);                                   
                    request.ContentType = MimeType.UrlEncoded;
                    request.ContentLength = buffer.GetLongLength(0);
                    writter = request.GetRequestStream();
                    writter.Write(buffer, 0, buffer.Length);
                }

                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), encoding);
                if (reader != null)
                    result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (this.OnError == null)
                    this.OnError += _onError;
                this.OnError.Invoke(ex);
            }
            finally
            {
                if (writter != null)
                    writter.Close();
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }

            return result;
        }

        /// <summary>
        /// 提交POST请求并返回服务器响应的内容
        /// </summary>
        /// <param name="requestUri">要请求的URI</param>
        /// <param name="encoding">设置提交内容的编码类型</param>
        /// <param name="content">设置POST请求时提交的内容文本</param>
        /// <returns>返回服务器响应的内容</returns>
        public string Post(string requestUri, Encoding encoding, string content)
        {
            return Submit(requestUri, HttpMethod.POST, encoding, content);
        }
        
        /// <summary>
        /// 提交POST请求并返回服务器响应的内容
        /// </summary>
        /// <param name="requestUri">要请求的URI</param>
        /// <param name="content">设置POST请求时提交的内容文本</param>
        /// <returns>返回服务器响应的内容</returns>
        public string Post(string requestUri, string content)
        {
            return Post(requestUri, this.Encoding, content);
        }
        
        /// <summary>
        /// 提交POST请求并返回服务器响应的内容
        /// </summary>
        /// <param name="requestUri">要请求的URI</param>
        /// <returns>返回服务器响应的内容</returns>
        public string Post(string requestUri)
        {
            return Post(requestUri, this.Encoding, this.Content);
        }

        /// <summary>
        /// 提交POST请求并返回服务器响应的内容
        /// </summary>
        /// <returns>返回服务器响应的内容</returns>
        public string Post()
        {
            return Post(this.RequestUir, this.Encoding, this.Content);
        }

        /// <summary>
        /// 提交GET请求并返回服务器响应的内容
        /// </summary>
        /// <param name="requestUri">要请求的URI</param>
        /// <param name="parameters">提交时附加在URI上的参数列表，key=value形式的字符串</param>
        /// <returns>返回服务器响应的内容</returns>
        public string Get(string requestUri, params string[] parameters)
        {
            string p = string.Empty;
            int index = requestUri.LastIndexOf("?");
            
            if (!General.IsNullable(parameters))
            {
                foreach (string pa in parameters)
                {
                    if (General.IsQuery(pa))
                        p += "&" + pa;
                }

                p = p.TrimStart('&');

                if (index == -1)
                    p = p.Insert(0, "?");
            }

            requestUri += p;
            return Submit(requestUri, HttpMethod.GET);
        }

        /// <summary>
        /// 提交GET请求并返回服务器响应的内容
        /// </summary>
        /// <param name="requestUri">要请求的URI</param>
        /// <param name="parameters">提交时附加在URI上的参数列表</param>
        /// <returns>返回服务器响应的内容</returns>
        public string Get(string requestUri, NameValueCollection parameters)
        {
            List<string> paraList = new List<string>();
            if (!General.IsNullable(parameters))
            {
                foreach (string key in parameters.AllKeys)
                {
                    string p = key + "=" + parameters[key];
                    paraList.Add(p);
                }
            }

            return Get(requestUri, paraList.ToArray());
        }

        /// <summary>
        /// 提交GET请求并返回服务器响应的内容
        /// </summary>
        /// <param name="requestUri">要请求的URI</param>
        /// <param name="parameters">提交时附加在URI上的参数列表</param>
        /// <returns>返回服务器响应的内容</returns>
        public string Get(string requestUri, Dictionary<string, string> parameters)
        {
            List<string> paraList = new List<string>();
            if (!General.IsNullable(parameters))
            {
                foreach (KeyValuePair<string,string> pair in parameters)
                {
                    string p = pair.Key + "=" + pair.Value;
                    paraList.Add(p);
                }
            }

            return Get(requestUri, paraList.ToArray());
        }

        /// <summary>
        /// 提交GET请求并返回服务器响应的内容
        /// </summary>
        /// <param name="parameters">提交时附加在URI上的参数列表，key=value形式的字符串</param>
        /// <returns>返回服务器响应的内容</returns>
        public string Get(params string[] parameters)
        {
            return Get(this.RequestUir,parameters);
        }

        /// <summary>
        /// 提交请求出错时默认的错误处理方法
        /// </summary>
        /// <param name="ex">错误信息</param>
        private void _onError(Exception ex)
        {
            if(ex.InnerException!=null)
                this.Error = ex.InnerException.Message;
            else
                this.Error = ex.Message;
        }
    }
}
