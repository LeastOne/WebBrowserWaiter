// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebBrowserWaiterTests.cs" company="WebBrowserWaiter">
//   Copyright © 2014 WebBrowserWaiter. All rights reserved.
// </copyright>
// <summary>
//   The web browser waiter tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WebBrowserWaiter.Tests
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nancy.Hosting.Self;

    using global::WebBrowserWaiter.Tests.Infrastructure;

    /// <summary>
    /// The web browser waiter tests.
    /// </summary>
    public class WebBrowserWaiterTests
    {
        /// <summary>
        /// The await tests.
        /// </summary>
        [TestClass]
        public class AwaitTests
        {
            /// <summary>
            /// The uri.
            /// </summary>
            private static Uri uri;

            /// <summary>
            /// The host.
            /// </summary>
            private static NancyHost host;

            #region Public Methods and Operators

            /// <summary>
            /// The initialize.
            /// </summary>
            /// <param name="context">
            /// The context.
            /// </param>
            [ClassInitialize]
            public static void Initialize(TestContext context)
            {
                host = new NancyHost(
                    new HostConfiguration {
                        UrlReservations = new UrlReservations {
                            CreateAutomatically = true
                        }
                    },
                    uri = new Uri(
                        string.Format(
                            "http://localhost:{0}/web-browser-waiter-tests/",
                            PortHelper.GetOrCreateCachedPort()
                        )
                    )
                );

                host.Start();
            }

            /// <summary>
            /// The cleanup.
            /// </summary>
            [ClassCleanup]
            public static void Cleanup()
            {
                host.Stop();
            }

            /// <summary>
            /// The assert navigate.
            /// </summary>
            [TestMethod]
            public void AssertNavigate()
            {
                using (var waiter = new WebBrowserWaiter())
                {
                    string text = null;

                    waiter.Await(
                        p => p.Navigate(uri + "greet/World"),
                        p => text = p.DocumentText
                    );

                    Assert.IsTrue(
                        text.Contains("Hello World")
                    );
                }
            }

            /// <summary>
            /// The assert redirect followed.
            /// </summary>
            [TestMethod]
            public void AssertRedirectFollowed()
            {
                using (var waiter = new WebBrowserWaiter())
                {
                    var url = waiter.Await(
                        p => { p.Navigate(uri + "redirect"); return null; },
                        p => p.Url.ToString()
                    ).Last();

                    Assert.AreEqual(uri + "landing", url);
                }
            }

            /// <summary>
            /// The assert get then post.
            /// </summary>
            [TestMethod]
            public void AssertGetThenSubmit()
            {
                var search = Guid.NewGuid().ToString();

                using (var waiter = new WebBrowserWaiter())
                {
                    waiter.Await(
                        p => p.Navigate(uri + "search"), 
                        p => {
                            // ReSharper disable PossibleNullReferenceException
                            p.Document.All["search"].SetAttribute("value", search);
                            p.Document.All.Cast<HtmlElement>().First(q => q.TagName == "FORM").InvokeMember("Submit");
                            // ReSharper restore PossibleNullReferenceException
                        }
                    );

                    var text = waiter.Await(
                        p => p.DocumentText
                    );

                    Assert.IsTrue(
                        text.Contains(search)
                    );
                }
            }

            #endregion
        }
    }
}