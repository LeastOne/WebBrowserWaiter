// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Module.cs" company="WebBrowserWaiter">
//   Copyright © 2014 WebBrowserWaiter. All rights reserved.
// </copyright>
// <summary>
//   The module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WebBrowserWaiter.Tests.Infrastructure.Nancy
{
    using global::Nancy;

    /// <summary>
    /// The module.
    /// </summary>
    public class Module : NancyModule
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        public Module()
        {
            this.Get["/greet/{name}"] = p => string.Concat("Hello ", p.name);

            this.Get["/redirect"] = p => this.Response.AsRedirect("./landing");

            this.Get["/landing"] = p => null;

            this.Get["/search"] = p => this.View["Search"];

            this.Get["/results"] = p => this.Request.Query.search;
        }

        #endregion
    }
}