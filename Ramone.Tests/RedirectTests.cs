﻿using NUnit.Framework;
using Ramone.Tests.Common;


namespace Ramone.Tests
{
  [TestFixture]
  public class RedirectTests : TestHelper
  {
    protected override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();
      TestService.CodecManager.AddFormUrlEncoded<RedirectArgs>();
    }

    static object[] ValidRedirectCases = 
    {
      new object[] { 301, "GET" },
      new object[] { 302, "GET" },
      new object[] { 303, "GET" },
      new object[] { 307, "GET" },
      new object[] { 301, "HEAD" },
      new object[] { 302, "HEAD" },
      new object[] { 303, "HEAD" },
      new object[] { 307, "HEAD" },
      new object[] { 303, "POST" },
      new object[] { 303, "PUT" }
    };


    static object[] InvalidRedirectCases = 
    {
      new object[] { 301, "POST" },
      new object[] { 302, "POST" },
      new object[] { 307, "POST" },
      new object[] { 301, "PUT" },
      new object[] { 302, "PUT" },
      new object[] { 307, "PUT" }
    };


    [Test, TestCaseSource("ValidRedirectCases")]
    public void ItFollowsRedirectOnValidMethodsAndStatuses(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });

      // Act
      using (Response<RedirectArgs> resp = req.Execute<RedirectArgs>(method))
      {
        // Assert
        Assert.AreEqual(4, resp.RedirectCount);
      }
    }


    [Test, TestCaseSource("ValidRedirectCases")]
    public void ItFollowsRedirectOnValidMethodsAndStatuses_AsyncEvent(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });

      // Act
      TestAsyncEvent(wh =>
        {
          req.AsyncEvent().Execute<RedirectArgs>(method, response =>
            {
              // Assert
              Assert.AreEqual(4, response.RedirectCount);
              wh.Set();
            });
        });
    }


    [Test]
    public void WhenRedirectingItOnlyCallsOnCompleteOnce_AsyncEvent()
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = 301, count = 1, v = 0 });
      int onCompleteCount = 0;

      // Act
      req.AsyncEvent().OnComplete(() =>
          {
            ++onCompleteCount;
          })
          .Get<RedirectArgs>(response => {});

      System.Threading.Thread.Sleep(3000);

      // Asert
      Assert.AreEqual(1, onCompleteCount);
    }


    [Test, TestCaseSource("InvalidRedirectCases")]
    public void ItDoesNotFollowRedirectOnInvalidMethodsAndStatuses(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });

      // Act
      using (Response<RedirectArgs> resp = req.Execute<RedirectArgs>(method))
      {
        // Assert
        Assert.AreEqual(0, resp.RedirectCount);
      }
    }


    [Test, TestCaseSource("InvalidRedirectCases")]
    public void ItDoesNotFollowRedirectOnInvalidMethodsAndStatuses_AsyncEvent(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });

      // Act
      TestAsyncEvent(wh =>
        {
          req.AsyncEvent().Execute<RedirectArgs>(method, response =>
            {
              // Assert
              Assert.AreEqual(0, response.RedirectCount);
              wh.Set();
            });
        });
    }


    static object[] AFewValidRedirectCases = 
    {
      new object[] { 301, "GET" },
      new object[] { 307, "HEAD" },
      new object[] { 301, "GET" },
      new object[] { 303, "PUT" },
      new object[] { 303, "POST" }
    };


    [Test, TestCaseSource("AFewValidRedirectCases")]
    public void RedirectCountCanBeSpecified(int responseCode, string method)
    {
      // Arrange
      Session.SetAllowedRedirects(responseCode, 2);
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });

      // Act
      using (Response<RedirectArgs> resp = req.Execute<RedirectArgs>(method))
      {
        // Assert
        Assert.AreEqual(2, resp.RedirectCount);
      }
    }


    [Test, TestCaseSource("AFewValidRedirectCases")]
    public void RedirectCountCanBeSpecified_AsyncEvent(int responseCode, string method)
    {
      // Arrange
      Session.SetAllowedRedirects(responseCode, 2);
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });

      // Act
      TestAsyncEvent(wh =>
      {
        req.AsyncEvent().Execute<RedirectArgs>(method, response =>
        {
          // Assert
          Assert.AreEqual(2, response.RedirectCount);
          wh.Set();
        });
      });
    }


    [Test, TestCaseSource("AFewValidRedirectCases")]
    public void WithRedirectCountSetToZeroItDoesNotFollowRedirects(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });
      Session.SetAllowedRedirects(responseCode, 0);

      // Act
      using (Response<RedirectArgs> resp = req.Execute<RedirectArgs>(method))
      {
        // Assert
        Assert.AreEqual(0, resp.RedirectCount);
        Assert.AreEqual(responseCode, (int)resp.WebResponse.StatusCode);
      }
    }


    [Test, TestCaseSource("AFewValidRedirectCases")]
    public void WithRedirectCountSetToZeroItDoesNotFollowEmptyRedirects(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 1 });
      Session.SetAllowedRedirects(responseCode, 0);

      // Act
      using (Response<RedirectArgs> resp = req.Execute<RedirectArgs>(method))
      {
        // Assert
        Assert.AreEqual(0, resp.RedirectCount);
        Assert.AreEqual(responseCode, (int)resp.WebResponse.StatusCode);
      }
    }


    [Test, TestCaseSource("AFewValidRedirectCases")]
    public void WithRedirectCountSetToZeroItDoesNotFollowRedirects_AsyncEvent(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });
      Session.SetAllowedRedirects(responseCode, 0);

      // Act
      TestAsyncEvent(wh =>
      {
        req.AsyncEvent().Execute<RedirectArgs>(method, response =>
        {
          // Assert
          Assert.AreEqual(0, response.RedirectCount);
          Assert.AreEqual(responseCode, (int)response.WebResponse.StatusCode);
          wh.Set();
        });
      });
    }


    [Test, TestCaseSource("AFewValidRedirectCases")]
    public void WhenFollowingRedirectsItAppliesRequestInterceptors(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });
      Session.SetAllowedRedirects(responseCode, 5);
      Session.RequestInterceptors.Add(new RequestInterceptor());
      InterceptorCount = 0;

      // Act
      using (Response<RedirectArgs> resp = req.Execute<RedirectArgs>(method))
      {
        // Assert
        Assert.AreEqual(5, InterceptorCount);
      }
    }


    [Test, TestCaseSource("AFewValidRedirectCases")]
    public void WhenFollowingRedirectsItAppliesRequestInterceptors_AsyncEvent(int responseCode, string method)
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = responseCode, count = 1, v = 0 });
      Session.SetAllowedRedirects(responseCode, 5);
      Session.RequestInterceptors.Add(new RequestInterceptor());
      InterceptorCount = 0;

      // Act
      TestAsyncEvent(wh =>
      {
        req.AsyncEvent().Execute<RedirectArgs>(method, response =>
        {
          // Assert
          Assert.AreEqual(5, InterceptorCount);
          wh.Set();
        });
      });
    }


    [Test]
    public void WhenNotFollowingARedirectItWillNotCrashDueToMissingMediaType()
    {
      // Arrange
      Request req = Session.Bind(RedirectTemplate, new { code = 303, count = -1, v = 0 });

      Session.SetAllowedRedirects(303, 0);

      // Act
      using (Response resp = req.Post())
      {
        // Assert
        Assert.AreEqual(0, resp.RedirectCount);
        Assert.IsNull(resp.ContentType);
      }
    }


    static int InterceptorCount;


    class RequestInterceptor : IRequestInterceptor
    {
      public void HeadersReady(RequestContext context)
      {
        ++InterceptorCount;
      }

      public void DataSent(RequestContext context)
      {
      }
    }

  }
}
