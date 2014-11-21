Build & Publish Instructions
============================

For help or more information concerning nuget builds see, [Creating and Publishing a Package](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package).

1. From the command line navigate to the WebBrowserWaiter project folder.

2. Pack the project.

    `nuget pack WebBrowserWaiter.csproj -Prop Configuration=Release`

3. Push the package.

    `nuget push WebBrowserWaiter.#.#.#.#.nupkg`
