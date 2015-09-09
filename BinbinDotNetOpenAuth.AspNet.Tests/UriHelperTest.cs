using System.Collections.Specialized;
// <copyright file="UriHelperTest.cs" company="lotosbin">Copyright ? 2013</copyright>

using System;
using BinbinDotNetOpenAuth.AspNet.Clients;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [TestClass]
    [PexClass(typeof(UriHelper))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class UriHelperTest
    {
        [PexMethod]
        public Uri BuildUri(
            string baseUri,
            NameValueCollection queryParameters,
            string frangment
        )
        {
            Uri result = UriHelper.BuildUri("www.guangchi.net", new NameValueCollection {{"response_type", "code"}}, "webchat");
            return result;
            // TODO: add assertions to method UriHelperTest.BuildUri(String, NameValueCollection, String)
        }

        [TestMethod]
        public void builduritest()
        {
            Uri result = UriHelper.BuildUri("www.guangchi.net", new NameValueCollection { { "response_type", "code" } });
            Console.WriteLine(result);
        }
        [TestMethod]
        public void getserviceurltest()
        {
            var w = new WeixinClient("wx661a840ecedb554d", "163e891e7b7e144ff4b40bb0eed6b810");
            var result=w.GetServiceLoginUrlTest(new Uri("http://weixinjs.guangchi.net/wx"));
            Console.WriteLine(result);
        }
        [TestMethod]
        public void QueryTokenTest()
        {
            var w = new WeixinClient("wx661a840ecedb554d", "163e891e7b7e144ff4b40bb0eed6b810");
            var result = w.QueryAccessTokenTest(new Uri("http://weixinjs.guangchi.net/wx"), "00122e41b7e538f1fdf6a699f0e218dr");
            
            Console.WriteLine(result);
            
              
        }

    }
}
