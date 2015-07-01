using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jumpcity.SMS;

public partial class SiteIndex : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ZTSmsAdapter zt = new ZTSmsAdapter("yuecheng", "1qaz3edc%TGB", "676766");
            var result = zt.Send(new List<string> { "18321725663", "13916464377" }, "尊敬的用户，您的短信验证码是000000,该消息将在24小时后过期");

            Response.Write(result.Source);
        }
    }
}