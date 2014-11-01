// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebBrowserWaiter.cs" company="WebBrowserWaiter">
//   Copyright © 2014 WebBrowserWaiter. All rights reserved.
// </copyright>
// <summary>
//   The web browser waiter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WebBrowserWaiter
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The web browser waiter.
    /// </summary>
    public class WebBrowserWaiter : IDisposable
    {
        #region Fields

        /// <summary>
        /// The default wait.
        /// </summary>
        private static TimeSpan defaultWait = TimeSpan.FromSeconds(2);

        /// <summary>
        /// The signal.
        /// </summary>
        private readonly ManualResetEvent signal;

        /// <summary>
        /// The browser.
        /// </summary>
        private WebBrowser browser;

        /// <summary>
        /// The form.
        /// </summary>
        private HeadlessForm form;

        /// <summary>
        /// The last completed.
        /// </summary>
        private DateTime? lastCompleted;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBrowserWaiter"/> class.
        /// </summary>
        public WebBrowserWaiter()
            // ReSharper disable once RedundantArgumentDefaultValue
            : this(visibility: false)
        {
            // NOTE: This constructor must provide at least one named argument to the other
            // constructor to avoid calling this constructor recursively.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBrowserWaiter"/> class.
        /// </summary>
        /// <param name="visibility">
        /// The visibility.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="top">
        /// The top.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        public WebBrowserWaiter(bool visibility = false, FormStartPosition position = FormStartPosition.CenterScreen, int width = -1, int height = -1, int top = 0, int left = 0)
        {
            this.signal = new ManualResetEvent(false);

            var thread = new Thread(
                () => {
                    this.browser = new WebBrowser {
                        Width = width < 0 ? Screen.PrimaryScreen.WorkingArea.Width * 3 / 4 : width, 
                        Height = height < 0 ? Screen.PrimaryScreen.WorkingArea.Height * 3 / 4 : height
                    };

                    this.browser.Navigating += (p, q) => this.lastCompleted = null;
                    this.browser.DocumentCompleted += (p, q) => this.lastCompleted = DateTime.UtcNow;

                    this.form = new HeadlessForm {
                        Width = this.browser.Width, 
                        Height = this.browser.Height, 
                        StartPosition = position, 
                        Top = top, 
                        Left = left
                    };

                    this.form.Controls.Add(this.browser);
                    this.form.HandleCreated += (p, q) => this.signal.Set();
                    this.form.InitialVisibility = visibility;
                    this.form.Visible = visibility;

                    Application.Run(this.form);
                }
            );

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            this.signal.WaitOne();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the default wait.
        /// </summary>
        public virtual TimeSpan DefaultWait
        {
            get { return defaultWait; }
            set { defaultWait = value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        public virtual void Await(Action<WebBrowser> order)
        {
            this.Await(
                this.DefaultWait,
                order
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        public virtual void Await(int wait, Action<WebBrowser> order)
        {
            this.Await(
                this.CreateWaitTimeSpan(wait),
                order
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        public virtual void Await(TimeSpan wait, Action<WebBrowser> order)
        {
            this.Await(
                wait,
                new[] { order }
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        public virtual void Await(params Action<WebBrowser>[] orders)
        {
            this.Await(
                this.DefaultWait,
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        public virtual void Await(int wait, params Action<WebBrowser>[] orders)
        {
            this.Await(
                this.CreateWaitTimeSpan(wait),
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="waits">
        /// The waits.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        public virtual void Await(int[] waits, params Action<WebBrowser>[] orders)
        {
            this.Await(
                waits.Select(this.CreateWaitTimeSpan).ToArray(),
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        public virtual void Await(TimeSpan wait, params Action<WebBrowser>[] orders)
        {
            this.Await(
                orders.Select(p => wait).ToArray(),
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="waits">
        /// The waits.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Throws ArgumentException if waits and orders differ in length.
        /// </exception>
        public virtual void Await(TimeSpan[] waits, params Action<WebBrowser>[] orders)
        {
            this.Await(
                waits,
                orders.Select(
                    (p, q) => (Func<WebBrowser, object>)(r => { p(r); return null; })
                ).ToArray()
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <typeparam name="T">The return type.</typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public virtual T Await<T>(Func<WebBrowser, T> order)
        {
            return this.Await(
                this.DefaultWait,
                order
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public virtual T Await<T>(int wait, Func<WebBrowser, T> order)
        {
            return this.Await(
                this.CreateWaitTimeSpan(wait),
                order
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public virtual T Await<T>(TimeSpan wait, Func<WebBrowser, T> order)
        {
            return this.Await(
                wait,
                new[] { order }
            ).First();
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <returns>
        /// The an array of the return type.
        /// </returns>
        public virtual T[] Await<T>(params Func<WebBrowser, T>[] orders)
        {
            return this.Await(
                this.DefaultWait,
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <returns>
        /// The an array of the return type.
        /// </returns>
        public virtual T[] Await<T>(int wait, params Func<WebBrowser, T>[] orders)
        {
            return this.Await(
                this.CreateWaitTimeSpan(wait),
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="waits">
        /// The waits.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <returns>
        /// The an array of the return type.
        /// </returns>
        public virtual T[] Await<T>(int[] waits, params Func<WebBrowser, T>[] orders)
        {
            return this.Await(
                waits.Select(this.CreateWaitTimeSpan).ToArray(),
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <returns>
        /// The an array of the return type.
        /// </returns>
        public virtual T[] Await<T>(TimeSpan wait, params Func<WebBrowser, T>[] orders)
        {
            return this.Await(
                orders.Select(p => wait).ToArray(),
                orders
            );
        }

        /// <summary>
        /// The await.
        /// </summary>
        /// <param name="waits">
        /// The waits.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <returns>
        /// The an array of the return type.
        /// </returns>
        public virtual T[] Await<T>(TimeSpan[] waits, params Func<WebBrowser, T>[] orders)
        {
            if (waits.Length != orders.Length)
                throw new ArgumentException("The waits and orders arguments must have the same length.");

            var results = new T[orders.Length];

            for (var i = 0; i < orders.Length; i++)
            {
                this.form.Execute(
                    () => results[i] = orders[i](this.browser)
                );

                while (true)
                {
                    if (!this.lastCompleted.HasValue)
                    {
                        Thread.Sleep(50);
                        continue;
                    }

                    var diff = this.lastCompleted.Value.Add(waits[i]) - DateTime.UtcNow;

                    if (diff.Ticks < 0)
                        break;

                    Thread.Sleep(diff);
                }
            }

            return results;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public virtual void Dispose()
        {
            this.form.Execute(
                () => this.form.Close()
            );
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create wait time span.
        /// </summary>
        /// <param name="wait">
        /// The wait.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        private TimeSpan CreateWaitTimeSpan(int wait)
        {
            return TimeSpan.FromMilliseconds(wait);
        }

        #endregion

        /// <summary>
        /// The headless form.
        /// </summary>
        private class HeadlessForm : Form
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets a value indicating whether initial visibility.
            /// </summary>
            public bool InitialVisibility { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The invoke.
            /// </summary>
            /// <param name="action">
            /// The action.
            /// </param>
            public virtual void Execute(Action action)
            {
                this.Invoke(action);
            }

            #endregion

            #region Methods

            /// <summary>
            /// The set visible core.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            protected override void SetVisibleCore(bool value)
            {
                if (!this.IsHandleCreated)
                    this.CreateHandle();

                base.SetVisibleCore(this.InitialVisibility);
            }

            #endregion
        }
    }
}