BinbinDotNetOpenAuthClients
===========================
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



