using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jumpcity.Net;
using Jumpcity.Utility.Extend;
using Jumpcity.Configuration;
using Jumpcity.Configuration.SMS;

namespace Jumpcity.SMS
{
    public class SmsAdapter
    {
        #region 成员变量...

        private SmsSettings _settings = null;
        private SmsSendFunction _sendFunction = null;
        private string _host = null;
        private string _username = null;
        private string _password = null;
        private Encoding _encoding = null;
        private int _maxMobileLength = 50;
        private int _maxContentLength = 140;
        private string _sign = string.Empty;

        #endregion 成员变量...

        public SmsAdapter()
        {
            SmsSection section = ConfigurationManagers.SmsSection;

            if (section != null)
            {
                this._settings = section.Settings;
                this._sendFunction = section.SendFunction;

                if (_settings != null)
                {
                    _encoding = _settings.ContentEncoding;
                    if (_settings.MaxMobileLength > 0)
                        _maxMobileLength = _settings.MaxMobileLength;
                    if (_settings.MaxContentLength > 0)
                        _maxContentLength = _settings.MaxContentLength;
                }
            }
        }

        #region 成员属性...

        public SmsSettings Settings
        {
            get { return _settings; }
        }

        public SmsSendFunction SendFunction
        {
            get { return _sendFunction; }
        }

        public string Host
        {
            get 
            {
                if (General.IsNullable(_host))
                {
                    if(_settings != null)
                        _host = _settings.Host;
                }
                return _host;
            }
            set { _host = value; }
        }

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public Encoding ContentEncoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public int MaxMobileLength
        {
            get { return _maxMobileLength; }
            set { _maxMobileLength = value; }
        }

        public int MaxContentLength
        {
            get { return _maxContentLength; }
            set { _maxContentLength = value; }
        }

        public string Sign
        {
            get
            {
                if (General.IsNullable(_sign))
                {
                    if (_settings.Sign != null)
                        _sign = _settings.Sign;
                }
                return _sign;
            }
            set { _sign = value; }
        }

        #endregion 成员属性...

        #region 成员方法...

        protected virtual string EncodeContent(string content)
        {
            if (!General.IsNullable(content))
            {
                content = content.Trim();
                int maxLength = this.MaxContentLength;
                string sign = this.Sign;

                if (content.Length > maxLength)
                    content = content.Remove(maxLength - sign.Length - 1);

                if (!General.IsNullable(sign))
                    content += sign;

                return HttpUtility.UrlEncode(content, this.ContentEncoding);
            }

            return null;
        }

        protected virtual List<string> FilterNumbers(List<string> numbers)
        {
            if (!General.IsNullable(numbers))
                numbers = numbers.FindAll(n => General.IsMobile(n)).Take(this.MaxMobileLength).ToList();
            return numbers;
        }

        public virtual string Send(List<string> mobileNumbers, string content, string userName, string password, Dictionary<string, string> otherParameters = null)
        {
            mobileNumbers = this.FilterNumbers(mobileNumbers);
            content = this.EncodeContent(content);
            
            if (
                General.IsNullable(mobileNumbers)
             || General.IsNullable(content)
             || General.IsNullable(userName)
             || General.IsNullable(password)
            )
                return null;

            NetClient cilent = new NetClient();
            string method = this._sendFunction.Method;
            string requestUri = this.Host + this._sendFunction.FunctionName;
            SmsSendFunction.SmsRequestParameterCollection parameters = this._sendFunction.Parameters;

            string request = string.Format(
                "{0}={1}&{2}={3}&{4}={5}&{6}={7}",
                this._sendFunction.UserNameParameterName,
                userName,
                this._sendFunction.PasswordParameterName,
                password,
                this._sendFunction.MobileParameterName,
                string.Join(",", mobileNumbers),
                this._sendFunction.ContentParameterName,
                content
            );

            if (!General.IsNullable(parameters))
            {
                bool newValue = !General.IsNullable(otherParameters);

                for (int i = 0, c = parameters.Count; i < c; i++)
                {
                    SmsRequestParameter p = parameters[i];
                    string name = p.Name;
                    string value = p.DefaultValue;
                    if (newValue && otherParameters.ContainsKey(name))
                        value = otherParameters[name];

                    if (!General.IsNullable(value))
                        request += string.Format("&{0}={1}", name, value);
                }
            }

            return cilent.Submit(requestUri, method, this.ContentEncoding, request);
        }
        public string Send(List<string> mobileNumbers, string content, Dictionary<string, string> otherParameters = null)
        {
            return Send(mobileNumbers, content, this.UserName, this.Password, otherParameters);
        }
        public string Send(string mobileNumber, string content, string userName, string password, Dictionary<string, string> otherParameters = null)
        {
            return Send(new List<string> { mobileNumber }, content, userName, password, otherParameters);
        }
        public string Send(string mobileNumber, string content, Dictionary<string, string> otherParameters = null)
        {
            return Send(mobileNumber, content, this.UserName, this.Password, otherParameters);
        }

        #endregion 成员方法...
    }
}
