<hr/>
<h1 align="center">
	Argonaut Core
</h1>
<p align="center">
    These are fundamental functions and generally reusable code that I use across multiple projects.
</p>
<p align="center">
    <a href="https://www.nuget.org/packages/ArgonautCore">
        <img alt="Argonaut Core" src="https://img.shields.io/nuget/vpre/ArgonautCore.svg?maxAge=2592000?style=plastic">
    </a>
    <a href="https://www.nuget.org/packages/ArgonautCore.Database/">
        <img src="https://img.shields.io/nuget/vpre/ArgonautCore.Database.svg?maxAge=2592000?style=plastic">
    </a>
</p>
<hr/>

# !! Each project has their own in-depth readme. !!
This Readme will just have general info. For usage and more info read the readme in the respective project folder :)

# Functional Wrappers and Helpers
<a href="https://www.nuget.org/packages/ArgonautCore">
    <img alt="Argonaut Core" src="https://img.shields.io/nuget/vpre/ArgonautCore.svg?maxAge=2592000?style=plastic">
</a>

This includes functional types that help building more robust and provable functions. The usage is pretty
self explanatory from the XML documentations of the actual functions and types.

### Error handling
I also prefer to use `Result` or `Maybe` to propagate errors instead of throwing exceptions everytime. Exceptions in my mind are for exceptional cases, when shit hits the fan. Not when some API call might return an error that it often does. For that i prefer Go's error handling in just returning an error object that can be handled. 


# Database Fundamental Functions
<a href="https://www.nuget.org/packages/ArgonautCore.Database/">
    <img src="https://img.shields.io/nuget/vpre/ArgonautCore.Database.svg?maxAge=2592000?style=plastic">
</a>

This mainly includes my implementation of the DB wrapper and transactor allowing you 
to easily write repositories and add normal but also atomic queries.
