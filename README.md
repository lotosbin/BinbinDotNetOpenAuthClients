BinbinDotNetOpenAuthClients
===========================


支持列表
========
QQ
Weibo
Taobao


PM> Install-Package BinbinDotNetOpenAuth.AspNet


AccountController.cs

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            HttpContext.RewriteRequestWhenUseState();//添加




        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            HttpContext.RewriteRequestWhenUseState();//添加
            
            
            
            
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            OAuthWebSecurity.RegisterClient(new TaobaoClient("xxxxx", "xxxxxx"), "淘宝",
                new Dictionary<string, object>
                {
                    {"beta", "beta"}
                });
             OAuthWebSecurity.RegisterClient(new QQClient("xxxxx", "xxxxxx"), "QQ",
                new Dictionary<string, object>
                {
                    {"beta", "beta"}
                });    
             OAuthWebSecurity.RegisterClient(new WeiboClient("xxxxx", "xxxxxx"), "微博",
                new Dictionary<string, object>
                {
                    {"beta", "beta"}
                });    



