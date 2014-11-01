// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortHelper.cs" company="WebBrowserWaiter">
//   Copyright © 2014 WebBrowserWaiter. All rights reserved.
// </copyright>
// <summary>
//   The port helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WebBrowserWaiter.Tests.Infrastructure
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;

    /// <summary>
    /// The port helper.
    /// </summary>
    public static class PortHelper
    {
        #region Constants

        /// <summary>
        /// The max port.
        /// </summary>
        private const int MaxPort = 60000;

        /// <summary>
        /// The min port.
        /// </summary>
        private const int MinPort = 50000;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get or create cached port.
        /// </summary>
        /// <param name="pathToPortCache">
        /// The path to port cache.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetOrCreateCachedPort(string pathToPortCache = ".port")
        {
            if (!File.Exists(pathToPortCache))
                File.WriteAllText(
                    pathToPortCache,
                    GetOpenPort().ToString()
                );

            return int.Parse(
                File.ReadAllText(pathToPortCache)
            );
        }

        /// <summary>
        /// The get open port.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetOpenPort()
        {
            // Get the used ports between min and max
            var used =
                IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpConnections()
                    .Select(p => p.LocalEndPoint.Port)
                    .Where(p => p >= MinPort && p <= MaxPort)
                    .ToArray();

            var random = new Random();

            // Try a finite number of times to find an open port
            for (var i = MaxPort - MinPort; i > 0; i--)
            {
                var port = random.Next(50000, 60000);
                if (!used.Contains(port))
                    return port;
            }

            throw new Exception("Could not find an open port.");
        }

        #endregion
    }
}