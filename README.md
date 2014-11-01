WebBrowserWaiter
================

Summary
-------
A headless wrapper of the .NET WebBrowser.

### Installation
```
Install-Package WebBrowserWaiter
```

### Sample
```csharp
using (var waiter = new WebBrowserWaiter())
{
    waiter.Await(
        p => p.Navigate("https://www.google.com/search?q=pi"),
        p => Assert.IsTrue(
            p.DocumentText.Contains("3.14159265")
        )
    );
}
```

Overview
--------
The WebBrowserWaiter serves up functionality of the [.NET WebBrowser](http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser(v=vs.110).aspx) in a manor that can be more easily consumed than with the native component. If you’ve ever tried using the .NET WebBrowser for automated browser testing or screen scraping you’ve likely discovered the challenges it presents due to its [Message Loop](http://en.wikipedia.org/wiki/Message_loop_in_Microsoft_Windows) implementation. WebBrowserWaiter seeks to solve this by abstracting the nuances of the message loop and providing a more familiar (though somewhat peculiar) method of interaction.

Highlights
----------
Here are a few of the noteworthy benefits to using WebBrowserWaiter:
* **Small**. Its essentially a single type that wraps the already provided implementation of the .NET WebBrowser.
* **Simple**. Unlike some other headless browser solutions there are no plugins or browser extensions to install or integrate. WebBrowserWaiter leverages the pre-integrated configuration of Internet Explorer and the WebBrowser control.
* **Powerful**. Because WebBrowser is based on Internet Explorer interaction with and execution of JavaScript makes it possible to test and process modern web applications based on popular libraries and frameworks such as jQuery, AngularJS, etc.
* **Familiar**. If you’ve used the .NET WebBrowser before you’ll be able to utilize that same knowledge as the WebBrowserWaiter provides a WebBrowser instance to invoke actions on.
* **Testable**. Not only can you use WebBrowserWaiter to perform integration tests on production code, but if you need to use it in production code you can easily mock and test it also.
* **Configurable**. Though designed to be headless the WebBrowserWaiter can display the WebBrowser instance in an on screen .NET Form to view navigation transitions. Helpful when you need to see why your instructions may not be executing as you expected.

Compatibility
-------------
.NET 3.5 and above.

Examples
--------

### Extract Document Text
Retrieve a page and extract its text to check for an expected string. Note that the first lambda issues the navigation command which causes the waiter to wait until navigation completes before executing the second lambda which checks the document text. If the instructions were not split into separate lambda’s the document text would be checked before the navigation had completed, almost assuredly providing an incorrect result.

```csharp
using (var waiter = new WebBrowserWaiter())
{
    string text = null;

    waiter.Await(
        p => p.Navigate("http://www.google.com/"),
        p => text = p.DocumentText
    );

    Assert.IsTrue(
        text.Contains("Google")
    );
}
```

### Follow Redirect
Retrieve a page and assert that a 302 redirect is automatically followed to the final expected url. This example also demonstrates that if all the lambda's provided return a value (note the return null in the first lambda) an array of those values are returned as a result of the Await which can be assigned / queried directly to a variable.

```csharp
using (var waiter = new WebBrowserWaiter())
{
    var url = waiter.Await(
        p => { p.Navigate("http://www.google.com/"); return null; },
        p => p.Url.ToString()
    ).Last();

    Assert.AreEqual("https://www.google.com/?gws_rd=ssl", url);
}
```

### Get Then Submit
Get’s a page, sets a form value and then submits the form.

```csharp
using (var waiter = new WebBrowserWaiter())
{
    waiter.Await(
        p => p.Navigate("https://www.google.com/"),
        p => {
            p.Document.All["q"].SetAttribute("value", "time in duluth");
            p.Document.All.Cast<HtmlElement>().First(q => q.TagName == "FORM").InvokeMember("Submit");
        }
    );

    var text = waiter.Await(
        p => p.DocumentText
    );

    Assert.IsTrue(
        text.Contains("Time in Duluth, MN")
    );
}
```

### More
The goal is add more examples overtime, especially examples demonstrating JavaScript interaction and capabilities.

Warnings
--------
### Place Navigation Instructions In Separate Lambda’s
If more than one navigation instruction is contained within the same lambda its likely to result in unpredictable behavior including causing the WebBrowserWaiter to hang. The following demonstrates a lambda with multiple navigations that is NOT recommended.

```csharp
using (var waiter = new WebBrowserWaiter())
{
    waiter.Await(
        p => {
            p.Navigate(uri + "search");
            p.Document.All.Cast<HtmlElement>().First(q => q.TagName == "FORM").InvokeMember("Submit");
        }
    );
}
```

### Do NOT Extract WebBrowser
Its highly recommended that WebBrowser not be extracted from the WebBrowserWaiter into an outside scope. Doing so can have unpredictable results if the WebBrowser is acted upon in the outside scope as the WebBrowser is meant to be operated on from a lone [ApartmentState](http://msdn.microsoft.com/en-us/library/system.threading.thread.apartmentstate(v=vs.110).aspx) thread. The following demonstrates an extraction that is NOT recommended.

```csharp
using (var waiter = new WebBrowserWaiter())
{
    WebBrowser browser = null;

    waiter.Await(
        p => browser = p // < DO NOT ATTEMPT
    );

    browser.Navigate("/path");
}
```

Troubleshooting
---------------

### See Warnings
Before doing anything be sure you’ve read and understand the warnings.

### Questions
If you still have questions please post your question on [Stack Overflow](http://stackoverflow.com/) using the tag [WebBrowserWaiter].

### Bugs
Create a GitHub issue.

Contributions
-------------
Pull requests welcome!
