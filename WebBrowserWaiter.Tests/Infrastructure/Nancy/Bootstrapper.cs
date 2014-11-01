// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="WebBrowserWaiter">
//   Copyright © 2014 WebBrowserWaiter. All rights reserved.
// </copyright>
// <summary>
//   The bootstrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WebBrowserWaiter.Tests.Infrastructure.Nancy
{
    using System.Reflection;

    using global::Nancy;
    using global::Nancy.Bootstrapper;
    using global::Nancy.TinyIoc;
    using global::Nancy.ViewEngines;

    /// <summary>
    /// The bootstrapper.
    /// </summary>
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        #region Methods

        /// <summary>
        /// Gets the internal configuration.
        /// </summary>
        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(
                    x => x.ViewLocationProvider = typeof(ResourceViewLocationProvider)
                );
            }
        }

        /// <summary>
        /// The configure application container.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            StaticConfiguration.DisableErrorTraces = false;

            ResourceViewLocationProvider.RootNamespaces.Add(
                Assembly.GetExecutingAssembly(),
                "WebBrowserWaiter.Tests.Infrastructure.Nancy.Views"
            );
        }

        #endregion
    }
}